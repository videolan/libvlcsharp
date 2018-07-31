
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Example2 : Window
    {
        public Example2()
        {
            InitializeComponent();

            test.Children.Add(new Label
            {
                Content = "TEST",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Foreground = new SolidColorBrush(Colors.Red)
            });
        }
    }
}