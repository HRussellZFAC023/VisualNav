using RadialMenu.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VisualThreading.ToolWindows
{
    public partial class RadialDialControl
    {
        private readonly Dictionary<string, List<RadialMenuItem>> _menuCollection = new();

        public RadialDialControl()
        {
            InitializeComponent();

            var mainMenuItems = new List<RadialMenuItem>
            {
                // I the know the color looks bad
                // Background controls the Fan-shaped area
                // ArrowBackground controls the littel arrow on each button
                // EdgeBackground controls the Edge of the Fan-shaped 
                new() { Content = new TextBlock { Text = "Thread" }, Background = Brushes.LightBlue, ArrowBackground = Brushes.LightBlue, EdgeBackground = Brushes.LightGreen}, 
                new() { Content = new TextBlock { Text = "Test" }},
                new() { Content = new TextBlock { Text = "Code" }},
                new() { Content = new TextBlock { Text = "UI" }}
            };

            // Set default menu to Main menu
            MainMenu.Items = mainMenuItems;

            var threadSubMenu = new List<RadialMenuItem> {
                new() { Content = new TextBlock { Text = "New Thread " } }
            };

            var testSubMenu = new List<RadialMenuItem> {
                new() { Content = new TextBlock { Text = "Assert" } }
            };

            var codeSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "I/O" } },
                new(){ Content = new TextBlock { Text = "Method/Keyword" } },
                new(){ Content = new TextBlock { Text = "Condition" } },
                new(){ Content = new TextBlock { Text = "Loop" } },
                new(){ Content = new TextBlock { Text = "Variable" } },
                new(){ Content = new TextBlock { Text = "Operator" } },
                new(){ Content = new TextBlock { Text = "Comparator" } }
            };

            var methodKeywordSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "Static" } },
                new(){ Content = new TextBlock { Text = "Public" } },
                new(){ Content = new TextBlock { Text = "Private" } },
                new(){ Content = new TextBlock { Text = "Protected" } },
                new(){ Content = new TextBlock { Text = "Internal" } },
                new(){ Content = new TextBlock { Text = "Return" } }
            };

            var loopSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "Break" } },
                new(){ Content = new TextBlock { Text = "Continue" } },
                new(){ Content = new TextBlock { Text = "For" } },
                new(){ Content = new TextBlock { Text = "While" } }
            }; 

            var operatorSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "equal" } },
                new(){ Content = new TextBlock { Text = "plus" } },
                new(){ Content = new TextBlock { Text = "minus" } },
                new(){ Content = new TextBlock { Text = "multiply" } },
                new(){ Content = new TextBlock { Text = "divide" } },
                new(){ Content = new TextBlock { Text = "modulus" } },
                new(){ Content = new TextBlock { Text = "and" } },
                new(){ Content = new TextBlock { Text = "or" } },
                new(){ Content = new TextBlock { Text = "()" } }

            };

            var uiSubMenu = new List<RadialMenuItem>
            {
                new() { Content = new TextBlock { Text = "Sub Item 1" } },
                new() { Content = new TextBlock { Text = "Sub Item 2" } },
                new() { Content = new TextBlock { Text = "Sub Item 3" } }
            };

            _menuCollection.Add("ThreadSubMenu", threadSubMenu);
            _menuCollection.Add("TestSubMenu", testSubMenu);
            _menuCollection.Add("CodeSubMenu", codeSubMenu);
            _menuCollection.Add("UISubMenu", uiSubMenu);
            _menuCollection.Add("MainMenuItems", mainMenuItems);

            // Go to ThreadSubMenu
            mainMenuItems[0].Click += (sender, e) =>
                RadialDialControl_Click(sender, e, "ThreadSubMenu");
            // Go to TestSubMenu
            mainMenuItems[1].Click += (sender, e) =>
                RadialDialControl_Click(sender, e, "TestSubMenu");
            // Go to CodeSubMenu
            mainMenuItems[2].Click += (sender, e) =>
                RadialDialControl_Click(sender, e, "CodeSubMenu");
            // Go to UISubMenu
            mainMenuItems[3].Click += (sender, e) =>
                RadialDialControl_Click(sender, e, "UISubMenu");
            // Back to Home on center item
            MainMenu.CentralItem.Click += (sender, e) =>
                RadialDialControl_Click(sender, e, "MainMenuItems");


            mainMenuItems[0].MouseEnter += (sender, e) =>
                RadialDialControl_Hover(sender, e, "ThreadSubMenu");  // handles the mouse hover action, display corresponding preview 

            mainMenuItems[0].MouseLeave += (sender, e) =>  // handles the mouse left hover action(when mouse leaves a button), clear the preview area
                RadialDialControl_ExitHover(sender, e);
        }

        private void RadialDialControl_Click(object sender, RoutedEventArgs e, String subMenu)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(100);
                Console.WriteLine("sssss");
                MainMenu.Items = _menuCollection[subMenu];
            }
            ).FireAndForget();
        }

        private void RadialDialControl_Hover(object sender, RoutedEventArgs e, String PreviewName)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(100);
                // take the name of the element and pull png file from json.
            }
            ).FireAndForget();
        }

        private void RadialDialControl_ExitHover(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(100);
                // clear preview area
            }
            ).FireAndForget();
        }
    }
}
