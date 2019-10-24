using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LibVLCSharp.Platforms.UWP;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Uno.Sample
{
    /// <summary>
    /// Main view model
    /// </summary>
    public class MainViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Initialized a new instance of <see cref="MainViewModel"/> class
        /// </summary>
        public MainViewModel()
        {
            InitializedCommand = new RelayCommand<InitializedEventArgs>(Initialize);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MainViewModel()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the command for the initialization
        /// </summary>
        public ICommand InitializedCommand { get; }

        private LibVLC LibVLC { get; set; }

        private Shared.MediaPlayer _mediaPlayer;
        /// <summary>
        /// Gets the media player
        /// </summary>
        public Shared.MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        private void Initialize(InitializedEventArgs eventArgs)
        {
            LibVLC = new LibVLC(eventArgs.SwapChainOptions);
            MediaPlayer = new Shared.MediaPlayer(LibVLC);
            MediaPlayer.Play(new Media(LibVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
                FromType.FromLocation));
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
