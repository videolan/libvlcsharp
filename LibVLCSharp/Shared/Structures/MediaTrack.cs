using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct MediaTrackStructure
    {
        internal readonly uint Codec;
        internal readonly uint OriginalFourcc;
        internal readonly int Id;
        internal readonly TrackType TrackType;
        internal readonly int Profile;
        internal readonly int Level;
        internal readonly MediaTrackData Data;
        internal readonly uint Bitrate;
        internal readonly IntPtr Language;
        internal readonly IntPtr Description;
    }

    /// <summary>
    /// Media track information
    /// </summary>
    public readonly struct MediaTrack
    {
        internal MediaTrack(uint codec, uint originalFourcc, int id, TrackType trackType, int profile, 
            int level, MediaTrackData data, uint bitrate, string language, string description)
        {
            Codec = codec;
            OriginalFourcc = originalFourcc;
            Id = id;
            TrackType = trackType;
            Profile = profile;
            Level = level;
            Data = data;
            Bitrate = bitrate;
            Language = language;
            Description = description;
        }
        /// <summary>
        /// Media track codec
        /// </summary>
        public readonly uint Codec;

        /// <summary>
        /// Media track original fourcc
        /// </summary>
        public readonly uint OriginalFourcc;

        /// <summary>
        /// Media track id
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// Media track type
        /// </summary>
        public readonly TrackType TrackType;

        /// <summary>
        /// Media track profile
        /// </summary>
        public readonly int Profile;

        /// <summary>
        /// Media track level
        /// </summary>
        public readonly int Level;

        /// <summary>
        /// Media track data
        /// </summary>
        public readonly MediaTrackData Data;

        /// <summary>
        /// Media track bitrate
        /// </summary>
        public readonly uint Bitrate;

        /// <summary>
        /// Media track language
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// Media track description
        /// </summary>
        public readonly string Description;
    }
}
