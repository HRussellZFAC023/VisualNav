using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisualThreading.ToolWindows
{
    public class PreviewWindow : BaseToolWindow<PreviewWindow>
    {
        public static PreviewWindowControl Instance;

        public override string GetTitle(int toolWindowId) => "Preview Command";

        public override Type PaneType => typeof(Pane);

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            var commands = await Schema.Schema.LoadAsync();
            var buffer = await VS.Documents.GetActiveDocumentViewAsync();
            var root = Path.GetDirectoryName(typeof(VisualStudioServices).Assembly.Location);
            var blockly = await ReadFileAsync(Path.Combine(root!, "Resources", "html", "blocklyHTML.html"));
            var toolbox = await ReadFileAsync(Path.Combine(root!, "Resources", "xml", "blocklyToolbox.xml"));
            var workspace = await ReadFileAsync(Path.Combine(root!, "Resources", "xml", "blocklyWorkspace.xml"));

            var fileExt = "";
            if (buffer?.TextBuffer != null)
            {
                fileExt =
                Path.GetExtension(buffer.TextBuffer.GetFileName());
            }

            Instance = new PreviewWindowControl(commands, fileExt, blockly, toolbox, workspace);
            return Instance;
        }
        private static async Task<string> ReadFileAsync(string file)
        {
            using var reader = new StreamReader(file);
            var content = await reader.ReadToEndAsync();
            return content;
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