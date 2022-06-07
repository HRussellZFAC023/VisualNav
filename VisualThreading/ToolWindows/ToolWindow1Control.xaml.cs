using System.Windows;
using System.Windows.Controls;

namespace VisualThreading
{
    public partial class ToolWindow1Control : UserControl
    {
        public ToolWindow1Control()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("ToolWindow1Control", "Button clicked");
        }
    }
}
