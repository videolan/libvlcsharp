using System;
using System.Linq;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Aspect ratio manager
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.VideoView"/> and <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> properties
    /// need to be set in order to work</remarks>
    internal class AspectRatioManager : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Occurs when <see cref="AspectRatio"/> property value changes
        /// </summary>
        public event EventHandler? AspectRatioChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="AspectRatioManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        /// <param name="displayInformation">display information</param>
        public AspectRatioManager(IDispatcher? dispatcher, IDisplayInformation displayInformation) : base(dispatcher)
        {
            DisplayInformation = displayInformation;
        }

        private IDisplayInformation DisplayInformation { get; }

        private AspectRatio? _aspectRatio = null;
        /// <summary>
        /// Gets the aspect ratio
        /// </summary>
        public AspectRatio AspectRatio
        {
            get => _aspectRatio ?? AspectRatio.BestFit;
            set { UpdateAspectRatio(value); }
        }

        /// <summary>
        /// Called when <see cref="MediaPlayerElementManagerBase.VideoView"/> property value changes
        /// </summary>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        protected override void OnVideoViewChanged(IVideoControl? oldValue, IVideoControl? newValue)
        {
            base.OnVideoViewChanged(oldValue, newValue);
            if (oldValue != null)
            {
                oldValue.SizeChanged -= VideoView_SizeChanged;
            }
            if (newValue != null)
            {
                newValue.SizeChanged += VideoView_SizeChanged;
            }
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(Shared.MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            mediaPlayer.ESSelected += MediaPlayer_ESSelectedAsync;
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void UnsubscribeEvents(Shared.MediaPlayer mediaPlayer)
        {
            base.UnsubscribeEvents(mediaPlayer);
            mediaPlayer.ESSelected -= MediaPlayer_ESSelectedAsync;
        }

        private async void MediaPlayer_ESSelectedAsync(object? sender, MediaPlayerESSelectedEventArgs e)
        {
            await DispatcherInvokeAsync(() => UpdateAspectRatio());
        }

        private void VideoView_SizeChanged(object? sender, EventArgs args)
        {
            UpdateAspectRatio();
        }

        private VideoTrack? GetVideoTrack(Shared.MediaPlayer mediaPlayer)
        {
            if (mediaPlayer == null)
            {
                return null;
            }
            var selectedVideoTrack = mediaPlayer.VideoTrack;
            if (selectedVideoTrack == -1)
            {
                return null;
            }

            try
            {
                var media = mediaPlayer.Media;
                MediaTrack? videoTrack = null;
                if (media != null)
                {
                    videoTrack = media.Tracks?.FirstOrDefault(t => t.Id == selectedVideoTrack);
                    media.Dispose();
                }
                return videoTrack == null ? (VideoTrack?)null : ((MediaTrack)videoTrack).Data.Video;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool IsVideoSwapped(VideoTrack videoTrack)
        {
            var orientation = videoTrack.Orientation;
            return orientation == VideoOrientation.LeftBottom || orientation == VideoOrientation.RightTop;
        }

        private AspectRatio? GetAspectRatio(Shared.MediaPlayer? mediaPlayer)
        {
            if (mediaPlayer == null)
            {
                return null;
            }

            var aspectRatio = mediaPlayer.AspectRatio;
            return aspectRatio == null ?
                (mediaPlayer.Scale == 0 ? AspectRatio.BestFit : (mediaPlayer.Scale == 1 ? AspectRatio.Original : AspectRatio.FitScreen)) :
                (aspectRatio == "4:3" ? AspectRatio._4_3 : aspectRatio == "16:9" ? AspectRatio._16_9 : AspectRatio.Fill);
        }

        private void UpdateAspectRatio(AspectRatio? aspectRatio = null)
        {
            var mediaPlayer = MediaPlayer;
            var videoView = VideoView;
            if (aspectRatio == null)
            {
                aspectRatio = _aspectRatio ?? GetAspectRatio(mediaPlayer);
                if (aspectRatio == null)
                {
                    return;
                }
            }
            if (videoView != null && mediaPlayer != null)
            {
                switch (aspectRatio)
                {
                    case AspectRatio.Original:
                        mediaPlayer.AspectRatio = null;
                        mediaPlayer.Scale = 1;
                        break;
                    case AspectRatio.Fill:
                        var videoTrack = GetVideoTrack(mediaPlayer);
                        if (videoTrack == null)
                        {
                            break;
                        }
                        mediaPlayer.Scale = 0;
                        mediaPlayer.AspectRatio = IsVideoSwapped((VideoTrack)videoTrack) ? $"{videoView.Height}:{videoView.Width}" :
                            $"{videoView.Width}:{videoView.Height}";
                        break;
                    case AspectRatio.BestFit:
                        mediaPlayer.AspectRatio = null;
                        mediaPlayer.Scale = 0;
                        break;
                    case AspectRatio.FitScreen:
                        videoTrack = GetVideoTrack(mediaPlayer);
                        if (videoTrack == null)
                        {
                            break;
                        }
                        var track = (VideoTrack)videoTrack;
                        var videoSwapped = IsVideoSwapped(track);
                        var videoWidth = videoSwapped ? track.Height : track.Width;
                        var videoHeigth = videoSwapped ? track.Width : track.Height;
                        if (videoWidth == 0 || videoHeigth == 0)
                        {
                            mediaPlayer.Scale = 0;
                        }
                        else
                        {
                            if (track.SarNum != track.SarDen)
                            {
                                videoWidth = videoWidth * track.SarNum / track.SarDen;
                            }

                            var ar = videoWidth / (double)videoHeigth;
                            var videoViewWidth = videoView.Width;
                            var videoViewHeight = videoView.Height;
                            var dar = videoViewWidth / videoViewHeight;

                            var rawPixelsPerViewPixel = DisplayInformation.ScalingFactor;
                            var displayWidth = videoViewWidth * rawPixelsPerViewPixel;
                            var displayHeight = videoViewHeight * rawPixelsPerViewPixel;
                            mediaPlayer.Scale = (float)(dar >= ar ? (displayWidth / videoWidth) : (displayHeight / videoHeigth));
                        }
                        mediaPlayer.AspectRatio = null;
                        break;
                    case AspectRatio._16_9:
                        mediaPlayer.AspectRatio = "16:9";
                        mediaPlayer.Scale = 0;
                        break;
                    case AspectRatio._4_3:
                        mediaPlayer.AspectRatio = "4:3";
                        mediaPlayer.Scale = 0;
                        break;
                }
            }

            if (_aspectRatio != aspectRatio)
            {
                _aspectRatio = aspectRatio;
                AspectRatioChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
