using System.IO;
using System.Windows;
using System.Windows.Controls;
using VisualThreading.ToolWindows.SharedComponents;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

namespace VisualThreading.ToolWindows
{
    public partial class PreviewWindowControl : UserControl
    {
        private readonly Schema.Schema _commands;
        private string _currentCommand;
        private string _currentLanguage; // file extension for language

        public PreviewWindowControl(Schema.Schema commands, string language)
        {
            _commands = commands;
            _currentCommand = "";
            _currentLanguage = language;
            InitializeComponent();

            SetCurrentCommand("if");
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

        // TODO: call this from Radial Dial
        public void SetCurrentCommand(string command)
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
            foreach (var language in _commands.RadialMenu)
            {
                // file extension matches current language Eg c#
                if (!language.FileExt.Equals(_currentLanguage)) { continue; }
                var languageDescription = new Label { Content = language.Text };
                Widgets.Children.Add(languageDescription); // Eg. Displays c#

                // loop through each json "command" eg. "if", "else", "variable"....
                foreach (var command in language.Commands)
                {
                    if (!command.Text.Equals(_currentCommand)) { continue; }
                    Widgets.Children.Add(new Label { Content = command.Text });
                    TextBlock tb = CodeBlockFactory.CodeBlock(command); // preview
                    tb.Margin = new Thickness(5);
                    Preview.Children.Add(tb);
                }
            }
        }
    }
}