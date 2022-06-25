using CefSharp;
using System.Windows;
using Command = VisualThreading.Schema.Command;
using MessageBox = System.Windows.MessageBox;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl
    {
        private Command currentCommand;
        private readonly string _workspace;
        private readonly string _toolbox;

        public BuildingWindowControl(Schema.Schema commands, string fileExt, string blockly, string toolbox, string workspace)
        {
            currentCommand = null;
            _toolbox = toolbox;
            _workspace = workspace;

            InitializeComponent();
            Focus();
            Browser.LoadHtml(blockly);

            Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
        }

        private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
                return;
            
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Browser.EvaluateScriptAsync("init", _toolbox, _workspace);
            }).FireAndForget();
        }

        private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var result = await Browser.EvaluateScriptAsync(
                    "showCode", new object[] { });

                MessageBox.Show(result.Message);
            }).FireAndForget();
        }

        public void SetCurrentCommand(Command c)
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

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Browser.EvaluateScriptAsync("addNewBlockToArea", parent, text, color);
            }).FireAndForget();
        }
    }
}