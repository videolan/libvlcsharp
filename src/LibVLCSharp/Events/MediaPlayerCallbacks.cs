using LibVLCSharp.Helpers;
using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// Player capabilities reported by <c>on_capabilities_changed</c> (libvlc_capability_t).
    /// </summary>
    [Flags]
    internal enum Capability
    {
        Seek = 0x1,
        Pause = 0x2,
        ChangeRate = 0x4,
        Rewind = 0x8
    }

    /// <summary>
    /// Whether a list item was added, removed or updated (libvlc_list_action_t).
    /// </summary>
    internal enum ListAction
    {
        Added = 0,
        Removed = 1,
        Updated = 2
    }

    /// <summary>
    /// Reason why the player is stopping the current media (libvlc_stopping_reason_t).
    /// </summary>
    internal enum StoppingReason
    {
        Error = 0,
        Eos = 1,
        User = 2
    }

    /// <summary>
    /// Managed mirror of the native <c>libvlc_media_player_cbs</c> struct passed to
    /// <c>libvlc_media_player_new</c>/<c>libvlc_media_player_new_from_media</c> in LibVLC 4.
    ///
    /// The struct content (the function pointers) is identical for every media player; only the
    /// <c>cbs_opaque</c> pointer differs per instance. We therefore build a single immutable native
    /// copy once and share its pointer across all media players, as recommended by the native API.
    /// Unused (non-bridged) callbacks are left as NULL function pointers, which is allowed since every
    /// callback is documented as optional.
    /// </summary>
    internal static class MediaPlayerCallbacks
    {
        // libvlc_media_player_cbs: all callbacks are "available since version 0", so version 0 exposes them all.
        const uint Version = 0;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void MediaChangedCb(IntPtr opaque, IntPtr media);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void StateChangedCb(IntPtr opaque, VLCState state);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void BufferingChangedCb(IntPtr opaque, float buffering);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void CapabilitiesChangedCb(IntPtr opaque, Capability oldCaps, Capability newCaps);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void PositionChangedCb(IntPtr opaque, long time, double pos);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void LengthChangedCb(IntPtr opaque, long length);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void TrackListChangedCb(IntPtr opaque, ListAction action, TrackType type, IntPtr id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void TrackSelectionChangedCb(IntPtr opaque, TrackType type, IntPtr unselectedId, IntPtr selectedId);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ProgramListChangedCb(IntPtr opaque, ListAction action, int groupId);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ProgramSelectionChangedCb(IntPtr opaque, int unselectedGroupId, int selectedGroupId);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ChapterSelectionChangedCb(IntPtr opaque, IntPtr title, uint titleIdx, IntPtr chapter, uint chapterIdx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void RecordingChangedCb(IntPtr opaque, [MarshalAs(UnmanagedType.I1)] bool recording, IntPtr filePath);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ScreenshotTakenCb(IntPtr opaque, IntPtr filePath);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void VoutChangedCb(IntPtr opaque, uint voutCount);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void CorkChangedCb(IntPtr opaque, [MarshalAs(UnmanagedType.I1)] bool corked);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void AudioVolumeChangedCb(IntPtr opaque, float volume);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void AudioMuteChangedCb(IntPtr opaque, [MarshalAs(UnmanagedType.I1)] bool muted);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void AudioDeviceChangedCb(IntPtr opaque, IntPtr device);

        // Native struct layout. Every member is a function pointer (or NULL); order MUST match
        // struct libvlc_media_player_cbs exactly.
        [StructLayout(LayoutKind.Sequential)]
        struct NativeCallbacks
        {
            public uint Version;
            public IntPtr OnMediaChanged;
            public IntPtr OnMediaStopping;       // not bridged (NULL)
            public IntPtr OnStateChanged;
            public IntPtr OnBufferingChanged;
            public IntPtr OnCapabilitiesChanged;
            public IntPtr OnPositionChanged;
            public IntPtr OnLengthChanged;
            public IntPtr OnTrackListChanged;
            public IntPtr OnTrackSelectionChanged;
            public IntPtr OnProgramListChanged;
            public IntPtr OnProgramSelectionChanged;
            public IntPtr OnTitlesChanged;       // not bridged (NULL)
            public IntPtr OnTitleSelectionChanged; // not bridged (NULL)
            public IntPtr OnChapterSelectionChanged;
            public IntPtr OnRecordingChanged;
            public IntPtr OnScreenshotTaken;
            public IntPtr OnMediaParsed;
            public IntPtr OnMediaMetaChanged;
            public IntPtr OnMediaSubitemsChanged;
            public IntPtr OnMediaAttachmentsAdded;
            public IntPtr OnVoutChanged;
            public IntPtr OnCorkChanged;
            public IntPtr OnAudioVolumeChanged;
            public IntPtr OnAudioMuteChanged;
            public IntPtr OnAudioDeviceChanged;
        }

        // Keep the delegate instances rooted for the lifetime of the process so that the native function
        // pointers stored in the shared struct remain valid.
        static readonly MediaChangedCb s_mediaChanged = OnMediaChanged;
        static readonly StateChangedCb s_stateChanged = OnStateChanged;
        static readonly BufferingChangedCb s_bufferingChanged = OnBufferingChanged;
        static readonly CapabilitiesChangedCb s_capabilitiesChanged = OnCapabilitiesChanged;
        static readonly PositionChangedCb s_positionChanged = OnPositionChanged;
        static readonly LengthChangedCb s_lengthChanged = OnLengthChanged;
        static readonly TrackListChangedCb s_trackListChanged = OnTrackListChanged;
        static readonly TrackSelectionChangedCb s_trackSelectionChanged = OnTrackSelectionChanged;
        static readonly ProgramListChangedCb s_programListChanged = OnProgramListChanged;
        static readonly ProgramSelectionChangedCb s_programSelectionChanged = OnProgramSelectionChanged;
        static readonly ChapterSelectionChangedCb s_chapterSelectionChanged = OnChapterSelectionChanged;
        static readonly RecordingChangedCb s_recordingChanged = OnRecordingChanged;
        static readonly ScreenshotTakenCb s_screenshotTaken = OnScreenshotTaken;
        static readonly VoutChangedCb s_voutChanged = OnVoutChanged;
        static readonly CorkChangedCb s_corkChanged = OnCorkChanged;
        static readonly AudioVolumeChangedCb s_audioVolumeChanged = OnAudioVolumeChanged;
        static readonly AudioMuteChangedCb s_audioMuteChanged = OnAudioMuteChanged;
        static readonly AudioDeviceChangedCb s_audioDeviceChanged = OnAudioDeviceChanged;

        static readonly IntPtr s_pointer = Build();

        /// <summary>
        /// Pointer to the shared, process-lifetime native callbacks struct.
        /// </summary>
        internal static IntPtr Pointer => s_pointer;

        static IntPtr Build()
        {
            var cbs = new NativeCallbacks
            {
                Version = Version,
                OnMediaChanged = Marshal.GetFunctionPointerForDelegate(s_mediaChanged),
                OnStateChanged = Marshal.GetFunctionPointerForDelegate(s_stateChanged),
                OnBufferingChanged = Marshal.GetFunctionPointerForDelegate(s_bufferingChanged),
                OnCapabilitiesChanged = Marshal.GetFunctionPointerForDelegate(s_capabilitiesChanged),
                OnPositionChanged = Marshal.GetFunctionPointerForDelegate(s_positionChanged),
                OnLengthChanged = Marshal.GetFunctionPointerForDelegate(s_lengthChanged),
                OnTrackListChanged = Marshal.GetFunctionPointerForDelegate(s_trackListChanged),
                OnTrackSelectionChanged = Marshal.GetFunctionPointerForDelegate(s_trackSelectionChanged),
                OnProgramListChanged = Marshal.GetFunctionPointerForDelegate(s_programListChanged),
                OnProgramSelectionChanged = Marshal.GetFunctionPointerForDelegate(s_programSelectionChanged),
                OnChapterSelectionChanged = Marshal.GetFunctionPointerForDelegate(s_chapterSelectionChanged),
                OnRecordingChanged = Marshal.GetFunctionPointerForDelegate(s_recordingChanged),
                OnScreenshotTaken = Marshal.GetFunctionPointerForDelegate(s_screenshotTaken),
                OnVoutChanged = Marshal.GetFunctionPointerForDelegate(s_voutChanged),
                OnCorkChanged = Marshal.GetFunctionPointerForDelegate(s_corkChanged),
                OnAudioVolumeChanged = Marshal.GetFunctionPointerForDelegate(s_audioVolumeChanged),
                OnAudioMuteChanged = Marshal.GetFunctionPointerForDelegate(s_audioMuteChanged),
                OnAudioDeviceChanged = Marshal.GetFunctionPointerForDelegate(s_audioDeviceChanged),
            };

            var ptr = Marshal.AllocHGlobal(MarshalUtils.SizeOf(cbs));
            Marshal.StructureToPtr(cbs, ptr, false);
            return ptr;
        }

        static MediaPlayerEventManager? Manager(IntPtr opaque) => EventManager.FromOpaque<MediaPlayerEventManager>(opaque);

        static void Guarded(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                // A managed exception must never cross back into native code.
                Core.Log(ex.ToString());
            }
        }

        static void OnMediaChanged(IntPtr opaque, IntPtr media) => Guarded(() => Manager(opaque)?.OnMediaChanged(media));
        static void OnStateChanged(IntPtr opaque, VLCState state) => Guarded(() => Manager(opaque)?.OnStateChanged(state));
        static void OnBufferingChanged(IntPtr opaque, float buffering) => Guarded(() => Manager(opaque)?.OnBuffering(buffering));
        static void OnCapabilitiesChanged(IntPtr opaque, Capability oldCaps, Capability newCaps) => Guarded(() => Manager(opaque)?.OnCapabilitiesChanged(newCaps));
        static void OnPositionChanged(IntPtr opaque, long time, double pos) => Guarded(() => Manager(opaque)?.OnPositionChanged(time, pos));
        static void OnLengthChanged(IntPtr opaque, long length) => Guarded(() => Manager(opaque)?.OnLengthChanged(length));
        static void OnTrackListChanged(IntPtr opaque, ListAction action, TrackType type, IntPtr id) => Guarded(() => Manager(opaque)?.OnTrackListChanged(action, type, id));
        static void OnTrackSelectionChanged(IntPtr opaque, TrackType type, IntPtr unselectedId, IntPtr selectedId) => Guarded(() => Manager(opaque)?.OnTrackSelectionChanged(type, unselectedId, selectedId));
        static void OnProgramListChanged(IntPtr opaque, ListAction action, int groupId) => Guarded(() => Manager(opaque)?.OnProgramListChanged(action, groupId));
        static void OnProgramSelectionChanged(IntPtr opaque, int unselectedGroupId, int selectedGroupId) => Guarded(() => Manager(opaque)?.OnProgramSelectionChanged(unselectedGroupId, selectedGroupId));
        static void OnChapterSelectionChanged(IntPtr opaque, IntPtr title, uint titleIdx, IntPtr chapter, uint chapterIdx) => Guarded(() => Manager(opaque)?.OnChapterSelectionChanged((int)chapterIdx));
        static void OnRecordingChanged(IntPtr opaque, bool recording, IntPtr filePath) => Guarded(() => Manager(opaque)?.OnRecordingChanged(recording, filePath));
        static void OnScreenshotTaken(IntPtr opaque, IntPtr filePath) => Guarded(() => Manager(opaque)?.OnScreenshotTaken(filePath));
        static void OnVoutChanged(IntPtr opaque, uint voutCount) => Guarded(() => Manager(opaque)?.OnVoutChanged((int)voutCount));
        static void OnCorkChanged(IntPtr opaque, bool corked) => Guarded(() => Manager(opaque)?.OnCorkChanged(corked));
        static void OnAudioVolumeChanged(IntPtr opaque, float volume) => Guarded(() => Manager(opaque)?.OnAudioVolumeChanged(volume));
        static void OnAudioMuteChanged(IntPtr opaque, bool muted) => Guarded(() => Manager(opaque)?.OnAudioMuteChanged(muted));
        static void OnAudioDeviceChanged(IntPtr opaque, IntPtr device) => Guarded(() => Manager(opaque)?.OnAudioDeviceChanged(device));
    }
}
