using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.Clip)]
internal sealed class BuidlingWindowCopy : BaseCommand<BuidlingWindowCopy>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        BuildingWindow.Instance.ClipboardButton_Click(null, null);
        return Task.CompletedTask;
    }
}