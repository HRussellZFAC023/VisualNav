using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading
{
    public class ToolWindow1 : BaseToolWindow<ToolWindow1>
    {
        public override string GetTitle(int toolWindowId) => "ToolWindow1";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new ToolWindow1Control());
        }

        [Guid("0ea6b182-db3a-4f77-abf4-492c7b31036b")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}