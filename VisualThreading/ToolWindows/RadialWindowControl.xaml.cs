using RadialMenu.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections;
using static System.Windows.Forms.AxHost;

namespace VisualThreading.ToolWindows
{
    public partial class RadialWindowControl
    {
        private readonly Dictionary<string, List<RadialMenuItem>> _menuCollection = new();
        private Stack state = new Stack();
        private String currentState = "";
        public RadialWindowControl()
        {
            InitializeComponent();
            
            // comment out code use for icon displaying
            // StackPanel threadPanel = new StackPanel();
            // threadPanel.Children.Add(new Image { Source = new BitmapImage(new Uri("D:/visualthreading/VisualThreading/Resources/Icon.png", UriKind.RelativeOrAbsolute)) });
            // threadPanel.Children.Add(new TextBlock { Text = "Thread" });
            var mainMenuItems = new List<RadialMenuItem>
            {
                // I the know the color looks bad
                // Background controls the Fan-shaped area
                // ArrowBackground controls the littel arrow on each button
                // EdgeBackground controls the Edge of the Fan-shaped 
                // new() { Content = threadPanel},
                new() { Content = new TextBlock { Text = "Thread" }},
                new() { Content = new TextBlock { Text = "Test" }},
                new() { Content = new TextBlock { Text = "Code" }},
                new() { Content = new TextBlock { Text = "UI" }}
            };
            _menuCollection.Add("MainMenuItems", mainMenuItems);
            mainMenuItems[0].Click += (sender, e) => RadialDialControl_Click(sender, e, "ThreadSubMenu"); // Go to ThreadSubMenu
            mainMenuItems[1].Click += (sender, e) => RadialDialControl_Click(sender, e, "TestSubMenu"); // Go to TestSubMenu
            mainMenuItems[2].Click += (sender, e) => RadialDialControl_Click(sender, e, "CodeSubMenu"); // Go to CodeSubMenu
            mainMenuItems[3].Click += (sender, e) => RadialDialControl_Click(sender, e, "UISubMenu"); // Go to UISubMenu

            // Set default menu to Main menu
            MainMenu.Items = mainMenuItems;

            // Layer 1 sub menu
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

            // -----------------------------ThreadSubMenu-------------------------------------------------------------------
            threadSubMenu[0].Click += (sender, e) => RadialDialElement_Click(sender, e, "New Thread");
            threadSubMenu[0].MouseEnter += (sender, e) => RadialDialElement_Hover(sender, e, "New Thread");  // handles the mouse hover action, display corresponding preview 
            threadSubMenu[0].MouseLeave += (sender, e) => RadialDialElement_ExitHover(sender, e); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            // -------------------------------------------------------------------------------------------------------------


            // -----------------------------Test sub menu-------------------------------------------------------------------
            testSubMenu[0].Click += (sender, e) => RadialDialElement_Click(sender, e, "Assert");
            testSubMenu[0].MouseEnter += (sender, e) => RadialDialElement_Hover(sender, e, "Assert");  // handles the mouse hover action, display corresponding preview 
            testSubMenu[0].MouseLeave += (sender, e) => RadialDialElement_ExitHover(sender, e); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            // -------------------------------------------------------------------------------------------------------------


            // -----------------------------Test sub menu-------------------------------------------------------------------
            codeSubMenu[0].Click += (sender, e) => RadialDialControl_Click(sender, e, "CodeSubMenu");
            var IOSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "Print" } },
                new(){ Content = new TextBlock { Text = "Write" } },
                new(){ Content = new TextBlock { Text = "ReadKey" } },
                new(){ Content = new TextBlock { Text = "UserInput" } },
                new(){ Content = new TextBlock { Text = "UserInputLine" } },
                new(){ Content = new TextBlock { Text = "OpenText" } },
                new(){ Content = new TextBlock { Text = "WriteToFile" } }
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
            var conditionKeywordSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "If" } },
                new(){ Content = new TextBlock { Text = "Else" } },
                new(){ Content = new TextBlock { Text = "ElseIf" } },
                new(){ Content = new TextBlock { Text = "Switch" } },
                new(){ Content = new TextBlock { Text = "Case" } },
            };
            var loopSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "Break" } },
                new(){ Content = new TextBlock { Text = "Continue" } },
                new(){ Content = new TextBlock { Text = "For" } },
                new(){ Content = new TextBlock { Text = "While" } }
            };
            var variableSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "Void" } },
                new(){ Content = new TextBlock { Text = "Integer" } },
                new(){ Content = new TextBlock { Text = "Double" } },
                new(){ Content = new TextBlock { Text = "Character" }},
                new(){ Content = new TextBlock { Text = "Boolean" } },
                new(){ Content = new TextBlock { Text = "String" } },
                new(){ Content = new TextBlock { Text = "Array" } },
            };
            var operatorSubMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "Equal" } },
                new(){ Content = new TextBlock { Text = "Plus" } },
                new(){ Content = new TextBlock { Text = "Minus" } },
                new(){ Content = new TextBlock { Text = "Multiply" } },
                new(){ Content = new TextBlock { Text = "Divide" } },
                new(){ Content = new TextBlock { Text = "Modulus" } },
                new(){ Content = new TextBlock { Text = "And" } },
                new(){ Content = new TextBlock { Text = "Or" } },
                new(){ Content = new TextBlock { Text = "()" } }
            };
            var comparatorMenu = new List<RadialMenuItem>
            {
                new(){ Content = new TextBlock { Text = "&&" } },
                new(){ Content = new TextBlock { Text = "||" } },
                new(){ Content = new TextBlock { Text = "==" } },
                new(){ Content = new TextBlock { Text = ">" } },
                new(){ Content = new TextBlock { Text = "<" } },
                new(){ Content = new TextBlock { Text = ">=" } },
                new(){ Content = new TextBlock { Text = "<=" } },
                new(){ Content = new TextBlock { Text = "!=" } },
                new(){ Content = new TextBlock { Text = "()" } },
            };
            _menuCollection.Add("IOSubMenu", IOSubMenu);
            _menuCollection.Add("methodKeywordSubMenu", methodKeywordSubMenu);
            _menuCollection.Add("conditionKeywordSubMenu", conditionKeywordSubMenu);
            _menuCollection.Add("loopSubMenu", loopSubMenu);
            _menuCollection.Add("variableSubMenu", variableSubMenu);
            _menuCollection.Add("operatorSubMenu", operatorSubMenu);
            _menuCollection.Add("comparatorMenu", comparatorMenu);
            codeSubMenu[0].Click += (sender, e) => RadialDialControl_Click(sender, e, "IOSubMenu"); // Go to Input/Ouput
            codeSubMenu[1].Click += (sender, e) => RadialDialControl_Click(sender, e, "methodKeywordSubMenu"); // Go to Keyword/Decorator
            codeSubMenu[2].Click += (sender, e) => RadialDialControl_Click(sender, e, "conditionKeywordSubMenu");// Go to Keyword/Decorator
            codeSubMenu[3].Click += (sender, e) => RadialDialControl_Click(sender, e, "loopSubMenu");// Go to Loop
            codeSubMenu[4].Click += (sender, e) => RadialDialControl_Click(sender, e, "variableSubMenu");// Go to Variables
            codeSubMenu[5].Click += (sender, e) => RadialDialControl_Click(sender, e, "operatorSubMenu");// Go to Operator
            codeSubMenu[6].Click += (sender, e) => RadialDialControl_Click(sender, e, "comparatorMenu");// Go to Comparator
            // -------------------------------------------------------------------------------------------------------------


            // -----------------------------UI sub menu---------------------------------------------------------------------
            uiSubMenu[0].Click += (sender, e) => RadialDialElement_Click(sender, e, "UISubMenu");
            uiSubMenu[0].MouseEnter += (sender, e) => RadialDialElement_Hover(sender, e, "UISubMenu");  // handles the mouse hover action, display corresponding preview 
            uiSubMenu[0].MouseLeave += (sender, e) => RadialDialElement_ExitHover(sender, e); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            // -------------------------------------------------------------------------------------------------------------


            // Back to Home on center item
            MainMenu.CentralItem.Click += (sender, e) => RadialDialControl_Back(sender, e);
        }
        private void RadialDialElement_Click(object sender, RoutedEventArgs e, String element) // handles the eventuall element like a veriable 
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                // extract element to working area
                MainMenu.Items = _menuCollection[element];
            }
            ).FireAndForget();
        }

        private void RadialDialElement_Hover(object sender, RoutedEventArgs e, String PreviewName)  // handles the eventuall element like a veriable 
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                // take the name of the element and pull png file from json.
            }
            ).FireAndForget();
        }

        private void RadialDialElement_ExitHover(object sender, RoutedEventArgs e)  // handles the eventuall element like a veriable 
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                // clear preview area
            }
            ).FireAndForget();
        }

        private void RadialDialControl_Click(object sender, RoutedEventArgs e, String subMenu) // handles the subfolder element like loop
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                MainMenu.Items = _menuCollection[subMenu];
             
                if (state.Count == 0)
                {
                    state.Push("MainMenuItems");
                }
                else
                {
                    state.Push(currentState);
                }
                currentState = subMenu;

                // register();
            }
            ).FireAndForget();
        }

        public void register()
        {
            PreviewWindowControl.ClickCompleted += PreviewWindowControl_ClickCompleted; // register with an event
            PreviewWindowControl.notify();

            BuildingWindowControl.ClickCompleted += BuildingWindowControl_ClickCompleted; // register with an event
            BuildingWindowControl.notify();
        }

        private void RadialDialControl_Back(object sender, RoutedEventArgs e) // handles the subfolder element like loop
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                String temp = "";
                if (state.Count == 0)
                {
                    temp = "MainMenuItems";
                }
                else
                {
                    temp = state.Pop().ToString();
                }
                
                MainMenu.Items = _menuCollection[temp];
            }
            ).FireAndForget();
        }

        // event handler
        public static void bl_ProcessCompleted()
        {
            Console.WriteLine("Process Completed!");
        }
    }
}
