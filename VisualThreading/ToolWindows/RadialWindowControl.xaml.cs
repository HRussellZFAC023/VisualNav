using RadialMenu.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace VisualThreading.ToolWindows
{
    public partial class RadialWindowControl
    {
        private readonly Dictionary<string, List<RadialMenuItem>> _menuCollection = new();
        private readonly Stack _state = new();
        private string _currentState = "";

        public RadialWindowControl()
        {
            InitializeComponent();
            RadialMenuGeneration();



            // Back to Home on center item
            MainMenu.CentralItem.Click += (_, _) => RadialDialControl_Back();
        }

        private void RadialMenuGeneration()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var json = await Schema.Schema.LoadAsync();
                var buffer = await VS.Documents.GetActiveDocumentViewAsync();
                IDictionary<string, List<RadialMenuItem>> menu = new Dictionary<string, List<RadialMenuItem>>(); // Store all menu levels without heriachy

                foreach (var language in json.RadialMenu)
                {
                    foreach (var menuItem in language.MenuItems)
                    {
                        if (menu.ContainsKey(menuItem.parent))
                        {
                            menu[menuItem.parent].Add(new() { Content = new TextBlock { Text = menuItem.name } });
                        }
                        else
                        {
                            menu.Add(menuItem.parent, new List<RadialMenuItem> { });
                            menu[menuItem.parent].Add(new() { Content = new TextBlock { Text = menuItem.name } });
                        }
                    }  // Generate the menu structure to the menu dictionary 

                    MainMenu.Items = menu["Main"];  // Set default menu to Main menu

                    foreach (var command in language.commands)
                    {
                        menu[command.parent].Add(new() { Content = new TextBlock { Text = command.text } });
                    }  // Generate the command structure to the menu dictionary 

                }  // For each Language, generate menu, commands, links, and handler

            }
            ).FireAndForget();
        }

        private void RadialDialElement_Click(string element) // handles the eventuall element like a veriable
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                //BuildingWindow.Instance.SetCurrentCommand(element.ToLower());
                // extract element to working area
                MainMenu.Items = _menuCollection[element];
            }
            ).FireAndForget();
        }

        private void RadialDialElement_Hover(string previewName)  // handles the eventuall element like a veriable
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                // take the name of the element and pull preview from json.
                //PreviewWindow.Instance.SetCurrentCommand(previewName.ToLower());
                //BuildingWindow.Instance.SetCurrentCommand(PreviewName.ToLower());
            }
            ).FireAndForget();
        }

        private void RadialDialElement_ExitHover()  // handles the eventuall element like a veriable
        {
            // clear preview area
            //PreviewWindow.Instance.SetCurrentCommand("");
        }

        private void RadialDialControl_Click(string subMenu) // handles the subfolder element like loop
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                MainMenu.Items = _menuCollection[subMenu];
                _state.Push(_state.Count == 0 ? "MainMenuItems" : _currentState);
                _currentState = subMenu;
            }
            ).FireAndForget();
        }

        private void RadialDialControl_Back() // handles the subfolder element like loop
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                var temp = _state.Count == 0 ? "MainMenuItems" : _state.Pop().ToString();
                _currentState = temp;
                MainMenu.Items = _menuCollection[temp];
            }
            ).FireAndForget();
        }
    }
}
