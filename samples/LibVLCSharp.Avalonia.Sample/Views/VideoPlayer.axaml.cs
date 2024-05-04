using System;
using Avalonia.Controls;
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
    }
}