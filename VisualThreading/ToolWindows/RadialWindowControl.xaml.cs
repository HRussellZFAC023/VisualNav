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
    private const string WhiteIce = "#DCEDF9";
    private const string Chambray = "#38499B";
    private const string Varden = "#FFF6E0";
    private const string CreamBrulee = "#FFE4A1";

    // create a list of languages that require the "insertion" button
    private readonly HashSet<string> _insertion = new(); // languages requiring insertion button

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
        SizeChanged += (_, _) => RadialMenuGeneration(); // Dynamic resize
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

                foreach (var l in _json.RadialMenu)
                    if (l.allow_insertion_from_menu)
                        // populate list
                        _insertion.Add(l.FileExt);

                // get the current language + Check if it is contained in the list.
                InsertionPanel.Visibility =
                    _insertion.Contains(LanguageMediator.GetCurrentActiveFileExtension())
                        ? Visibility.Visible
                        : Visibility.Hidden;
                var language =
                    (from lang in _json.RadialMenu
                     where lang.FileExt == LanguageMediator.GetCurrentActiveFileExtension()
                     select lang).FirstOrDefault();

                if (language == null)
                {
                    ProgressText.Text =
                        "File type not yet supported or no file is open.\nTo get started load a file in the editor.\nSupported file types: .cs, .xaml";
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
                MainGrid.MouseLeave += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();
                MainGrid.MouseEnter += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();

                var radius = Math.Min(RenderSize.Width * 0.45, RenderSize.Height * 0.45);
                var fontSize = Math.Min(Math.Max(Math.Ceiling(12 * radius / 150), 9), 32);
                ProgressText.FontSize = fontSize;
                Insertion.Height = fontSize;
                InsertionLabel.FontSize = fontSize;

                foreach (var command in language.Commands) // commands
                {
                    var menuBlock = MenuBlock(new TextBlock { Text = command.Text }, Varden, CreamBrulee);
                    menuBlock.Click += (_, _) => RadialDialElement_Click(command); //Handler of the command
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
                    page1Next.Click += (_, _) => RadialDialControl_Click(parent + "\x00A0 [Page 2]", true);
                    page1.Add(page1Next);

                    var page2 = _menu[parent].GetRange(_menu[parent].Count / 2, _menu[parent].Count / 2);
                    var page2Prev = MenuBlock(BuildIcon("BrowsePrevious"), Varden, CreamBrulee);
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
        var radius = Math.Min(RenderSize.Width * 0.45, RenderSize.Height * 0.45); //150
        var fontSize = Math.Min(Math.Max(Math.Ceiling(12 * radius / 150), 9), 32);

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

    /// <summary>
    ///     returns a 25:25 icon given a known moniker (icon) name.
    /// </summary>
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
                    await VS.StatusBar.ShowMessageAsync("Copied to clipboard.");
                }
            }).FireAndForget();
        else
            BuildingWindow.Instance.SetCurrentCommand(element);
    }

    private void DecreaseSize(object sender, RoutedEventArgs e)
    {
        //Settings.Instance.RadialSize -= 0.1; //@jianxuan originally I changed the settings to a single scale int, but then decided to just make it dynamic
        //Settings.Instance.Save();
        DockToEditor();
        RadialMenuGeneration();
    }

    private void IncreaseSize(object sender, RoutedEventArgs e)
    {
        //Settings.Instance.RadialSize += 0.1;
        //Settings.Instance.Save();
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

            // Get the current tool window frame mode.
            IVsWindowFrame frame = await VS.Windows.FindWindowAsync(PackageGuids.RadialMenu);
            if (frame == null) return;

            frame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out var currentFrameMode);

            // If currently docked switch to floating mode.
            if ((VSFRAMEMODE)currentFrameMode == VSFRAMEMODE.VSFM_Dock)
                frame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_Float);

            // manually set the size to fullscreen only. I am not sure how to get the x,y co-ordinates correct
            // todo: have this in the correct position @Jianxuan can you help?
            // Is there a better way to get things fullscreen?
            //@see https://docs.microsoft.com/en-us/dotnet/api/?view=visualstudiosdk-2022
            // it must be possible as there is this extension:
            // https://marketplace.visualstudio.com/items?itemName=VisualStudioPlatformTeam.Double-ClickMaximize&ssr=false#overview

            //current size
            frame.GetFramePos(new[] { VSSETFRAMEPOS.SFP_fSize }, out var pguidRelativeTo, out var px,
                out var py,
                out var pcx,
                out var pcy);
            // new size
            var maxWidth = (int)SystemParameters.PrimaryScreenWidth;
            var maxHeight = (int)SystemParameters.PrimaryScreenHeight;
            frame.SetFramePos(VSSETFRAMEPOS.SFP_fSize, pguidRelativeTo, 0, 0, maxWidth,
                maxHeight);

        }).FireAndForget();
    }

    private static void DockToEditor()
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // Get the current tool window frame mode.
            IVsWindowFrame frame = await VS.Windows.FindWindowAsync(PackageGuids.RadialMenu);
            if (frame == null) return;
            frame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out var currentFrameMode);

            // If currently floating switch to docked mode.
            if ((VSFRAMEMODE)currentFrameMode == VSFRAMEMODE.VSFM_Float)
                frame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_Dock);
        }).FireAndForget();
    }
}