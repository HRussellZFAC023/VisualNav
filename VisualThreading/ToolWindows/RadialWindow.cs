using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading.ToolWindows
{
    public class RadialWindow : BaseToolWindow<RadialWindow>
    {
        public RadialWindowControl rwc;

        public override string GetTitle(int toolWindowId) => "Command Pallet";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            var rwc = new RadialWindowControl();
            this.rwc = rwc;

            return Task.FromResult<FrameworkElement>(rwc);
        }

        public RadialWindowControl getRwc()
        {
            return rwc;
        }

        [Guid("0ea6b182-db3a-4f77-abf4-492c7b31036b")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.PieChart;
            }
        }
    }
}