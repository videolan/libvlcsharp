using System.Windows;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Example1 : Window
    {
        public Example1()
        {
            InitializeComponent();

            Player.Content = new Controls() { VideoView = Player };
        }
    }
}