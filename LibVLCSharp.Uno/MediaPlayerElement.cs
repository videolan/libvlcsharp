using System;
using LibVLCSharp.Shared;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Represents an object that uses a <see cref="Shared.MediaPlayer"/> to render audio and video to the display
    /// </summary>
    public partial class MediaPlayerElement : ContentControl
    {
        /// <summary>
        /// Occurs when the <see cref="MediaPlayerElement"/> is fully loaded
        /// </summary>
        public event EventHandler<InitializedEventArgs>? Initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerElement"/> class
        /// </summary>
        public MediaPlayerElement()
        {
            DefaultStyleKey = typeof(MediaPlayerElement);
            SizeChanged += (sender, args) => AspectRatioManager.UpdateAspectRatio(VideoView, MediaPlayer);
        }

        private AspectRatioManager AspectRatioManager { get; } = new AspectRatioManager();
        private VideoView? VideoView { get; set; }

        /// <summary>
        /// Identifies the <see cref="AreTransportControlsEnabled"/> dependency property
        /// </summary>
        public static readonly DependencyProperty AreTransportControlsEnabledProperty = DependencyProperty.Register(
            nameof(AreTransportControlsEnabled), typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value that determines whether the standard transport controls are enabled
        /// </summary>
        public bool AreTransportControlsEnabled
        {
            get => (bool)GetValue(AreTransportControlsEnabledProperty);
            set => SetValue(AreTransportControlsEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property
        /// </summary>
        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(nameof(MediaPlayer), typeof(Shared.MediaPlayer),
            typeof(MediaPlayerElement), new PropertyMetadata(null, (d, args) =>
                ((MediaPlayerElement)d).OnMediaPlayerChanged((Shared.MediaPlayer)args.OldValue, (Shared.MediaPlayer)args.NewValue)));
        /// <summary>
        /// Gets the <see cref="Shared.MediaPlayer"/> instance
        /// </summary>
        public Shared.MediaPlayer? MediaPlayer
        {
            get => (Shared.MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TransportControls"/> dependency property
        /// </summary>
        private static readonly DependencyProperty TransportControlsProperty = DependencyProperty.Register(nameof(TransportControls),
            typeof(MediaTransportControls), typeof(MediaPlayerElement),
            new PropertyMetadata(new MediaTransportControls(),
                (d, args) => ((MediaPlayerElement)d).OnTransportControlsChanged((MediaTransportControls)args.OldValue)));
        /// <summary>
        /// Gets or sets the transport controls for the media
        /// </summary>
        public MediaTransportControls? TransportControls
        {
            get => (MediaTransportControls)GetValue(TransportControlsProperty);
            set => SetValue(TransportControlsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property.
        /// </summary>
        public static DependencyProperty StretchProperty { get; } = DependencyProperty.Register(nameof(Stretch), typeof(Stretch),
            typeof(MediaPlayerElement), new PropertyMetadata(Stretch.Uniform, (d, args) => ((MediaPlayerElement)d).OnStretchChanged()));
        /// <summary>
        /// Gets or sets a value that describes how a <see cref="MediaPlayerElement"/> should be stretched to fill the destination rectangle
        /// </summary>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="Control.ApplyTemplate"/>.
        /// In simplest terms, this means the method is called just before a UI element displays in your app
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VideoView = (VideoView)GetTemplateChild("VideoView");
            VideoView.Initialized += (sender, e) => Initialized?.Invoke(this, e);
            if (GetTemplateChild("ContentPresenter") is UIElement contentGrid)
            {
                contentGrid.PointerEntered += OnPointerMoved;
                contentGrid.PointerMoved += OnPointerMoved;
                contentGrid.Tapped += OnPointerMoved;
            }
            OnStretchChanged();
            OnMediaPlayerChanged();
        }

        private void OnPointerMoved(object sender, RoutedEventArgs e)
        {
            TransportControls?.Show();
        }

        private void OnMediaPlayerChanged(Shared.MediaPlayer oldValue, Shared.MediaPlayer newValue)
        {
            if (oldValue != null)
            {
                oldValue.ESSelected -= MediaPlayer_ESSelectedAsync;
            }
            if (newValue != null)
            {
                newValue.ESSelected += MediaPlayer_ESSelectedAsync;
            }
            OnMediaPlayerChanged();
        }

        private void OnMediaPlayerChanged()
        {
            OnStretchChanged();
            OnTransportControlsChanged();
        }

        private void OnTransportControlsChanged(MediaTransportControls? oldValue = null)
        {
            if (oldValue != null)
            {
                oldValue.VideoView = null;
            }

            var transportControls = TransportControls;
            if (transportControls != null)
            {
                transportControls.VideoView = VideoView;
                transportControls.MediaPlayer = MediaPlayer;
            }
        }

        private async void MediaPlayer_ESSelectedAsync(object sender, MediaPlayerESSelectedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, OnStretchChanged);
        }

        private void OnStretchChanged()
        {
            AspectRatioManager.UpdateAspectRatio(VideoView, MediaPlayer, Stretch);
        }
    }
}
