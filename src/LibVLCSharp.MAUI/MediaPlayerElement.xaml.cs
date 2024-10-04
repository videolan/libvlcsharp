using System;
using LibVLCSharp.Shared;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// Represents event data for page-related events.
    /// </summary>
    public class PageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the associated <see cref="Page"/> instance.
        /// </summary>
        public Page Page { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageEventArgs"/> class with the specified page.
        /// </summary>
        /// <param name="page">The page associated with the event.</param>
        public PageEventArgs(Page page)
        {
            Page = page;
        }
    }

    /// <summary>
    /// Provides helper methods for handling page lifecycle events.
    /// </summary>
    public static class LifecycleHelper
    {
        private static readonly WeakEventManager _pageAppearingEventManager = new();
        private static readonly WeakEventManager _pageDisappearingEventManager = new();

        /// <summary>
        /// Occurs when a page is appearing.
        /// </summary>
        public static event EventHandler<PageEventArgs> PageAppearing
        {
            add => _pageAppearingEventManager.AddEventHandler(value);
            remove => _pageAppearingEventManager.RemoveEventHandler(value);
        }

        /// <summary>
        /// Occurs when a page is disappearing.
        /// </summary>
        public static event EventHandler<PageEventArgs> PageDisappearing
        {
            add => _pageDisappearingEventManager.AddEventHandler(value);
            remove => _pageDisappearingEventManager.RemoveEventHandler(value);
        }

        /// <summary>
        /// Registers the page lifecycle events to track page appearing and disappearing.
        /// </summary>
        public static void RegisterPageLifecycleEvents()
        {
            if (Application.Current != null)
            {
                Application.Current.PageAppearing += (s, e) => _pageAppearingEventManager.HandleEvent(s ?? Application.Current, new PageEventArgs(e), nameof(PageAppearing));
                Application.Current.PageDisappearing += (s, e) => _pageDisappearingEventManager.HandleEvent(s ?? Application.Current, new PageEventArgs(e), nameof(PageDisappearing));
            }
        }
    }

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
            LifecycleHelper.RegisterPageLifecycleEvents();
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

        /// <summary>
        /// Identifies the <see cref="VideoView"/> dependency property.
        /// </summary>
        private static readonly BindableProperty VideoViewProperty = BindableProperty.Create(nameof(VideoView), typeof(VideoView),
         typeof(MediaPlayerElement), propertyChanged: VideoViewPropertyChanged);

        /// <summary>
        /// Gets or sets the video view.
        /// </summary>
        public VideoView? VideoView
        {
            get => (VideoView)GetValue(VideoViewProperty);
            private set => SetValue(VideoViewProperty, value);
        }

        private static readonly BindableProperty EnableRendererDiscoveryProperty = BindableProperty.Create(nameof(EnableRendererDiscovery),
            typeof(bool), typeof(PlaybackControls), true, propertyChanged: EnableRendererDiscoveryPropertyChanged);

        /// <summary>
        /// Enable or disable renderer discovery.
        /// </summary>
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

                AttachLifecycleEvents();
            }
        }

        private void AttachLifecycleEvents()
        {
            LifecycleHelper.PageAppearing += OnPageAppearing;
            LifecycleHelper.PageDisappearing += OnPageDisappearing;
        }

        /// <summary>
        /// Handle page appearing logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageAppearing(object? sender, EventArgs e)
        {
            if (sender is Page page && page == this.FindAncestor<Page>())
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
        }

        /// <summary>
        /// Handle page disappearing logic
        /// </summary> minimize change.
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageDisappearing(object? sender, EventArgs e)
        {
            if (sender is Page page && page == this.FindAncestor<Page>())
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
        }

        private void GestureRecognized(object sender, EventArgs e)
        {
            PlaybackControls.Show();
        }
    }
}
