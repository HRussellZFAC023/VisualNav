using RadialMenu.Controls;
using System.Collections;
using System.Collections.Generic;
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
            mainMenuItems[0].Click += (_, _) => RadialDialControl_Click("ThreadSubMenu"); // Go to ThreadSubMenu
            mainMenuItems[1].Click += (_, _) => RadialDialControl_Click("TestSubMenu"); // Go to TestSubMenu
            mainMenuItems[2].Click += (_, _) => RadialDialControl_Click("CodeSubMenu"); // Go to CodeSubMenu
            mainMenuItems[3].Click += (_, _) => RadialDialControl_Click("UISubMenu"); // Go to UISubMenu

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
            threadSubMenu[0].Click += (_, _) => RadialDialElement_Click("New Thread");
            threadSubMenu[0].MouseEnter += (_, _) => RadialDialElement_Hover("New Thread");  // handles the mouse hover action, display corresponding preview
            threadSubMenu[0].MouseLeave += (_, _) => RadialDialElement_ExitHover(); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            // -------------------------------------------------------------------------------------------------------------

            // -----------------------------Test sub menu-------------------------------------------------------------------
            testSubMenu[0].Click += (_, _) => RadialDialElement_Click("Assert");
            testSubMenu[0].MouseEnter += (_, _) => RadialDialElement_Hover("Assert");  // handles the mouse hover action, display corresponding preview
            testSubMenu[0].MouseLeave += (_, _) => RadialDialElement_ExitHover(); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            // -------------------------------------------------------------------------------------------------------------

            // -----------------------------Test sub menu-------------------------------------------------------------------
            codeSubMenu[0].Click += (_, _) => RadialDialControl_Click("CodeSubMenu");
            codeSubMenu[0].MouseEnter += (_, _) => RadialDialElement_Hover("CodeSubMenu");
            codeSubMenu[0].MouseLeave += (_, _) => RadialDialElement_ExitHover();

            var ioSubMenu = new List<RadialMenuItem>
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
            conditionKeywordSubMenu[0].Click += (_, _) => RadialDialElement_Click("If");
            conditionKeywordSubMenu[0].MouseEnter += (_, _) => RadialDialElement_Hover("If");  // handles the mouse hover action, display corresponding preview
            conditionKeywordSubMenu[0].MouseLeave += (_, _) => RadialDialElement_ExitHover(); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            conditionKeywordSubMenu[1].Click += (_, _) => RadialDialElement_Click("Else");
            conditionKeywordSubMenu[1].MouseEnter += (_, _) => RadialDialElement_Hover("Else");  // handles the mouse hover action, display corresponding preview
            conditionKeywordSubMenu[1].MouseLeave += (_, _) => RadialDialElement_ExitHover(); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            conditionKeywordSubMenu[2].Click += (_, _) => RadialDialElement_Click("ElseIf");
            conditionKeywordSubMenu[2].MouseEnter += (_, _) => RadialDialElement_Hover("ElseIf");  // handles the mouse hover action, display corresponding preview
            conditionKeywordSubMenu[2].MouseLeave += (_, _) => RadialDialElement_ExitHover(); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            conditionKeywordSubMenu[3].Click += (_, _) => RadialDialElement_Click("Switch");
            conditionKeywordSubMenu[3].MouseEnter += (_, _) => RadialDialElement_Hover("Switch");  // handles the mouse hover action, display corresponding preview
            conditionKeywordSubMenu[3].MouseLeave += (_, _) => RadialDialElement_ExitHover(); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            conditionKeywordSubMenu[4].Click += (_, _) => RadialDialElement_Click("Case");
            conditionKeywordSubMenu[4].MouseEnter += (_, _) => RadialDialElement_Hover("Case");  // handles the mouse hover action, display corresponding preview
            conditionKeywordSubMenu[4].MouseLeave += (_, _) => RadialDialElement_ExitHover(); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
                                                                                              // -------------------------------------------------------------------------------------------------------------

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
            _menuCollection.Add("IOSubMenu", ioSubMenu);
            _menuCollection.Add("methodKeywordSubMenu", methodKeywordSubMenu);
            _menuCollection.Add("conditionKeywordSubMenu", conditionKeywordSubMenu);
            _menuCollection.Add("loopSubMenu", loopSubMenu);
            _menuCollection.Add("variableSubMenu", variableSubMenu);
            _menuCollection.Add("operatorSubMenu", operatorSubMenu);
            _menuCollection.Add("comparatorMenu", comparatorMenu);

            codeSubMenu[0].Click += (_, _) => RadialDialControl_Click("IOSubMenu"); // Go to Input/Ouput
            codeSubMenu[1].Click += (_, _) => RadialDialControl_Click("methodKeywordSubMenu"); // Go to Keyword/Decorator
            codeSubMenu[2].Click += (_, _) => RadialDialControl_Click("conditionKeywordSubMenu");// Go to Keyword/Decorator
            codeSubMenu[3].Click += (_, _) => RadialDialControl_Click("loopSubMenu");// Go to Loop
            codeSubMenu[4].Click += (_, _) => RadialDialControl_Click("variableSubMenu");// Go to Variables
            codeSubMenu[5].Click += (_, _) => RadialDialControl_Click("operatorSubMenu");// Go to Operator
            codeSubMenu[6].Click += (_, _) => RadialDialControl_Click("comparatorMenu");// Go to Comparator
            // -------------------------------------------------------------------------------------------------------------

            // -----------------------------UI sub menu---------------------------------------------------------------------
            uiSubMenu[0].Click += (_, _) => RadialDialElement_Click("UISubMenu");
            uiSubMenu[0].MouseEnter += (_, _) => RadialDialElement_Hover("UISubMenu");  // handles the mouse hover action, display corresponding preview
            uiSubMenu[0].MouseLeave += (_, _) => RadialDialElement_ExitHover(); // handles the mouse left hover action(when mouse leaves a button), clear the preview area
            // -------------------------------------------------------------------------------------------------------------

            // Back to Home on center item
            MainMenu.CentralItem.Click += (_, _) => RadialDialControl_Back();
        }

        private void RadialDialElement_Click(string element) // handles the eventuall element like a veriable
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(20);
                /*BuildingWindow.Instance.SetCurrentCommand(element.ToLower());*/
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
                PreviewWindow.Instance.SetCurrentCommand(previewName.ToLower());
                //BuildingWindow.Instance.SetCurrentCommand(PreviewName.ToLower());
            }
            ).FireAndForget();
        }

        private void RadialDialElement_ExitHover()  // handles the eventuall element like a veriable
        {
            // clear preview area
            PreviewWindow.Instance.SetCurrentCommand("");
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

                MainMenu.Items = _menuCollection[temp];
            }
            ).FireAndForget();
        }
    }
}