using CefSharp;
using Newtonsoft.Json;
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
        private readonly dynamic _schema;

        public BuildingWindowControl(Schema.Schema commands, string fileExt, string blockly, string toolbox, string workspace, string schema)
        {
            currentCommand = null;
            _toolbox = toolbox;
            _workspace = workspace;
            _schema = JsonConvert.DeserializeObject(schema);

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
                await Browser.EvaluateScriptAsync("init", _toolbox, _workspace, false);
            }).FireAndForget();
        }

        private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var result = await Browser.EvaluateScriptAsync(
                    "showCode", new object[] { });

                MessageBox.Show((string)result.Result);
            }).FireAndForget();
        }

        public void SetCurrentCommand(Command c)
        {
            // Color: 
            // Parent: Logic
            // Preview: 
            // Text: controls_if
            // System.Diagnostics.Debug.WriteLine(c);

            currentCommand = c;
            var color = c.Color;
            var parent = c.Parent;
            var text = c.Text;

            var blocks = _schema["RadialMenu"][0]["commands"];
            var blockType = "";
            foreach (var block in blocks)
            {
                if (block["parent"] == parent && block["text"] == text)
                {
                    blockType = block["type"];
                }
            }

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Browser.EvaluateScriptAsync("addNewBlockToArea", blockType, color);
            }).FireAndForget();
        }
    }
}