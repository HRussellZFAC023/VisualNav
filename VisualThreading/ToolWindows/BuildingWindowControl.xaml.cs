using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VisualThreading
{
    public partial class BuildingWindowControl : UserControl
    {
        public BuildingWindowControl()
        {
            InitializeComponent();
        }

        private void Label_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Handled = true;
        }

        private void Label_Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine(e.Data.GetFormats());
            Label label = sender as Label;
            // drop text
            if (e.Data.GetDataPresent(DataFormats.Text))
            {

                object previousContext = label.Content;
                object dragContext = e.Data.GetData(DataFormats.Text);
                Console.WriteLine((string)previousContext);
                Console.WriteLine((string)dragContext);

                object conbination = (object)string.Join(" ", (string)previousContext, (string)dragContext);

                label.Content = conbination;

            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop)) // drop file
            {
                string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                label.Content = System.IO.File.ReadAllText(path);

            }
            else if (e.Data.GetDataPresent(typeof(TextBlock)))
            {
                Console.WriteLine("text block");
                label.Content = ((TextBlock)e.Data.GetData(typeof(TextBlock)));
            }

            e.Handled = true;
        }

        private void TextBox_TextChanged_variable(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_operator(object sender, TextChangedEventArgs e)
        {


        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
            var position = docView.TextView?.Selection.Start.Position.Position;

            if (position.HasValue)
            {
                docView.TextBuffer.Insert(position.Value, label_1.Content.ToString());
                label_1.Content = "";
            }
        }
    }

}
