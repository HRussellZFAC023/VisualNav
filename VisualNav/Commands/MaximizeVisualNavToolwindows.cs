namespace VisualNav.Commands;

//[Command("<insert guid from .vsct file>", 0x0100)]
//[Command(PackageIds.OpenBuildingWindow)]
internal sealed class MaximizeVisualNavToolwindows : BaseCommand<MaximizeVisualNavToolwindows>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowWarningAsync("Command1", "Button clicked");
    }
}