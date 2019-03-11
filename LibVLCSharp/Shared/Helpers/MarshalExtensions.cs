using LibVLCSharp.Shared.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVLCSharp.Shared.Helpers
{
    internal static class MarshalExtensions
    {
        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">AudioOutputDescriptionStructure from interop</param>
        /// <returns>public AudioOutputDescription to be consumed by the user</returns>
        internal static AudioOutputDescription Build(this AudioOutputDescriptionStructure s) => 
            new AudioOutputDescription(s.Name.FromUtf8(), s.Description.FromUtf8());

        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">AudioOutputDeviceStructure from interop</param>
        /// <returns>public AudioOutputDevice to be consumed by the user</returns>
        internal static AudioOutputDevice Build(this AudioOutputDeviceStructure s) =>
            new AudioOutputDevice(s.DeviceIdentifier.FromUtf8(), s.Description.FromUtf8());

        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">ModuleDescriptionStructure from interop</param>
        /// <returns>public ModuleDescription to be consumed by the user</returns>
        internal static ModuleDescription Build(this ModuleDescriptionStructure s) =>
            new ModuleDescription(s.Name.FromUtf8(), s.ShortName.FromUtf8(), s.LongName.FromUtf8(), s.Help.FromUtf8());

        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">TrackDescriptionStructure from interop</param>
        /// <returns>public TrackDescription to be consumed by the user</returns>
        internal static TrackDescription Build(this TrackDescriptionStructure s) =>
            new TrackDescription(s.Id, s.Name.FromUtf8());

        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">MediaSlaveStructure from interop</param>
        /// <returns>public MediaSlave to be consumed by the user</returns>
        internal static MediaSlave Build(this MediaSlaveStructure s) => 
            new MediaSlave(s.Uri.FromUtf8(), s.Type, s.Priority);

        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">TrackDescriptionStructure from interop</param>
        /// <returns>public TrackDescription to be consumed by the user</returns>
        internal static MediaTrack Build(this MediaTrackStructure s)
        {
            AudioTrack audioTrack = default;
            VideoTrack videoTrack = default;
            SubtitleTrack subtitleTrack = default;

            switch (s.TrackType)
            {
                case TrackType.Audio:
                    audioTrack = MarshalUtils.PtrToStructure<AudioTrack>(s.TrackData);
                    break;
                case TrackType.Video:
                    videoTrack = MarshalUtils.PtrToStructure<VideoTrack>(s.TrackData);
                    break;
                case TrackType.Text:
                    subtitleTrack = MarshalUtils.PtrToStructure<SubtitleTrackStructure>(s.TrackData).Build();
                    break;
                case TrackType.Unknown:
                    break;
            }

            return new MediaTrack(s.Codec, 
                s.OriginalFourcc, 
                s.Id, 
                s.TrackType, 
                s.Profile, 
                s.Level, 
                new MediaTrackData(audioTrack, videoTrack, subtitleTrack), s.Bitrate,
                s.Language.FromUtf8(),
                s.Description.FromUtf8());
        }
        
        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">SubtitleTrackStructure from interop</param>
        /// <returns>public SubtitleTrack to be consumed by the user</returns>
        internal static SubtitleTrack Build(this SubtitleTrackStructure s) => new SubtitleTrack(s.Encoding.FromUtf8());

        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">MediaDiscovererDescriptionStructure from interop</param>
        /// <returns>public MediaDiscovererDescription to be consumed by the user</returns>
        internal static MediaDiscovererDescription Build(this MediaDiscovererDescriptionStructure s) =>
            new MediaDiscovererDescription(s.Name.FromUtf8(), s.LongName.FromUtf8(), s.Category);

        /// <summary>
        /// Helper method that creates a user friendly type from the internal interop structure.
        /// </summary>
        /// <param name="s">RendererDescriptionStructure from interop</param>
        /// <returns>public RendererDescription to be consumed by the user</returns>
        internal static RendererDescription Build(this RendererDescriptionStructure s) => 
            new RendererDescription(s.Name.FromUtf8(), s.LongName.FromUtf8());

        internal static IntPtr ToUtf8(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return IntPtr.Zero;

            byte[] utf8bytes = Encoding.UTF8.GetBytes(str);
            IntPtr ptr = Marshal.AllocCoTaskMem(utf8bytes.Length + 1);
            Marshal.Copy(utf8bytes, 0, ptr, utf8bytes.Length);
            Marshal.WriteByte(ptr, utf8bytes.Length, 0);
            return ptr;
        }

        internal static string FromUtf8(this IntPtr nativeString, bool libvlcFree = false)
        {
            if (nativeString == IntPtr.Zero)
                return null;

            var bytes = new List<byte>();
            for (int offset = 0; ; offset++)
            {
                byte b = Marshal.ReadByte(nativeString, offset);
                if (b == 0)
                    break;
                else bytes.Add(b);
            }

            var str = Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
            if (libvlcFree)
                MarshalUtils.LibVLCFree(ref nativeString);
            return str;
        }
    }
}