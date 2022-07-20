using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
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

namespace VisualThreading.ToolWindows;

public partial class RadialWindowControl
{
    private readonly Dictionary<string, string> _colorMap = new()
    {
            { "WhiteIce", "#DCEDF9" },
            { "Chambray", "#38499B" },
            { "Varden", "#FFF6E0" },
            { "CreamBrulee", "#FFE4A1" },
            { "DarkWhiteIce", "#a0ceef" },
            { "DarkVarden", "#ffebbe" },
        };

    // create a list of languages that require the "insertion" button
    private string _currentState = "";
    private Schema.Schema _json;
    private IDictionary<string, List<RadialMenuItem>> _menu; // Store all menu levels without hierarchy
    private string _progress = "";
    private Stack<string> _state = new(); // Store the current state of the menu

    public RadialWindowControl()
    {
        InitializeComponent();
        RadialMenuGeneration();
        VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            RadialMenuGeneration(); // Dynamic resize
        }
    }

    private void SelectionEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Focus();
        RadialMenuGeneration();
    }

    private void RadialMenuGeneration()
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                _menu = new Dictionary<string, List<RadialMenuItem>>();
                _state = new Stack<string>();
                MainMenu.Items = new List<RadialMenuItem>();
                _json ??= await Schema.Schema.LoadAsync();


                // get the current language + Check if it is contained in the list.
                Radialmenu language = null;
                foreach (var lan in _json.RadialMenu)
                {
                    foreach(var ext in lan.FileExt)
                    {
                        if (ext.Equals(LanguageMediator.GetCurrentActiveFileExtension()))
                        {
                            language = lan;
                        }
                    }
                }

                if (language == null)
                {
                    ProgressText.Text =
                        "File type not yet supported or no file is open.\nTo get started load a file in the editor.\nSupported file types: .cs, .xaml!";
                    MainMenu.CentralItem = null;
                    return;
                }

                InsertionPanel.Visibility = language.allow_insertion_from_menu
                                        ? Visibility.Visible
                                        : Visibility.Hidden;

                MainGrid.ClipToBounds = true;

                MainMenu.CentralItem = new RadialMenuCentralItem
                {
                    Content = BuildIcon("Backwards"),
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_colorMap["WhiteIce"])
                };
                MainMenu.CentralItem.Click += (_, _) => RadialDialControl_Back();

                foreach (var menuItem in language.MenuItems) // menu
                {
                    var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                    var item = MenuBlock(stackPanel, _colorMap["WhiteIce"], _colorMap["Chambray"]);
                    item.MouseEnter += (_, _) => IncreaseOnHover(item, _colorMap);
                    item.MouseLeave += (_, _) => ResetSizeOnExitHover(item, _colorMap);
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
                MainGrid.MouseLeave += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();
                MainGrid.MouseEnter += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();

                var radius = Math.Min(RenderSize.Width * 0.4, RenderSize.Height * 0.4);
                var ratio = radius / 150;
                var fontSize = Math.Min(Math.Max(Math.Ceiling(12 * ratio), 9), 32);
                ProgressText.FontSize = fontSize;
                Insertion.Height = fontSize;
                InsertionLabel.FontSize = fontSize;
                MainMenu.CentralItem.Height = Convert.ToDouble(60 * ratio);
                MainMenu.CentralItem.Width = Convert.ToDouble(60 * ratio);

                foreach (var command in language.Commands) // commands
                {
                    var menuBlock = MenuBlock(new TextBlock { Text = command.Text }, _colorMap["Varden"], _colorMap["CreamBrulee"]);
                    menuBlock.Click += (_, _) => RadialDialElement_Click(command); //Handler of the command
                    menuBlock.MouseEnter += (_, _) => PreviewWindow.Instance.SetCurrentCommand(command);
                    menuBlock.MouseLeave += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();

                    menuBlock.MouseEnter += (_, _) => IncreaseOnHover(menuBlock, _colorMap);
                    menuBlock.MouseLeave += (_, _) => ResetSizeOnExitHover(menuBlock, _colorMap);

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
                    var page1Next = MenuBlock(BuildIcon("BrowseNext"), _colorMap["Varden"], _colorMap["CreamBrulee"]);
                    page1Next.MouseEnter += (_, _) => IncreaseOnHover(page1Next, _colorMap);
                    page1Next.MouseLeave += (_, _) => ResetSizeOnExitHover(page1Next, _colorMap);
                    page1Next.Click += (_, _) => RadialDialControl_Click(parent + "\x00A0 [Page 2]", true);
                    page1.Add(page1Next);

                    var page2 = _menu[parent].GetRange(_menu[parent].Count / 2, _menu[parent].Count / 2);
                    var page2Prev = MenuBlock(BuildIcon("BrowsePrevious"), _colorMap["Varden"], _colorMap["CreamBrulee"]);
                    page2Prev.MouseEnter += (_, _) => IncreaseOnHover(page2Prev, _colorMap);
                    page2Prev.MouseLeave += (_, _) => ResetSizeOnExitHover(page2Prev, _colorMap);
                    page2Prev.Click += (_, _) => RadialDialControl_Click(parent, true);
                    page2.Add(page2Prev);

                    tempMenu.Add(parent, page1);
                    tempMenu.Add(parent + "\x00A0 [Page 2]", page2);
                }

                foreach (var key in tempMenu.Keys)
                    if (key.Any(char.IsDigit))
                        _menu.Add(key, tempMenu[key]);
                    else
                        _menu[key] = tempMenu[key];
            }
        ).FireAndForget();
    }

    private RadialMenuItem MenuBlock(object contentPanel, string c1, string c2)
    {
        var radius = Math.Min(RenderSize.Width * 0.4, RenderSize.Height * 0.4); //150
        var ratio = radius / 150;
        var fontSize = Math.Min(Math.Max(Math.Ceiling(12 * ratio), 9), 32);

        return new RadialMenuItem
        {
            Content = contentPanel,
            // Changed according to current setting

            FontSize = fontSize, //12,
            OuterRadius = radius, //150 ,
            ContentRadius = radius * 0.55, //82.5,
            EdgeInnerRadius = radius * 0.9, // 135,
            EdgeOuterRadius = radius, // 150,
            ArrowRadius = 0.95 * radius, //142.5,

            // DO NOT CHANGE THESE VALUE!
            Padding = 0,
            InnerRadius = 10,
            EdgePadding = 0,
            Background = (SolidColorBrush)new BrushConverter().ConvertFrom(c1),
            EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom(c2)
        };
    }

    private static void IncreaseOnHover(RadialMenuItem menuItem, IReadOnlyDictionary<string, string> colorMap)
    {
        menuItem.OuterRadius *= 1.08;
        menuItem.EdgeInnerRadius *= 1.08;
        menuItem.EdgeOuterRadius *= 1.08;
        menuItem.ArrowRadius *= 1.08;
        menuItem.Padding = 3;
        menuItem.EdgePadding = 3;
        var myKey = "Dark" + colorMap.FirstOrDefault(x => ((SolidColorBrush)new BrushConverter().ConvertFrom(x.Value))?.ToString() == menuItem.Background.ToString()).Key;
        var darkColor = colorMap[myKey];
        menuItem.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(darkColor);
    }

    private static void ResetSizeOnExitHover(RadialMenuItem menuItem, IReadOnlyDictionary<string, string> colorMap)
    {
        menuItem.OuterRadius /= 1.08;
        menuItem.EdgeInnerRadius /= 1.08;
        menuItem.EdgeOuterRadius /= 1.08;
        menuItem.ArrowRadius /= 1.08;
        menuItem.Padding = 0;
        menuItem.EdgePadding = 0;
        var myKey = colorMap.FirstOrDefault(x => ((SolidColorBrush)new BrushConverter().ConvertFrom(x.Value))?.ToString() == menuItem.Background.ToString()).Key;
        var originalColor = colorMap[myKey.Substring(4)];
        menuItem.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(originalColor);
    }

    /// <summary>
    ///     returns a 25:25 icon given a known moniker (icon) name.
    /// </summary>
    private CrispImage BuildIcon(string i)
    {
        var radius = Math.Min(RenderSize.Width * 0.4, RenderSize.Height * 0.4); //150
        var ratio = radius / 150;

        var propertyInfo = typeof(KnownMonikers).GetProperty(i);
        var icon = (ImageMoniker)propertyInfo?.GetValue(null, null)!;
        var image = new CrispImage { Width = 25 * ratio, Height = 25 * ratio, Moniker = icon };
        var binding = new Binding("Background")
        {
            Converter = new BrushToColorConverter(),
            RelativeSource =
                new RelativeSource(RelativeSourceMode.FindAncestor, typeof(RadialWindow), 2)
        };
        image.SetBinding(ImageThemingUtilities.ImageBackgroundColorProperty, binding);
        return image;
    }

    private void RadialDialElement_Click(Command element)
    {
        if (element.Type.Equals("UI"))
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                if (Insertion.IsChecked != null && (bool)Insertion.IsChecked)
                {
                    var docView = await VS.Documents.GetActiveDocumentViewAsync();
                    if (docView?.TextView == null) return; //not a text window
                    var position = docView.TextView.Caret.Position.BufferPosition;
                    docView.TextBuffer?.Insert(position, element.Preview); // Inserts text at the caret
                }
                else
                {
                    Clipboard.SetText(element.Preview);
                    await VS.StatusBar.ShowMessageAsync("Copied to clipboard.");
                    await InfoNotificationWrapper.ShowSimpleAsync("Copied to clipboard.", "Copy", PackageGuids.RadialWindowString, 1500);
                }
            }).FireAndForget();
        else
            BuildingWindow.Instance.SetCurrentCommand(element);
    }

    private void DecreaseSize(object sender, RoutedEventArgs e)
    {
        DockToEditor();
        RadialMenuGeneration();
    }

    private void IncreaseSize(object sender, RoutedEventArgs e)
    {
        MakeFullscreen();
        RadialMenuGeneration();
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
        foreach (var item in _state) _progress = item + " → " + _progress;
        ProgressText.Text = _progress + subMenu;
    }

    private void RadialDialControl_Back()
    {
        if (!ProgressText.Text.Equals("Main"))
        {
            _progress = "";
            foreach (var item in _state) _progress = _progress.Equals("") ? item : item + " → " + _progress;
            _progress.Remove(_progress.Length - 4);
            ProgressText.Text = _progress;
        }

        var temp = _state.Count == 0 ? "Main" : _state.Pop();
        _currentState = temp;
        MainMenu.Items = _menu[temp];
    }

    private static void MakeFullscreen()
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            IVsWindowFrame radialFrame = await VS.Windows.FindWindowAsync(PackageGuids.RadialMenu);
            IVsWindowFrame previewFrame = await VS.Windows.FindWindowAsync(PackageGuids.PreviewWindow);
            IVsWindowFrame buildingFrame = await VS.Windows.FindWindowAsync(PackageGuids.BuildingWindow);

            if (radialFrame == null || previewFrame == null || buildingFrame == null) return;

            var radialWindow = VsShellUtilities.GetWindowObject(radialFrame);
            radialWindow.IsFloating = true;
            radialWindow.Width = (int)((int)SystemParameters.PrimaryScreenWidth * 0.5);
            radialWindow.Height = (int)((int)SystemParameters.PrimaryScreenHeight * 0.75 - 80);
            radialWindow.Left = 0;
            radialWindow.Top = 0;
            radialWindow.WindowState = EnvDTE.vsWindowState.vsWindowStateMaximize;

            var previewWindow = VsShellUtilities.GetWindowObject(previewFrame);
            previewWindow.IsFloating = true;
            previewWindow.Width = (int)((int)SystemParameters.PrimaryScreenWidth * 0.5);
            previewWindow.Height = (int)((int)SystemParameters.PrimaryScreenHeight * 0.25 - 20);
            previewWindow.Left = 0;
            previewWindow.Top = (int)((int)SystemParameters.PrimaryScreenHeight * 0.75 - 60);
            previewWindow.WindowState = EnvDTE.vsWindowState.vsWindowStateMaximize;

            var buildingWindow = VsShellUtilities.GetWindowObject(buildingFrame);
            buildingWindow.IsFloating = true;
            buildingWindow.Width = (int)((int)SystemParameters.PrimaryScreenWidth * 0.5);
            buildingWindow.Height = (int)SystemParameters.PrimaryScreenHeight - 80;
            buildingWindow.Left = (int)((int)SystemParameters.PrimaryScreenWidth * 0.5);
            buildingWindow.Top = 0;
            buildingWindow.WindowState = EnvDTE.vsWindowState.vsWindowStateMaximize;
        }).FireAndForget();
    }

    private static void DockToEditor()
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsWindowFrame radialFrame = await VS.Windows.FindWindowAsync(PackageGuids.RadialMenu);
            IVsWindowFrame previewFrame = await VS.Windows.FindWindowAsync(PackageGuids.PreviewWindow);
            IVsWindowFrame buildingFrame = await VS.Windows.FindWindowAsync(PackageGuids.BuildingWindow);

            if (radialFrame == null || previewFrame == null) return;
            var radialWindow = VsShellUtilities.GetWindowObject(radialFrame);
            radialWindow.IsFloating = false;
            var previewWindow = VsShellUtilities.GetWindowObject(previewFrame);
            previewWindow.IsFloating = false;
            var buildingWindow = VsShellUtilities.GetWindowObject(buildingFrame);
            buildingWindow.IsFloating = false;
        }).FireAndForget();
    }
}