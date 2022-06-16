using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl
    {
        private object DraggedItem { get; set; }
        private Point ItemRelativePosition { get; set; }

        public void notify()
        {
            Console.WriteLine("Process Started!");  

            OnClickCompleted();
        }

        protected virtual void OnClickCompleted() //protected virtual method
        {
            //if ProcessCompleted is not null then call delegate
            ClickCompleted?.Invoke(); 
        }

        public BuildingWindowControl()
        {
            InitializeComponent();
            DraggedItem = null;
        }

        private void Variable_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock || e.LeftButton != MouseButtonState.Pressed) return;
            var data = new DataObject();
            data.SetData("type", "variable");
            data.SetData("text", Variable.Text);
            data.SetData("background", Variable.Background);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
        }

        private void Operator_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock || e.LeftButton != MouseButtonState.Pressed) return;
            var data = new DataObject();
            data.SetData("type", "operator");
            data.SetData("text", Operator.Text);
            data.SetData("background", Operator.Background);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
        }

        private void If_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock label || e.LeftButton != MouseButtonState.Pressed) return;
            var data = new DataObject();
            data.SetData("type", "if");
            data.SetData("text", Iflabel.Text);
            data.SetData("background", Iflabel.Background);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
        }

        private void IfElse_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock || e.LeftButton != MouseButtonState.Pressed) return;
            var data = new DataObject();
            data.SetData("type", "ifelse");
            data.SetData("text", Ifelselabel.Text);
            data.SetData("background", Ifelselabel.Background);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
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
            var dragType = (string)e.Data.GetData("type");

            var dropPoint = e.GetPosition(canvasLabels);

            if (dragText != null && dragBackground != null)
            {
                /*Label copy = new Label();
                if (dragType == "variable")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(VariableLabel)) as Label;
                    copy.FontSize = 18;
                }
                else if (dragType == "operator")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(OperatorLabel)) as Label;
                    copy.FontSize = 18;
                }
                else if (dragType == "if")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(IfLabelLabel)) as Label;
                    copy.FontSize = 24;
                }
                else if (dragType == "ifelse")
                {
                    copy = XamlReader.Parse(XamlWriter.Save(IfelseLabelLabel)) as Label;
                    copy.FontSize = 24;
                }

                Debug.Assert(copy != null, nameof(copy) + " != null");

                copy.Margin = new Thickness(0, 0, 0, 0);
                copy.Height = Double.NaN;
                copy.Width = Double.NaN;
                copy.FontWeight = FontWeights.Bold;
                copy.MouseLeftButtonDown += Label_MouseLeftButtonDown;

                ((Canvas)sender).Children.Add(copy);
                Canvas.SetLeft(copy, dropPoint.X);
                Canvas.SetTop(copy, dropPoint.Y);*/

                var tb = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 0)
                };
                tb.MouseLeftButtonDown += Label_MouseLeftButtonDown;


                addDraggedItem(tb, dragText, dragBackground, dragType);

                switch (dragType)
                {
                    case "variable":
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Red,
                                Text = "Variable",
                                FontWeight = FontWeights.Bold,
                                FontSize = 18
                            });
                        break;

                    case "operator":
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Green,
                                Text = "Operator",
                                FontWeight = FontWeights.Bold,
                                FontSize = 18
                            });
                        break;

                    case "if":
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Aqua,
                                Text = "if (                                   ) {\n\t\n} ",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        break;

                    case "ifelse":
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = "if ( ",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = "           ",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = "           ",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = "           ",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = " ) { \n",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = "\t\n",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = "} else { \n",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = "\t\n",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        tb.Inlines.Add(
                            new Run()
                            {
                                Background = Brushes.Yellow,
                                Text = "} ",
                                FontWeight = FontWeights.Bold,
                                FontSize = 24
                            });
                        break;
                }

                ((Canvas)sender).Children.Add(tb);
                Canvas.SetLeft(tb, dropPoint.X);
                Canvas.SetTop(tb, dropPoint.Y);
            }

            e.Handled = true;
        }

        private void addDraggedItem(TextBlock tb, object dragText, object dragBackground, string dragType)
        {

            
            tb.Inlines.Add(
                new Run()
                {
                    Background = (Brush)dragBackground,
                    Text = (string)dragText,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18
                });
            throw new NotImplementedException();
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