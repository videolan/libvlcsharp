using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;


namespace AvaVLCControl.Views
{
    public partial class PlayerControls : UserControl
    {
        public Panel _playerControl;
        private static PlayerControls _this;
        public PlayerControls()
        {
            InitializeComponent();
            _this = this;
            _playerControl = this.Get<Panel>("ControlPlayer");
        }

        public static PlayerControls GetInstance()
        {
            return _this;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void StartPlay(object? sender, RoutedEventArgs e)
        {
            var tmp = PlayerControl.GetInstance();
            tmp.viewModel.Play();
        }

        private void StopPlay(object? sender, RoutedEventArgs e)
        {
            var tmp = PlayerControl.GetInstance();
            tmp.viewModel.Stop();
        }
    }
}
