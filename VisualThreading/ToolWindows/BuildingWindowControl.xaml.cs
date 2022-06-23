using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl : UserControl
    {
        private Schema.Command currentCommand;
        private readonly string _workspace;
        private readonly string _toolbox;

        public BuildingWindowControl(Schema.Schema commands, string fileExt, string blockly, string toolbox, string workspace)
        {
            InitializeComponent();
            ShowCodeButton.IsEnabled = false;
            currentCommand = null;
            _toolbox = toolbox;
            _workspace = workspace;

            Browser.NavigateToString(blockly);
        }

        private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
        {
            var result = Browser.InvokeScript("showCode", new object[] { });
            System.Windows.MessageBox.Show(result.ToString());
        }

        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ShowCodeButton.IsEnabled = true;
            Browser.InvokeScript("init", _toolbox, _workspace);  //Initialize blocky using toolbox and workspace
        }

        public void SetCurrentCommand(Schema.Command c)
        {
            // Color: #FF00FFFF
            // Parent: Loop
            // Preview: for ( statement 1; statement 2; statement 3 ){\n  statements;\n}
            // Text: for
            currentCommand = c;
            var color = c.Color;
            var parent = c.Parent;
            var preview = c.Preview;
            var text = c.Text;

            Browser.InvokeScript("addNewBlockToArea", parent, text, color);
        }
    }
}