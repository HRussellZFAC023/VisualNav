using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using RadialMenu.Controls;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Binding = System.Windows.Data.Binding;
using Clipboard = System.Windows.Clipboard;
using Orientation = System.Windows.Controls.Orientation;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

namespace VisualThreading.ToolWindows
{
    public partial class RadialWindowControl
    {
        private IDictionary<string, List<RadialMenuItem>> _menu; // Store all menu levels without hierarchy
        private readonly Stack _state = new();
        private string _currentState = "";
        private string _progress = "";
        private string _currentLanguage = ""; // file extension for language
        private Schema.Schema _json;

        public RadialWindowControl()
        {
            InitializeComponent();
            RadialMenuGeneration();

            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged; // get file type
        }

        private void SelectionEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileExt = "";
            if (e.To != null)
            {
                var buffer = e.To.Name;
                fileExt = Path.GetExtension(buffer);
            }

            if (fileExt == _currentLanguage)
                return;
            _currentLanguage = fileExt;
            RadialMenuGeneration();
        }

        private void RadialMenuGeneration()
        {
            _menu = new Dictionary<string, List<RadialMenuItem>>();
            MainMenu.Items = new List<RadialMenuItem>();

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                _json ??= await Schema.Schema.LoadAsync();
                var language = (from lang in _json.RadialMenu where lang.FileExt == _currentLanguage select lang).FirstOrDefault();
                if (language == null)
                {
                    ProgressText.Text = "File type not yet supported or no file is open.\nTo get started load a file in the editor.\nSupported file types: .cs, .xaml";
                    MainMenu.CentralItem.Visibility = Visibility.Hidden;
                    return;
                }

                ProgressText.Text = "Main";

                // Back on center item
                MainGrid.ClipToBounds = true;
                MainMenu.CentralItem.Visibility = Visibility.Visible;
                MainMenu.CentralItem = null;

                var Backwardsicon = (ImageMoniker)typeof(KnownMonikers).GetProperty("Backwards")?.GetValue(null, null)!;
                var Backwardsimage = new CrispImage { Width = 25, Height = 25, Moniker = Backwardsicon };
                var Backwardsbinding = new Binding("Background")
                {
                    Converter = new BrushToColorConverter(),
                    RelativeSource =
                        new RelativeSource(RelativeSourceMode.FindAncestor, typeof(RadialWindow), 2)
                };
                Backwardsimage.SetBinding(ImageThemingUtilities.ImageBackgroundColorProperty, Backwardsbinding);
                var BackwardsStackPanel = new StackPanel { Orientation = Orientation.Vertical };
                BackwardsStackPanel.Children.Add(Backwardsimage);

                MainMenu.CentralItem = new RadialMenuCentralItem
                {
                    Content = BackwardsStackPanel,
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#DCEDF9")
                };
                MainMenu.CentralItem.Click += (_, _) => RadialDialControl_Back();

                foreach (var menuItem in language.MenuItems) // menu
                {
                    var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                    // the text within the radial menu is not under control of VsTheme, however, the progress text box is.
                    // so I decided to use a set color as beckground to prevent text from beging unreadable
                    var item = new RadialMenuItem
                    {
                        Content = stackPanel,
                        FontSize = 12,
                        Padding = 0,
                        InnerRadius = 10,
                        EdgePadding = 0,
                        Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#DCEDF9"),
                        EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom("#38499B")
                    };
                    // icon
                    var propertyInfo = typeof(KnownMonikers).GetProperty(menuItem.Icon);
                    var icon = (ImageMoniker)propertyInfo?.GetValue(null, null)!;
                    var image = new CrispImage { Width = 25, Height = 25, Moniker = icon };
                    var binding = new Binding("Background")
                    {
                        Converter = new BrushToColorConverter(),
                        RelativeSource =
                            new RelativeSource(RelativeSourceMode.FindAncestor, typeof(RadialWindow), 2)
                    };
                    image.SetBinding(ImageThemingUtilities.ImageBackgroundColorProperty, binding);
                    stackPanel.Children.Add(new TextBlock { Text = menuItem.Name });
                    stackPanel.Children.Add(image);

                    // event handler
                    item.Click += (_, _) => RadialDialControl_Click(menuItem.Name, false);

                    if (!_menu.ContainsKey(menuItem.Parent))
                        _menu.Add(menuItem.Parent, new List<RadialMenuItem>());

                    _menu[menuItem.Parent].Add(item);
                }  // Generate the menu structure to the menu dictionary

                MainMenu.Items = _menu["Main"];
                foreach (var command in language.Commands) // commands
                {
                    var temp = new RadialMenuItem
                    {
                        Content = new TextBlock { Text = command.Text },
                        Padding = 0,
                        InnerRadius = 35,
                        EdgePadding = 0,
                        // This color is just a place hodler, will adapt to the future json of the blockly defination of each code type
                        Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF6E0"),
                        EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE4A1"),
                    };

                    temp.Click += (_, _) => RadialDialElement_Click(command);  //Handler of the command
                    temp.MouseEnter += (_, _) => RadialDialElement_Hover(command);
                    temp.MouseLeave += (_, _) => RadialDialElement_ExitHover();

                    if (!_menu.ContainsKey(command.Parent)) _menu.Add(command.Parent, new List<RadialMenuItem>());

                    _menu[command.Parent].Add(temp);
                }  // Generate the command structure to the menu dictionary

                IDictionary<string, List<RadialMenuItem>> tempMenu = new Dictionary<string, List<RadialMenuItem>>();
                foreach (var parent in _menu.Keys)
                { // each menu
                    if (_menu[parent].Count > 6 && _menu[parent][0].Background.ToString().Equals("#FFFFF6E0"))  // 
                    {
                        List<RadialMenuItem> page1 = _menu[parent].GetRange(0, _menu[parent].Count / 2);
                        var page1Next = new RadialMenuItem
                        {
                            Content = new TextBlock { Text = "Next Page" },
                            Padding = 0,
                            InnerRadius = 35,
                            EdgePadding = 0,
                            Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF6E0"),
                            EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE4A1"),
                        };
                        page1Next.Click += (_, _) => RadialDialControl_Click(parent + "_Page2", true);
                        page1.Add(page1Next);

                        List<RadialMenuItem> page2 = _menu[parent].GetRange(_menu[parent].Count / 2, _menu[parent].Count / 2);
                        var page2Prev = new RadialMenuItem
                        {
                            Content = new TextBlock { Text = "Prev Page" },
                            Padding = 0,
                            InnerRadius = 35,
                            EdgePadding = 0,
                            // This color is just a place hodler, will adapt to the future json of the blockly defination of each code type
                            Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF6E0"),
                            EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE4A1"),
                        };
                        page2Prev.Click += (_, _) => RadialDialControl_Click(parent, true);
                        page2.Add(page2Prev);

                        tempMenu.Add(parent, page1);
                        tempMenu.Add(parent + "_Page2", page2);
                        // MessageBoxResult result = System.Windows.MessageBox.Show(_menu[parent][0].Background.ToString());
                        // MessageBoxResult result1 = System.Windows.MessageBox.Show(_menu[parent + "2"].Count + " " + parent + "2");
                    }
                }
                foreach (var key in tempMenu.Keys)
                {
                    if (key.Any(char.IsDigit))
                    {
                        _menu.Add(key, tempMenu[key]);
                    }
                    else
                    {
                        _menu[key] = tempMenu[key];
                    }
                }
            }
            ).FireAndForget();
        }

        private static void RadialDialElement_Click(Schema.Command element)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                if (element.Type.Equals("UI"))
                {
                    await VS.StatusBar.ShowMessageAsync("Copied to clipboard."); // needs to be done first, other wise it won't display,  something to do this threads
                    Clipboard.SetText(element.Preview);
                }
                else
                {
                    BuildingWindow.Instance.SetCurrentCommand(element);
                    // extract element to working area
                    // MainMenu.Items = _menu[element.Text];
                }
            }
            ).FireAndForget();
        }

        private static void RadialDialElement_Hover(Schema.Command preview)
        {
            PreviewWindow.Instance.SetCurrentCommand(preview);
        }

        private static void RadialDialElement_ExitHover()
        {
            PreviewWindow.Instance.ClearCurrentCommand();
        }

        private void DecreaseSize(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                if (ProgressText.FontSize - 3 > 10)
                {
                    ProgressText.FontSize -= 3;
                    foreach (var entry in _menu)
                    {
                        foreach (var element in entry.Value)
                        {
                            element.FontSize -= 3;
                            element.OuterRadius /= 1.2;
                            element.ContentRadius /= 1.2;
                            element.EdgeInnerRadius /= 1.2;
                            element.EdgeOuterRadius /= 1.2;
                            element.ArrowRadius /= 1.2;
                        }
                    }
                }
                else
                {
                    await VS.MessageBox.ShowAsync("Raidial Menu", "Too Small.");
                }
            }
            ).FireAndForget();
        }

        private void IncreaseSize(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                double width = this.RenderSize.Width;
                double height = this.RenderSize.Height;
                // MessageBoxResult result = System.Windows.MessageBox.Show(element.OuterRadius + "");

                bool limit_reached = false;
                foreach (var entry in _menu)
                {
                    foreach (var element in entry.Value)
                    {
                        if (element.OuterRadius * 1.2 < width / 2 && element.OuterRadius * 1.2 < height / 2)
                        {
                            element.FontSize += 3;
                            element.OuterRadius *= 1.2;
                            element.ContentRadius *= 1.2;
                            element.EdgeInnerRadius *= 1.2;
                            element.EdgeOuterRadius *= 1.2;
                            element.ArrowRadius *= 1.2;
                        }
                        else
                        {
                            limit_reached = true;
                            break;
                        }
                    }
                }
                if (!limit_reached)
                {
                    ProgressText.FontSize += 3;
                }
                else
                {
                    await VS.MessageBox.ShowAsync("Raidial Menu", "Too Large, increase the windows size and try again.");
                }
            }
            ).FireAndForget();
        }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);

            }
            ).FireAndForget();
        }
        private void NextPage(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);

            }
            ).FireAndForget();
        }

        private void RadialDialControl_Click(string subMenu, bool pageTuring)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                MainMenu.Items = _menu[subMenu];

                if (!pageTuring)
                {
                    _state.Push(_state.Count == 0 ? "Main" : _currentState);
                    _currentState = subMenu;
                }
                    
                _progress = "";
                foreach (var item in _state)
                {
                    _progress = item + " → " + _progress;
                }
                ProgressText.Text = _progress + subMenu;
  
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
                    _progress = "";
                    foreach (var item in _state)
                    {
                        if (_progress.Equals(""))
                        {
                            _progress = "" + item;
                        }
                        else
                        {
                            _progress = item + " → " + _progress;
                        }
                    }
                    _progress.Remove(_progress.Length - 4);
                    ProgressText.Text = _progress;
                }

                var temp = _state.Count == 0 ? "Main" : _state.Pop().ToString();
                // MessageBoxResult result = System.Windows.MessageBox.Show(temp);
                _currentState = temp;
                MainMenu.Items = _menu[temp];
            }
            ).FireAndForget();
        }
    }
}