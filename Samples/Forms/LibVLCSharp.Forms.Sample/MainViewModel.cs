using LibVLCSharp.Shared;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LibVLCSharp.Forms.Sample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            Task.Run((Action)Initialize);
        }

        private LibVLC LibVLC { get; set; }

        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        private bool IsLoaded { get; set; }

        private void Set<T>(string propertyName, ref T field, T value)
        {
            if (field == null && value != null || field != null && !field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Initialize()
        {
            Core.Initialize();

            LibVLC = new LibVLC();
            MediaPlayer = new MediaPlayer(LibVLC)
            {
                Media = new Media(LibVLC,
                    "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                    FromType.FromLocation)
            };
        }

        public void OnAppearing()
        {
            IsLoaded = true;
            Play();
        }

        public void Play()
        {
            if (IsLoaded)
            {
                MediaPlayer?.Play();
            }
        }
    }
}