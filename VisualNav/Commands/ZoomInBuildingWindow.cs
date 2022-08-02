using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.ZoomIn)]
internal sealed class ZoomInBuildingWindow : BaseCommand<ZoomInBuildingWindow>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await BuildingWindow.Instance.Blockly.ZoomInAsync();
    }
}