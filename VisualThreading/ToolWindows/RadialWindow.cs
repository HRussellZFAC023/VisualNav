using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading.ToolWindows
{
    public class RadialWindow : BaseToolWindow<RadialWindow>
    {
        public override string GetTitle(int toolWindowId) => "Command Pallet";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new RadialWindowControl());
        }

        [Guid(PackageGuids.RadialWindowString)]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.PieChart;
            }
        }
    }
}