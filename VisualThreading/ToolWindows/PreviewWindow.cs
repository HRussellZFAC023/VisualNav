using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VisualThreading.Utilities;

namespace VisualThreading.ToolWindows
{
    public class PreviewWindow : BaseToolWindow<PreviewWindow>
    {
        public static PreviewWindowControl Instance;

        public override string GetTitle(int toolWindowId) => "Preview Command";

        public override Type PaneType => typeof(Pane);

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            Instance = new PreviewWindowControl();
            return Instance;
        }

        [Guid("8d4fca2b-a66b-485a-a01f-58a3b98aa35e")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}