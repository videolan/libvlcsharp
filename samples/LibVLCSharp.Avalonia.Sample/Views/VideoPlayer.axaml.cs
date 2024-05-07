using System;
using Avalonia.Controls;
using Avalonia.Input;
using LibVLCSharp.Avalonia.Sample.ViewModels;

namespace LibVLCSharp.Avalonia.Sample.Views
{
    public partial class VideoPlayer : UserControl
    {
        public VideoPlayer()
        {
            InitializeComponent();
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.Play();
            }
        }

        private void VideoViewOnPointerEntered(object sender, PointerEventArgs e)
        {
            ControlsPanel.IsVisible = true;
        }

        private void VideoViewOnPointerExited(object sender, PointerEventArgs e)
        {
            ControlsPanel.IsVisible = false;
        }
    }
}