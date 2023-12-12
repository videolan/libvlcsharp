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
        public readonly ulong ReadBytes;

        /// <summary>
        /// The input bitrate
        /// </summary>
        public readonly float InputBitrate;

        /// <summary>
        /// The number of bytes read by the demux
        /// </summary>
        public readonly ulong DemuxReadBytes;

        /// <summary>
        /// The demux bitrate
        /// </summary>
        public readonly float DemuxBitrate;

        /// <summary>
        /// The number of frame discarded
        /// </summary>
        public readonly ulong DemuxCorrupted;

        /// <summary>
        /// The number of frame dropped
        /// </summary>
        public readonly ulong DemuxDiscontinuity;

        /// <summary>
        /// The number of decoded video blocks
        /// </summary>
        public readonly ulong DecodedVideo;

        /// <summary>
        /// The number of decoded audio blocks
        /// </summary>
        public readonly ulong DecodedAudio;

        /// <summary>
        /// The number of frames displayed
        /// </summary>
        public readonly ulong DisplayedPictures;

        /// <summary>
        /// The number of late frames
        /// </summary>
        public readonly ulong LatePictures;

        /// <summary>
        /// The number of frames lost
        /// </summary>
        public readonly ulong LostPictures;

        /// <summary>
        /// The number of buffers played
        /// </summary>
        public readonly ulong PlayedAudioBuffers;

        /// <summary>
        /// The number of buffers lost
        /// </summary>
        public readonly ulong LostAudioBuffers;
    }
}
