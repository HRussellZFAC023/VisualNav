using System.Windows;
using System.Windows.Controls;

namespace VisualThreading
{
    public partial class BuildingWindowControl : UserControl
    {
        public BuildingWindowControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("BuildingWindowControl", "Button clicked");
        }
    }
}
