using System.Windows;
using System.Windows.Controls;

namespace VisualThreading
{
    public partial class RadialDialControl : UserControl
    {
        public RadialDialControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("RadialDialControl", "Button clicked");
        }
    }
}