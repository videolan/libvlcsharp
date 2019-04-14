using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LibVLCSharp.Shared;

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

        private void Init()
        {
            Core.Initialize();

            LibVLC = new LibVLC();

            var media = new Media(LibVLC,
                "http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_surround-fix.avi",
                FromType.FromLocation);
            var mediaConfiguration = new MediaConfiguration();
            mediaConfiguration.EnableHardwareDecoding();
            media.AddOption(mediaConfiguration);

            MediaPlayer = new MediaPlayer(media);
            MediaPlayer.Play();
        }
    }
}
