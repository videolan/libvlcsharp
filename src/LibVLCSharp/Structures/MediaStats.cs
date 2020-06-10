using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// Statistics of a Media
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MediaStats
    {
        /// <summary>
        /// The number of bytes read
        /// </summary>
        public readonly int ReadBytes;

        /// <summary>
        /// The input bitrate
        /// </summary>
        public readonly float InputBitrate;

        /// <summary>
        /// The number of bytes read by the demux
        /// </summary>
        public readonly int DemuxReadBytes;

        /// <summary>
        /// The demux bitrate
        /// </summary>
        public readonly float DemuxBitrate;

        /// <summary>
        /// The number of frame discarded
        /// </summary>
        public readonly int DemuxCorrupted;

        /// <summary>
        /// The number of frame dropped
        /// </summary>
        public readonly int DemuxDiscontinuity;

        /// <summary>
        /// The number of decoded video blocks
        /// </summary>
        public readonly int DecodedVideo;

        /// <summary>
        /// The number of decoded audio blocks
        /// </summary>
        public readonly int DecodedAudio;

        /// <summary>
        /// The number of frames displayed
        /// </summary>
        public readonly int DisplayedPictures;

        /// <summary>
        /// The number of frames lost
        /// </summary>
        public readonly int LostPictures;

        /// <summary>
        /// The number of buffers played
        /// </summary>
        public readonly int PlayedAudioBuffers;

        /// <summary>
        /// The number of buffers lost
        /// </summary>
        public readonly int LostAudioBuffers;
    }
}
