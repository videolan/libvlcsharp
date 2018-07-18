using System.Windows;

namespace LibVLCSharp.WPF.Sample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Example1Btn.Click += Example1Btn_Click;
            Example2Btn.Click += Example2Btn_Click;
        }

        void Example1Btn_Click(object sender, RoutedEventArgs e)
        {
            var window = new Example1();
            window.Show();
        }

        void Example2Btn_Click(object sender, RoutedEventArgs e)
        {
            var window = new Example2();
            window.Show();
        }
    }
}