using LibVLCSharp.Shared;
using System;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    public class VideoView : View
    {
        public event EventHandler<SourceChangedEventArgs> SourceChanged;

        public VideoView()
        {
        }

        public static readonly BindableProperty MediaSourceProperty = BindableProperty.Create(nameof(Source), typeof(IMediaSource), typeof(VideoView),
            propertyChanged: OnSourceChanged);
        public IMediaSource Source
        {
            get { return GetValue(MediaSourceProperty) as IMediaSource; }
            set { SetValue(MediaSourceProperty, value); }
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            videoView.SourceChanged?.Invoke(videoView, new SourceChangedEventArgs(oldValue as IMediaSource, newValue as IMediaSource));
        }
    }
}