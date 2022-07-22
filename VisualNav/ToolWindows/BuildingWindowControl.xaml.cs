using CefSharp;
using System.Windows;
using VisualNav.Utilities;
using Command = VisualNav.Schema.Command;

namespace VisualNav.ToolWindows;

public partial class BuildingWindowControl
{
    private readonly BlocklyAdapter _blockly;

    public BuildingWindowControl()
    {
        InitializeComponent();
        Focus();
        _blockly = new BlocklyAdapter(Browser, false);
        ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.LoadHtmlAsync(); }).FireAndForget();

        Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
    }

    private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
    {
        if (!e.IsLoading)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.InitAsync(); }).FireAndForget();
        }
    }

    private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            var ret = await _blockly.ShowCodeAsync();

            if (ret.Success != true)
            {
                await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 1500);
            }
            else
            {
                await VS.MessageBox.ShowAsync((string)ret.Result);
            }
        }).FireAndForget();
    }

    private void InsertCodeButton_Click(object sender, RoutedEventArgs e)
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            var ret = await _blockly.ShowCodeAsync();

            if (ret.Success != true)
            {
                await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 1500);
            }
            else
            {
                var re = (string)ret.Result;
                var docView = await VS.Documents.GetActiveDocumentViewAsync();
                if (docView?.TextView == null) return;
                var position = docView.TextView.Caret.Position.BufferPosition;
                docView.TextBuffer?.Insert(position, re);
            }
        }).FireAndForget();
    }

    private void ClipboardButton_Click(object sender, RoutedEventArgs e)
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            var ret = await _blockly.ShowCodeAsync();

            if (ret.Success != true)
            {
                await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 1500);
            }
            else
            {
                Clipboard.SetText((string)ret.Result);
                await InfoNotificationWrapper.ShowSimpleAsync("Copied to clipboard.", "Copy", PackageGuids.BuildingWindowString, 1500);
            }
        }).FireAndForget();
    }

    public void SetCurrentCommand(Command c)
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            var ret = await _blockly.AddNewBlockToAreaAsync(c);
            if (ret.Success != true)
            {
                await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 1500);
            }
        }).FireAndForget();
    }
}