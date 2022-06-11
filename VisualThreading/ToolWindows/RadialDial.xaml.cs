using System.Collections.Generic;
using System.Windows.Controls;
using RadialMenu.Controls;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows;

namespace VisualThreading
{
    public partial class RadialDialControl : UserControl
    {
        

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool isOpen = false;
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
                RaisePropertyChanged();
            }
        }

        public RadialDialControl()
        {
            InitializeComponent();

            var MainMenuItems = new List<RadialMenuItem>
            {
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Thread" },
                    ArrowBackground = Brushes.Transparent
                },
                new RadialMenuItem
                {
                    Content = new TextBlock { Text = "Test" },
                    ArrowBackground = Brushes.Transparent
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

            // Go to ThreadSubMenu
            MainMenuItems[0].Click += async (sender, args) =>
            {
                isOpen = false;
                await Task.Delay(400);
                MainMenu.Items = ThreadSubMenu;
                isOpen = true;
            };
            
            // Go to TestSubMenu
            MainMenuItems[1].Click += async (sender, args) =>
            {
                isOpen = false;
                await Task.Delay(400);
                MainMenu.Items = TestSubMenu;
                isOpen = true;
            };

            // Go to CodeSubMenu
            MainMenuItems[2].Click += async (sender, args) =>
            {
                isOpen = false;
                await Task.Delay(400);
                MainMenu.Items = CodeSubMenu;
                isOpen = true;
            };

            // Go to UISubMenu
            MainMenuItems[3].Click += async (sender, args) =>
            {
                isOpen = false;
                await Task.Delay(400);
                MainMenu.Items = UISubMenu;
                isOpen = true;
            };
        }

}