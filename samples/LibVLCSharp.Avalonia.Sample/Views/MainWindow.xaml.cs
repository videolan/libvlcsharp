using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Avalonia.Sample.ViewModels;
using System;

namespace LibVLCSharp.Avalonia.Sample.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnOpened(object sender, EventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            vm?.Play();
        }
                    
    }
}
