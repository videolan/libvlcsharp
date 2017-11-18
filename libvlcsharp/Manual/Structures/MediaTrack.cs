using System;

namespace VideoLAN.LibVLC
{
    public partial class Media
    {
        public struct MediaTrack
        {
            public uint Codec;
            public uint OriginalFourcc;
            public int Id;
            public TrackType TrackType;
            public int Profile;
            public int Level;
            public MediaTrackData Data;
            public uint Bitrate;
            public IntPtr Language;
            public IntPtr Description;
        }
    }
}