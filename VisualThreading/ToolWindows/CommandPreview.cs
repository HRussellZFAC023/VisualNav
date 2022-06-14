using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading
{
    public class CommandPreview : BaseToolWindow<CommandPreview>
    {
        public override string GetTitle(int toolWindowId) => "CommandPreview";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new CommandPreviewControl());
        }

        [Guid("2d4c9b3a-5fdf-4546-b1e4-76e89a4002d1")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}