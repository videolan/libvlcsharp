﻿using LibVLCSharp.Shared;
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

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ISource), typeof(VideoView),
            propertyChanged: OnSourceChanged);
        public ISource Source
        {
            get { return GetValue(SourceProperty) as ISource; }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            videoView.SourceChanged?.Invoke(videoView, new SourceChangedEventArgs(oldValue as ISource, newValue as ISource));
        }
    }
}