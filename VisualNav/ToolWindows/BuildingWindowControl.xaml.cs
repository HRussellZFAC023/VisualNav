﻿using CefSharp;
using EnvDTE;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using VisualNav.Utilities;
using static System.Windows.Forms.DataFormats;
using Command = VisualNav.Schema.Command;

namespace VisualNav.ToolWindows;

public partial class BuildingWindowControl
{
    public readonly BlocklyAdapter Blockly;
    

    public BuildingWindowControl()
    {
        InitializeComponent();
        Focus();
        Blockly = new BlocklyAdapter(Browser, false);
        ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await Blockly.LoadHtmlAsync(); }).FireAndForget();

        Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
        //init the block size with setting
        Blockly.CenterAsync().FireAndForget();
        SizeChanged += (_, _) => Blockly.CenterAsync().FireAndForget();
    }

    private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
    {
        if (!e.IsLoading)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await Blockly.InitAsync(); }).FireAndForget();
        }
    }

    private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            var ret = await Blockly.ShowCodeAsync();

            if (!ret.Success)
            {
                await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 3000);
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
            var ret = await Blockly.ShowCodeAsync();

            if (!ret.Success)
            {
                await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 3000);
            }
            else
            {
                var re = (string)ret.Result;
                var docView = await VS.Documents.GetActiveDocumentViewAsync();
                if (docView?.TextView == null) return;
                var position = docView.TextView.Caret.Position.BufferPosition;
                var newRes = new StringBuilder();
              
                foreach (var c in re)
                {
                    newRes.Append(c);
                }

                docView.TextBuffer?.Insert(position, newRes.ToString());
              
            }

            await Task.Delay(100);
            Formatter.Format();
            await InfoNotificationWrapper.ShowSimpleAsync("Inserted into Document.", "InsertPage", PackageGuids.BuildingWindowString, 1500);


        }).FireAndForget();

    }

    public void ClipboardButton_Click(object sender, RoutedEventArgs e)
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            var ret = await Blockly.ShowCodeAsync();

            if (!ret.Success)
            {
                await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 3000);
            }
            else
            {
                Clipboard.SetText((string)ret.Result);
                await InfoNotificationWrapper.ShowSimpleAsync("Copied to clipboard.", "Copy", PackageGuids.BuildingWindowString, 1500);
            }
        }).FireAndForget();
    }
    public void Redo(object sender, RoutedEventArgs e)
    {
        Blockly.RedoAsync().FireAndForget();
    }

    public void Undo(object sender, RoutedEventArgs e)
    {
        Blockly.UndoAsync().FireAndForget();
    }

    public void DecreaseSize(object sender, RoutedEventArgs e)
    {
        Blockly.ZoomOutAsync().FireAndForget();
        if (Options.Settings.Instance.BlockSize > -7)
        {
            Options.Settings.Instance.BlockSize--;
        }

        _ = Options.Settings.Instance.SaveAsync();
    }

    public void IncreaseSize(object sender, RoutedEventArgs e)
    {
        Blockly.ZoomInAsync().FireAndForget();
        if (Options.Settings.Instance.BlockSize < 7)
        {
            Options.Settings.Instance.BlockSize++;
        }

        _ = Options.Settings.Instance.SaveAsync();
    }

    public void SetCurrentCommand(Command c)
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            JavascriptResponse ret;

            await Blockly.AddNewBlockToAreaAsync(c, false, true); // try custom

            ret = await Blockly.AddNewBlockToAreaAsync(c, false, false);
            await InfoNotificationWrapper.ShowSimpleAsync(ret.Message, "StatusError", PackageGuids.BuildingWindowString, 1500);
        }).FireAndForget();
    }

    private void ResetZoom(object sender, MouseButtonEventArgs e)
    {
        Blockly.ResetZoomAsync().FireAndForget();
    }

    private void ClearAllButton_Click(object sender, RoutedEventArgs e)
    {
        Blockly.ClearAsync().FireAndForget();
    }
}