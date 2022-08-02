using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using RadialMenu.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VisualNav.Schema;
using VisualNav.Utilities;

namespace VisualNav.ToolWindows;

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
    private bool fullScreen = false;
    private Schema.Schema _json;
    private IDictionary<string, List<RadialMenuItem>> _menu; // Store all menu levels without hierarchy
    private string _progress = "";
    private Stack<string> _state = new(); // maintains the progress/state of the menu levels, (e.g.Main->Code->Array)

    public RadialWindowControl()
    {
        InitializeComponent();
        RadialMenuGeneration();
        VS.Events.SelectionEvents.SelectionChanged += (_, _) => RadialMenuGeneration();
        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// When the size of a window/panle chenges, pass the event and regerate the radial menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            RadialMenuGeneration(); // Dynamic resize
        }
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
                var defaultTxt = "File type not yet supported or no file is open.\nTo get started load a file in the editor.\nSupported file types:"; // .cs, .xaml

                foreach (var lan in _json.RadialMenu)
                {
                    foreach (var ext in lan.FileExt)
                    {
                        defaultTxt = defaultTxt + ext + " ";
                        if (ext.Equals(LanguageMediator.GetCurrentActiveFileExtension()))
                        {
                            language = lan;
                        }
                    }
                }

                if (language == null) // if not a file, hide insertion checkbox, and show message
                {
                    ProgressText.Text = defaultTxt;
                    MainMenu.CentralItem = null;
                    InsertionPanel.Visibility = Visibility.Hidden;
                    return;
                }

                InsertionPanel.Visibility = language.allow_insertion_from_menu // if a file type supports insert mode, val from schema.json
                                        ? Visibility.Visible
                                        : Visibility.Hidden;

                MainGrid.ClipToBounds = true;

                MainMenu.CentralItem = new RadialMenuCentralItem // central back button
                {
                    Content = BuildIcon("Backwards"),
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_colorMap["WhiteIce"])
                };
                MainMenu.CentralItem.Click += (_, _) => RadialDialControl_Back();

                // Populate menu
                foreach (var menuItem in language.MenuItems) // iterates menus in the language in schema.json
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
                    {
                        _menu.Add(menuItem.Parent, new List<RadialMenuItem>());
                    }

                    _menu[menuItem.Parent].Add(item); // Add menu structure to menu dictionary
                }

                ProgressText.Text = "Main";  // default progress indicator is Main (top level menu)
                MainMenu.Items = _menu["Main"];
                MainGrid.MouseLeave += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();
                MainGrid.MouseEnter += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();

                var radius = Math.Min(RenderSize.Width * 0.4, RenderSize.Height * 0.4); // RenderSize is the size of window, choose the min between height and width
                var ratio = radius / 150; // conversion rate of radius to size on screen
                var fontSize = Math.Min(Math.Max(Math.Ceiling(12 * ratio), 9), 32); // sync all font size in this plugin
                ProgressText.FontSize = fontSize;
                Insertion.Height = fontSize;
                InsertionLabel.FontSize = fontSize;
                MainMenu.CentralItem.Height = Convert.ToDouble(60 * ratio);
                MainMenu.CentralItem.Width = Convert.ToDouble(60 * ratio);

                foreach (var command in language.Commands) // iterates commands in the language in schema.json
                {
                    var menuBlock = MenuBlock(new TextBlock { Text = command.Text }, _colorMap["Varden"], _colorMap["CreamBrulee"]);
                    menuBlock.Click += (_, _) => RadialDialElement_Click(command, language); //Handler of the command
                    menuBlock.MouseRightButtonDown += (_, _) => RadialDialElement_Remove(command, language); // right click to delete
                    menuBlock.MouseEnter += (_, _) => PreviewWindow.Instance.SetCurrentCommand(command);
                    menuBlock.MouseLeave += (_, _) => PreviewWindow.Instance.ClearCurrentCommand();
                    // Highlight by increasing the radius of radial button
                    menuBlock.MouseEnter += (_, _) => IncreaseOnHover(menuBlock, _colorMap);
                    menuBlock.MouseLeave += (_, _) => ResetSizeOnExitHover(menuBlock, _colorMap);

                    if (!_menu.ContainsKey(command.Parent))
                        _menu.Add(command.Parent, new List<RadialMenuItem>());
                    _menu[command.Parent].Add(menuBlock);
                }

                // if a menu contains too many items, it will get unreadable, divide the menu into two
                IDictionary<string, List<RadialMenuItem>> tempMenu = new Dictionary<string, List<RadialMenuItem>>();
                foreach (var parent in _menu.Keys)
                {
                    if (_menu[parent].Count <= 6 || !_menu[parent][0].Background.ToString().Equals("#FFFFF6E0")) // each command menu
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

                foreach (var key in tempMenu.Keys)  // replace the orinigal into the divided menus
                    if (key.Any(char.IsDigit))
                        _menu.Add(key, tempMenu[key]);
                    else
                        _menu[key] = tempMenu[key];
            }
        ).FireAndForget();
    }

    /// <summary>
    /// RadialMenu item generator, sets the size, color scheme, icon, icon size here
    /// </summary>
    /// <param name="contentPanel"></param>
    /// <param name="c1">Background HEX color in String</param>
    /// <param name="c2">EdgeBackground HEX color in String</param>
    /// <returns></returns>
    private RadialMenuItem MenuBlock(object contentPanel, string c1, string c2)
    {
        var radius = Math.Min(RenderSize.Width * 0.4, RenderSize.Height * 0.4); //150
        var ratio = radius / 150;
        var fontSize = Math.Min(Math.Max(Math.Ceiling(12 * ratio), 9), 32);

        return new RadialMenuItem
        {
            Content = contentPanel,

            // Changes according to current setting
            FontSize = fontSize, //12,
            OuterRadius = radius, //150 ,
            ContentRadius = radius * 0.55, //82.5,
            EdgeInnerRadius = radius * 0.9, // 135,
            EdgeOuterRadius = radius, // 150,
            ArrowRadius = 0.95 * radius, //142.5,

            // DO NOT CHANGE THESE VALUE TO MAINTAIN CONSISTENT LOOK
            Padding = 0,
            InnerRadius = 10,
            EdgePadding = 0,
            Background = (SolidColorBrush)new BrushConverter().ConvertFrom(c1),
            EdgeBackground = (SolidColorBrush)new BrushConverter().ConvertFrom(c2)
        };
    }

    /// <summary>
    /// When hover on a radial button, increase the radius and color of the button to give visual clue to user
    /// </summary>
    /// <param name="menuItem">The RadialMenuItem to increase size</param>
    /// <param name="colorMap">The color lookup table used to highlight a button</param>
    private static void IncreaseOnHover(RadialMenuItem menuItem, IReadOnlyDictionary<string, string> colorMap)
    {
        menuItem.OuterRadius *= 1.08;
        menuItem.EdgeInnerRadius *= 1.08;
        menuItem.EdgeOuterRadius *= 1.08;
        menuItem.ArrowRadius *= 1.08;
        menuItem.Padding = 3;
        menuItem.EdgePadding = 3;
        // "Dark"+original color is the "highlight"
        // normal HEX value is not the same with menuItem.Background, instead we need to compare two Color obj.
        var myKey = "Dark" + colorMap.FirstOrDefault(x => ((SolidColorBrush)new BrushConverter().ConvertFrom(x.Value))?.ToString() == menuItem.Background.ToString()).Key;
        var darkColor = colorMap[myKey];
        menuItem.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(darkColor);
    }

    /// <summary>
    /// When exit hover on a radial button, reset the radius and color of the button
    /// </summary>
    /// <param name="menuItem">The RadialMenuItem to increase size</param>
    /// <param name="colorMap">The color lookup table used to resotre color scheme of a button</param>
    private static void ResetSizeOnExitHover(RadialMenuItem menuItem, IReadOnlyDictionary<string, string> colorMap)
    {
        menuItem.OuterRadius /= 1.08;
        menuItem.EdgeInnerRadius /= 1.08;
        menuItem.EdgeOuterRadius /= 1.08;
        menuItem.ArrowRadius /= 1.08;
        menuItem.Padding = 0;
        menuItem.EdgePadding = 0;
        var myKey = colorMap.FirstOrDefault(x => ((SolidColorBrush)new BrushConverter().ConvertFrom(x.Value))?.ToString() == menuItem.Background.ToString()).Key;
        var originalColor = colorMap[myKey.Substring(4)]; // remove the "Dark" from color name
        menuItem.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(originalColor);
    }

    /// <summary>
    ///  returns a 25:25 icon given a known moniker (icon) name.
    /// </summary>
    /// <param name="iconName">name of the icon in knownmoniker</param>
    /// <returns>CrispImage</returns>
    private CrispImage BuildIcon(string iconName)
    {
        var radius = Math.Min(RenderSize.Width * 0.4, RenderSize.Height * 0.4); //150
        var ratio = radius / 150;

        var propertyInfo = typeof(KnownMonikers).GetProperty(iconName);
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

    /// <summary>
    /// Button click handler, if the element clicked is a member of UI menu, adopt different event
    /// </summary>
    /// <param name="element"></param>
    private async void RadialDialElement_Click(Command element, Radialmenu language)
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
        else switch (element.Text)
            {
                case "New Layer":
                    {
                        DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
                        string userInput = docView.TextView.Selection.StreamSelectionSpan.GetText();
                        if (userInput.Equals(""))
                        {
                            MessageBoxResult result = System.Windows.MessageBox.Show("Select layer name in coding area");
                            return;
                        }
                        // Modify Menuitems section of the json file
                        var menuItem = new Menuitem[language.MenuItems.Length + 1];
                        for (var i = 0; i < language.MenuItems.Length; i++)//copy to new array
                        {
                            if (language.MenuItems[i].Name.Equals(element.Parent))
                            {
                                // add "Custom TEST"(just a test val, need user input for the name) to submenu to represent a sub menu
                                var elementTemp = language.MenuItems[i].Submenu;
                                Array.Resize(ref elementTemp, language.MenuItems[i].Submenu.Length + 1);
                                elementTemp[language.MenuItems[i].Submenu.Length] = userInput;
                                language.MenuItems[i].Submenu = elementTemp;

                                //from children remove "New Layer"
                                var childrenTemp = language.MenuItems[i].Children;
                                childrenTemp = childrenTemp.Where((source, index) => index != 0).ToArray();
                                language.MenuItems[i].Children = childrenTemp;
                            }
                            menuItem[i] = language.MenuItems[i];
                        }

                        var item = new Menuitem
                        {
                            Name = userInput,
                            Parent = element.Parent,
                            Submenu = Array.Empty<string>(),
                            Children = new[] { "Custom Object", "Custom Function", "New Layer" },
                            Icon = "Code"
                        };
                        menuItem[menuItem.Length - 1] = item;

                        language.MenuItems = menuItem;

                        // create corresponding commands ("New Layer" and "Create Command") in the Commands section
                        var commandsList = new Command[language.Commands.Length + 3];
                        for (var i = 0; i < language.Commands.Length; i++)//copy to new array
                        {
                            commandsList[i] = language.Commands[i];
                        }

                        var customFunction = new Command
                        {
                            Text = "Custom Function",
                            Parent = userInput,
                            Preview = "",
                            Color = "#FF00FFFF",
                            Type = ""
                        };

                        var customObject = new Command
                        {
                            Text = "Custom Object",
                            Parent = userInput,
                            Preview = "",
                            Color = "#FF00FFFF",
                            Type = ""
                        };

                        var newSubMenu = new Command
                        {
                            Text = "New Layer",
                            Parent = userInput,
                            Preview = "",
                            Color = "#FF00FFFF",
                            Type = ""
                        };
                        commandsList[commandsList.Length - 3] = customFunction;
                        commandsList[commandsList.Length - 2] = customObject;
                        commandsList[commandsList.Length - 1] = newSubMenu;

                        language.Commands = commandsList;

                        var dir = Path.GetDirectoryName(typeof(RadialWindowControl).Assembly.Location);
                        var file = Path.Combine(dir!, "Schema", "Modified.json");
                        File.WriteAllText(file, JsonConvert.SerializeObject(_json));

                        Options.Settings.Instance.CustomBlock = true;
                        Options.Settings.Instance.Save();

                        RadialMenuGeneration();
                        break;
                    }
                case "Custom Object":
                case "Custom Function":
                    {
                        DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
                        string userInput = docView.TextView.Selection.StreamSelectionSpan.GetText();
                        if (userInput.Equals("")) //prevent empty name input
                        {
                            MessageBoxResult result = System.Windows.MessageBox.Show("Select object name in coding area");
                            return;
                        }
                        // modify children list (add the command into the children string array)
                        foreach (var item in language.MenuItems)
                        {
                            if (!item.Name.Equals(element.Parent)) continue;
                            var newChildList = new string[item.Children.Length + 1];
                            for (var i = 0; i < item.Children.Length; i++)
                            {
                                newChildList[i] = item.Children[i];
                            }
                            newChildList[newChildList.Length - 1] = userInput;
                        }
                        // create corresponding commands ("New Layer" and "Create Command") in the Commands section
                        var commandsList = new Command[language.Commands.Length + 1];
                        for (var i = 0; i < language.Commands.Length; i++)//copy to new array
                        {
                            commandsList[i] = language.Commands[i];
                        }

                        string type = element.Text.Equals("Custom Object") ? "custom_object_" : "custom_function_";
                        type = type + userInput;
                        userInput = element.Text.Equals("Custom Function") ? userInput + "( )" : userInput;

                        foreach(var temp in language.Commands) //prevent duplicates
                        {
                            if (temp.Text.Equals(userInput))
                            {
                                MessageBoxResult result = System.Windows.MessageBox.Show("Duplicate object/function found, try another name.");
                                return;
                            }
                        }

                        var newCommand = new Command
                        {
                            Text = userInput,
                            Parent = element.Parent,
                            Preview = "",
                            Color = "#FFBF00", // TODO: program a input dialog to get the color
                            Type = type
                        };

                        commandsList[commandsList.Length - 1] = newCommand;

                        language.Commands = commandsList;
                        var dir = Path.GetDirectoryName(typeof(RadialWindowControl).Assembly.Location);
                        var file = Path.Combine(dir!, "Schema", "Modified.json");
                        File.WriteAllText(file, JsonConvert.SerializeObject(_json));

                        Options.Settings.Instance.CustomBlock = true;
                        Options.Settings.Instance.Save();

                        RadialMenuGeneration();
                        break;
                    }
                default:
                    BuildingWindow.Instance.SetCurrentCommand(element);
                    break;
            }
    }

    private void RadialDialElement_Remove(Command element, Radialmenu language)
    {
        if (element.Type.Contains("custom")){ // if it is a custom button

            foreach (var item in language.MenuItems) // remove from child array
            {
                if (!item.Name.Equals(element.Text)) continue;
                var newChildList = new string[item.Children.Length - 1];
                for (var i = 0; i < item.Children.Length; i++)
                {
                    if (!item.Children[i].Equals(element.Text))
                    {
                        newChildList[i] = item.Children[i];
                    }
                }
            }

            var commandsList = new Command[language.Commands.Length - 1]; 
            for (var i = 0; i < language.Commands.Length; i++) //remove from command list
            {
                if (!language.Commands[i].Text.Equals(element.Text))
                {
                    commandsList[i] = language.Commands[i];
                }
            }
                
            language.Commands = commandsList;
            var dir = Path.GetDirectoryName(typeof(RadialWindowControl).Assembly.Location);
            var file = Path.Combine(dir!, "Schema", "Modified.json");
            File.WriteAllText(file, JsonConvert.SerializeObject(_json));

            RadialMenuGeneration();
        }
        else
        {
            return;
        }
        
    }

    /// <summary>
    /// The handler of the exit full screen button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DecreaseSize(object sender = null, RoutedEventArgs e = null)
    {
        DockToEditor();
        RadialMenuGeneration();
    }


    public void ToggleFullscreen(object sender = null, RoutedEventArgs e = null)
    {
        // set fullscreen = false
        // else
        // IncreaseSize
        // set fullscreen = true
        if (fullScreen == true) 
        {
            DecreaseSize();
        }
        else
        {
            IncreaseSize();
        }
        fullScreen = !fullScreen;
        
    }



    /// <summary>
    /// The handler of the full screen button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void IncreaseSize(object sender = null, RoutedEventArgs e = null)
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

    /// <summary>
    /// Center back button handler
    /// </summary>
    private void RadialDialControl_Back()
    {
        if (!ProgressText.Text.Equals("Main")) // if not at home page, go back to the previous level according to _progress
        {
            _progress = "";
            foreach (var item in _state) _progress = _progress.Equals("") ? item : item + " → " + _progress;
            _progress.Remove(_progress.Length - 4);
            ProgressText.Text = _progress;  // update progress display
        }
        // maintain _state stack
        var temp = _state.Count == 0 ? "Main" : _state.Pop();
        _currentState = temp;
        MainMenu.Items = _menu[temp];
    }

    /// <summary>
    /// Undock radialFrame, previewFrame, buildingFrame undock and resize to fill screen
    /// </summary>
    private static void MakeFullscreen()
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            IVsWindowFrame radialFrame = await VS.Windows.FindWindowAsync(PackageGuids.RadialWindow);
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

    /// <summary>
    /// Dock radialFrame, previewFrame, buildingFrame
    /// </summary>
    private static void DockToEditor()
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsWindowFrame radialFrame = await VS.Windows.FindWindowAsync(PackageGuids.RadialWindow);
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