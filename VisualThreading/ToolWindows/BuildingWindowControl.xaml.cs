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
        int top = 300;
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

        private async void Label_Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine(e.Data.GetFormats());
            Label label = sender as Label;
            // drop text
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                
                object previousContext = label.Content;
                object dragContext = e.Data.GetData(DataFormats.Text);

                object conbination = (object)string.Join(" ", (string)previousContext, (string)dragContext);

                label.Content = conbination;
                
                
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop)) // drop file
            {
                string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                TextBox TextBox = new TextBox()
                {
                    Width = Double.NaN,
                    Height = 85,
                    AllowDrop = true,
                    
                };
                TextBox.MouseMove += textbox_MouseMove;
                TextBox.Background = System.Windows.Media.Brushes.AliceBlue;
                TextBox.Margin = new Thickness(0, top, 0, 0);

                if (path.Contains("elif"))
                {
                    TextBox.Text += "if ()";
                    TextBox.Text += Environment.NewLine;
                    TextBox.Text += "{";
                    TextBox.Text += Environment.NewLine;
                    TextBox.Text += "}";
                    TextBox.Text += Environment.NewLine;
                    TextBox.Text += "else if ()";
                    TextBox.Text += Environment.NewLine;
                    TextBox.Text += "{";
                    TextBox.Text += Environment.NewLine;
                    TextBox.Text += "}";

                    grid1.Children.Add(TextBox);
                    top += 90;
                }


                // label.Content = System.IO.File.ReadAllText(path);

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

        private void TextBox_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void textbox_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            TextBox textbBox = sender as TextBox;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject();
                data.SetData(DataFormats.Text, textbBox.Text);

            }

            e.Handled = true;
        }

        private void textblock_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            TextBox textbBox = sender as TextBox;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject();
                data.SetData(DataFormats.Text, textbBox.Text);

                DragDrop.DoDragDrop(textbBox, data, DragDropEffects.Copy);
            }

            e.Handled = true;
        }

    }

}
