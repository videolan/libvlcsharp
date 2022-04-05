using Avalonia.Controls;
using AvaVLCControl.ViewModels;
using LibVLCSharp.Avalonia;

namespace AvaVLCControl.Views
{
    public partial class MainWindow : Window
    {        
        public MainWindowViewModel? viewModel;
        

        public MainWindow()
        {
            InitializeComponent();
            
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;

           

            if (!Avalonia.Controls.Design.IsDesignMode)
            {               
                Opened += MainWindow_Opened;
            }

        }
        
        private void MainWindow_Opened(object? sender, System.EventArgs e)
        {                        
            var tmp = PlayerControl.GetInstance();
            tmp.SetPlayerHandle();
            
        }
    }
}
