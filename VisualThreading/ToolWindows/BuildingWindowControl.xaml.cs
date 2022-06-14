using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl
    {
        private object DraggedItem { get; set; }
        private Point ItemRelativePosition { get; set; }

        public BuildingWindowControl()
        {
            InitializeComponent();
            DraggedItem = null;
        }

        private void Variable_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Label label && e.LeftButton == MouseButtonState.Pressed)
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
            if (sender is Label && e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData("type", "operator");
                data.SetData("text", Operator.Text);
                data.SetData("background", Operator.Background);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

                Debug.WriteLine(data.GetData("text"));
                Debug.WriteLine(data.GetData("background"));
            }
        }

        private void If_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not Label || e.LeftButton != MouseButtonState.Pressed) return;
            DataObject data = new DataObject();
            data.SetData("type", "if");
            data.SetData("text", Iflabel.Text);
            data.SetData("background", Iflabel.Background);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
        }

        private void IfElse_MouseMove(object sender, MouseEventArgs e)
        {
            DraggedItem = (UIElement)sender;
            //var text = "if (                   ) { &#xD;&#xA;    } else { &#xD;&#xA;   } ";

            if (sender is Label label && e.LeftButton == MouseButtonState.Pressed)
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
            string dragType = (string)e.Data.GetData("type");

            Point dropPoint = e.GetPosition(canvasLabels);

            if (dragText != null && dragBackground != null)
            {
                Label copy = new Label();
                if (dragType == "variable")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(VariableLabel)) as Label;
                }
                else if (dragType == "operator")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(OperatorLabel)) as Label;
                }
                else if (dragType == "if")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(IfLabelLabel)) as Label;
                }
                else if (dragType == "ifelse")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(IfelseLabelLabel)) as Label;
                }

                Debug.Assert(copy != null, nameof(copy) + " != null");

                copy.Margin = new Thickness(0, 0, 0, 0);
                copy.Height = double.NaN;
                copy.Width = double.NaN;
                copy.FontSize = 24;
                copy.FontWeight = FontWeights.Bold;
                copy.MouseLeftButtonDown += Label_MouseLeftButtonDown;

                ((Canvas)sender).Children.Add(copy);
                Canvas.SetLeft(copy, dropPoint.X);
                Canvas.SetTop(copy, dropPoint.Y);
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = ((Array)e.Data.GetData(DataFormats.FileDrop))?.GetValue(0).ToString();
                Debug.Assert(path != null, nameof(path) + " != null");
                if (path.Contains("elif"))
                {
                    Label label = new Label
                    {
                        Height = double.NaN,
                        Width = double.NaN,
                        Content = "if...else...",
                        Margin = new Thickness(0, 300, 0, 0),
                        Foreground = Brushes.Black,
                        Background = Brushes.AntiqueWhite
                    };

                    history_list.Children.Add(label);
                }
            }

            e.Handled = true;
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DraggedItem = (UIElement)sender;
            ItemRelativePosition = e.GetPosition((IInputElement)DraggedItem);
            e.Handled = true;
        }

        private void CanvasLabel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (DraggedItem == null)
                return;
            var newPos = e.GetPosition(canvasLabels) - ItemRelativePosition;
            Canvas.SetTop((UIElement)DraggedItem, newPos.Y);
            Canvas.SetLeft((UIElement)DraggedItem, newPos.X);
            canvasLabels.CaptureMouse();
            e.Handled = true;
        }

        private void CanvasLabel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DraggedItem == null) return;
            DraggedItem = null;
            canvasLabels.ReleaseMouseCapture();
            e.Handled = true;
        }
    }
}