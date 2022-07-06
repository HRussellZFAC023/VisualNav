using CefSharp;
using System.Windows;
using VisualThreading.Utilities;
using Command = VisualThreading.Schema.Command;

namespace VisualThreading.ToolWindows;

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
            var result = await _blockly.ShowCodeAsync();

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
            var result = await _blockly.ShowCodeAsync();

            if (result.Success != true)
            {
                System.Windows.MessageBox.Show(result.Message);
            }
            else
            {
                var re = (string)result.Result;
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
            var result = await _blockly.ShowCodeAsync();

            if (result.Success != true)
            {
                System.Windows.MessageBox.Show(result.Message);
            }
            else
            {
                Clipboard.SetText((string)result.Result);
                System.Windows.MessageBox.Show("Copy Successfully!");
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
                System.Windows.MessageBox.Show(ret.Message);
            }
        }).FireAndForget();
    }
}