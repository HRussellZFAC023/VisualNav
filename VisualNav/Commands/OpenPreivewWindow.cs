using VisualNav.ToolWindows;

namespace VisualNav.Commands
{
    [Command(PackageIds.OpenCommandPreview)]
    internal sealed class OpenCommandPreview : BaseCommand<OpenCommandPreview>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await PreviewWindow.ShowAsync();
        }
    }
}