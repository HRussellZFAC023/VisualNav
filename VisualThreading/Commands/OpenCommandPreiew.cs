﻿using VisualThreading.ToolWindows;

namespace VisualThreading.Commands
{
    [Command(PackageIds.OpenCommandPreview)]
    internal sealed class OpenCommandPreview : BaseCommand<OpenCommandPreview>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await CommandWindow.ShowAsync();
        }
    }
}