using System;
using LibVLCSharp.Shared;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LibVLCSharp.MAUI.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPlayerElement : ContentView
    {
        private readonly WeakEventManager _weakEventManager = new WeakEventManager();

        public MediaPlayerElement()
        {
            InitializeComponent();
        }

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

                Application.Current.ModalPushed += OnAppModalPushed;
                Application.Current.ModalPopped += OnAppModalPopped;
            }
        }

        private void OnAppModalPushed(object sender, EventArgs e)
        {
            var page = this.FindAncestor<Page>();
            if (page != null)
            {
                _weakEventManager.AddEventHandler<EventArgs>(OnSleep, nameof(Application.ModalPushed));
            }
        }

        private void OnAppModalPopped(object sender, EventArgs e)
        {
            var page = this.FindAncestor<Page>();
            if (page != null)
            {
                _weakEventManager.AddEventHandler<EventArgs>(OnResume, nameof(Application.ModalPopped));
            }
        }

        private void OnSleep(object sender, EventArgs e)
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                Preferences.Set($"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_Position", mediaPlayer.Position);
                Preferences.Set($"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_IsPlaying", mediaPlayer.State == VLCState.Playing);
                mediaPlayer.Stop();
            }
            VideoView = null;
        }

        private void OnResume(object sender, EventArgs e)
        {
            VideoView = new VideoView();
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                if (Preferences.Get($"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_IsPlaying", false))
                {
                    mediaPlayer.Play();
                    mediaPlayer.Position = Preferences.Get($"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_Position", 0f);
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
                Console.WriteLine($"Error in GestureRecognized: {ex.Message}");
            }
        }
    }
}
