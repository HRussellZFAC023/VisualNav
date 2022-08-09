using CefSharp;
using System.Windows;
using VisualNav.Schema;
using VisualNav.Utilities;
using Label = System.Windows.Controls.Label;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

namespace VisualNav.ToolWindows;

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
        _blockly.ResetZoomAsync().FireAndForget();
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

    public void SetCurrentCommand(Command c)
    {
        // if there is no preview, then it is a blockly block
        if (c.Preview.Equals(""))
        {
            BrowserBorder.Visibility = Visibility.Visible;
            TextBorder.Visibility = Visibility.Hidden;
            JavascriptResponse ret1 = null, ret2 = null;
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                if (c.Type.Contains("custom"))
                    ret2 = await _blockly.AddNewBlockToAreaAsync(c, true, true);
                else
                    ret1 = await _blockly.AddNewBlockToAreaAsync(c, true, false);

                
            }).FireAndForget();
            
            
            if (ret1 == null && ret2 == null) return;
            _currentCommand = c;
            UpdateCommands();
        }
        else
        {
            BrowserBorder.Visibility = Visibility.Hidden;
            TextBorder.Visibility = Visibility.Visible;
            PreviewText.Text = c.Preview;
            PreviewText.Foreground =
                new System.Windows.Media.BrushConverter().ConvertFromString(c.Color) as
                    System.Windows.Media.SolidColorBrush;
            _currentCommand = c;
            UpdateCommands();
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

    public async Task ClearCurrentCommandAsync()
    {
        await _blockly.ClearAsync();
        
        _currentCommand = null;
        UpdateCommands();
    }
}