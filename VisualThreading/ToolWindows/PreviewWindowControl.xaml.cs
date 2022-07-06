using CefSharp;
using VisualThreading.Schema;
using VisualThreading.Utilities;
using Label = System.Windows.Controls.Label;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

namespace VisualThreading.ToolWindows
{
    public partial class PreviewWindowControl
    {
        private Command _currentCommand;
        private readonly BlocklyAdapter _blockly;

        public PreviewWindowControl()
        {
            _currentCommand = null;
            InitializeComponent();
            Focus();

            _blockly = new BlocklyAdapter(Browser, true);
            ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.LoadHtmlAsync(); }).FireAndForget();
            Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged; // get file type
        }

        private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.InitAsync(); }).FireAndForget();
            }
        }

        private void SelectionEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        { UpdateCommands(); }

        public void SetCurrentCommand(Command c)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await _blockly.ClearAsync();
                await _blockly.AddNewBlockToAreaAsync(c);
            }).FireAndForget();

            UpdateCommands();
        }

        private void UpdateCommands()
        {
            Widgets.Children.Clear();
            Widgets.Children.Add(_currentCommand != null
                ? new Label { Content = LanguageMediator.GetCurrentActiveFileExtension() + " - " + _currentCommand.Text }
                : new Label { Content = LanguageMediator.GetCurrentActiveFileExtension() });
        }

        public void ClearCurrentCommand()
        {
            _currentCommand = null;
            ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.ClearAsync(); }).FireAndForget();
            UpdateCommands();
        }
    }
}