using LibVLCSharp.Shared;
using System.ComponentModel;

namespace LibVLCSharp.Forms.Sample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        LibVLC _libVLC;

        private MediaPlayer _mediaPlayer;
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

        public void Initialize()
        {
            Core.Initialize();

            _libVLC = new LibVLC();
            MediaPlayer = new MediaPlayer(_libVLC)
            {
                Media = new Media(_libVLC,
                "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", 
                Media.FromType.FromLocation)
            };
        }
    }
}