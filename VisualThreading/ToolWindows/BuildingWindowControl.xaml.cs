using CefSharp;
using System.IO;
using System.Windows;
using Command = VisualThreading.Schema.Command;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl
    {
        private Command currentCommand;
        private string language;
        private readonly string _workspace;
        private readonly string _toolbox;
        private readonly Schema.Schema _schema;

        public BuildingWindowControl(Schema.Schema schema, string fileExt, string blockly, string toolbox, string workspace)
        {
            currentCommand = null;
            _toolbox = toolbox;
            _workspace = workspace;
            _schema = schema;
            //_schema = JsonConvert.DeserializeObject(schema);

            InitializeComponent();
            Focus();
            Browser.LoadHtml(blockly);

            Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
        }

        private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
                return;

            var root = Path.GetDirectoryName(typeof(VisualStudioServices).Assembly.Location);
            var blockly = Path.Combine(root!, "Resources", "js", "blockly");
            var fr = new FileReader();

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "blockly_compressed.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "blocks_compressed.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "msg", "js", "en.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp", "colour.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp", "lists.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp", "logic.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp", "loops.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp", "math.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp", "procedures.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp", "text.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "generators", "csharp", "variables.js")));

                //Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "javascript_compressed.js")));
                //Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "dart_compressed.js")));
                //Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "python_compressed.js")));
                //Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "lua_compressed.js")));
                //Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "php_compressed")));

                await Browser.EvaluateScriptAsync("init", _toolbox, _workspace, false);
            }).FireAndForget();
        }

        private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                //var result = await Browser.EvaluateScriptAsync(
                //    "showCode", new object[] { });

                //MessageBox.Show((string)result.Result);
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

            //var blocks = _schema["RadialMenu"][0]["commands"];
            // todo: fix hardcoded "0" - this should be dynamic based on filetype
            var blocks = _schema.RadialMenu[0].Commands;
            var blockType = "";
            foreach (var block in blocks)
            {
                if (block.Parent == parent && block.Text == text)
                {
                    blockType = block.Type;
                }
            }

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Browser.EvaluateScriptAsync("addNewBlockToArea", blockType, color);
            }).FireAndForget();
        }
    }
}