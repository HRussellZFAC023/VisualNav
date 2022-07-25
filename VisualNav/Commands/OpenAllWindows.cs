using VisualNav.ToolWindows;

namespace VisualNav.Commands;

[Command(PackageIds.OpenAllWindows)]
internal sealed class OpenAllWindows : BaseCommand<OpenAllWindows>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await BuildingWindow.ShowAsync();
        await RadialWindow.ShowAsync();
        await PreviewWindow.ShowAsync();
    }
}