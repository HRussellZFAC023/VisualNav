using CefSharp;
using System.Windows;
using System.Windows.Controls;
using VisualNav.Schema;
using VisualNav.Utilities;

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
        SizeChanged += (_, _) => _blockly.PreviewCentreAsync().FireAndForget();
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
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await Task.Delay(100);
            Widgets.Children.Clear();
            Descriptions.Children.Clear();
            Widgets.Children.Add(
            new TextBlock
            {
                Text = LanguageMediator.GetCurrentActiveFileExtension() + " - " + c.Text,
                TextWrapping = TextWrapping.Wrap,
                Width = RootGrid.Width,
                FontSize = GetFontSize()
            });
            Descriptions.Children.Add(
        new TextBlock
        {
            Text = c.Description,
            TextWrapping = TextWrapping.Wrap,
            Width = RootGrid.Width,
            FontSize = c.Description.Contains("\n") ? Math.Max(GetFontSize() / 1.5, 12) : GetFontSize()
        });
        }).FireAndForget();
    }

    public void SetCurrentMenu(Menuitem m)
    {
        ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        {
            await Task.Delay(100);
            Descriptions.Children.Clear();
            Widgets.Children.Clear();
            Widgets.Children.Add(
                new TextBlock
                {
                    Text = LanguageMediator.GetCurrentActiveFileExtension() + " - " + m.Name,
                    TextWrapping = TextWrapping.Wrap,
                    Width = RootGrid.Width,
                    FontSize = GetFontSize()
                });
            if (m.Description != null)
            {
                Descriptions.Children.Add(
                   new TextBlock
                   {
                       Text = m.Description,
                       TextWrapping = TextWrapping.Wrap,
                       Width = RootGrid.Width,
                       FontSize = m.Description.Contains("\n") ? Math.Max(GetFontSize() / 1.5, 12) : GetFontSize()
                   }
                );
            }
           
        }).FireAndForget();
    }

    private double GetFontSize()
    {
        var radius = RenderSize.Width * 0.4; // RenderSize is the width of window
        var ratio = radius / 150; // conversion rate of radial dial radius to size on screen
        var fontSize = Math.Min(Math.Max(Math.Ceiling(12 * ratio), 12), 24);
        return fontSize;
    }

    public void SetCurrentCommand(Command c)
    {
        UpdateCommands(c);
        // if there is no preview, then it is a blockly block
        if (c.Preview.Equals(""))
        {
            if (c.Type == "") return; // no block type to display
            icons.Visibility = Visibility.Visible;
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
            icons.Visibility = Visibility.Hidden;
            BrowserBorder.Visibility = Visibility.Hidden;
            TextBorder.Visibility = Visibility.Visible;
            PreviewText.FontSize = GetFontSize() * 1.5;
            PreviewText.Text = c.Preview;
            PreviewText.Foreground =
                new System.Windows.Media.BrushConverter().ConvertFromString(c.Color) as
                    System.Windows.Media.SolidColorBrush;
        }
    }

    public void DecreaseSize(object sender, RoutedEventArgs e)
    {
        _blockly.ZoomOutAsync().FireAndForget();
        if (Options.Settings.Instance.BlockSize_Preview > -7)
        {
            Options.Settings.Instance.BlockSize_Preview--;
        }

        Options.Settings.Instance.SaveAsync().FireAndForget();
    }

    public void IncreaseSize(object sender, RoutedEventArgs e)
    {
        _blockly.ZoomInAsync().FireAndForget();
        if (Options.Settings.Instance.BlockSize_Preview < 7)
        {
            Options.Settings.Instance.BlockSize_Preview++;
        }

        Options.Settings.Instance.SaveAsync().FireAndForget();
    }

    public async Task ClearCurrentCommandAsync()
    {
        await _blockly.ClearAsync();
        Widgets.Children.Clear();
        Descriptions.Children.Clear();
    }
}