using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
    }
}
