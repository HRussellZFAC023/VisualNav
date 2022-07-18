using CefSharp;
using System.Windows;
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
            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged;
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

        private void UpdateCommands()
        {
            Widgets.Children.Clear();
            Widgets.Children.Add(_currentCommand != null
                ? new Label { Content = LanguageMediator.GetCurrentActiveFileExtension() + " - " + _currentCommand.Text }
                : new Label { Content = LanguageMediator.GetCurrentActiveFileExtension() });
        }

        private static bool _hover;

        public void SetCurrentCommand(Command c)
        {
            PreviewText.Text = "";
            switch (_hover)
            {
                case false when c.Preview.Equals(""):
                    BrowserBorder.Visibility = Visibility.Visible;
                    TextBorder.Visibility = Visibility.Hidden;
                    ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                    {
                        await _blockly.ClearAsync();
                        await _blockly.AddNewBlockToAreaAsync(c);
                        _hover = true;
                    }).FireAndForget();
                    break;

                case false:
                    BrowserBorder.Visibility = Visibility.Hidden;
                    TextBorder.Visibility = Visibility.Visible;
                    PreviewText.Text = c.Preview;
                    PreviewText.Foreground = new System.Windows.Media.BrushConverter().ConvertFromString(c.Color) as System.Windows.Media.SolidColorBrush;
                    break;
            }

            _currentCommand = c;
            UpdateCommands();
        }

        public void ClearCurrentCommand()
        {
            PreviewText.Text = "";
            _hover = false;
            _currentCommand = null;
            ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.ClearAsync(); }).FireAndForget();
            UpdateCommands();
        }
    }
}
