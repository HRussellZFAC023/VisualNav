using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.Insert)]
internal sealed class BuildingWindowInsert : BaseCommand<BuildingWindowInsert>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        BuildingWindow.Instance.InsertCodeButton_Click(null, null);
        return Task.CompletedTask;
    }
}