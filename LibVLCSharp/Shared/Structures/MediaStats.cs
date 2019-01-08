using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Statistics of a Media
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MediaStats
    {
        /* Input */
        public readonly int ReadBytes;
        public readonly float InputBitrate;

        /* Demux */
        public readonly int DemuxReadBytes;
        public readonly float DemuxBitrate;
        public readonly int DemuxCorrupted;
        public readonly int DemuxDiscontinuity;

        /* Decoders */
        public readonly int DecodedVideo;
        public readonly int DecodedAudio;

        /* Video Output */
        public readonly int DisplayedPictures;
        public readonly int LostPictures;

        /* Audio output */
        public readonly int PlayedAudioBuffers;
        public readonly int LostAudioBuffers;

        /* Stream output */
        public readonly int SentPackets;
        public readonly int SentBytes;
        public readonly float SendBitrate;
    }
}
