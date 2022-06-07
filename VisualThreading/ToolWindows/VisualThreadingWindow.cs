using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading
{
    public class VisualThreadingWindow : BaseToolWindow<VisualThreadingWindow>
    {
        public override string GetTitle(int toolWindowId) => "Visual Threading Window";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new VisualThreadingWindowControl());
        }

        [Guid("159da447-c93c-4cb3-81f3-9da7686addb8")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.Processor;
            }
        }
    }
}