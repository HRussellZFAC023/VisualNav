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

        public static BuildingWindowControl Instance;

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            Instance = new BuildingWindowControl();
            return Task.FromResult<FrameworkElement>(Instance);
        }

        [Guid("6a0155f8-b16a-4fba-90bb-8c9fab68de1b")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}