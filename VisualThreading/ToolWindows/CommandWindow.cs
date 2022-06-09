﻿using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading
{
    public class CommandWindow : BaseToolWindow<CommandWindow>
    {
        public override string GetTitle(int toolWindowId) => "Preview Command";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new CommandWindowControl());
        }

        [Guid("8d4fca2b-a66b-485a-a01f-58a3b98aa35e")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}