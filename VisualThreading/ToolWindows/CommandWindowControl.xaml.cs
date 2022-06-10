using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisualThreading
{
    public partial class CommandWindowControl : UserControl
    {
        private readonly Schema.Schema _commands;
        private string currentCommand;
        private const string Language = "c#";

        public CommandWindowControl(Schema.Schema commands)
        {
            _commands = commands;
            currentCommand = "";
            InitializeComponent();

            SetCurrentCommand("If"); // temporary
            UpdateCommands();
        }

        // TODO: call this from Radial Dial
        private void SetCurrentCommand(string command)
        {
            currentCommand = command;
        }

        private void UpdateCommands()
        {
            foreach (var entry in _commands.RadialMenu)
            {
                if (!entry.Project_type.Contains(Language))
                {
                    continue;
                }

                var languageDescription = new Label() { Content = entry.Text };
                Widgets.Children.Add(languageDescription);

                foreach (var link in entry.Commands)
                {
                    var hyperlink = new Label { Content = link.Text };
                    //var hl = new Hyperlink() { NavigateUri = new Uri(link.Url), text
                    Widgets.Children.Add(hyperlink);
                }

                Debug.WriteLine(entry.Text);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("CommandWindowControl", "Button clicked");
        }
    }
}