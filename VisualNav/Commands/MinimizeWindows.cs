using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.Minimize)]
internal sealed class MinimizeWindows : BaseCommand<MinimizeWindows>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        RadialWindow.Instance.DecreaseSize();
        return Task.CompletedTask;
    }
}