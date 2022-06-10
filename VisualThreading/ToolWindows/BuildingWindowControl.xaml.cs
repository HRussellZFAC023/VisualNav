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
            Console.WriteLine("Label_DragEnter========================");

            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
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
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) // drop file
            {
                string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                Bitmap bmp = new Bitmap(path);
                ImageSource img = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                myImg.Source = img;

            }
        }


        private void If_block_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject();
                data.SetData(DataFormats.StringFormat, e.Source.ToString());

                // Initiate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

    }

}
