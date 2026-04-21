using System;
using LibVLCSharp;
using Avalonia.Controls;

namespace LibVLCSharp.Avalonia.Sample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly LibVLC _libVlc = new LibVLC();
        
        public MediaPlayer MediaPlayer { get; }
        
        public MainWindowViewModel()
        {
            MediaPlayer = new MediaPlayer(_libVlc);
        }

        public void Play()
        {
            if (Design.IsDesignMode)
            {
                return;
            }
            
            using var media = new Media(new Uri("https://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.avi"));
            MediaPlayer.Play(media);
        }
        
        public void Stop()
        {            
            MediaPlayer.Stop();
        }
   
        public void Dispose()
        {
            MediaPlayer?.Dispose();
            _libVlc?.Dispose();
        }
    }
}
