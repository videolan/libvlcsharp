using LibVLCSharp.Shared;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    public class VideoView : View
    {
        /// <summary>
        /// Constructor with extra configuration
        /// </summary>
        /// <param name="cliOptions">https://wiki.videolan.org/VLC_command-line_help/</param>
        public VideoView(string[] cliOptions)
        {
            CliOptions = cliOptions;
        }

        public VideoView()
        {
        }

        public string[] CliOptions { get; }

        public LibVLCSharp.Shared.MediaPlayer MediaPlayer { get; set; }
        public Instance Instance { get; set; }
    }
}