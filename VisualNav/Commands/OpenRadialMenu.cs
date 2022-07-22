using VisualNav.ToolWindows;

namespace VisualNav.Commands
{
    [Command(PackageIds.OpenRadialMenu)]
    internal sealed class OpenRadialMenu : BaseCommand<OpenRadialMenu>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await RadialWindow.ShowAsync();
        }
    }
}