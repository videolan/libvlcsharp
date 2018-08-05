using LibVLCSharp.Shared;
using System.ComponentModel;
using System.Windows.Input;

namespace LibVLCSharp.Forms.Sample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            AppearingCommand = new RelayCommand(Appearing);
            DisappearingCommand = new RelayCommand(Disappearing);
        }

        public ICommand AppearingCommand { get; }
        public ICommand DisappearingCommand { get; }

        private IMediaSource _mediaSource;
        public IMediaSource MediaSource
        {
            get => _mediaSource;
            private set => Set(nameof(MediaSource), ref _mediaSource, value);
        }

        private void Set<T>(string propertyName, ref T field, T value)
        {
            if (field == null && value != null || field != null && !field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Appearing()
        {
            MediaSource = new MediaSource("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4");
            MediaSource.MediaPlayer.Play();
        }

        private void Disappearing()
        {
            MediaSource.Dispose();
            MediaSource = null;
        }
    }
}
