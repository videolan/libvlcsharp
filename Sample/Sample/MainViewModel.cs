using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LibVLCSharp.Shared;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Sample
{
    /// <summary>
    /// Represents the main viewmodel.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            InitCommand = new RelayCommand(Init);
        }

        private LibVLC _libVLC;
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
        /// </summary>
        public LibVLC LibVLC
        {
            get => _libVLC;
            private set => Set(nameof(LibVLC), ref _libVLC, value);
        }

        private MediaPlayer _mediaPlayer;
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        /// <summary>
        /// Gets the initialization command.
        /// </summary>
        public ICommand InitCommand { get; }

        /// <summary>
        /// Initializes the media player.
        /// </summary>
        public void Init()
        {
            Core.Initialize();

            LibVLC = new LibVLC();
            var mediaPlayer = new MediaPlayer(LibVLC)
            {
                Media = new Media(LibVLC,
                    "http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_surround-fix.avi",
                    FromType.FromLocation)
            };
            mediaPlayer.Buffering += (sender, e) => OnMediaPlayerStateChanged(VLCState.Buffering);
            mediaPlayer.Opening += (sender, e) => OnMediaPlayerStateChanged(VLCState.Opening);
            mediaPlayer.EncounteredError += (sender, e) => OnMediaPlayerStateChanged(VLCState.Error);
            mediaPlayer.EndReached += (sender, e) => OnMediaPlayerStateChanged(VLCState.Ended);
            mediaPlayer.Paused += (sender, e) => OnMediaPlayerStateChanged(VLCState.Paused);
            mediaPlayer.Playing += (sender, e) => OnMediaPlayerStateChanged(VLCState.Playing);
            mediaPlayer.Stopped += (sender, e) => OnMediaPlayerStateChanged(VLCState.Stopped);
            MediaPlayer = mediaPlayer;
            mediaPlayer.Play();
        }

        private void OnMediaPlayerStateChanged(VLCState state)
        {
            switch (state)
            {
                case VLCState.Ended:
                case VLCState.Error:
                case VLCState.Paused:
                case VLCState.Stopped:
                    KeepScreenOn(false);
                    break;
                default:
                    KeepScreenOn(true);
                    break;
            }
        }

        private void KeepScreenOn(bool keepScreenOn)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    DeviceDisplay.KeepScreenOn = keepScreenOn;
                }
                catch (Exception) { }
            });
        }
    }
}
