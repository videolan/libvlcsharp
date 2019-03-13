
using LibVLCSharp.Forms.Shared;
using LibVLCSharp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibVLCSharp.Forms
{
    /// <summary>
    /// Represents an object that uses a <see cref="LibVLCSharp.Shared.MediaPlayer"/> to render audio and video to the display.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPlayerElement : TemplatedView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerElement"/> class.
        /// </summary>
        public MediaPlayerElement()
        {
            InitializeComponent();
        }

        private static readonly BindableProperty VideoViewProperty = BindableProperty.Create(nameof(VideoView), typeof(VideoView),
            typeof(MediaPlayerElement), propertyChanged: VideoViewPropertyChanged);
        /// <summary>
        /// Gets or sets the video view.
        /// </summary>
        public VideoView VideoView
        {
            get => (VideoView)GetValue(VideoViewProperty);
            set => SetValue(VideoViewProperty, value);
        }

        private static readonly BindableProperty PlaybackControlsProperty = BindableProperty.Create(nameof(PlaybackControls), typeof(PlaybackControls),
             typeof(MediaPlayerElement), propertyChanged: PlaybackControlsPropertyChanged);
        /// <summary>
        /// Gets or sets the playback controls for the media.
        /// </summary>
        public PlaybackControls PlaybackControls
        {
            get => (PlaybackControls)GetValue(PlaybackControlsProperty);
            set => SetValue(PlaybackControlsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property.
        /// </summary>
        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer), typeof(MediaPlayer),
            typeof(MediaPlayerElement), propertyChanged: MediaPlayerPropertyChanged);
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => (MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        private void OnVideoViewChanged(VideoView videoView)
        {
            if (videoView != null)
            {
                videoView.MediaPlayer = MediaPlayer;
            }
        }

        private void OnPlayControlsChanged(PlaybackControls playbackControls)
        {
            if (playbackControls != null)
            {
                playbackControls.MediaPlayer = MediaPlayer;
            }
        }

        private void OnMediaPlayerChanged(MediaPlayer mediaPlayer)
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

        private static void VideoViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnVideoViewChanged((VideoView)newValue);
        }

        private static void PlaybackControlsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnPlayControlsChanged((PlaybackControls)newValue);
        }

        private static void MediaPlayerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnMediaPlayerChanged((MediaPlayer)newValue);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Element.Parent"/> of an element is set. 
        /// Implement this method in order to add behavior when the element is added to a parent.
        /// </summary>
        /// <remarks>Implementors must call the base method.</remarks>
        protected override void OnParentSet()
        {
            base.OnParentSet();
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
}
