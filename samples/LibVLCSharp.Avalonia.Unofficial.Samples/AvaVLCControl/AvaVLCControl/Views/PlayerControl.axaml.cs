using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaVLCControl.ViewModels;
using LibVLCSharp.Avalonia;
using System;

namespace AvaVLCControl.Views
{
    public partial class PlayerControl : UserControl
    {
        private static PlayerControl _this;        
        private VideoView? _videoViewer;
        public PlayerControlModel viewModel;        

        public PlayerControl()
        {
            InitializeComponent();
            
            _this = this;

            viewModel = new PlayerControlModel();
            DataContext = viewModel;

            _videoViewer = this.Get<VideoView>("VideoViewer");
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static PlayerControl GetInstance()
        {
            return _this;
        }

        public void SetPlayerHandle()
        {
            if (_videoViewer != null && viewModel.MediaPlayer != null)
            {
                _videoViewer.MediaPlayer = viewModel.MediaPlayer;
                _videoViewer.MediaPlayer.Hwnd = _videoViewer.hndl.Handle;
            }
            
        }
    }
}
