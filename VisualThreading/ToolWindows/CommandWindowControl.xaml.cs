using System.Windows;
using System.Windows.Controls;

namespace VisualThreading
{
    public partial class CommandWindowControl : UserControl
    {
        public CommandWindowControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("CommandWindowControl", "Button clicked");
        }
    }
}
