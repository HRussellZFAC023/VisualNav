using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.Clip)]
internal sealed class BuildingWindowClip : BaseCommand<BuildingWindowClip>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        BuildingWindow.Instance.ClipboardButton_Click(null, null);
        return Task.CompletedTask;
    }
}