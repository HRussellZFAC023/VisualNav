using CefSharp;
using System.Windows;
using VisualNav.Utilities;
using Command = VisualNav.Schema.Command;

namespace VisualNav.ToolWindows;

public partial class BuildingWindowControl
{
    public readonly BlocklyAdapter _blockly;

    public BuildingWindowControl()
    {
        InitializeComponent();
        Focus();
        _blockly = new BlocklyAdapter(Browser, false);
        ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.LoadHtmlAsync(); }).FireAndForget();

        Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
        _blockly.CenterAsync().FireAndForget();
        SizeChanged += (_, _) => _blockly.CenterAsync().FireAndForget();
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

    public void InsertCodeButton_Click(object sender, RoutedEventArgs e)
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
                String re = (string)ret.Result;
                var docView = await VS.Documents.GetActiveDocumentViewAsync();
                if (docView?.TextView == null) return;
                int position = docView.TextView.Caret.Position.BufferPosition;
                int spaceNum = docView.TextView.Caret.Position.VirtualSpaces;

                int space_num = docView.TextView.Caret.Position.VirtualSpaces;

                String spaces = "";
                String new_res = "";
                for (int i = 0; i < space_num; i++)
                {
                    spaces += ' ';
                }

                new_res += spaces;
                foreach (char c in re)
                {
                    if (c == '\n')
                    {
                        new_res += c;
                        new_res += spaces;
                    }
                    else
                    {
                        new_res += c;
                    }
                }

                docView.TextBuffer?.Insert(position, new_res);

                // another implementation?
                //EnvDTE.DTE myDTE = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                //myDTE.ExecuteCommand("Edit.FormatDocument", string.Empty);

            }
        }).FireAndForget();
    }

    public void ClipboardButton_Click(object sender, RoutedEventArgs e)
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
            await _blockly.AddCustomBlockToAreaAsync(c);
            var ret = await _blockly.AddNewBlockToAreaAsync(c);
            if (ret.Success != true)
            {
                await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 1500);
            }

        }).FireAndForget();
    }
}