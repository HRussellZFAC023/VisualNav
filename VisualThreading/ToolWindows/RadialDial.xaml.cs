using System.Collections.Generic;
using System.Windows.Controls;
using RadialMenu.Controls;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using RadialMenu.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace VisualThreading
{
    public partial class RadialDialControl : UserControl
    {
        Dictionary<string, List<RadialMenuItem>> menuCollection = new Dictionary<string, List<RadialMenuItem>>();


        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public RadialDialControl()
        {
            InitializeComponent();

            var MainMenuItems = new List<RadialMenuItem>
            {
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Thread" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Test" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Code" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "UI" }
                }
            };

            // Set default menu to Main menu
            MainMenu.Items = MainMenuItems;

            var ThreadSubMenu = new List<RadialMenuItem>
            {
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "New Thread " }
                }
            };

            var TestSubMenu = new List<RadialMenuItem>
            {
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Assert" }
                }
            };

            var CodeSubMenu = new List<RadialMenuItem>
            {
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "I/O" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Method/Keyword" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Condition" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Loop" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Variable" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Operator" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Comparator" }
                }
            };

            var UISubMenu = new List<RadialMenuItem>
            {
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Sub Item 1" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Sub Item 2" }
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Sub Item 3" }
                }
            };

            menuCollection.Add("ThreadSubMenu", ThreadSubMenu);
            menuCollection.Add("TestSubMenu", TestSubMenu);
            menuCollection.Add("CodeSubMenu", CodeSubMenu);
            menuCollection.Add("UISubMenu", UISubMenu);
            menuCollection.Add("MainMenuItems", MainMenuItems);


            // Go to ThreadSubMenu
            MainMenuItems[0].Click += (sender, e) => RadialDialControl_Click(sender, e, "ThreadSubMenu");

            // Go to TestSubMenu
            MainMenuItems[1].Click += (sender, e) => RadialDialControl_Click(sender, e, "TestSubMenu");

            // Go to CodeSubMenu
            MainMenuItems[2].Click += (sender, e) => RadialDialControl_Click(sender, e, "CodeSubMenu");

            // Go to UISubMenu
            MainMenuItems[3].Click += (sender, e) => RadialDialControl_Click(sender, e, "UISubMenu");

            // Back to Home on center item
            MainMenu.CentralItem.Click += (sender, e) => RadialDialControl_Click(sender, e, "MainMenuItems");

        }
        private void RadialDialControl_Click(object sender, RoutedEventArgs e, String subMenu)
                    {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(100);
                MainMenu.Items = menuCollection[subMenu];
            }
            ).FireAndForget();
        }
    }
}
