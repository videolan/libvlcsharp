using System;
using System.Diagnostics;
using LibVLCSharp;

namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// Generic MAUI VideoView
    /// </summary>
    public class VideoView : View, IVideoControl
    {
        /// <summary>
        /// Raised when a new MediaPlayer is set and will be attached to the view
        /// </summary>
        public event EventHandler<MediaPlayerChangingEventArgs>? MediaPlayerChanging;

        /// <summary>
        /// Raised when a new MediaPlayer is set and attached to the view
        /// </summary>
        public event EventHandler<MediaPlayerChangedEventArgs>? MediaPlayerChanged;

        /// <summary>
        /// Xamarin.Forms MediaPlayer databinded property
        /// </summary>
        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer),
                typeof(MediaPlayer),
                typeof(VideoView),
                propertyChanging: OnMediaPlayerChanging,
                propertyChanged: OnMediaPlayerChanged);

        /// <summary>
        /// The MediaPlayer object attached to this view
        /// </summary>
        public MediaPlayer? MediaPlayer
        {
            get { return GetValue(MediaPlayerProperty) as MediaPlayer; }
            set { SetValue(MediaPlayerProperty, value); }
        }

        private static void OnMediaPlayerChanging(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            Debug.WriteLine("OnMediaPlayerChanging");
            videoView.MediaPlayerChanging?.Invoke(videoView, new MediaPlayerChangingEventArgs(oldValue as MediaPlayer, newValue as MediaPlayer));
        }

        private static void OnMediaPlayerChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            Debug.WriteLine("OnMediaPlayerChanged");
            videoView.MediaPlayerChanged?.Invoke(videoView, new MediaPlayerChangedEventArgs(oldValue as MediaPlayer, newValue as MediaPlayer));
        }
    }
}
