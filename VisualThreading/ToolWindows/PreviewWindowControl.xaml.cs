using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using VisualThreading.ToolWindows.SharedComponents;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

namespace VisualThreading.ToolWindows
{
    public partial class PreviewWindowControl : UserControl
    {
        private readonly Schema.Schema _commands;
        private Schema.Command _currentCommand;
        private string _currentLanguage; // file extension for language

        public PreviewWindowControl(Schema.Schema commands, string language)
        {
            _commands = commands;
            _currentCommand = null;
            _currentLanguage = language;
            InitializeComponent();

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
            SetCurrentLanguage(fileExt);
        }

        public void SetCurrentCommand(Schema.Command command)
        {
            _currentCommand = command;
            UpdateCommands();
        }

        private void SetCurrentLanguage(string language)
        {
            _currentLanguage = language;
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            // |******************|
            // |WIDGETS stackPanel|
            // |******************|
            // |PREVIEW stackPanel|
            // |******************|

            // remove all elements
            Widgets.Children.Clear();
            Preview.Children.Clear();

            // check if command is set
            if (_currentCommand == null)
                return;

            // widgets
            Widgets.Children.Add(new Label { Content = _currentCommand.Text });
            // Preview
            var tb = CodeBlockFactory.CodeBlock(_currentCommand);
            tb.Margin = new Thickness(5);
            Preview.Children.Add(tb);
        }

        public void ClearCurrentCommand()
        {
            _currentCommand = null;
            UpdateCommands();
        }

        private void Label_MouseMove_From_List(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;

            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var type = label.Name;
                var runObject = (Run)label.Content;
                var bgColor = runObject.Background;
                var text = runObject.Text;
                DataObject data = new DataObject();
                data.SetData("type", type);
                data.SetData("background", bgColor);
                data.SetData("text", text);
                data.SetData("draggedItem", (UIElement)sender);
                data.SetData("itemRelativePosition", e.GetPosition((UIElement)sender));

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            }
        }

    }
}