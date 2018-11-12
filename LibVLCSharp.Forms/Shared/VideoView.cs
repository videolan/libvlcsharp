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

        /// <summary>
        /// Raised after the first mediaplayer has been set. 
        /// This is mostly needed with LibVLCSharp.Forms.WPF to prevent timing issues regarding HWND creation. 
        /// It is safe to call Play() when Loaded has been raised.
        /// </summary>
        public event EventHandler Loaded;

        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer), 
                typeof(LibVLCSharp.Shared.MediaPlayer), 
                typeof(VideoView),
                propertyChanged: OnMediaPlayerChanged);

        /// <summary>
        /// The MediaPlayer object attached to this view
        /// </summary>
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
            
            if(oldValue == null && newValue != null) // assuming this is the first MediaPlayer set
                videoView.Loaded?.Invoke(videoView, EventArgs.Empty);
        }
    }
}