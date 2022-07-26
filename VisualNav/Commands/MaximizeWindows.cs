using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.Maximize)]
internal sealed class MaximizeVisualNavToolwindows : BaseCommand<MaximizeVisualNavToolwindows>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        RadialWindow.Instance.IncreaseSize();
        return Task.CompletedTask;
    }
}