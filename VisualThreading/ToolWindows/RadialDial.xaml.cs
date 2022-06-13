using RadialMenu.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
                new() { Content = new TextBlock { Text = "Test" }, Background = Brushes.AliceBlue, ArrowBackground = Brushes.Firebrick}, //  "Background" take Brushes.color
                new() { Content = new TextBlock { Text = "Test" }, Background = Brushes.AliceBlue, ArrowBackground = Brushes.Firebrick},
                new() { Content = new TextBlock { Text = "Code" }, Background = Brushes.AliceBlue, ArrowBackground = Brushes.Firebrick},
                new() { Content = new TextBlock { Text = "UI" }, Background = Brushes.AliceBlue, ArrowBackground = Brushes.Firebrick}
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
        }

        private void RadialDialControl_Click(object sender, RoutedEventArgs e, String subMenu)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(100);
                MainMenu.Items = _menuCollection[subMenu];
            }
            ).FireAndForget();
        }
    }
}
