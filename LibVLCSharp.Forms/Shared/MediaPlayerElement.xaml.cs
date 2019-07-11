﻿using System;
using LibVLCSharp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Represents an object that uses a <see cref="LibVLCSharp.Shared.MediaPlayer"/> to render audio and video to the display.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPlayerElement : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerElement"/> class.
        /// </summary>
        public MediaPlayerElement()
        {
            InitializeComponent();
        }

        private bool Initialized { get; set; }

        /// <summary>
        /// Identifies the <see cref="LibVLC"/> dependency property.
        /// </summary>
        public static readonly BindableProperty LibVLCProperty = BindableProperty.Create(nameof(LibVLC), typeof(LibVLC),
            typeof(MediaPlayerElement), propertyChanged: LibVLCPropertyChanged);
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
        /// </summary>
        public LibVLC LibVLC
        {
            get => (LibVLC)GetValue(LibVLCProperty);
            set => SetValue(LibVLCProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property.
        /// </summary>
        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer),
            typeof(LibVLCSharp.Shared.MediaPlayer), typeof(MediaPlayerElement), propertyChanged: MediaPlayerPropertyChanged);
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get => (LibVLCSharp.Shared.MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        private static readonly BindableProperty PlaybackControlsProperty = BindableProperty.Create(nameof(PlaybackControls),
            typeof(PlaybackControls), typeof(MediaPlayerElement), propertyChanged: PlaybackControlsPropertyChanged);
        /// <summary>
        /// Gets or sets the playback controls for the media.
        /// </summary>
        public PlaybackControls PlaybackControls
        {
            get => (PlaybackControls)GetValue(PlaybackControlsProperty);
            set => SetValue(PlaybackControlsProperty, value);
        }

        private static readonly BindableProperty VideoViewProperty = BindableProperty.Create(nameof(VideoView), typeof(VideoView),
         typeof(MediaPlayerElement), propertyChanged: VideoViewPropertyChanged);
        /// <summary>
        /// Gets or sets the video view.
        /// </summary>
        public VideoView VideoView
        {
            get => (VideoView)GetValue(VideoViewProperty);
            private set => SetValue(VideoViewProperty, value);
        }

        private void OnVideoViewChanged(VideoView videoView)
        {
            if (videoView != null)
            {
                videoView.MediaPlayer = MediaPlayer;
                var playbackControls = PlaybackControls;
                if (playbackControls != null)
                {
                    playbackControls.VideoView = videoView;
                }
            }
        }

        private void OnLibVLCChanged(LibVLC libVLC)
        {
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.LibVLC = LibVLC;
            }
        }

        private void OnMediaPlayerChanged(LibVLCSharp.Shared.MediaPlayer mediaPlayer)
        {
            var videoView = VideoView;
            if (videoView != null)
            {
                videoView.MediaPlayer = mediaPlayer;
            }
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.MediaPlayer = mediaPlayer;
            }
        }

        private void OnPlayControlsChanged(PlaybackControls playbackControls)
        {
            if (playbackControls != null)
            {
                playbackControls.MediaPlayer = MediaPlayer;
                playbackControls.VideoView = VideoView;
            }
        }

        private static void VideoViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnVideoViewChanged((VideoView)newValue);
        }

        private static void LibVLCPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnLibVLCChanged((LibVLC)newValue);
        }

        private static void MediaPlayerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnMediaPlayerChanged((LibVLCSharp.Shared.MediaPlayer)newValue);
        }

        private static void PlaybackControlsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnPlayControlsChanged((PlaybackControls)newValue);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Element.Parent"/> of an element is set. 
        /// Implement this method in order to add behavior when the element is added to a parent.
        /// </summary>
        /// <remarks>Implementors must call the base method.</remarks>
        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null && !Initialized)
            {
                Initialized = true;

                if (VideoView == null)
                {
                    VideoView = new VideoView();
                }
                if (PlaybackControls == null)
                {
                    PlaybackControls = new PlaybackControls();
                }

                var application = Application.Current;
                application.PageAppearing += PageAppearing;
                application.PageDisappearing += PageDisappearing;
            }
        }

        private void PageAppearing(object sender, Page e)
        {
            if (e == this.FindAncestor<Page>())
            {
                MessagingCenter.Subscribe<LifecycleMessage>(this, "OnSleep", m =>
                {
                    var applicationProperties = Application.Current.Properties;
                    applicationProperties["VLC_MediaPlayerElement_Position"] = MediaPlayer?.Position;
                    applicationProperties["VLC_MediaPlayerElement_IsPlaying"] = MediaPlayer?.State == VLCState.Playing;
                    MediaPlayer?.Stop();
                    VideoView = null;
                });
                MessagingCenter.Subscribe<LifecycleMessage>(this, "OnResume", m =>
                {
                    VideoView = new VideoView();
                    var mediaPlayer = MediaPlayer;
                    if (mediaPlayer != null)
                    {
                        var applicationProperties = Application.Current.Properties;
                        if (applicationProperties["VLC_MediaPlayerElement_IsPlaying"] is bool play && play)
                        {
                            mediaPlayer.Play();
                            mediaPlayer.Position = applicationProperties["VLC_MediaPlayerElement_Position"] is float position ? position : 0;
                        }
                    }
                });
            }
        }

        private void PageDisappearing(object sender, Page e)
        {
            if (e == this.FindAncestor<Page>())
            {
                MessagingCenter.Unsubscribe<LifecycleMessage>(this, "OnSleep");
                MessagingCenter.Unsubscribe<LifecycleMessage>(this, "OnResume");
            }
        }

        private async void GestureRecognized(object sender, EventArgs e)
        {
            await PlaybackControls.FadeInAsync();
        }
    }
}
