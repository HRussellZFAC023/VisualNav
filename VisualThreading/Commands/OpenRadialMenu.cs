namespace VisualThreading
{
    [Command(PackageIds.OpenRadialMenu)]
    internal sealed class OpenRadialMenu : BaseCommand<OpenRadialMenu>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await RadialDial.ShowAsync();
        }
    }
}