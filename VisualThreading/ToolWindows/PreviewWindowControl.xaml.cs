using CefSharp;
using System.IO;
using VisualThreading.Schema;
using VisualThreading.Utilities;
using Label = System.Windows.Controls.Label;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace VisualThreading.ToolWindows
{
    public partial class PreviewWindowControl : UserControl
    {
        private Schema.Command _currentCommand;
        private string _currentLanguage; // file extension for language
        private readonly string _toolbox;
        private readonly string _workspace;

        //private readonly dynamic _schema;
        private readonly Schema.Schema _schema;

        public PreviewWindowControl(Schema.Schema schema, string fileExt, string blockly, string toolbox, string workspace)
        {
            _currentCommand = null;
            _currentLanguage = fileExt;
            _toolbox = toolbox;
            _workspace = workspace;
            InitializeComponent();
            Focus();
            _schema = schema;

            Browser.LoadHtml(blockly);

            Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged; // get file type
        }

        private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
                return;

            var root = Path.GetDirectoryName(typeof(VisualStudioServices).Assembly.Location);
            var blockly = Path.Combine(root!, "Resources", "js", "blockly");
            var fr = new FileReaderAdapter();

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

                await Browser.EvaluateScriptAsync("init", _toolbox, _workspace, true);
            }).FireAndForget();
        }

        private void SelectionEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileExt = "";
            if (e.To != null)
            {
                var buffer = e.To.Name;
                fileExt =
                        Path.GetExtension(buffer);
            }
            SetCurrentLanguage(fileExt);
            UpdateCommands();
        }

        public void SetCurrentCommand(Command c)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                //await VS.MessageBox.ShowAsync("wow 1");
                await Browser.GetMainFrame().EvaluateScriptAsync("Blockly.mainWorkspace.clear()");
                //await VS.MessageBox.ShowAsync("wow 2");
            }).FireAndForget();

            // Color:
            // Parent: Logic
            // Preview:
            // Text: controls_if
            // System.Diagnostics.Debug.WriteLine(c);

            _currentCommand = c;
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

            UpdateCommands();
        }

        private void SetCurrentLanguage(string language)
        {
            _currentLanguage = language;
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            Widgets.Children.Clear();

            Widgets.Children.Add(_currentCommand != null
                ? new Label { Content = _currentLanguage + " - " + _currentCommand.Text }
                : new Label { Content = _currentLanguage });
        }

        public void ClearCurrentCommand()
        {
            _currentCommand = null;
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                //await VS.MessageBox.ShowAsync("wow 1");
                await Browser.GetMainFrame().EvaluateScriptAsync("Blockly.mainWorkspace.clear()");
                //await VS.MessageBox.ShowAsync("wow 2");
            }).FireAndForget();

            UpdateCommands();
        }
    }
}