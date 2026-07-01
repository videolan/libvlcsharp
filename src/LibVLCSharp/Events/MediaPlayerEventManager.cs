using System;
using LibVLCSharp.Helpers;

namespace LibVLCSharp
{
    /// <summary>
    /// Managed dispatcher for media player events.
    ///
    /// In LibVLC 4 the native event manager was removed; events are delivered through the
    /// <c>libvlc_media_player_cbs</c> struct (see <see cref="MediaPlayerCallbacks"/>) registered at
    /// construction. This class only stores the managed subscribers and maps the typed native callbacks
    /// onto the existing <c>MediaPlayer</c> events, preserving the public event API.
    /// </summary>
    internal class MediaPlayerEventManager : EventManager
    {
        internal event EventHandler<MediaPlayerPositionChangedEventArgs>? PositionChanged;
        internal event EventHandler<MediaPlayerMediaChangedEventArgs>? MediaChanged;
        internal event EventHandler<EventArgs>? NothingSpecial;
        internal event EventHandler<EventArgs>? Opening;
        internal event EventHandler<MediaPlayerBufferingEventArgs>? Buffering;
        internal event EventHandler<EventArgs>? Playing;
        internal event EventHandler<EventArgs>? Paused;
        internal event EventHandler<EventArgs>? Stopped;
        internal event EventHandler<EventArgs>? Stopping;
        internal event EventHandler<EventArgs>? EncounteredError;
        internal event EventHandler<MediaPlayerTimeChangedEventArgs>? TimeChanged;
        internal event EventHandler<MediaPlayerSeekableChangedEventArgs>? SeekableChanged;
        internal event EventHandler<MediaPlayerPausableChangedEventArgs>? PausableChanged;
        internal event EventHandler<MediaPlayerChapterChangedEventArgs>? ChapterChanged;
        internal event EventHandler<MediaPlayerSnapshotTakenEventArgs>? SnapshotTaken;
        internal event EventHandler<MediaPlayerLengthChangedEventArgs>? LengthChanged;
        internal event EventHandler<MediaPlayerVoutEventArgs>? Vout;
        internal event EventHandler<MediaPlayerESAddedEventArgs>? ESAdded;
        internal event EventHandler<MediaPlayerESDeletedEventArgs>? ESDeleted;
        internal event EventHandler<MediaPlayerESSelectedEventArgs>? ESSelected;
        internal event EventHandler<MediaPlayerAudioDeviceEventArgs>? AudioDevice;
        internal event EventHandler<EventArgs>? Corked;
        internal event EventHandler<EventArgs>? Uncorked;
        internal event EventHandler<EventArgs>? Muted;
        internal event EventHandler<EventArgs>? Unmuted;
        internal event EventHandler<MediaPlayerVolumeChangedEventArgs>? AudioVolume;
        internal event EventHandler<MediaPlayerProgramAddedEventArgs>? ProgramAdded;
        internal event EventHandler<MediaPlayerProgramDeletedEventArgs>? ProgramDeleted;
        internal event EventHandler<MediaPlayerProgramSelectedEventArgs>? ProgramSelected;
        internal event EventHandler<MediaPlayerProgramUpdatedEventArgs>? ProgramUpdated;
        internal event EventHandler<MediaPlayerRecordChangedEventArgs>? RecordChanged;

        #region native callback dispatch

        internal void OnMediaChanged(IntPtr media)
            => MediaChanged?.Invoke(this, new MediaPlayerMediaChangedEventArgs(media));

        internal void OnStateChanged(VLCState state)
        {
            switch (state)
            {
                case VLCState.NothingSpecial:
                    NothingSpecial?.Invoke(this, EventArgs.Empty);
                    break;
                case VLCState.Opening:
                    Opening?.Invoke(this, EventArgs.Empty);
                    break;
                case VLCState.Playing:
                    Playing?.Invoke(this, EventArgs.Empty);
                    break;
                case VLCState.Paused:
                    Paused?.Invoke(this, EventArgs.Empty);
                    break;
                case VLCState.Stopped:
                    Stopped?.Invoke(this, EventArgs.Empty);
                    break;
                case VLCState.Stopping:
                    Stopping?.Invoke(this, EventArgs.Empty);
                    break;
                case VLCState.Error:
                    EncounteredError?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        internal void OnBuffering(float cache)
            => Buffering?.Invoke(this, new MediaPlayerBufferingEventArgs(cache));

        internal void OnCapabilitiesChanged(Capability newCaps)
        {
            SeekableChanged?.Invoke(this, new MediaPlayerSeekableChangedEventArgs((newCaps & Capability.Seek) != 0 ? 1 : 0));
            PausableChanged?.Invoke(this, new MediaPlayerPausableChangedEventArgs((newCaps & Capability.Pause) != 0 ? 1 : 0));
        }

        internal void OnPositionChanged(long time, double position)
        {
            PositionChanged?.Invoke(this, new MediaPlayerPositionChangedEventArgs(position));
            TimeChanged?.Invoke(this, new MediaPlayerTimeChangedEventArgs(time));
        }

        internal void OnLengthChanged(long length)
        {
            LengthChanged?.Invoke(this, new MediaPlayerLengthChangedEventArgs(length));
        }

        internal void OnTrackListChanged(ListAction action, TrackType type, IntPtr id)
        {
            var trackId = id.FromUtf8() ?? string.Empty;
            switch (action)
            {
                case ListAction.Added:
                    ESAdded?.Invoke(this, new MediaPlayerESAddedEventArgs(trackId, type));
                    break;
                case ListAction.Removed:
                    ESDeleted?.Invoke(this, new MediaPlayerESDeletedEventArgs(trackId, type));
                    break;
                case ListAction.Updated:
                    // No equivalent legacy event for track update.
                    break;
            }
        }

        internal void OnTrackSelectionChanged(TrackType type, IntPtr unselectedId, IntPtr selectedId)
        {
            if (selectedId != IntPtr.Zero)
                ESSelected?.Invoke(this, new MediaPlayerESSelectedEventArgs(selectedId.FromUtf8() ?? string.Empty, type));
        }

        internal void OnProgramListChanged(ListAction action, int groupId)
        {
            switch (action)
            {
                case ListAction.Added:
                    ProgramAdded?.Invoke(this, new MediaPlayerProgramAddedEventArgs(groupId));
                    break;
                case ListAction.Removed:
                    ProgramDeleted?.Invoke(this, new MediaPlayerProgramDeletedEventArgs(groupId));
                    break;
                case ListAction.Updated:
                    ProgramUpdated?.Invoke(this, new MediaPlayerProgramUpdatedEventArgs(groupId));
                    break;
            }
        }

        internal void OnProgramSelectionChanged(int unselectedGroupId, int selectedGroupId)
            => ProgramSelected?.Invoke(this, new MediaPlayerProgramSelectedEventArgs(unselectedGroupId, selectedGroupId));

        internal void OnChapterSelectionChanged(int chapter)
            => ChapterChanged?.Invoke(this, new MediaPlayerChapterChangedEventArgs(chapter));

        internal void OnRecordingChanged(bool isRecording, IntPtr filePath)
            => RecordChanged?.Invoke(this, new MediaPlayerRecordChangedEventArgs(isRecording, filePath.FromUtf8()));

        internal void OnScreenshotTaken(IntPtr filePath)
            => SnapshotTaken?.Invoke(this, new MediaPlayerSnapshotTakenEventArgs(filePath.FromUtf8() ?? string.Empty));

        internal void OnVoutChanged(int count)
            => Vout?.Invoke(this, new MediaPlayerVoutEventArgs(count));

        internal void OnCorkChanged(bool corked)
        {
            if (corked)
                Corked?.Invoke(this, EventArgs.Empty);
            else
                Uncorked?.Invoke(this, EventArgs.Empty);
        }

        internal void OnAudioVolumeChanged(float volume)
            => AudioVolume?.Invoke(this, new MediaPlayerVolumeChangedEventArgs(volume));

        internal void OnAudioMuteChanged(bool muted)
        {
            if (muted)
                Muted?.Invoke(this, EventArgs.Empty);
            else
                Unmuted?.Invoke(this, EventArgs.Empty);
        }

        internal void OnAudioDeviceChanged(IntPtr device)
            => AudioDevice?.Invoke(this, new MediaPlayerAudioDeviceEventArgs(device.FromUtf8() ?? string.Empty));

        #endregion
    }
}
