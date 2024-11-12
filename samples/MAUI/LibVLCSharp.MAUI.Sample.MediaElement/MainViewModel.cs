using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LibVLCSharp;

namespace LibVLCSharp.MAUI.Sample.MediaElement
{
    /// <summary>
    /// Represents the main viewmodel.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = null!;

        /// <summary>
        /// Initializes a new instance of <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
        }

        private LibVLC _libVLC;
		
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.LibVLC"/> instance.
        /// </summary>
        public LibVLC LibVLC
        {
            get => _libVLC;
            private set => SetProperty(ref _libVLC, value);
        }

        private LibVLCSharp.MediaPlayer _mediaPlayer;
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.MediaPlayer"/> instance.
        /// </summary>
        public LibVLCSharp.MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => SetProperty(ref _mediaPlayer, value);
        }

        /// <summary>
        /// Initialize LibVLC and playback when page appears
        /// </summary>
        public void OnAppearing()
        {
            if (LibVLC == null)
            {
                LibVLC = new LibVLC(enableDebugLogs: true);
            }

            if (MediaPlayer == null)
            {
                var media = new Media(new Uri("http://streams.videolan.org/streams/mkv/multiple_tracks.mkv"));
                MediaPlayer = new MediaPlayer(LibVLC)
                {
                    EnableHardwareDecoding = true
                };
                media.Dispose();
                MediaPlayer.Play();
            }
        }

        /// <summary>
        /// Dispose MediaPlayer and LibVLC when page disappears
        /// </summary>
        public void OnDisappearing()
        {
            MediaPlayer?.Dispose();
            MediaPlayer = null;

            LibVLC?.Dispose();
            LibVLC = null;
        }

        /// <summary>
        /// Set property and notify UI
        /// </summary>
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
