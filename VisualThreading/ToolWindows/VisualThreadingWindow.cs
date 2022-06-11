using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading.ToolWindows
{
    public class VisualThreadingWindow : BaseToolWindow<VisualThreadingWindow>
    {
        public override string GetTitle(int toolWindowId) => "VisualThreadingWindow";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new VisualThreadingWindowControl());
        }

        [Guid("a9784502-89e5-4df8-bfed-de973fdb4e5c")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}