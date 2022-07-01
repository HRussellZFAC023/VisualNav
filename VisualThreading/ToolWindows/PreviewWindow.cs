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
            var fr = new FileReaderAdapter();
            var schema = await Schema.Schema.LoadAsync();

            var buffer = await VS.Documents.GetActiveDocumentViewAsync();
            var root = Path.GetDirectoryName(typeof(VisualStudioServices).Assembly.Location);
            var blockly = await fr.ReadFileAsync(Path.Combine(root!, "Resources", "html", "blocklyHTML.html"));
            var toolbox = await fr.ReadFileAsync(Path.Combine(root!, "Resources", "xml", "blocklyToolbox.xml"));
            var workspace = await fr.ReadFileAsync(Path.Combine(root!, "Resources", "xml", "blocklyWorkspace.xml"));

            var fileExt = "";
            if (buffer?.TextBuffer != null)
            {
                fileExt =
                Path.GetExtension(buffer.TextBuffer.GetFileName());
            }

            Instance = new PreviewWindowControl(schema, fileExt, blockly, toolbox, workspace);
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