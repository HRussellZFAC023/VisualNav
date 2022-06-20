using RadialMenu.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace VisualThreading.ToolWindows
{
    public partial class RadialWindowControl
    {
        IDictionary<string, List<RadialMenuItem>> menu = new Dictionary<string, List<RadialMenuItem>>(); // Store all menu levels without heriachy
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

                foreach (var language in json.RadialMenu)
                {
                    foreach (var menuItem in language.MenuItems)
                    {
                        if (menu.ContainsKey(menuItem.parent))
                        {
                            var temp = new RadialMenuItem { Content = new TextBlock { Text = menuItem.name } };
                            temp.Click += (_, _) => RadialDialControl_Click(menuItem.name); // Go to ThreadSubMenu
                            menu[menuItem.parent].Add(temp);
                        }
                        else
                        {
                            menu.Add(menuItem.parent, new List<RadialMenuItem> { });
                            var temp = new RadialMenuItem { Content = new TextBlock { Text = menuItem.name } };
                            temp.Click += (_, _) => RadialDialControl_Click(menuItem.name); // Go to ThreadSubMenu
                            menu[menuItem.parent].Add(temp);
                        }
                    }  // Generate the menu structure to the menu dictionary 

                    MainMenu.Items = menu["Main"];  // Set default menu to Main menu

                    foreach (var command in language.commands)
                    {
                        if (menu.ContainsKey(command.parent))
                        {
                            var temp = new RadialMenuItem { Content = new TextBlock { Text = command.text } };
                            // ---------------------------This is the handler of the command, please make sure you fill in the one on line 71 as well---------------------------
                            // temp.Click += (_, _) => RadialDialControl_Click(); 
                            // temp.MouseEnter += (_, _) => RadialDialElement_Hover();
                            // temp.MouseLeave += (_, _) => RadialDialElement_ExitHover();
                            menu[command.parent].Add(temp);
                        }
                        else
                        {
                            menu.Add(command.parent, new List<RadialMenuItem> { });
                            var temp = new RadialMenuItem { Content = new TextBlock { Text = command.text } };
                            // ---------------------------This is the handler of the command----------------------------------
                            // temp.Click += (_, _) => RadialDialControl_Click(); 
                            // temp.MouseEnter += (_, _) => RadialDialElement_Hover();
                            // temp.MouseLeave += (_, _) => RadialDialElement_ExitHover();
                            menu[command.parent].Add(temp);
                        }
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
                MainMenu.Items = menu[element];
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
                MainMenu.Items = menu[subMenu];
                _state.Push(_state.Count == 0 ? "Main" : _currentState);
                _currentState = subMenu;
            }
            ).FireAndForget();
        }

        private void RadialDialControl_Back() // handles the subfolder element like loop
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                var temp = _state.Count == 0 ? "Main" : _state.Pop().ToString();
                _currentState = temp;
                MainMenu.Items = menu[temp];
            }
            ).FireAndForget();
        }
    }
}
