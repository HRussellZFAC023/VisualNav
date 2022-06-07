using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading
{
    public class RadialMenu : BaseToolWindow<RadialMenu>
    {
        public override string GetTitle(int toolWindowId) => "Radial Menu for Visual Threading";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new RadialMenuControl());
        }

        [Guid("94732a08-9fa8-4cd0-878d-1c272df1d2cd")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.PieChart;
            }
        }
    }
}