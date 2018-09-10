using System;
using System.Windows;

namespace LibVLCSharp.WPF.Sample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExampleBtn_Click(object sender, RoutedEventArgs e)
        {
            ((Window)Activator.CreateInstance((Type)((FrameworkElement)sender).Tag)).Show();
        }
    }
}