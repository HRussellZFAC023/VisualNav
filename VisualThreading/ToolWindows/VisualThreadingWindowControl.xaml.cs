using System.Windows;
using System.Windows.Controls;

namespace VisualThreading.ToolWindows
{
    public partial class VisualThreadingWindowControl : UserControl
    {
        public VisualThreadingWindowControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("VisualThreadingWindowControl", "Button clicked");
        }
    }
}