using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading.ToolWindows
{
    public class BuildingWindow : BaseToolWindow<BuildingWindow>
    {
        public override string GetTitle(int toolWindowId) => "BuildingWindow";

        public override Type PaneType => typeof(Pane);

        public static readonly BuildingWindowControl Instance = new();

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(Instance);
        }

        [Guid(PackageGuids.BuildingWindowString)]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}