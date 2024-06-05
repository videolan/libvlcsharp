using System;
using LibVLCSharp.Shared;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace LibVLCSharp.MAUI.Shared
{
    public static class LifecycleHelper
    {
        private static readonly WeakEventManager _pageAppearingEventManager = new();
        private static readonly WeakEventManager _pageDisappearingEventManager = new();

        public static event EventHandler PageAppearing
        {
            add => _pageAppearingEventManager.AddEventHandler(value);
            remove => _pageAppearingEventManager.RemoveEventHandler(value);
        }

        public static event EventHandler PageDisappearing
        {
            add => _pageDisappearingEventManager.AddEventHandler(value);
            remove => _pageDisappearingEventManager.RemoveEventHandler(value);
        }

        public static void RegisterPageLifecycleEvents()
        {
            if (Application.Current != null)
            {
                Application.Current.PageAppearing += (s, e) => _pageAppearingEventManager.HandleEvent(s, e, nameof(PageAppearing));
                Application.Current.PageDisappearing += (s, e) => _pageDisappearingEventManager.HandleEvent(s, e, nameof(PageDisappearing));
            }
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPlayerElement : ContentView
    {
        private bool Initialized { get; set; }

        public static readonly BindableProperty LibVLCProperty = BindableProperty.Create(nameof(LibVLC), typeof(LibVLC),
            typeof(MediaPlayerElement), propertyChanged: LibVLCPropertyChanged);

        public LibVLC LibVLC
        {
            get => (LibVLC)GetValue(LibVLCProperty);
            set => SetValue(LibVLCProperty, value);
        }

        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer),
            typeof(LibVLCSharp.Shared.MediaPlayer), typeof(MediaPlayerElement), propertyChanged: MediaPlayerPropertyChanged);

        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get => (LibVLCSharp.Shared.MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        private static readonly BindableProperty PlaybackControlsProperty = BindableProperty.Create(nameof(PlaybackControls),
            typeof(PlaybackControls), typeof(MediaPlayerElement), propertyChanged: PlaybackControlsPropertyChanged);

        public PlaybackControls PlaybackControls
        {
            get => (PlaybackControls)GetValue(PlaybackControlsProperty);
            set => SetValue(PlaybackControlsProperty, value);
        }

        private static readonly BindableProperty VideoViewProperty = BindableProperty.Create(nameof(VideoView), typeof(VideoView),
            typeof(MediaPlayerElement), propertyChanged: VideoViewPropertyChanged);

        public VideoView? VideoView
        {
            get => (VideoView)GetValue(VideoViewProperty);
            private set => SetValue(VideoViewProperty, value);
        }

        private static readonly BindableProperty EnableRendererDiscoveryProperty = BindableProperty.Create(nameof(EnableRendererDiscovery),
            typeof(bool), typeof(PlaybackControls), true, propertyChanged: EnableRendererDiscoveryPropertyChanged);

        public bool EnableRendererDiscovery
        {
            get => (bool)GetValue(EnableRendererDiscoveryProperty);
            set => SetValue(EnableRendererDiscoveryProperty, value);
        }

        public MediaPlayerElement()
        {
            InitializeComponent();
            AttachLifecycleEvents();
        }

        private void AttachLifecycleEvents()
        {
            LifecycleHelper.RegisterPageLifecycleEvents();
        }

        private void OnPageAppearing()
        {
            var currentPage = Application.Current?.MainPage;
            if (currentPage != null && currentPage == this.FindAncestor<Page>())
            {
                if (VideoView == null)
                {
                    VideoView = new VideoView();
                }

                if (PlaybackControls == null)
                {
                    PlaybackControls = new PlaybackControls();
                }
            }
        }

        private void OnPageDisappearing()
        {
            var currentPage = Application.Current?.MainPage;
            if (currentPage != null && currentPage == this.FindAncestor<Page>())
            {
                var mediaPlayer = MediaPlayer;
                if (mediaPlayer != null)
                {
                    var keyPrefix = $"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement";
                    Preferences.Set($"{keyPrefix}_Position", mediaPlayer.Position);
                    Preferences.Set($"{keyPrefix}_IsPlaying", mediaPlayer.State == VLCState.Playing);
                    mediaPlayer.Stop();
                }
                VideoView = null;
            }
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
                playbackControls.LibVLC = libVLC;
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
                playbackControls.IsCastButtonVisible = EnableRendererDiscovery;
                playbackControls.LibVLC = LibVLC;
                playbackControls.MediaPlayer = MediaPlayer;
                playbackControls.VideoView = VideoView;
            }
        }

        private void OnEnableRendererDiscoveryChanged(bool enableRendererDiscovery)
        {
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.IsCastButtonVisible = enableRendererDiscovery;
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

        private static void EnableRendererDiscoveryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnEnableRendererDiscoveryChanged((bool)newValue);
        }

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
            }
        }

        private void GestureRecognized(object? sender, EventArgs e)
        {
            try
            {
                PlaybackControls.Show();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
