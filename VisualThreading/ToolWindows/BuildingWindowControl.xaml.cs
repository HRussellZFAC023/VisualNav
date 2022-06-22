using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl : UserControl
    {
        public BuildingWindowControl(Schema.Schema commands, string fileExt)
        {
            InitializeComponent();
            ShowCodeButton.IsEnabled = false;

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
            // TODO
        }
    }
}