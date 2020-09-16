using System;
using System.ComponentModel;
using System.Windows.Input;
using LibVLCSharp.Shared;
using LibVLCSharp.Uno;

namespace Sample.MediaPlayerElement
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
        /// Initialized a new instance of <see cref="MainViewModel"/> class
        /// </summary>
        public MainViewModel()
        {
            InitializedCommand = new RelayCommand<string[]>(Initialize);
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

        private LibVLC _libVLC;
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance
        /// </summary>
        public LibVLC LibVLC
        {
            get => _libVLC;
            private set => Set(nameof(LibVLC), ref _libVLC, value);
        }

        private bool _isSuspended;
        private bool IsSuspended
        {
            get => _isSuspended;
            set
            {
                if (_isSuspended != value)
                {
                    _isSuspended = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MediaPlayer)));
                }
            }
        }

        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer;
        /// <summary>
        /// Gets the media player
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get => IsSuspended ? null : _mediaPlayer;
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

        private void Initialize(string[] swapChainOptions)
        {
            LibVLC = new LibVLC(enableDebugLogs: true, swapChainOptions);
            MediaPlayer = new LibVLCSharp.Shared.MediaPlayer(LibVLC);
            MediaPlayer.Play(new Media(LibVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
                FromType.FromLocation));
        }

        /// <summary>
        /// Suspension
        /// </summary>
        public void Suspend()
        {
            SuspensionHelper.Save(MediaPlayer);
            IsSuspended = true;
        }

        /// <summary>
        /// Resuming
        /// </summary>
        public void Resume()
        {
            IsSuspended = false;
            SuspensionHelper.Restore(MediaPlayer);
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
