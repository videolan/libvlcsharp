using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LibVLCSharp.Shared;

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

        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer1;
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer2;

        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
        /// </summary>
        public LibVLC LibVLC
        {
            get => _libVLC;
            private set => SetProperty(ref _libVLC, value);
        }

        /// <summary>
        /// Gets the first <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer1
        {
            get => _mediaPlayer1;
            private set => SetProperty(ref _mediaPlayer1, value);
        }

        /// <summary>
        /// Gets the second <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer2
        {
            get => _mediaPlayer2;
            private set => SetProperty(ref _mediaPlayer2, value);
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

            if (MediaPlayer1 == null)
            {
                var media1 = new Media(LibVLC, new Uri("http://streams.videolan.org/streams/mkv/multiple_tracks.mkv"));
                MediaPlayer1 = new LibVLCSharp.Shared.MediaPlayer(media1)
                {
                    EnableHardwareDecoding = true
                };
                media1.Dispose();
                MediaPlayer1.Play();
            }

            if (MediaPlayer2 == null)
            {
                var media2 = new Media(LibVLC, new Uri("https://streams.videolan.org/streams/mp4/h264-sample-thefluff.mp4"));
                MediaPlayer2 = new LibVLCSharp.Shared.MediaPlayer(media2)
                {
                    EnableHardwareDecoding = true
                };
                media2.Dispose();
                MediaPlayer2.Play();
            }
        }

        /// <summary>
        /// Dispose MediaPlayer and LibVLC when page disappears
        /// </summary>
        public void OnDisappearing()
        {
            MediaPlayer1?.Dispose();
            MediaPlayer1 = null;

            MediaPlayer2?.Dispose();
            MediaPlayer2 = null;

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
