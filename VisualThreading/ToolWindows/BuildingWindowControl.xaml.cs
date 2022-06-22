using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl : UserControl
    {

        private Schema.Command currentCommand;

        public BuildingWindowControl(Schema.Schema commands, string fileExt)
        {
            InitializeComponent();
            ShowCodeButton.IsEnabled = false;
            currentCommand = null;
            Browser.NavigateToString(System.IO.File.ReadAllText("../../Resources/html/blocklyHTML.html"));
        }

        private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
        {
            var result = Browser.InvokeScript("showCode", new object[] { });
            System.Windows.MessageBox.Show(result.ToString());
        }

        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ShowCodeButton.IsEnabled = true;
            var toolboxXML = System.IO.File.ReadAllText("../../Resources/xml/blocklyToolbox.xml");
            var workspaceXML = System.IO.File.ReadAllText("../../Resources/xml/blocklyWorkspace.xml");
            //Initialize blocky using toolbox and workspace
            Browser.InvokeScript("init", new object[] { toolboxXML, workspaceXML });
        }

        public void SetCurrentCommand(Schema.Command c)
        {
            // Color: #FF00FFFF
            // Parent: Loop
            // Preview: for ( statement 1; statement 2; statement 3 ){\n  statements;\n}
            // Text: for
            this.currentCommand = c;
            var color = c.Color;
            var parent = c.Parent;
            var preview = c.Preview;
            var text = c.Text;

            Browser.InvokeScript("addNewBlockToArea", new object[] { parent, text, color });

        }
    }
}