using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;

namespace VisualThreading.ToolWindows
{
    public partial class CommandWindowControl : UserControl
    {
        private readonly Schema.Schema _commands;
        private string _currentCommand;
        private string _currentLanguage; // file extension for language

        public CommandWindowControl(Schema.Schema commands, string language)
        {
            _commands = commands;
            _currentCommand = "";
            _currentLanguage = language;
            InitializeComponent();

            SetCurrentCommand("If");
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
            Widgets.Children.Clear();
            foreach (var entry in _commands.RadialMenu)
            {
                if (!entry.FileExt.Equals(_currentLanguage))
                {
                    continue;
                }

                var languageDescription = new Label { Content = entry.Text };
                Widgets.Children.Add(languageDescription);

                foreach (var link in entry.Commands)
                {
                    if (link.Text != _currentCommand)
                    {
                        continue;
                    }

                    var hyperlink = new Label { Content = link.Text };
                    var hl = new Hyperlink { NavigateUri = new Uri(link.Url) };
                    hl.Inlines.Add(link.Text);
                    Widgets.Children.Add(hyperlink);
                }

                Debug.WriteLine(entry.Text);
            }
        }
    }
}