using System.Runtime.InteropServices;

namespace VideoLAN.LibVLC
{
    public partial class Media
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MediaStats
        {
            /* Input */
            public int ReadBytes;
            public float InputBitrate;

            /* Demux */
            public int DemuxReadBytes;
            public float DemuxBitrate;
            public int DemuxCorrupted;
            public int DemuxDiscontinuity;

            /* Decoders */
            public int DecodedVideo;
            public int DecodedAudio;


            /* Video Output */
            public int DisplayedPictures;
            public int LostPictures;

            /* Audio output */
            public int PlayedAudioBuffers;
            public int LostAudioBuffers;

            /* Stream output */
            public int SentPackets;
            public int SentBytes;
            public float SendBitrate;
        }
    }
}