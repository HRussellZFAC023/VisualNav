using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using VisualThreading.Schema;

namespace VisualThreading.ToolWindows.SharedComponents
{
    internal static class CodeBlockFactory
    {
        public static TextBlock CodeBlock(Command command)
        {
            Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(command.color)!);
            var tb = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0)
            };

            tb.Inlines.Add(
                new Run()
                {
                    Background = brush,
                    Text = command.preview,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18
                }
            );
            return tb;
        }
    }
}