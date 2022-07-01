using CefSharp;
using System.IO;
using System.Windows.Controls;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

namespace VisualThreading.ToolWindows
{
    public partial class PreviewWindowControl : UserControl
    {
        private readonly Schema.Schema _commands;
        private Schema.Command _currentCommand;
        private string _currentLanguage; // file extension for language
        private readonly string _toolbox;
        private readonly string _workspace;

        public PreviewWindowControl(Schema.Schema commands, string fileExt, string blockly, string toolbox, string workspace)
        {
            _commands = commands;
            _currentCommand = null;
            _currentLanguage = fileExt;
            _toolbox = toolbox;
            _workspace = workspace;
            InitializeComponent();
            Focus();

            Browser.LoadHtml(blockly);

            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged; // extends the selection even
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
        }

        public void SetCurrentCommand(Schema.Command c)
        {
            _currentCommand = c;
            UpdateCommands();

            var color = c.Color;
            var parent = c.Parent;
            var preview = c.Preview;
            var text = c.Text;

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Browser.EvaluateScriptAsync("addNewBlockToArea", parent, text, color);
            }).FireAndForget();
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
                ? new Label { Content = _currentLanguage + _currentCommand.Text }
                : new Label { Content = _currentLanguage });
        }

        public void ClearCurrentCommand()
        {
            _currentCommand = null;
            UpdateCommands();
        }
    }
}
