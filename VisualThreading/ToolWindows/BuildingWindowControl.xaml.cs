using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisualThreading.ToolWindows.SharedComponents;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

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
            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged; // extends the selection event
        }

        private void SelectionEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileExt = "";
            if (e.To != null)
            {
                var buffer = e.To.Name;
                fileExt =
                    Path.GetExtension(buffer);
            }
            _currentLanguage = fileExt;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
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
                    tb.MouseLeftButtonDown += Label_MouseLeftButtonDown;
                    Canvas.SetLeft(tb, 0);
                    Canvas.SetTop(tb, 0);
                    canvasLabels.Children.Add(tb);
                }
            }
        }
    }
}