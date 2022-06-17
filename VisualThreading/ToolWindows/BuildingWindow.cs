using System.IO;
using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Text;
using VisualThreading.ToolWindows;

namespace VisualThreading
{
    public class BuildingWindow : BaseToolWindow<BuildingWindow>
    {
        public override string GetTitle(int toolWindowId) => "BuildingWindow";

        public override Type PaneType => typeof(Pane);

        public static BuildingWindowControl Instance;

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            var commands = await Schema.Schema.LoadAsync();
            var buffer = await VS.Documents.GetActiveDocumentViewAsync();
            var fileExt = "";
            if (buffer?.TextBuffer != null)
            {
                fileExt =
                    Path.GetExtension(buffer.TextBuffer.GetFileName());
            }
            Instance = new BuildingWindowControl(commands, fileExt);
            return Instance;
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