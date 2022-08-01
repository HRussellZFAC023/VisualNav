using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.ZoomOut)]
internal sealed class ZoomOutBuildingWindow : BaseCommand<ZoomOutBuildingWindow>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await BuildingWindow.Instance._blockly.ZoomOutAsync();
    }
}