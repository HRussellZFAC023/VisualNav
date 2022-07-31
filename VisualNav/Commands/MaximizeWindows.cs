using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.Maximize)]
internal sealed class MaximizeWindows : BaseCommand<MaximizeWindows>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        RadialWindow.Instance.IncreaseSize();
        return Task.CompletedTask;
    }
}