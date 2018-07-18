using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void Example1Btn_Click(object sender, RoutedEventArgs e)
        {
            Example1 window = new Example1();
            window.Show();
        }

        private void Example2Btn_Click(object sender, RoutedEventArgs e)
        {
            Example2 window = new Example2();
            window.Show();
        }
    }
}