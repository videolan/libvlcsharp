using System;
using System.ComponentModel;
using LibVLCSharp.Platforms.Windows;

namespace LibVLCSharp.UWP.Modern
{
    /// <summary>
    /// Main view model.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Finalizes an instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        ~MainViewModel()
        {
            Dispose();
        }

        LibVLC LibVLC { get; set; }

        MediaPlayer _mediaPlayer;
        /// <summary>
        /// Gets the media player.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        /// <summary>
        /// Initializes LibVLC and starts playback once the <see cref="VideoView"/> is ready.
        /// </summary>
        /// <param name="eventArgs">the VideoView initialization arguments, carrying the swap chain options</param>
        public void Initialize(InitializedEventArgs eventArgs)
        {
            LibVLC = new LibVLC(enableDebugLogs: true, eventArgs.SwapChainOptions);
            MediaPlayer = new MediaPlayer(LibVLC);
            using var media = new Media(new Uri("https://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.avi"));
            MediaPlayer.Play(media);
        }

        void Set<T>(string propertyName, ref T field, T value)
        {
            if (field == null && value != null || field != null && !field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Cleaning.
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
