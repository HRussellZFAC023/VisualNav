
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace VisualThreading
{
    public partial class BuildingWindowControl : UserControl
    {
        public object draggedItem { get; set; }
        public System.Windows.Point itemRelativePosition { get;  set; }

        public BuildingWindowControl()
        {
            InitializeComponent();
            this.draggedItem = null;
        }

        Label preview_effect = new Label()
        {
            Width = Double.NaN,
            Height = Double.NaN,
            Opacity = 0.5
        };


        private void Variable_MouseMove(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;

            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {

                DataObject data = new DataObject();
                data.SetData("type", "variable");
                data.SetData("text", Variable.Text);
                data.SetData("background", Variable.Background);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            }
        }
        private void Operator_MouseMove(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;

            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {

                DataObject data = new DataObject();
                data.SetData("type", "operator");
                data.SetData("text", Operator.Text);
                data.SetData("background", Operator.Background);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

                System.Diagnostics.Debug.WriteLine(data.GetData("text"));
                System.Diagnostics.Debug.WriteLine(data.GetData("background"));

            }
        }
        private void If_MouseMove(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;

            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {

                DataObject data = new DataObject();
                data.SetData("type", "if");
                data.SetData("text", Iflabel.Text);
                data.SetData("background", Iflabel.Background);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            }
        }
        private void Ifelse_MouseMove(object sender, MouseEventArgs e)
        {
            this.draggedItem = (UIElement)sender;
            Label label = sender as Label;
            var text = "if (                   ) { &#xD;&#xA;    } else { &#xD;&#xA;   } ";

            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {

                DataObject data = new DataObject();
                data.SetData("type", "ifelse");
                data.SetData("text", Ifelselabel.Text);
                data.SetData("background", Ifelselabel.Background);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            }
        }


        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Handled = true;
        }


        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            var dragText = e.Data.GetData("text");
            var dragBackground = e.Data.GetData("background");
            String dragType = (String)e.Data.GetData("type");

            Point dropPoint = e.GetPosition(this.canvasLabels);

            if (dragText != null && dragBackground != null)
            {
                Label copy = new Label();
                if (dragType == "variable")
                {
                    Label temp_label = e.Data.GetData("element") as Label;
                    copy.Content = e.Data.GetData("text");
                    copy.Background = (Brush)e.Data.GetData("background");
                    
                } else if(dragType == "operator")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(OperatorLabel)) as Label;
                } else if(dragType == "if")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(IfLabelLabel)) as Label;
                } else if(dragType == "ifelse")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(IfelseLabelLabel)) as Label;
                }

                copy.Margin = new Thickness(0, 0, 0, 0);
                copy.Height = Double.NaN;
                copy.Width = Double.NaN;
                copy.FontSize = 24;
                copy.FontWeight = FontWeights.Bold;
                copy.MouseLeftButtonDown += Label_MouseLeftButtonDown;
                ((Canvas)sender).Children.Add(copy);

                Canvas.SetLeft(copy, dropPoint.X);
                Canvas.SetTop(copy, dropPoint.Y);

                canvasLabels.Children.Remove(preview_effect);

            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                // create UI element based on image path
                string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                if (path.Contains("variable"))
                {
                    TextBox textbox = new TextBox()
                    {
                        Text = "variable",
                        Background = Brushes.Orange,
                        Height = Double.NaN,
                        Width = 75,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                        
                    };
                    textbox.MouseMove += textbox1_MouseMove;

                    Label label = new Label()
                    {
                        Height = 42,
                        Width = 85,
                        Content = textbox,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 0),
                        Background = Brushes.AntiqueWhite
                        
                    };

                    label.MouseMove += Label_MouseMove;
                    
                    history_list.Children.Add(label);
                    canvasLabels.Children.Remove(preview_effect);
                }
            }

            e.Handled = true;
        }


        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.draggedItem = (UIElement)sender;
            itemRelativePosition = e.GetPosition((IInputElement)this.draggedItem);
            e.Handled = true;
        }

        private void CanvasLabel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.draggedItem == null)
                return;
            var newPos = e.GetPosition(canvasLabels) - itemRelativePosition;
            Canvas.SetTop((UIElement)this.draggedItem, newPos.Y);
            Canvas.SetLeft((UIElement)this.draggedItem, newPos.X);

            Point dropPoint = e.GetPosition(preview_effect);

            Canvas.SetLeft(preview_effect, dropPoint.X);
            Canvas.SetTop(preview_effect, dropPoint.Y);

            canvasLabels.CaptureMouse();

            e.Handled = true;
        }

        private void CanvasLabel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.draggedItem != null)
            {
                this.draggedItem = null;
                canvasLabels.ReleaseMouseCapture();

                e.Handled = true;
            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Label label = sender as Label;

            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {
                TextBox textbox = label.Content as TextBox;
                DataObject data = new DataObject();
                data.SetData("type", "variable");
                data.SetData("element", label);
                data.SetData("text", textbox.Text);
                data.SetData("background", textbox.Background);



                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            }

            e.Handled = true;
        }

        private void textbox1_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            e.Handled = true;
        }

        private void canvasLabels_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                preview_effect.Content = e.Data.GetData("text");
                preview_effect.Background = (Brush)e.Data.GetData("background");

                Point dropPoint = e.GetPosition(this.canvasLabels);

                canvasLabels.Children.Remove(preview_effect);
                canvasLabels.Children.Add(preview_effect);

                Canvas.SetLeft(preview_effect, dropPoint.X);
                Canvas.SetTop(preview_effect, dropPoint.Y);

            }

            e.Handled = true;

        }

        private void canvasLabels_MouseMove(object sender, MouseEventArgs e)
        {

            Point dropPoint = e.GetPosition(preview_effect);

            var newPos = e.GetPosition(canvasLabels) - dropPoint;

            Canvas.SetLeft(preview_effect, newPos.X);
            Canvas.SetTop(preview_effect, newPos.Y);

            e.Handled = true;
        }

    }

}
