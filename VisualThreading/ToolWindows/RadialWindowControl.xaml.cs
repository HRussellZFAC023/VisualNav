using RadialMenu.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VisualThreading.ToolWindows
{
    public partial class RadialWindowControl
    {
        private readonly IDictionary<string, List<RadialMenuItem>> _menu = new Dictionary<string, List<RadialMenuItem>>(); // Store all menu levels without heriachy
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
                        if (_menu.ContainsKey(menuItem.parent))
                        {
                            var temp = new RadialMenuItem { Content = new TextBlock { Text = menuItem.name } };
                            temp.Click += (_, _) => RadialDialControl_Click(menuItem.name); // Go to ThreadSubMenu
                            _menu[menuItem.parent].Add(temp);
                        }
                        else
                        {
                            _menu.Add(menuItem.parent, new List<RadialMenuItem>());
                            var temp = new RadialMenuItem { Content = new TextBlock { Text = menuItem.name } };
                            temp.Click += (_, _) => RadialDialControl_Click(menuItem.name); // Go to ThreadSubMenu
                            _menu[menuItem.parent].Add(temp);
                        }
                    }  // Generate the menu structure to the menu dictionary

                    MainMenu.Items = _menu["Main"];  // Set default menu to Main menu

                    foreach (var command in language.commands)
                    {
                        if (_menu.ContainsKey(command.parent))
                        {
                            var temp = new RadialMenuItem { Content = new TextBlock { Text = command.text } };

                            // This is the handler of the command
                            temp.Click += (_, _) => RadialDialElement_Click(command);
                            temp.MouseEnter += (_, _) => RadialDialElement_Hover(command);
                            temp.MouseLeave += (_, _) => RadialDialElement_ExitHover();
                            _menu[command.parent].Add(temp);
                        }
                        else
                        {
                            _menu.Add(command.parent, new List<RadialMenuItem>());
                            var temp = new RadialMenuItem { Content = new TextBlock { Text = command.text } };
                            // This is the handler of the command
                            temp.Click += (_, _) => RadialDialElement_Click(command);
                            temp.MouseEnter += (_, _) => RadialDialElement_Hover(command);
                            temp.MouseLeave += (_, _) => RadialDialElement_ExitHover();
                            _menu[command.parent].Add(temp);
                        }
                    }  // Generate the command structure to the menu dictionary
                }  // For each Language, generate menu, commands, links, and handler
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
                MainMenu.Items = _menu[element.text];
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
            }
            ).FireAndForget();
        }

        private void RadialDialControl_Back()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                var temp = _state.Count == 0 ? "Main" : _state.Pop().ToString();
                _currentState = temp;
                MainMenu.Items = _menu[temp];

            }
            ).FireAndForget();
        }
    }
}