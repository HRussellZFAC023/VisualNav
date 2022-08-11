using CefSharp;
using System.Windows;
using System.Windows.Input;
using VisualNav.Schema;
using VisualNav.Utilities;
using Label = System.Windows.Controls.Label;

namespace VisualNav.ToolWindows;

public partial class PreviewWindowControl
{
    private readonly BlocklyAdapter _blockly;

    public PreviewWindowControl()
    {
        InitializeComponent();
        Focus();
        _blockly = new BlocklyAdapter(Browser, true);
        ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.LoadHtmlAsync(); }).FireAndForget();
        Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
        _blockly.ResetZoomAsync().FireAndForget();
    }

    private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
    {
        if (!e.IsLoading)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () => { await _blockly.InitAsync(); }).FireAndForget();
        }
    }

    private void UpdateCommands(Command c)
    {
        Widgets.Children.Clear();
        Descriptions.Children.Clear();
        Descriptions.Children.Add(new Label { Content = c.Description });
        Widgets.Children.Add(new Label { Content = LanguageMediator.GetCurrentActiveFileExtension() + " - " + c.Text });
    }

    public void SetCurrentMenu(Menuitem m)
    {
        Descriptions.Children.Clear();
        Descriptions.Children.Add(new Label { Content = m.Description });
    }

    public void SetCurrentCommand(Command c)
    {
        UpdateCommands(c);
        // if there is no preview, then it is a blockly block
        if (c.Preview.Equals(""))
        {
            if (c.Type == "") return;
            BrowserBorder.Visibility = Visibility.Visible;
            TextBorder.Visibility = Visibility.Hidden;
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                if (c.Type.Contains("custom"))
                    await _blockly.AddNewBlockToAreaAsync(c, true, true);
                else
                    await _blockly.AddNewBlockToAreaAsync(c, true, false);
            }).FireAndForget();
        }
        else
        {
            BrowserBorder.Visibility = Visibility.Hidden;
            TextBorder.Visibility = Visibility.Visible;
            PreviewText.Text = c.Preview;
            PreviewText.Foreground =
                new System.Windows.Media.BrushConverter().ConvertFromString(c.Color) as
                    System.Windows.Media.SolidColorBrush;
        }
    }

    public void DecreaseSize(object sender, RoutedEventArgs e)
    {
        _blockly.ZoomOutAsync().FireAndForget();
    }

    public void IncreaseSize(object sender, RoutedEventArgs e)
    {
        _blockly.ZoomInAsync().FireAndForget();
    }

    private void ResetZoom(object sender, MouseButtonEventArgs e)
    {
        _blockly.ResetZoomAsync().FireAndForget();
    }

    public async Task ClearCurrentCommandAsync()
    {
        await _blockly.ClearAsync();
        Widgets.Children.Clear();
        Descriptions.Children.Clear();
    }
}