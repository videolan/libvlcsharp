using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    public class VideoView : View
    {
        /// <summary>
        /// Raised when a new MediaPlayer is set and attached to the view
        /// </summary>
        public event EventHandler<MediaPlayerChangedEventArgs> MediaPlayerChanged;

        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer), 
                typeof(LibVLCSharp.Shared.MediaPlayer), 
                typeof(VideoView),
                propertyChanged: OnMediaPlayerChanged);

        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get { return GetValue(MediaPlayerProperty) as LibVLCSharp.Shared.MediaPlayer; }
            set { SetValue(MediaPlayerProperty, value); }
        }

        private static void OnMediaPlayerChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            Trace.WriteLine("OnMediaPlayerChanged");
            videoView.MediaPlayerChanged?.Invoke(videoView, new MediaPlayerChangedEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
        }
    }
}