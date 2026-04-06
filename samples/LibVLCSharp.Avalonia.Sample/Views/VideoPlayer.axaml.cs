using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Avalonia.Sample.Views
{
    public partial class VideoPlayer : UserControl
    {
        private MediaPlayer? _mediaPlayer = null;

        public static readonly DirectProperty<VideoPlayer, MediaPlayer?> MediaPlayerProperty =
            VideoView.MediaPlayerProperty.AddOwner<VideoPlayer>(
                getter: o => o.MediaPlayer,
                setter: (o, v) => o.MediaPlayer = v,
                defaultBindingMode: BindingMode.TwoWay);

        public MediaPlayer? MediaPlayer
        {
            get => _mediaPlayer;
            set => SetAndRaise(MediaPlayerProperty, ref _mediaPlayer, value);
        }

        public VideoPlayer()
        {
            InitializeComponent();
        }

        //We can forward pointer events (or any other as needed) from VideoView to VideoPlayer or just handle them using the child element of VideoPlayer.
        /// <vlc:VideoView Content="{TemplateBinding Content}"
        ///                PointerEntered="VideoView_PointerEvent"
        ///                MediaPlayer="{TemplateBinding MediaPlayer}"/>
        //private void VideoView_PointerEvent(object? sender, PointerEventArgs e)
        //{
        //    this.RaiseEvent(e);
        //}
    }
}
