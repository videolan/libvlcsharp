using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibVLCSharp.Helpers;

namespace LibVLCSharp
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct MediaTrackStructure
    {
        internal readonly uint Codec;
        internal readonly uint OriginalFourcc;
        internal readonly int Id; // deprecated from v4
        internal readonly TrackType TrackType;
        internal readonly int Profile;
        internal readonly int Level;
        internal readonly IntPtr TrackData;
        internal readonly uint Bitrate;
        internal readonly IntPtr Language;
        internal readonly IntPtr Description;
        internal readonly IntPtr StrId;
        internal readonly bool IdStable;
        internal readonly IntPtr Name;
        internal readonly bool Selected;
    }

    internal readonly struct SubtitleTrackStructure
    {
        internal readonly IntPtr Encoding;

        internal SubtitleTrackStructure(IntPtr encoding)
        {
            Encoding = encoding;
        }
    }

    /// <summary>
    /// Audio track
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct AudioTrack
    {
        internal AudioTrack(uint channels, uint rate)
        {
            Channels = channels;
            Rate = rate;
        }

        /// <summary>
        /// Audio track channels
        /// </summary>
        public readonly uint Channels;

        /// <summary>
        /// Audio track rate
        /// </summary>
        public readonly uint Rate;
    }

    /// <summary>
    /// Video track
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct VideoTrack
    {
        /// <summary>
        /// Video height
        /// </summary>
        public readonly uint Height;

        /// <summary>
        /// Video Width
        /// </summary>
        public readonly uint Width;

        /// <summary>
        /// Video SarNum
        /// </summary>
        public readonly uint SarNum;

        /// <summary>
        /// Video SarDen
        /// </summary>
        public readonly uint SarDen;

        /// <summary>
        /// Video frame rate num
        /// </summary>
        public readonly uint FrameRateNum;

        /// <summary>
        /// Video frame rate den
        /// </summary>
        public readonly uint FrameRateDen;

        /// <summary>
        /// Video orientation
        /// </summary>
        public readonly VideoOrientation Orientation;

        /// <summary>
        /// Video projection
        /// </summary>
        public readonly VideoProjection Projection;

        /// <summary>
        /// Video viewpoint
        /// </summary>
        public readonly VideoViewpoint Pose;
    }

    /// <summary>
    /// Subtitle track
    /// </summary>
    public readonly struct SubtitleTrack
    {
        internal SubtitleTrack(string? encoding)
        {
            Encoding = encoding;
        }
        /// <summary>
        /// Subtitle encoding
        /// </summary>
        public readonly string? Encoding;
    }

    /// <summary>
    /// Media track information
    /// </summary>
    public class MediaTrack : Internal
    {
        internal struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_track_hold")]
            internal static extern IntPtr LibVLCMediaTrackHold(IntPtr mediaTrack);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_track_release")]
            internal static extern void LibVLCMediaTrackRelease(IntPtr mediaTrack);
        }
        internal MediaTrack(IntPtr mediaTrackPtr)
            : base(() => Native.LibVLCMediaTrackHold(mediaTrackPtr), Native.LibVLCMediaTrackRelease)
        {
            MediaTrackStructure = MarshalUtils.PtrToStructure<MediaTrackStructure>(mediaTrackPtr);

            AudioTrack audioTrack = default;
            VideoTrack videoTrack = default;
            SubtitleTrack subtitleTrack = default;

            switch (MediaTrackStructure.TrackType)
            {
                case TrackType.Audio:
                    audioTrack = MarshalUtils.PtrToStructure<AudioTrack>(MediaTrackStructure.TrackData);
                    break;
                case TrackType.Video:
                    videoTrack = MarshalUtils.PtrToStructure<VideoTrack>(MediaTrackStructure.TrackData);
                    break;
                case TrackType.Text:
                    subtitleTrack = MarshalUtils.PtrToStructure<SubtitleTrackStructure>(MediaTrackStructure.TrackData).Build();
                    break;
                case TrackType.Unknown:
                    break;
            }

            Codec = MediaTrackStructure.Codec;
            OriginalFourcc = MediaTrackStructure.OriginalFourcc;
            TrackType = MediaTrackStructure.TrackType;
            Profile = MediaTrackStructure.Profile;
            Level = MediaTrackStructure.Level;
            Data = new MediaTrackData(audioTrack, videoTrack, subtitleTrack);
            Bitrate = MediaTrackStructure.Bitrate;
            Language = MediaTrackStructure.Language.FromUtf8();
            Description = MediaTrackStructure.Description.FromUtf8();
            Id = MediaTrackStructure.StrId.FromUtf8();
            if(string.IsNullOrEmpty(Id))
            {
                Id = MediaTrackStructure.Id.ToString();
            }
            Stable = MediaTrackStructure.IdStable;
            Name = MediaTrackStructure.Name.FromUtf8();
            Selected = MediaTrackStructure.Selected;
        }

        internal readonly MediaTrackStructure MediaTrackStructure;

        /// <summary>
        /// Media track codec
        /// </summary>
        public readonly uint Codec;

        /// <summary>
        /// Media track original fourcc
        /// </summary>
        public readonly uint OriginalFourcc;

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
        public readonly string? Language;

        /// <summary>
        /// Media track description
        /// </summary>
        public readonly string? Description;

        /// <summary>
        /// String identifier of track, can be used to save the track preference
        /// from another LibVLC run
        /// </summary>
        public readonly string? Id;

        /// <summary>
        /// A string identifier is stable when it is certified to be the same
        /// across different playback instances for the same track
        /// </summary>
        public readonly bool Stable;

        /// <summary>
        /// Name of the track, only valid when the track is fetch from a media_player
        /// </summary>
        public readonly string? Name;

        /// <summary>
        /// true if the track is selected, only valid when the track is fetch from a media_player
        /// </summary>
        public readonly bool Selected;
    }

    /// <summary>
    /// Type containing a list of tracks
    /// </summary>
    public class MediaTrackList : Internal, IEnumerable<MediaTrack>
    {
        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_tracklist_delete")]
            internal static extern void LibVLCMediaTrackListDelete(IntPtr mediaTrackList);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_tracklist_at")]
            internal static extern IntPtr LibVLCMediaTrackListAt(IntPtr mediaTrackList, UIntPtr index);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_tracklist_count")]
            internal static extern UIntPtr LibVLCMediaTrackListCount(IntPtr mediaTrackList);
        }

        internal MediaTrackList(IntPtr mediaTrackListPtr) : base(() => mediaTrackListPtr, Native.LibVLCMediaTrackListDelete)
        {
        }

        /// <summary>
        /// Get a track at a specific index
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public MediaTrack? this[uint position]
        {
            get
            {
                if (position >= Count)
                    return null;

                var ptr = Native.LibVLCMediaTrackListAt(NativeReference, (UIntPtr)position);
                if (ptr == IntPtr.Zero)
                    return null;
                return new MediaTrack(ptr);
            }
        }

        /// <summary>
        /// Get the number of tracks in a tracklist
        /// </summary>
        public uint Count => (uint)Native.LibVLCMediaTrackListCount(NativeReference);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<MediaTrack> GetEnumerator() => new MediaTrackListEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal class MediaTrackListEnumerator : IEnumerator<MediaTrack>
        {
            int position = -1;
            MediaTrackList? _mediaTrackList;

            internal MediaTrackListEnumerator(MediaTrackList mediaTrackList)
            {
                _mediaTrackList = mediaTrackList;
            }

            public bool MoveNext()
            {
                position++;
                return position < (_mediaTrackList?.Count ?? 0);
            }

            void IEnumerator.Reset()
            {
                position = -1;
            }

            public void Dispose()
            {
                position = -1;
                _mediaTrackList = default;
            }

            object IEnumerator.Current => Current;

            public MediaTrack Current
            {
                get
                {
                    if (_mediaTrackList == null || _mediaTrackList.NativeReference == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(nameof(MediaTrackListEnumerator));
                    }
                    return _mediaTrackList[position < 0 ? 0 : (uint)position] ?? throw new ArgumentOutOfRangeException(nameof(position));
                }
            }
        }
    }
}
