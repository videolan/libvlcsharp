using System;
using System.Linq;
using LibVLCSharp.Shared;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Aspect ratio manager
    /// </summary>
    internal class AspectRatioManager
    {
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
                var videoTrack = mediaPlayer.Media?.Tracks?.FirstOrDefault(t => t.Id == selectedVideoTrack);
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

        /// <summary>
        /// Gets current stretch
        /// </summary>
        /// <param name="mediaPlayer">media player instance</param>
        /// <returns>the current stretch</returns>
        public Stretch GetStretch(Shared.MediaPlayer mediaPlayer)
        {
            return mediaPlayer.AspectRatio == null ?
                (mediaPlayer.Scale == 0 ? Stretch.Uniform : (mediaPlayer.Scale == 1 ? Stretch.None : Stretch.UniformToFill)) :
                Stretch.Fill;
        }

        /// <summary>
        /// Updates aspect ratio
        /// </summary>
        /// <param name="videoView">video view instance</param>
        /// <param name="mediaPlayer">media player instance</param>
        /// <param name="stretch">aspect ratio to apply</param>
        public void UpdateAspectRatio(FrameworkElement? videoView, Shared.MediaPlayer? mediaPlayer, Stretch? stretch = null)
        {
            if (videoView == null || mediaPlayer == null)
            {
                return;
            }

            if (stretch == null)
            {
                stretch = GetStretch(mediaPlayer);
                if (stretch == Stretch.None || stretch == Stretch.Uniform)
                {
                    return;
                }
            }
            switch (stretch)
            {
                case Stretch.None:
                    mediaPlayer.AspectRatio = null;
                    mediaPlayer.Scale = 1;
                    break;
                case Stretch.Fill:
                    var videoTrack = GetVideoTrack(mediaPlayer);
                    if (videoTrack == null)
                    {
                        return;
                    }
                    mediaPlayer.Scale = 0;
                    mediaPlayer.AspectRatio = IsVideoSwapped((VideoTrack)videoTrack) ? $"{videoView.ActualHeight}:{videoView.ActualWidth}" :
                        $"{videoView.ActualWidth}:{videoView.ActualHeight}";
                    break;
                case Stretch.Uniform:
                    mediaPlayer.AspectRatio = null;
                    mediaPlayer.Scale = 0;
                    break;
                case Stretch.UniformToFill:
                    videoTrack = GetVideoTrack(mediaPlayer);
                    if (videoTrack == null)
                    {
                        return;
                    }
                    var track = (VideoTrack)videoTrack;
                    var videoSwapped = IsVideoSwapped(track);
                    var videoWidth = videoSwapped ? track.Height : track.Width;
                    var videoHeigth = videoSwapped ? track.Width : track.Height;
                    if (track.SarNum != track.SarDen)
                    {
                        videoWidth = videoWidth * track.SarNum / track.SarDen;
                    }

                    var ar = videoWidth / (double)videoHeigth;
                    var videoViewWidth = videoView.ActualWidth;
                    var videoViewHeight = videoView.ActualHeight;
                    var dar = videoViewWidth / videoViewHeight;

                    var rawPixelsPerViewPixel = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                    var displayWidth = videoViewWidth * rawPixelsPerViewPixel;
                    var displayHeight = videoViewHeight * rawPixelsPerViewPixel;
                    mediaPlayer.Scale = (float)(dar >= ar ? (displayWidth / videoWidth) : (displayHeight / videoHeigth));
                    mediaPlayer.AspectRatio = null;
                    break;
            }
        }
    }
}
