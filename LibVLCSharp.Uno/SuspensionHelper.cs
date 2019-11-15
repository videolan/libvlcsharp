using LibVLCSharp.Shared;
using Windows.Storage;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Suspension helpers methods
    /// </summary>
    public static class SuspensionHelper
    {
        /// <summary>
        /// Saves the movie position
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        /// <param name="identifier">optional identifier</param>
        public static void Save(Shared.MediaPlayer mediaPlayer, string? identifier = null)
        {
            if (mediaPlayer == null)
            {
                return;
            }
            identifier ??= string.Empty;
            var values = ApplicationData.Current.LocalSettings.Values;
            values[$"{identifier}_VLC_MediaPlayer_Position"] = mediaPlayer.Position;
            values[$"{identifier}_VLC_MediaPlayer_IsPlaying"] = mediaPlayer.State == VLCState.Playing;
            mediaPlayer.Stop();
        }

        /// <summary>
        /// Restores the movie position
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        /// <param name="identifier">optional identifier</param>
        public static void Restore(Shared.MediaPlayer mediaPlayer, string? identifier = null)
        {
            if (mediaPlayer == null)
            {
                return;
            }
            identifier ??= string.Empty;
            var values = ApplicationData.Current.LocalSettings.Values;
            if (values.TryGetValue($"{identifier}_VLC_MediaPlayer_IsPlaying", out var play) && play is true)
            {
                mediaPlayer.Play();
            }
            if (values.TryGetValue($"{identifier}_VLC_MediaPlayer_Position", out var p) && p is float position)
            {
                mediaPlayer.Position = position;
            }
        }
    }
}
