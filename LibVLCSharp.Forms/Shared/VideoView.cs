using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    public class VideoView : View
    {
        /// <summary>
        /// Raised when a new MediaPlayer is set and will be attached to the view
        /// </summary>
        public event EventHandler<MediaPlayerChangingEventArgs> MediaPlayerChanging;

        /// <summary>
        /// Raised when a new MediaPlayer is set and attached to the view
        /// </summary>
        public event EventHandler<MediaPlayerChangedEventArgs> MediaPlayerChanged;

        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer),
                typeof(LibVLCSharp.Shared.MediaPlayer),
                typeof(VideoView),
                propertyChanging: OnMediaPlayerChanging,
                propertyChanged: OnMediaPlayerChanged);

        /// <summary>
        /// The MediaPlayer object attached to this view
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get { return GetValue(MediaPlayerProperty) as LibVLCSharp.Shared.MediaPlayer; }
            set { SetValue(MediaPlayerProperty, value); }
        }

        private static void OnMediaPlayerChanging(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            Debug.WriteLine("OnMediaPlayerChanging");
            videoView.MediaPlayerChanging?.Invoke(videoView, new MediaPlayerChangingEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
        }

        private static void OnMediaPlayerChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            Debug.WriteLine("OnMediaPlayerChanged");
            videoView.MediaPlayerChanged?.Invoke(videoView, new MediaPlayerChangedEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
        }
    }
}