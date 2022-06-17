using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using VisualThreading.ToolWindows.SharedComponents;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl
    {
        private object DraggedItem { get; set; }
        private Point ItemRelativePosition { get; set; }
        private readonly Schema.Schema _commands;
        private string _currentLanguage; // file extension for language

        public BuildingWindowControl(Schema.Schema commands, string fileExt)
        {
            _commands = commands;
            _currentLanguage = fileExt;
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
                var tb = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 0)
                };
                tb.MouseLeftButtonDown += Label_MouseLeftButtonDown;

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

        public void SetCurrentCommand(string c)
        {
            foreach (var language in _commands.RadialMenu)
            {
                if (!language.FileExt.Equals(_currentLanguage)) { continue; }

                foreach (var command in language.Commands)
                {
                    if (!command.Text.Equals(c)) { continue; }
                    var tb = CodeBlockFactory.CodeBlock(command);
                    canvasLabels.Children.Add(tb);
                }
            }
        }
    }
}