using CefSharp;
using System.IO;
using System.Windows;
using VisualThreading.Schema;
using VisualThreading.Utilities;
using Command = VisualThreading.Schema.Command;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl
    {
        private Command currentCommand;
        private string _currentLanguage;
        private readonly string _workspace;
        private readonly string _toolbox;
        private readonly Schema.Schema _schema;

        public BuildingWindowControl(Schema.Schema schema, string fileExt, string blockly, string toolbox, string workspace)
        {
            currentCommand = null;
            _toolbox = toolbox;
            _workspace = workspace;
            _schema = schema;
            _currentLanguage = fileExt;

            InitializeComponent();
            Focus();
            Browser.LoadHtml(blockly);

            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged;
            Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
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

                // import code generator
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "csharp_compressed.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "javascript_compressed.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "python_compressed.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "php_compressed.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "dart_compressed.js")));
                Browser.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(blockly, "lua_compressed.js")));


                await Browser.EvaluateScriptAsync("init", _toolbox, _workspace, false);

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
            _currentLanguage = fileExt;
        }

        private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var result = await Browser.EvaluateScriptAsync(
                    "showCode", _currentLanguage);

                if (result.Success != true)
                {
                    System.Windows.MessageBox.Show(result.Message);

                }
                else
                {
                    System.Windows.MessageBox.Show((string)result.Result);

                }

            }).FireAndForget();
        }

        private void InsertCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {

                var result = await Browser.EvaluateScriptAsync(
                    "showCode", _currentLanguage);

                if (result.Success != true)
                {
                    System.Windows.MessageBox.Show(result.Message);

                }
                else
                {
                    var re = "";

                    re = (string)result.Result;
                    DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
                    if (docView?.TextView == null) return;
                    Microsoft.VisualStudio.Text.SnapshotPoint position = docView.TextView.Caret.Position.BufferPosition;
                    docView.TextBuffer?.Insert(position, re);
                }


            }).FireAndForget();
        }

        private void ClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var result = await Browser.EvaluateScriptAsync(
                    "showCode", _currentLanguage);

                if (result.Success != true)
                {
                    System.Windows.MessageBox.Show(result.Message);

                }
                else
                {
                    var re = "";
                    re = (string)result.Result;
                    Clipboard.SetText(re);

                    System.Windows.MessageBox.Show("Copy Successfully!");
                }
            }).FireAndForget();
        }

        public void SetCurrentCommand(Command c)
        {
            currentCommand = c;
            var color = c.Color;
            var parent = c.Parent;
            var text = c.Text;
            var type = c.Type;

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var ret = await Browser.EvaluateScriptAsync("addNewBlockToArea", type);
                if (ret.Success != true)
                {
                    System.Windows.MessageBox.Show(ret.Message);
                }
            }).FireAndForget();


        }
    }
}