using System.Windows;
using System.Windows.Controls;

namespace VisualThreading
{
    public partial class CommandPreviewControl : UserControl
    {
        public CommandPreviewControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("CommandPreviewControl", "Button clicked");
        }
    }
}
