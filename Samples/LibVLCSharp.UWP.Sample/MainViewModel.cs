using System;
using System.ComponentModel;
using LibVLCSharp.Shared;

namespace LibVLCSharp.UWP.Sample
{
    /// <summary>
    /// Main view model
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Destructor
        /// </summary>
        ~MainViewModel()
        {
            Dispose();
        }

        private string[] _swapChainOptions;
        /// <summary>
        /// Sets the swap chain parameters
        /// </summary>
        public string[] SwapChainOptions
        {
            private get => _swapChainOptions;
            set
            {
                if (_swapChainOptions != value)
                {
                    _swapChainOptions = value;
                    LibVLC = new LibVLC(value);
                    MediaPlayer = new MediaPlayer(LibVLC);
                    MediaPlayer.Play(new Media(LibVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
                        FromType.FromLocation));
                }
            }
        }

        private LibVLC LibVLC { get; set; }

        private MediaPlayer _mediaPlayer;
        /// <summary>
        /// Gets the media player
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        private void Set<T>(string propertyName, ref T field, T value)
        {
            if (field == null && value != null || field != null && !field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Cleaning
        /// </summary>
        public void Dispose()
        {
            var mediaPlayer = MediaPlayer;
            MediaPlayer = null;
            mediaPlayer?.Dispose();
            LibVLC?.Dispose();
            LibVLC = null;
        }
    }
}
