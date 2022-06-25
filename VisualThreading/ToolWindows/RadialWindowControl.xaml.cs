using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using RadialMenu.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.VisualStudio.Imaging.Interop;
using System.Windows.Media;

namespace VisualThreading.ToolWindows
{
    public partial class RadialWindowControl
    {
        private readonly IDictionary<string, List<RadialMenuItem>>
            _menu = new Dictionary<string, List<RadialMenuItem>>(); // Store all menu levels without hierarchy

        private readonly Stack _state = new();
        private string _currentState = "";
        String progress = "";
        public RadialWindowControl()
        {
            InitializeComponent();
            RadialMenuGeneration();

            // Back on center item
            MainMenu.CentralItem = new RadialMenuCentralItem
            {
                Content = MainMenu.CentralItem,
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#DCEDF9")
            };
            MainMenu.CentralItem.Click += (_, _) => RadialDialControl_Back();
        }

        private void RadialMenuGeneration()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var json = await Schema.Schema.LoadAsync();
                var buffer = await VS.Documents.GetActiveDocumentViewAsync(); // used to get language

                foreach (var language in json.RadialMenu)
                {
                    foreach (var menuItem in language.MenuItems)
                    {
                        var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                        // the text within the radial menu is not under control of VsTheme, however, the progress text box is.
                        // so I decided to use a set color as beckground to prevent text from beging unreadable
                        var item = new RadialMenuItem
                        {
                            Content = stackPanel,
                            Padding = 0,
                            InnerRadius = 10,
                            EdgePadding = 0,
                            Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#DCEDF9"),
                            EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom("#38499B")
                        };
                        var icon = menuItem.Icon;
                        PropertyInfo propertyInfo = typeof(KnownMonikers).GetProperty(menuItem.Icon);

                        var something = (ImageMoniker)propertyInfo.GetValue(null, null);

                        var image = new CrispImage { Width = 25, Height = 25, Moniker = something };

                        var binding = new Binding("Background")
                        {
                            Converter = new BrushToColorConverter(),
                            RelativeSource =
                                new RelativeSource(RelativeSourceMode.FindAncestor, typeof(RadialWindow), 2)
                        };

                        image.SetBinding(ImageThemingUtilities.ImageBackgroundColorProperty, binding);

                        stackPanel.Children.Add(new TextBlock { Text = menuItem.Name });
                        stackPanel.Children.Add(image);

                        item.Click += (_, _) => RadialDialControl_Click(menuItem.Name);

                        if (!_menu.ContainsKey(menuItem.Parent))
                            _menu.Add(menuItem.Parent, new List<RadialMenuItem>());

                        _menu[menuItem.Parent].Add(item);
                    }  // Generate the menu structure to the menu dictionary

                    MainMenu.Items = _menu["Main"];  // Set default menu to Main menu
                    foreach (var command in language.Commands)
                    {
                        var temp = new RadialMenuItem { 
                            Content = new TextBlock { Text = command.Text },
                            Padding = 0,
                            InnerRadius = 35,
                            EdgePadding = 0,
                            // This color is just a place hodler, will adapt to the future json of the blockly defination of each code type
                            Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF6E0"),
                            EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE4A1"),
                        };
                        // This is the handler of the command
                        temp.Click += (_, _) => RadialDialElement_Click(command);
                        temp.MouseEnter += (_, _) => RadialDialElement_Hover(command);
                        temp.MouseLeave += (_, _) => RadialDialElement_ExitHover();

                        if (!_menu.ContainsKey(command.Parent))
                            _menu.Add(command.Parent, new List<RadialMenuItem>());

                        _menu[command.Parent].Add(temp);
                    }  // Generate the command structure to the menu dictionary
                }  // For each Language, generate menu, Commands, links, and handler
            }
            ).FireAndForget();
        }

        private void RadialDialElement_Click(Schema.Command element)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);

                BuildingWindow.Instance.SetCurrentCommand(element);
                // extract element to working area
                MainMenu.Items = _menu[element.Text];
            }
            ).FireAndForget();
        }

        private static void RadialDialElement_Hover(Schema.Command preview)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                PreviewWindow.Instance.SetCurrentCommand(preview);
            }
            ).FireAndForget();
        }

        private static void RadialDialElement_ExitHover()
        {
            PreviewWindow.Instance.ClearCurrentCommand();
        }

        private void RadialDialControl_Click(string subMenu)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                MainMenu.Items = _menu[subMenu];
                _state.Push(_state.Count == 0 ? "Main" : _currentState);
                _currentState = subMenu;
                progress = "";
                foreach (var item in _state)
                {
                    progress = item + " → " + progress;
                }
                ProgressText.Text = progress + subMenu;
            }
            ).FireAndForget();
        }

        private void RadialDialControl_Back()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);

                if (!ProgressText.Text.Equals("Main"))
                {
                    progress = "";
                    foreach (var item in _state)
                    {
                        if (progress.Equals(""))
                        {
                            progress = "" + item;
                        }
                        else
                        {
                            progress = item + " → " + progress;
                        }

                    }
                    progress.Remove(progress.Length - 4);
                    ProgressText.Text = progress;
                }

                var temp = _state.Count == 0 ? "Main" : _state.Pop().ToString();
                _currentState = temp;
                MainMenu.Items = _menu[temp];

            }
            ).FireAndForget();
        }
    }
}
