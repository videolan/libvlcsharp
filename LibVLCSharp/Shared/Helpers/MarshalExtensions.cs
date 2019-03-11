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

        /// <summary>
        /// Helper method that marshals a UTF16 managed string to a UTF8 native string ptr
        /// </summary>
        /// <param name="str">the managed string to marshal to native</param>
        /// <returns>a ptr to the UTF8 string that needs to be freed after use</returns>
        internal static IntPtr ToUtf8(this string str)
        {
            if (str == null)
                return IntPtr.Zero;

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            var nativeString = Marshal.AllocHGlobal(bytes.Length + 1);
            try
            {
                Marshal.Copy(bytes, 0, nativeString, bytes.Length);
                Marshal.WriteByte(nativeString, bytes.Length, 0);
            }
            catch (Exception)
            {
                Marshal.FreeHGlobal(nativeString);
                throw;
            }

            return nativeString;
        }

        /// <summary>
        /// Helper method that mashals a UTF8 native string ptr to a UTF16 managed string.
        /// Optionally frees the native string ptr
        /// </summary>
        /// <param name="nativeString">the native string to marshal to managed</param>
        /// <param name="libvlcFree">frees the native pointer of the libvlc string (use only for char*)</param>
        /// <returns>a managed UTF16 string</returns>
        internal static string FromUtf8(this IntPtr nativeString, bool libvlcFree = false)
        {
            if (nativeString == IntPtr.Zero)
                return null;

            var length = 0;

            while (Marshal.ReadByte(nativeString, length) != 0)
            {
                length++;
            }

            byte[] buffer = new byte[length];
            Marshal.Copy(nativeString, buffer, 0, buffer.Length);
            if (libvlcFree)
                MarshalUtils.LibVLCFree(ref nativeString);
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
    }
}