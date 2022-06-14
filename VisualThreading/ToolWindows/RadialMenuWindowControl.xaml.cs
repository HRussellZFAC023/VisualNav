using System.Windows;
using System.Windows.Controls;

namespace VisualThreading
{
    public partial class RadialMenuControl : UserControl
    {
        public RadialMenuControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("RadialMenuControl", "Button clicked");
        }
    }
}