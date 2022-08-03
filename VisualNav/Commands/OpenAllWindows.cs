using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.OpenAllWindows)]
internal sealed class OpenAllWindows : BaseCommand<OpenAllWindows>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await RadialWindow.ShowAsync();
        await PreviewWindow.ShowAsync();
        await BuildingWindow.ShowAsync();
        await BuildingWindow.Instance.Blockly.CenterAsync();
    }
}