using System.IO;
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

        private void CanvasLabel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DraggedItem == null) return;
            DraggedItem = null;
            canvasLabels.ReleaseMouseCapture();
            e.Handled = true;
        }

        public void SetCurrentCommand(Schema.Command c)
        {

            var tb = CodeBlockFactory.CodeBlock(c);
            tb.MouseLeftButtonDown += Label_MouseLeftButtonDown;
            Canvas.SetLeft(tb, 0);
            Canvas.SetTop(tb, 0);
            canvasLabels.Children.Add(tb);

        }
    }
}
