using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading
{
    public class CommandWindow : BaseToolWindow<CommandWindow>
    {
        public override string GetTitle(int toolWindowId) => "Command Window";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new CommandWindowControl());
        }

        [Guid("b7972b7c-5b87-457b-b858-33669cd9027c")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.Thread;
            }
        }
    }
}