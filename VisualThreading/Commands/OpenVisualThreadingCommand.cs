namespace VisualThreading
{
    [Command(PackageIds.OpenVisualThreading)]
    internal sealed class OpenVisualThreading : BaseCommand<OpenVisualThreading>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VisualThreadingWindow.ShowAsync();
        }
    }
}