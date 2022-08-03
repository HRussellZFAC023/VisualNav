using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.ResetZoom)]
internal sealed class ResetZoomBuildingWindow : BaseCommand<ResetZoomBuildingWindow>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await BuildingWindow.Instance.Blockly.ResetZoomAsync();
    }
}