using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using RadialMenu.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VisualThreading.Schema;
using VisualThreading.Utilities;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

namespace VisualThreading.ToolWindows
{
    public partial class RadialWindowControl
    {
        private IDictionary<string, List<RadialMenuItem>> _menu; // Store all menu levels without hierarchy
        private readonly Stack<string> _state = new(); // Store the current state of the menu
        private string _currentState = "";
        private string _progress = "";
        private Schema.Schema _json;
        private const string WhiteIce = "#DCEDF9";
        private const string Chambray = "#38499B";
        private const string Varden = "#FFF6E0";
        private const string CreamBrulee = "#FFE4A1";

        public RadialWindowControl()
        {
            InitializeComponent();
            RadialMenuGeneration();
            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged;
        }

        private void SelectionEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RadialMenuGeneration();
        }

        private void RadialMenuGeneration()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                _menu = new Dictionary<string, List<RadialMenuItem>>();
                MainMenu.Items = new List<RadialMenuItem>();
                _json ??= await Schema.Schema.LoadAsync();
                var language = (from lang in _json.RadialMenu where lang.FileExt == LanguageMediator.GetCurrentActiveFileExtension() select lang).FirstOrDefault();

                if (language == null)
                {
                    ProgressText.Text = "File type not yet supported or no file is open.\nTo get started load a file in the editor.\nSupported file types: .cs, .xaml";
                    MainMenu.CentralItem = null;
                    return;
                }

                MainGrid.ClipToBounds = true;

                MainMenu.CentralItem = new RadialMenuCentralItem
                {
                    Content = BuildIcon("Backwards"),
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(WhiteIce)
                };
                MainMenu.CentralItem.Click += (_, _) => RadialDialControl_Back();

                foreach (var menuItem in language.MenuItems) // menu
                {
                    var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                    var item = MenuBlock(stackPanel, WhiteIce, Chambray);
                    // icon
                    var image = BuildIcon(menuItem.Icon);
                    stackPanel.Children.Add(image);
                    // name
                    stackPanel.Children.Add(new TextBlock { Text = menuItem.Name });
                    // event handler
                    item.Click += (_, _) => RadialDialControl_Click(menuItem.Name, false);

                    if (!_menu.ContainsKey(menuItem.Parent))
                        _menu.Add(menuItem.Parent, new List<RadialMenuItem>());

                    // Add menu structure to menu dictionary
                    _menu[menuItem.Parent].Add(item);
                }

                ProgressText.Text = "Main";
                MainMenu.Items = _menu["Main"];

                foreach (var command in language.Commands) // commands
                {
                    var menuBlock = MenuBlock(new TextBlock { Text = command.Text }, Varden, CreamBrulee);
                    menuBlock.Click += (_, _) => RadialDialElement_Click(command);  //Handler of the command
                    menuBlock.MouseEnter += (_, _) => PreviewWindow.Instance.SetCurrentCommand(command);
                    menuBlock.MouseLeave += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();

                    if (!_menu.ContainsKey(command.Parent))
                        _menu.Add(command.Parent, new List<RadialMenuItem>());
                    _menu[command.Parent].Add(menuBlock);
                }

                IDictionary<string, List<RadialMenuItem>> tempMenu = new Dictionary<string, List<RadialMenuItem>>();
                foreach (var parent in _menu.Keys)
                {
                    // each menu
                    if (_menu[parent].Count <= 6 || !_menu[parent][0].Background.ToString().Equals("#FFFFF6E0"))
                        continue;

                    var page1 = _menu[parent].GetRange(0, _menu[parent].Count / 2);
                    var page1Next = MenuBlock(BuildIcon("BrowseNext"), Varden, CreamBrulee);
                    page1Next.Click += (_, _) => RadialDialControl_Click(parent + "-Page2", true);
                    page1.Add(page1Next);

                    var page2 = _menu[parent].GetRange(_menu[parent].Count / 2, _menu[parent].Count / 2);
                    var page2Prev = MenuBlock(BuildIcon("BrowsePrevious"), Varden, CreamBrulee);
                    page2Prev.Click += (_, _) => RadialDialControl_Click(parent, true);
                    page2.Add(page2Prev);

                    tempMenu.Add(parent, page1);
                    tempMenu.Add(parent + "-Page2", page2);
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

        private static RadialMenuItem MenuBlock(object stackPanel, string c1, string c2)
        {
            return new RadialMenuItem
            {
                Content = stackPanel,
                FontSize = 12,
                Padding = 0,
                InnerRadius = 10,
                EdgePadding = 0,
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom(c1),
                EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom(c2)
            };
        }

        private static CrispImage BuildIcon(string i)
        {
            var propertyInfo = typeof(KnownMonikers).GetProperty(i);
            var icon = (ImageMoniker)propertyInfo?.GetValue(null, null)!;
            var image = new CrispImage { Width = 25, Height = 25, Moniker = icon };
            var binding = new Binding("Background")
            {
                Converter = new BrushToColorConverter(),
                RelativeSource =
                    new RelativeSource(RelativeSourceMode.FindAncestor, typeof(RadialWindow), 2)
            };
            image.SetBinding(ImageThemingUtilities.ImageBackgroundColorProperty, binding);
            return image;
        }

        private static void RadialDialElement_Click(Command element)
        {
            if (element.Type.Equals("UI"))
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    await VS.StatusBar.ShowMessageAsync("Copied to clipboard.");
                }).FireAndForget();
                Clipboard.SetText(element.Preview);
            }
            else
            {
                BuildingWindow.Instance.SetCurrentCommand(element);
            }
        }

        private void DecreaseSize(object sender, RoutedEventArgs e)
        {
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
                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    await VS.MessageBox.ShowAsync("Radial Menu", "Too Small.");
                }
                   ).FireAndForget();
            }
        }

        private void IncreaseSize(object sender, RoutedEventArgs e)
        {
            var width = RenderSize.Width;
            var height = RenderSize.Height;
            var limitReached = false;

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
                        limitReached = true;
                        break;
                    }
                }
            }
            if (!limitReached)
            {
                ProgressText.FontSize += 3;
            }
            else
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    await VS.MessageBox.ShowAsync("Radial Menu", "Too Large, increase the windows size and try again.");
                }).FireAndForget();
            }
        }

        private void RadialDialControl_Click(string subMenu, bool pageTuring)
        {
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

        private void RadialDialControl_Back()
        {
            if (!ProgressText.Text.Equals("Main"))
            {
                _progress = "";
                foreach (var item in _state)
                {
                    _progress = _progress.Equals("") ? item : item + " → " + _progress;
                }
                _progress.Remove(_progress.Length - 4);
                ProgressText.Text = _progress;
            }

            var temp = _state.Count == 0 ? "Main" : _state.Pop();
            _currentState = temp;
            MainMenu.Items = _menu[temp];
        }
    }
}