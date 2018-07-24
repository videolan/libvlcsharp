﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using LibVLCSharp.Shared.Structures;

namespace LibVLCSharp.Shared
{
    public class MediaPlayer : Internal
    {
        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_new")]
            internal static extern IntPtr LibVLCMediaPlayerNew(IntPtr libvlc);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_release")]
            internal static extern void LibVLCMediaPlayerRelease(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_new_from_media")]
            internal static extern IntPtr LibVLCMediaPlayerNewFromMedia(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_media")]
            internal static extern void LibVLCMediaPlayerSetMedia(IntPtr mediaPlayer, IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_media")]
            internal static extern IntPtr LibVLCMediaPlayerGetMedia(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_event_manager")]
            internal static extern IntPtr LibVLCMediaPlayerEventManager(IntPtr mediaPlayer);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_is_playing")]
            internal static extern int LibVLCMediaPlayerIsPlaying(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_play")]
            internal static extern int LibVLCMediaPlayerPlay(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_pause")]
            internal static extern void LibVLCMediaPlayerSetPause(IntPtr mediaPlayer, bool pause);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_pause")]
            internal static extern void LibVLCMediaPlayerPause(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_stop")]
            internal static extern void LibVLCMediaPlayerStop(IntPtr mediaPlayer);
#if COCOA
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_nsobject")]
            internal static extern void LibVLCMediaPlayerSetNsobject(IntPtr mediaPlayer, IntPtr drawable);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_nsobject")]
            internal static extern IntPtr LibVLCMediaPlayerGetNsobject(IntPtr mediaPlayer);
#endif
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_xwindow")]
            internal static extern void LibVLCMediaPlayerSetXwindow(IntPtr mediaPlayer, uint drawable);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_xwindow")]
            internal static extern uint LibVLCMediaPlayerGetXwindow(IntPtr mediaPlayer);
#if WINDOWS
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_hwnd")]
            internal static extern void LibVLCMediaPlayerSetHwnd(IntPtr mediaPlayer, IntPtr drawable);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_hwnd")]
            internal static extern IntPtr LibVLCMediaPlayerGetHwnd(IntPtr mediaPlayer);
#endif
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_length")]
            internal static extern long LibVLCMediaPlayerGetLength(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_time")]
            internal static extern long LibVLCMediaPlayerGetTime(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_time")]
            internal static extern void LibVLCMediaPlayerSetTime(IntPtr mediaPlayer, long time);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_position")]
            internal static extern float LibVLCMediaPlayerGetPosition(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_position")]
            internal static extern void LibVLCMediaPlayerSetPosition(IntPtr mediaPlayer, float position);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_chapter")]
            internal static extern void LibVLCMediaPlayerSetChapter(IntPtr mediaPlayer, int chapter);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_chapter")]
            internal static extern int LibVLCMediaPlayerGetChapter(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_chapter_count")]
            internal static extern int LibVLCMediaPlayerGetChapterCount(IntPtr mediaPlayer);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_will_play")]
            internal static extern int LibVLCMediaPlayerWillPlay(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_chapter_count_for_title")]
            internal static extern int LibVLCMediaPlayerGetChapterCountForTitle(IntPtr mediaPlayer, int title);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_title")]
            internal static extern void LibVLCMediaPlayerSetTitle(IntPtr mediaPlayer, int title);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_title")]
            internal static extern int LibVLCMediaPlayerGetTitle(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_title_count")]
            internal static extern int LibVLCMediaPlayerGetTitleCount(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_previous_chapter")]
            internal static extern void LibVLCMediaPlayerPreviousChapter(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_next_chapter")]
            internal static extern void LibVLCMediaPlayerNextChapter(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_rate")]
            internal static extern float LibVLCMediaPlayerGetRate(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_rate")]
            internal static extern int LibVLCMediaPlayerSetRate(IntPtr mediaPlayer, float rate);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_state")]
            internal static extern VLCState LibVLCMediaPlayerGetState(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_fps")]
            internal static extern float LibVLCMediaPlayerGetFps(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_has_vout")]
            internal static extern uint LibVLCMediaPlayerHasVout(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_is_seekable")]
            internal static extern int LibVLCMediaPlayerIsSeekable(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_can_pause")]
            internal static extern int LibVLCMediaPlayerCanPause(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_program_scrambled")]
            internal static extern int LibVLCMediaPlayerProgramScrambled(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_next_frame")]
            internal static extern void LibVLCMediaPlayerNextFrame(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_navigate")]
            internal static extern void LibVLCMediaPlayerNavigate(IntPtr mediaPlayer, uint navigate);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_video_title_display")]
            internal static extern void LibVLCMediaPlayerSetVideoTitleDisplay(IntPtr mediaPlayer, Position position, uint timeout);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_toggle_fullscreen")]
            internal static extern void LibVLCToggleFullscreen(IntPtr mediaPlayer);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_fullscreen")]
            internal static extern void LibVLCSetFullscreen(IntPtr mediaPlayer, int fullscreen);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_fullscreen")]
            internal static extern int LibVLCGetFullscreen(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_toggle_teletext")]
            internal static extern void LibVLCToggleTeletext(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_equalizer")]
            internal static extern int LibVLCMediaPlayerSetEqualizer(IntPtr mediaPlayer, IntPtr equalizer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_callbacks")]
            internal static extern void LibVLCAudioSetCallbacks(IntPtr mediaPlayer, LibVLCAudioPlayCb play, LibVLCAudioPauseCb pause, 
                LibVLCAudioResumeCb resume, LibVLCAudioFlushCb flush, LibVLCAudioDrainCb drain, IntPtr opaque);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_volume_callback")]
            internal static extern void LibVLCAudioSetVolumeCallback(IntPtr mediaPlayer, LibVLCVolumeCb volumeCallback);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_format_callbacks")]
            internal static extern void LibVLCAudioSetFormatCallbacks(IntPtr mediaPlayer, LibVLCAudioSetupCb setup, LibVLCAudioCleanupCb cleanup);

            // TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_format")] 
            internal static extern void LibVLCAudioSetFormat(IntPtr mediaPlayer, [MarshalAs(UnmanagedType.LPStr)] string format, 
                uint rate, uint channels);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_enum")]
            internal static extern IntPtr LibVLCAudioOutputDeviceEnum(IntPtr mediaPlayer);

            // TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_set")]
            internal static extern void LibVLCAudioOutputDeviceSet(IntPtr mediaPlayer, [MarshalAs(UnmanagedType.LPStr)] string module, 
                [MarshalAs(UnmanagedType.LPStr)] string deviceId);

            // TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_set")]
            internal static extern int LibVLCAudioOutputSet(IntPtr mediaPlayer, [MarshalAs(UnmanagedType.LPStr)] string name);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_toggle_mute")]
            internal static extern void LibVLCAudioToggleMute(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_mute")]
            internal static extern int LibVLCAudioGetMute(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_mute")]
            internal static extern void LibVLCAudioSetMute(IntPtr mediaPlayer, int status);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_volume")]
            internal static extern int LibVLCAudioGetVolume(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_volume")]
            internal static extern int LibVLCAudioSetVolume(IntPtr mediaPlayer, int volume);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_track_count")]
            internal static extern int LibVLCAudioGetTrackCount(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_track_description")]
            internal static extern IntPtr LibVLCAudioGetTrackDescription(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_track")]
            internal static extern int LibVLCAudioGetTrack(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_track")]
            internal static extern int LibVLCAudioSetTrack(IntPtr mediaPlayer, int track);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_channel")]
            internal static extern int LibVLCAudioGetChannel(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_channel")]
            internal static extern int LibVLCAudioSetChannel(IntPtr mediaPlayer, int channel);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_delay")]
            internal static extern long LibVLCAudioGetDelay(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_delay")]
            internal static extern int LibVLCAudioSetDelay(IntPtr mediaPlayer, long delay);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_callbacks")]
            internal static extern void LibVLCVideoSetCallbacks(IntPtr mediaPlayer, LibVLCVideoLockCb lockCallback, 
                LibVLCVideoUnlockCb unlock, LibVLCVideoDisplayCb display, IntPtr opaque);

            //TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_format")]
            internal static extern void LibVLCVideoSetFormat(IntPtr mediaPlayer, [MarshalAs(UnmanagedType.LPStr)] string chroma, 
                uint width, uint height, uint pitch);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_format_callbacks")]
            internal static extern void LibVLCVideoSetFormatCallbacks(IntPtr mediaPlayer, LibVLCVideoFormatCb setup, 
                LibVLCVideoCleanupCb cleanup);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_key_input")]
            internal static extern void LibVLCVideoSetKeyInput(IntPtr mediaPlayer, int enable);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_mouse_input")]
            internal static extern void LibVLCVideoSetMouseInput(IntPtr mediaPlayer, int enable);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_size")]
            internal static extern unsafe int LibVLCVideoGetSize(IntPtr mediaPlayer, uint num, uint* px, uint* py);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_cursor")]
            internal static extern unsafe int LibVLCVideoGetCursor(IntPtr mediaPlayer, uint num, int* px, int* py);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_scale")]
            internal static extern float LibVLCVideoGetScale(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_scale")]
            internal static extern void LibVLCVideoSetScale(IntPtr mediaPlayer, float factor);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_aspect_ratio")]
            internal static extern string LibVLCVideoGetAspectRatio(IntPtr mediaPlayer);

            //TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_aspect_ratio")]
            internal static extern void LibVLCVideoSetAspectRatio(IntPtr mediaPlayer, [MarshalAs(UnmanagedType.LPStr)] string aspect);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_spu")]
            internal static extern int LibVLCVideoGetSpu(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_spu_count")]
            internal static extern int LibVLCVideoGetSpuCount(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_spu_description")]
            internal static extern IntPtr LibVLCVideoGetSpuDescription(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_spu")]
            internal static extern int LibVLCVideoSetSpu(IntPtr mediaPlayer, int spu);

            //TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_subtitle_file")]
            internal static extern int LibVLCVideoSetSubtitleFile(IntPtr mediaPlayer,
                [MarshalAs(UnmanagedType.LPStr)] string subtitle);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_spu_delay")]
            internal static extern long LibVLCVideoGetSpuDelay(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_spu_delay")]
            internal static extern int LibVLCVideoSetSpuDelay(IntPtr mediaPlayer, long delay);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_title_description")]
            internal static extern IntPtr LibVLCVideoGetTitleDescription(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_full_title_descriptions")]
            internal static extern int LibVLCMediaPlayerGetFullTitleDescriptions(IntPtr mediaPlayer, IntPtr titles);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_chapter_description")]
            internal static extern IntPtr LibVLCVideoGetChapterDescription(IntPtr mediaPlayer,
                int title);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_title_descriptions_release")]
            internal static extern void LibVLCTitleDescriptionsRelease(IntPtr titles, uint count);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_full_chapter_descriptions")]
            internal static extern int LibVLCMediaPlayerGetFullChapterDescriptions(IntPtr mediaPlayer, int titleIndex, ref IntPtr chapters);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_chapter_descriptions_release")]
            internal static extern void LibVLCChapterDescriptionsRelease(IntPtr chapters, uint count);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_crop_geometry")]
            internal static extern string LibVLCVideoGetCropGeometry(IntPtr mediaPlayer);

            //TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_crop_geometry")]
            internal static extern void LibVLCVideoSetCropGeometry(IntPtr mediaPlayer, [MarshalAs(UnmanagedType.LPStr)] string geometry);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_teletext")]
            internal static extern int LibVLCVideoGetTeletext(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_teletext")]
            internal static extern void LibVLCVideoSetTeletext(IntPtr mediaPlayer, int page);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_track_count")]
            internal static extern int LibVLCVideoGetTrackCount(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_track_description")]
            internal static extern IntPtr LibVLCVideoGetTrackDescription(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_track")]
            internal static extern int LibVLCVideoGetTrack(IntPtr mediaPlayer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_track")]
            internal static extern int LibVLCVideoSetTrack(IntPtr mediaPlayer, int track);
            
            // TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_take_snapshot")]
            internal static extern int LibVLCVideoTakeSnapshot(IntPtr mediaPlayer, uint num, 
                [MarshalAs(UnmanagedType.LPStr)] string filepath, uint width, uint height);

            // TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_deinterlace")]
            internal static extern void LibVLCVideoSetDeinterlace(IntPtr mediaPlayer, [MarshalAs(UnmanagedType.LPStr)] string mode);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_marquee_int")]
            internal static extern int LibVLCVideoGetMarqueeInt(IntPtr mediaPlayer, VideoMarqueeOption option);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_marquee_string")]
            internal static extern string LibVLCVideoGetMarqueeString(IntPtr mediaPlayer, VideoMarqueeOption option);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_marquee_int")]
            internal static extern void LibVLCVideoSetMarqueeInt(IntPtr mediaPlayer, VideoMarqueeOption option, int marqueeValue);

            //TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_marquee_string")]
            internal static extern void LibVLCVideoSetMarqueeString(IntPtr mediaPlayer, VideoMarqueeOption option, [MarshalAs(UnmanagedType.LPStr)] string marqueeValue);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_logo_int")]
            internal static extern int LibVLCVideoGetLogoInt(IntPtr mediaPlayer, VideoLogoOption option);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_logo_int")]
            internal static extern void LibVLCVideoSetLogoInt(IntPtr mediaPlayer, VideoLogoOption option, int value);

            //TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_logo_string")]
            internal static extern void LibVLCVideoSetLogoString(IntPtr mediaPlayer, VideoLogoOption option, 
                [MarshalAs(UnmanagedType.LPStr)] string logoOptionValue);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_adjust_int")]
            internal static extern int LibVLCVideoGetAdjustInt(IntPtr mediaPlayer, VideoAdjustOption option);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_adjust_int")]
            internal static extern void LibVLCVideoSetAdjustInt(IntPtr mediaPlayer, VideoAdjustOption option, int value);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_adjust_float")]
            internal static extern float LibVLCVideoGetAdjustFloat(IntPtr mediaPlayer, VideoAdjustOption option);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_adjust_float")]
            internal static extern void LibVLCVideoSetAdjustFloat(IntPtr mediaPlayer, VideoAdjustOption option, float value);
            
            //TODO: UTF8
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_add_slave")]
            internal static extern int LibVLCMediaPlayerAddSlave(IntPtr mediaPlayer, MediaSlaveType mediaSlaveType, 
                [MarshalAs(UnmanagedType.LPStr)] string uri, bool selectWhenloaded);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_update_viewpoint")]
            internal static extern int LibVLCVideoUpdateViewpoint(IntPtr mediaPlayer, VideoViewpoint viewpoint, bool absolute);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_track_description_list_release")]
            internal static extern void LibVLCTrackDescriptionListRelease(IntPtr trackDescription);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_list_release")]
            internal static extern void LibVLCAudioOutputDeviceListRelease(IntPtr list);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_renderer")]
            internal static extern int LibVLCMediaPlayerSetRenderer(IntPtr mediaplayer, IntPtr renderItem);

#if ANDROID
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_android_context")]
            internal static extern void LibVLCMediaPlayerSetAndroidContext(IntPtr mediaPlayer, IntPtr aWindow);
#endif
        }
        
        MediaPlayerEventManager _eventManager;

        /// <summary>Create an empty Media Player object</summary>
        /// <param name="libVLC">
        /// <para>the libvlc instance in which the Media Player</para>
        /// <para>should be created.</para>
        /// </param>
        /// <returns>a new media player object, or NULL on error.</returns>
        public MediaPlayer(LibVLC libVLC) 
            : base(() => Native.LibVLCMediaPlayerNew(libVLC.NativeReference), Native.LibVLCMediaPlayerRelease)
        {
        }

        /// <summary>Create a Media Player object from a Media</summary>
        /// <param name="media">
        /// <para>the media. Afterwards the p_md can be safely</para>
        /// <para>destroyed.</para>
        /// </param>
        /// <returns>a new media player object, or NULL on error.</returns>
        public MediaPlayer(Media media)
            : base(() => Native.LibVLCMediaPlayerNewFromMedia(media.NativeReference), Native.LibVLCMediaPlayerRelease)
        {
        }
        
        /// <summary>
        /// Get the media used by the media_player.
        /// Set the media that will be used by the media_player. 
        /// If any, previous md will be released.
        /// </summary>
        public Media Media
        {
            get
            {
                var mediaPtr = Native.LibVLCMediaPlayerGetMedia(NativeReference);
                return mediaPtr == IntPtr.Zero ? null : new Media(mediaPtr);
            }
            set => Native.LibVLCMediaPlayerSetMedia(NativeReference, value.NativeReference);
        }

        /// <summary>
        /// Get the Event Manager from which the media player send event.
        /// </summary>
        public MediaPlayerEventManager EventManager
        {
            get
            {
                if (_eventManager == null)
                {
                    var eventManagerPtr = Native.LibVLCMediaPlayerEventManager(NativeReference);
                    _eventManager = new MediaPlayerEventManager(eventManagerPtr);
                }
                return _eventManager;
            }
        }

        /// <summary>
        /// return true if the media player is playing, false otherwise
        /// </summary>
        public bool IsPlaying => Native.LibVLCMediaPlayerIsPlaying(NativeReference) != 0;

        /// <summary>
        /// Start playback with Media that is set
        /// If playback was already started, this method has no effect
        /// </summary>
        /// <returns>true if successful</returns>
        public bool Play() => Native.LibVLCMediaPlayerPlay(NativeReference) == 0;

        /// <summary>
        /// Set media and start playback
        /// </summary>
        /// <param name="media"></param>
        /// <returns>true if successful</returns>
        public bool Play(Media media)
        {
            Media = media;
            return Play();
        }

        /// <summary>
        /// Pause or resume (no effect if there is no media).
        /// version LibVLC 1.1.1 or later
        /// </summary>
        /// <param name="pause">play/resume if true, pause if false</param>
        [ApiVersion(1, 1)]
        public void SetPause(bool pause) => Native.LibVLCMediaPlayerSetPause(NativeReference, pause);

        /// <summary>
        /// Toggle pause (no effect if there is no media)
        /// </summary>
        public void Pause() => Native.LibVLCMediaPlayerPause(NativeReference);

        /// <summary>
        /// Stop the playback (no effect if there is no media)
        /// warning:
        /// This is synchronous, and will block until all VLC threads have been joined.
        /// Calling this from a VLC callback is a bound to cause a deadlock.
        /// </summary>
        public void Stop() => Native.LibVLCMediaPlayerStop(NativeReference);

#if COCOA
        /// <summary>
        /// Get the NSView handler previously set
        /// return the NSView handler or 0 if none where set
        /// <para></para>
        /// <para></para>
        /// Set the NSView handler where the media player should render its video output.
        /// Use the vout called "macosx".
        /// <para></para>
        /// The drawable is an NSObject that follow the
        /// VLCOpenGLVideoViewEmbedding protocol: VLCOpenGLVideoViewEmbedding NSObject
        /// Or it can be an NSView object.
        /// If you want to use it along with Qt4 see the QMacCocoaViewContainer.
        /// Then the following code should work:  { NSView *video = [[NSView
        /// alloc] init]; QMacCocoaViewContainer *container = new
        /// QMacCocoaViewContainer(video, parent);
        /// libvlc_media_player_set_nsobject(mp, video); [video release]; }
        /// You can find a live example in VLCVideoView in VLCKit.framework.
        /// </summary>
        public IntPtr NsObject
        {
            get => Native.LibVLCMediaPlayerGetNsobject(NativeReference);
            set => Native.LibVLCMediaPlayerSetNsobject(NativeReference, value);
        }
#endif
        /// <summary>
        /// Set an X Window System drawable where the media player should render its video output. 
        /// The call takes effect when the playback starts. If it is already started, it might need to be stopped before changes apply. 
        /// If LibVLC was built without X11 output support, then this function has no effects.
        /// By default, LibVLC will capture input events on the video rendering area. 
        /// Use libvlc_video_set_mouse_input() and libvlc_video_set_key_input() to disable that and deliver events to the parent window / to the application instead. 
        /// By design, the X11 protocol delivers input events to only one recipient.
        /// <para></para>
        /// Warning:
        /// The application must call the XInitThreads() function from Xlib before libvlc_new(), and before any call to XOpenDisplay() directly
        /// or via any other library.Failure to call XInitThreads() will seriously impede LibVLC performance. 
        /// Calling XOpenDisplay() before XInitThreads() will eventually crash the process. That is a limitation of Xlib.
        /// uint: X11 window ID
        /// </summary>
        public uint XWindow
        {
            get => Native.LibVLCMediaPlayerGetXwindow(NativeReference);
            set => Native.LibVLCMediaPlayerSetXwindow(NativeReference, value);
        }

#if WINDOWS
        /// <summary>
        /// Set a Win32/Win64 API window handle (HWND) where the media player
        /// should render its video output. If LibVLC was built without
        /// Win32/Win64 API output support, then this has no effects.
        /// <para></para>
        /// Get the Windows API window handle (HWND) previously set
        /// </summary>
        public IntPtr Hwnd
        {
            get => Native.LibVLCMediaPlayerGetHwnd(NativeReference);
            set => Native.LibVLCMediaPlayerSetHwnd(NativeReference, value);
        }
#endif
        /// <summary>
        /// The movie length (in ms), or -1 if there is no media.
        /// </summary>
        public long Length => Native.LibVLCMediaPlayerGetLength(NativeReference);

        /// <summary>
        /// Set the movie time (in ms). This has no effect if no media is being
        /// played. Not all formats and protocols support this.
        /// <para></para>
        /// Get the movie time (in ms), or -1 if there is no media.
        /// </summary>
        public long Time
        {
            get => Native.LibVLCMediaPlayerGetTime(NativeReference);
            set => Native.LibVLCMediaPlayerSetTime(NativeReference, value);
        }

        /// <summary>
        /// Set movie position as percentage between 0.0 and 1.0. This has no
        /// effect if playback is not enabled. This might not work depending on
        /// the underlying input format and protocol.
        /// <para></para>
        /// Get movie position as percentage between 0.0 and 1.0.
        /// </summary>
        public float Position
        {
            get => Native.LibVLCMediaPlayerGetPosition(NativeReference);
            set => Native.LibVLCMediaPlayerSetPosition(NativeReference, value);
        }

        /// <summary>
        /// Set movie chapter (if applicable).
        /// <para></para>
        /// Get the movie chapter number currently playing, or -1 if there is no media.
        /// </summary>
        public int Chapter
        {
            get => Native.LibVLCMediaPlayerGetChapter(NativeReference);
            set => Native.LibVLCMediaPlayerSetChapter(NativeReference, value);
        }

        /// <summary>
        /// Get the number of chapters in movie, or -1.
        /// </summary>
        public int ChapterCount => Native.LibVLCMediaPlayerGetChapterCount(NativeReference);

        /// <summary>
        /// True if the player is able to play
        /// </summary>
        public bool WillPlay => Native.LibVLCMediaPlayerWillPlay(NativeReference) != 0;

        /// <summary>
        /// Get the number of chapters in title, or -1
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public int ChapterCountForTitle(int title) => Native.LibVLCMediaPlayerGetChapterCountForTitle(NativeReference, title);

        /// <summary>
        /// Set movie title number to play
        /// <para></para>
        /// Get movie title number currently playing, or -1
        /// </summary>
        public int Title
        {
            get => Native.LibVLCMediaPlayerGetTitle(NativeReference);
            set => Native.LibVLCMediaPlayerSetTitle(NativeReference, value);
        }

        /// <summary>
        /// The title number count, or -1
        /// </summary>
        public int TitleCount => Native.LibVLCMediaPlayerGetTitleCount(NativeReference);

        /// <summary>
        /// Set previous chapter (if applicable)
        /// </summary>
        public void PreviousChapter()
        {
            Native.LibVLCMediaPlayerPreviousChapter(NativeReference);    
        }

        /// <summary>
        /// Set next chapter (if applicable)
        /// </summary>
        public void NextChapter()
        {
            Native.LibVLCMediaPlayerNextChapter(NativeReference);
        }

        /// <summary>
        /// Get the requested movie play rate.
        /// warning
        /// <para></para> 
        /// Depending on the underlying media, the requested rate may be
        /// different from the real playback rate.
        /// </summary>
        public float Rate => Native.LibVLCMediaPlayerGetRate(NativeReference);

        /// <summary>
        /// Set movie play rate
        /// </summary>
        /// <param name="rate">movie play rate to set</param>
        /// <returns>
        /// return -1 if an error was detected, 0 otherwise (but even then, it
        /// might not actually work depending on the underlying media protocol)
        /// </returns>
        public int SetRate(float rate)
        {
            return Native.LibVLCMediaPlayerSetRate(NativeReference, rate);
        }

        /// <summary>
        /// Get the current state of the media player (playing, paused, ...)
        /// </summary>
        public VLCState State => Native.LibVLCMediaPlayerGetState(NativeReference);

        float _fps;
        /// <summary>
        /// Get the frames per second (fps) for this playing movie, or 0 if unspecified
        /// </summary>
        [ApiVersion(major: 3, minor: 0, min: false, strict: true)]
        public float Fps
        {
            get { _fps = Native.LibVLCMediaPlayerGetFps(NativeReference); return _fps; }
        } 
        
        /// <summary>
        /// Get the number of video outputs 
        /// </summary>
        public uint VoutCount => Native.LibVLCMediaPlayerHasVout(NativeReference);

        /// <summary>
        /// True if the media player can seek
        /// </summary>
        public bool IsSeekable => Native.LibVLCMediaPlayerIsSeekable(NativeReference) != 0;

        /// <summary>
        /// True if the media player can pause
        /// </summary>
        public bool CanPause => Native.LibVLCMediaPlayerCanPause(NativeReference) != 0;

        /// <summary>
        /// True if the current program is scrambled
        /// <para></para>
        /// LibVLC 2.2.0 or later
        /// </summary>
        public bool ProgramScambled => Native.LibVLCMediaPlayerProgramScrambled(NativeReference) != 0;

        /// <summary>
        /// Display the next frame (if supported)
        /// </summary>
        public void NextFrame()
        {
            Native.LibVLCMediaPlayerNextFrame(NativeReference);
        }

        /// <summary>
        /// Navigate through DVD Menu
        /// </summary>
        /// <param name="navigate">the Navigation mode</param>
        /// LibVLC 2.0.0 or later
        public void Navigate(uint navigate)
        {
            Native.LibVLCMediaPlayerNavigate(NativeReference, navigate);
        }

        /// <summary>
        /// Set if, and how, the video title will be shown when media is played.
        /// </summary>
        /// <param name="position">position at which to display the title, or libvlc_position_disable to prevent the title from being displayed</param>
        /// <param name="timeout">title display timeout in milliseconds (ignored if libvlc_position_disable)</param>
        /// LibVLC 2.1.0 or later
        public void SetVideoTitleDisplay(Position position, uint timeout)
        {
            Native.LibVLCMediaPlayerSetVideoTitleDisplay(NativeReference, position, timeout);
        }

        /// <summary>
        /// Toggle fullscreen status on non-embedded video outputs.
        /// <para></para>
        /// warning: The same limitations applies to this function as to MediaPlayer::setFullscreen()
        /// </summary>
        public void ToggleFullscreen()
        {
            Native.LibVLCToggleFullscreen(NativeReference);
        }

        /// <summary>
        /// Enable or disable fullscreen. 
        /// Warning
        /// With most window managers, only a top-level windows can be in full-screen mode.
        /// Hence, this function will not operate properly if libvlc_media_player_set_xwindow() was used to embed the video in a non-top-level window. 
        /// In that case, the embedding window must be reparented to the root window before fullscreen mode is enabled.
        /// You will want to reparent it back to its normal parent when disabling fullscreen.
        /// <para></para>
        /// return the fullscreen status (boolean)
        /// </summary>
        public bool Fullscreen
        {
            get => Native.LibVLCGetFullscreen(NativeReference) != 0;
            set => Native.LibVLCSetFullscreen(NativeReference, value ? 1 : 0);
        }

        /// <summary>
        /// Toggle teletext transparent status on video output.
        /// </summary>
        [ApiVersion(3, 0, false, true)]
        public void ToggleTeletext()
        {
            Native.LibVLCToggleTeletext(NativeReference);
        }

        /// <summary>
        /// Apply new equalizer settings to a media player.
        /// The equalizer is first created by invoking libvlc_audio_equalizer_new() or libvlc_audio_equalizer_new_from_preset().
        /// It is possible to apply new equalizer settings to a media player whether the media player is currently playing media or not.
        /// Invoking this method will immediately apply the new equalizer settings to the audio output of the currently playing media if there is any.
        /// If there is no currently playing media, the new equalizer settings will be applied later if and when new media is played.
        /// Equalizer settings will automatically be applied to subsequently played media.
        /// To disable the equalizer for a media player invoke this method passing NULL for the p_equalizer parameter.
        /// The media player does not keep a reference to the supplied equalizer so it is safe for an application to release the equalizer reference
        /// any time after this method returns.
        /// </summary>
        /// <param name="equalizer">opaque equalizer handle, or NULL to disable the equalizer for this media player</param>
        /// LibVLC 2.2.0 or later
        /// <returns>true on success, false otherwise.</returns>
        public bool SetEqualizer(Equalizer equalizer) => Native.LibVLCMediaPlayerSetEqualizer(NativeReference, equalizer.NativeReference) == 0;

        /// <summary>
        /// unsetEqualizer disable equalizer for this media player
        /// </summary>
        /// <returns>true on success, false otherwise.</returns>
        public bool UnsetEqualizer() => Native.LibVLCMediaPlayerSetEqualizer(NativeReference, IntPtr.Zero) == 0;

        /// <summary>
        /// Sets callbacks and private data for decoded audio. 
        /// Use libvlc_audio_set_format() or libvlc_audio_set_format_callbacks() to configure the decoded audio format.
        /// Note: The audio callbacks override any other audio output mechanism. If the callbacks are set, LibVLC will not output audio in any way.
        /// </summary>
        /// <param name="playCb">callback to play audio samples (must not be NULL) </param>
        /// <param name="pauseCb">callback to pause playback (or NULL to ignore) </param>
        /// <param name="resumeCb">callback to resume playback (or NULL to ignore) </param>
        /// <param name="flushCb">callback to flush audio buffers (or NULL to ignore) </param>
        /// <param name="drainCb">callback to drain audio buffers (or NULL to ignore) </param>
        public void SetAudioCallbacks(LibVLCAudioPlayCb playCb, LibVLCAudioPauseCb pauseCb,
            LibVLCAudioResumeCb resumeCb, LibVLCAudioFlushCb flushCb,
            LibVLCAudioDrainCb drainCb)
        {
            Native.LibVLCAudioSetCallbacks(NativeReference, playCb, pauseCb, resumeCb, flushCb, drainCb, IntPtr.Zero);
        }

        /// <summary>
        /// Set callbacks and private data for decoded audio. 
        /// This only works in combination with libvlc_audio_set_callbacks(). 
        /// Use libvlc_audio_set_format() or libvlc_audio_set_format_callbacks() to configure the decoded audio format.
        /// </summary>
        /// <param name="volumeCb">callback to apply audio volume, or NULL to apply volume in software</param>
        public void SetVolumeCallback(LibVLCVolumeCb volumeCb)
        {
            Native.LibVLCAudioSetVolumeCallback(NativeReference, volumeCb);
        }

        /// <summary>
        /// Sets decoded audio format via callbacks. 
        /// This only works in combination with libvlc_audio_set_callbacks().
        /// </summary>
        /// <param name="setupCb">callback to select the audio format (cannot be NULL)</param>
        /// <param name="cleanupCb">callback to release any allocated resources (or NULL)</param>
        public void SetAudioFormatCallback(LibVLCAudioSetupCb setupCb, LibVLCAudioCleanupCb cleanupCb)
        {
            Native.LibVLCAudioSetFormatCallbacks(NativeReference, setupCb, cleanupCb);
        }

        /// <summary>
        /// Sets a fixed decoded audio format. 
        /// This only works in combination with libvlc_audio_set_callbacks(), and is mutually exclusive with libvlc_audio_set_format_callbacks().
        /// </summary>
        /// <param name="format">a four-characters string identifying the sample format (e.g. "S16N" or "FL32")</param>
        /// <param name="rate">sample rate (expressed in Hz)</param>
        /// <param name="channels">channels count</param>
        public void SetAudioFormat(string format, uint rate, uint channels)
        {
            Native.LibVLCAudioSetFormat(NativeReference, format, rate, channels);    
        }

        /// <summary>
        /// Selects an audio output module.
        /// Note: 
        /// Any change will take be effect only after playback is stopped and restarted.Audio output cannot be changed while playing.
        /// </summary>
        /// <param name="name">name of audio output, use psz_name of</param>
        /// <returns>0 if function succeeded, -1 on error</returns>
        public int SetAudioOutput(string name) => Native.LibVLCAudioOutputSet(NativeReference, name);

        // TODO
        /// <summary>
        /// Gets a list of potential audio output devices,. 
        /// </summary>
        public AudioOutputDescription[] OutputDeviceEnum
        {
            get
            {
                return new AudioOutputDescription[0];
            }
        }

        public void OutputDeviceSet(string deviceId, string module = null)
        {
            Native.LibVLCAudioOutputDeviceSet(NativeReference, module, deviceId);
        }

        /// <summary>
        /// Toggle mute status. 
        /// Warning
        /// Toggling mute atomically is not always possible: On some platforms, other processes can mute the VLC audio playback 
        /// stream asynchronously.
        /// Thus, there is a small race condition where toggling will not work.
        /// See also the limitations of libvlc_audio_set_mute(). 
        /// </summary>
        public void ToggleMute()
        {
            Native.LibVLCAudioToggleMute(NativeReference);
        }

        /// <summary>
        /// Get current mute status.
        /// Set mute status.
        /// Warning
        /// This function does not always work.
        /// If there are no active audio playback stream, the mute status might not be available.
        /// If digital pass-through (S/PDIF, HDMI...) is in use, muting may be unapplicable.
        /// Also some audio output plugins do not support muting at all.
        /// Note
        /// To force silent playback, disable all audio tracks. This is more efficient and reliable than mute. 
        /// </summary>
        public bool Mute
        {
            get => Native.LibVLCAudioGetMute(NativeReference) == 1;
            set => Native.LibVLCAudioSetMute(NativeReference, value ? 1 : 0);
        }

        /// <summary>
        /// Get/Set the volume in percents (0 = mute, 100 = 0dB)
        /// </summary>
        public int Volume
        {
            get => Native.LibVLCAudioGetVolume(NativeReference);
            set => Native.LibVLCAudioSetVolume(NativeReference, value);
        }

        /// <summary>
        /// Get the number of available audio tracks (int), or -1 if unavailable
        /// </summary>
        public int AudioTrackCount => Native.LibVLCAudioGetTrackCount(NativeReference);

        public TrackDescription[] AudioTrackDescription
        {
            get
            {
                var r = Native.LibVLCAudioGetTrackDescription(NativeReference);
                return GetTrackDescription(r);
            }
        }

        TrackDescription[] GetTrackDescription(IntPtr trackPtr)
        {
            if (trackPtr == IntPtr.Zero) return Array.Empty<TrackDescription>();

            var trackDescriptions = new List<TrackDescription>();
            var track = Marshal.PtrToStructure<TrackDescription>(trackPtr);

            while (true)
            {       
                trackDescriptions.Add(track);
                if (track.Next != IntPtr.Zero)
                {
                    track = Marshal.PtrToStructure<TrackDescription>(track.Next);
                }
                else
                {
                    break;    
                }
            }
            return trackDescriptions.ToArray();
        }

        /// <summary>
        /// Get current audio track ID or -1 if no active input.
        /// </summary>
        public int AudioTrack => Native.LibVLCAudioGetTrack(NativeReference);

        /// <summary>
        /// Set current audio track.
        /// </summary>
        /// <param name="trackIndex">the track ID (i_id field from track description)</param>
        /// <returns>0 on success, -1 on error</returns>
        public bool SetAudioTrack(int trackIndex) => Native.LibVLCAudioSetTrack(NativeReference, trackIndex) == 0;

        /// <summary>
        /// Get current audio channel.
        /// </summary>
        public int Channel => Native.LibVLCAudioGetChannel(NativeReference);

        /// <summary>
        /// Set current audio channel.
        /// </summary>
        /// <param name="channel">the audio channel</param>
        /// <returns></returns>
        public bool SetChannel(int channel) => Native.LibVLCAudioSetChannel(NativeReference, channel) == 0;

        public override bool Equals(object obj)
        {
            return obj is MediaPlayer player &&
                   EqualityComparer<IntPtr>.Default.Equals(NativeReference, player.NativeReference);
        }

        public override int GetHashCode()
        {
            return this.NativeReference.GetHashCode();
        }

        /// <summary>
        /// Get current audio delay (microseconds).
        /// </summary>
        public long AudioDelay => Native.LibVLCAudioGetDelay(NativeReference);

        /// <summary>
        /// Set current audio delay. The audio delay will be reset to zero each
        /// time the media changes.
        /// </summary>
        /// <param name="delay">the audio delay (microseconds)</param>
        /// <returns>true on success, false on error </returns>
        public bool SetAudioDelay(long delay) => Native.LibVLCAudioSetDelay(NativeReference, delay) == 0;

        /// <summary>
        /// Set callbacks and private data to render decoded video to a custom area in memory.
        /// Use libvlc_video_set_format() or libvlc_video_set_format_callbacks() to configure the decoded format.
        /// Warning
        /// Rendering video into custom memory buffers is considerably less efficient than rendering in a custom window as normal.
        /// For optimal perfomances, VLC media player renders into a custom window, and does not use this function and associated callbacks. 
        /// It is highly recommended that other LibVLC-based application do likewise. 
        /// To embed video in a window, use libvlc_media_player_set_xid() or equivalent depending on the operating system.
        /// If window embedding does not fit the application use case, then a custom LibVLC video output display plugin is required to maintain optimal video rendering performances.
        /// The following limitations affect performance:
        /// Hardware video decoding acceleration will either be disabled completely, or require(relatively slow) copy from video/DSP memory to main memory.
        /// Sub-pictures(subtitles, on-screen display, etc.) must be blent into the main picture by the CPU instead of the GPU.
        /// Depending on the video format, pixel format conversion, picture scaling, cropping and/or picture re-orientation,
        /// must be performed by the CPU instead of the GPU.
        /// Memory copying is required between LibVLC reference picture buffers and application buffers (between lock and unlock callbacks).
        /// </summary>
        /// <param name="lockCb">callback to lock video memory (must not be NULL)</param>
        /// <param name="unlockCb">callback to unlock video memory (or NULL if not needed)</param>
        /// <param name="displayCb">callback to display video (or NULL if not needed)</param>
        public void SetVideoCallbacks(LibVLCVideoLockCb lockCb, LibVLCVideoUnlockCb unlockCb,
            LibVLCVideoDisplayCb displayCb)
        {
            Native.LibVLCVideoSetCallbacks(NativeReference, lockCb, unlockCb, displayCb, IntPtr.Zero);
        }

        /// <summary>
        /// Set decoded video chroma and dimensions. This only works in
        /// combination with MediaPlayer::setCallbacks() , and is mutually exclusive
        /// with MediaPlayer::setFormatCallbacks()
        /// </summary>
        /// <param name="chroma">a four-characters string identifying the chroma (e.g."RV32" or "YUYV")</param>
        /// <param name="width">pixel width</param>
        /// <param name="height">pixel height</param>
        /// <param name="pitch">line pitch (in bytes)</param>
        public void SetVideoFormat(string chroma, uint width, uint height, uint pitch)
        {
            Native.LibVLCVideoSetFormat(NativeReference, chroma, width, height, pitch);
        }

        /// <summary>
        /// Set decoded video chroma and dimensions. 
        /// This only works in combination with libvlc_video_set_callbacks().
        /// </summary>
        /// <param name="formatCb">callback to select the video format (cannot be NULL)</param>
        /// <param name="cleanupCb">callback to release any allocated resources (or NULL)</param>
        public void SetVideoFormatCallbacks(LibVLCVideoFormatCb formatCb, LibVLCVideoCleanupCb cleanupCb)
        {
            Native.LibVLCVideoSetFormatCallbacks(NativeReference, formatCb, cleanupCb);
        }

        /// <summary>
        /// Enable or disable key press events handling, according to the LibVLC hotkeys configuration. 
        /// By default and for historical reasons, keyboard events are handled by the LibVLC video widget.
        /// Note
        /// On X11, there can be only one subscriber for key press and mouse click events per window.
        /// If your application has subscribed to those events for the X window ID of the video widget, 
        /// then LibVLC will not be able to handle key presses and mouse clicks in any case.
        /// Warning
        /// This function is only implemented for X11 and Win32 at the moment.
        /// true to handle key press events, false to ignore them. 
        /// </summary>
        public bool EnableKeyInput
        {
            set => Native.LibVLCVideoSetKeyInput(NativeReference, value ? 1 : 0);
        }

        /// <summary>
        /// Enable or disable mouse click events handling. 
        /// By default, those events are handled. This is needed for DVD menus to work, as well as a few video filters such as "puzzle".
        /// Warning
        /// This function is only implemented for X11 and Win32 at the moment.
        /// true to handle mouse click events, false to ignore them. 
        /// </summary>
        public bool EnableMouseInput
        {
            set => Native.LibVLCVideoSetMouseInput(NativeReference, value ? 1 : 0);
        }

        /// <summary>
        /// Get the pixel dimensions of a video.
        /// </summary>
        /// <param name="num">number of the video (starting from, and most commonly 0)</param>
        /// <param name="px">pointer to get the pixel width [OUT]</param>
        /// <param name="py">pointer to get the pixel height [OUT]</param>
        /// <returns></returns>
        public bool Size(uint num, ref uint px, ref uint py)
        {
            unsafe
            {
                fixed (uint* refPx = &px)
                {
                    var pxPtr = refPx;
                    fixed (uint* refPy = &py)
                    {
                        var pyPtr = refPy;
                        return Native.LibVLCVideoGetSize(NativeReference, num, pxPtr, pyPtr) == 0;
                    }
                }
            }
        }

        /// <summary>
        /// Get the mouse pointer coordinates over a video. 
        /// Coordinates are expressed in terms of the decoded video resolution, not in terms of pixels on the screen/viewport
        /// (to get the latter, you can query your windowing system directly).
        /// Either of the coordinates may be negative or larger than the corresponding dimension of the video, 
        /// if the cursor is outside the rendering area.
        /// Warning
        /// The coordinates may be out-of-date if the pointer is not located on the video rendering area.
        /// LibVLC does not track the pointer if it is outside of the video widget.
        /// Note
        /// LibVLC does not support multiple pointers(it does of course support multiple input devices sharing the same pointer) at the moment.
        /// </summary>
        /// <param name="num">number of the video (starting from, and most commonly 0)</param>
        /// <param name="px">pointer to get the abscissa [OUT]</param>
        /// <param name="py">pointer to get the ordinate [OUT]</param>
        /// <returns>true on success, false on failure</returns>
        public bool Cursor(uint num, ref int px, ref int py)
        {
            unsafe
            {
                fixed (int* refPx = &px)
                {
                    var pxPtr = refPx;
                    fixed (int* refPy = &py)
                    {
                        var pyPtr = refPy;
                        return Native.LibVLCVideoGetCursor(NativeReference, num, pxPtr, pyPtr) == 0;
                    }
                }
            }
        }

        /// <summary>
        /// Get/Set the current video scaling factor. See also MediaPlayer::setScale() .
        /// That is the ratio of the number of
        /// pixels on screen to the number of pixels in the original decoded video
        /// in each dimension.Zero is a special value; it will adjust the video
        /// to the output window/drawable(in windowed mode) or the entire screen.
        /// Note that not all video outputs support scaling.
        /// </summary>
        public float Scale
        {
            get => Native.LibVLCVideoGetScale(NativeReference);
            set => Native.LibVLCVideoSetScale(NativeReference, value);
        }

        /// <summary>
        /// Get/set current video aspect ratio.
        /// Set empty string to reset to default
        /// Invalid aspect ratios are ignored.
        /// </summary>
        public string AspectRatio
        {
            get => Native.LibVLCVideoGetAspectRatio(NativeReference);
            set => Native.LibVLCVideoSetAspectRatio(NativeReference, value);
        }

        public int Spu => Native.LibVLCVideoGetSpu(NativeReference);

        public bool SetSpu(int spu)
        {
            return Native.LibVLCVideoSetSpu(NativeReference, spu) == 0;
        } 

        public int SpuCount => Native.LibVLCVideoGetSpuCount(NativeReference);

        public TrackDescription[] SpuDescription
        {
            get
            {
                var ptr = Native.LibVLCVideoGetSpuDescription(NativeReference);
                return GetTrackDescription(ptr);
            }
        }

        /// <summary>
        /// Set new video subtitle file.
        /// </summary>
        /// <param name="subtitle">new video subtitle file</param>
        /// <returns></returns>
        [ApiVersion(3, 0, false, true)]
        public bool SetSubtitleFile(string subtitle)
        {
            return Native.LibVLCVideoSetSubtitleFile(NativeReference, subtitle) != 0;
        }

        public long SpuDelay => Native.LibVLCVideoGetSpuDelay(NativeReference);

        public bool SetSpuDelay(long delay) => Native.LibVLCVideoSetSpuDelay(NativeReference, delay) == 0;

        /// <summary>
        /// Get the description of available titles.
        /// </summary>
        // [ApiVersion(3, 0, false, true)]
        public TrackDescription[] TitleDescription
        {
            get
            {
                var ptr = Native.LibVLCVideoGetTitleDescription(NativeReference);
                return GetTrackDescription(ptr);
            }
        }

        /// <summary>
        /// Get the description of available chapters for specific title.
        /// </summary>
        /// <param name="titleIndex">selected title</param>
        /// <returns></returns>
        [ApiVersion(3, 0, false, true)]
        public TrackDescription[] ChapterDescription(int titleIndex)
        {
            var ptr = Native.LibVLCVideoGetChapterDescription(NativeReference, titleIndex);
            return GetTrackDescription(ptr);
        }

        /// <summary>
        /// Get/Set current crop filter geometry.
        /// Empty string to unset
        /// </summary>
        public string CropGeometry
        {
            get => Native.LibVLCVideoGetCropGeometry(NativeReference);
            set => Native.LibVLCVideoSetCropGeometry(NativeReference, value);
        }

        /// <summary>
        /// Get current teletext page requested.
        /// Set new teletext page to retrieve.
        /// </summary>
        public int Teletext
        {
            get => Native.LibVLCVideoGetTeletext(NativeReference);
            set => Native.LibVLCVideoSetTeletext(NativeReference, value);
        }

        /// <summary>
        /// Get number of available video tracks.
        /// </summary>
        public int VideoTrackCount => Native.LibVLCVideoGetTrackCount(NativeReference);

        /// <summary>
        /// Get the description of available video tracks.
        /// </summary>
        public TrackDescription[] VideoTrackDescription
        {
            get
            {
                var ptr = Native.LibVLCVideoGetTrackDescription(NativeReference);
                return GetTrackDescription(ptr);
            }
        }

        /// <summary>
        /// Get current video track ID (int) or -1 if no active input.
        /// </summary>
        public int VideoTrack => Native.LibVLCVideoGetTrack(NativeReference);

        /// <summary>
        /// Set video track.
        /// </summary>
        /// <param name="trackIndex">the track ID (i_id field from track description)</param>
        /// <returns>true on sucess, false out of range</returns>
        public bool SetVideoTrack(int trackIndex) => Native.LibVLCAudioSetTrack(NativeReference, trackIndex) == 0;

        /// <summary>
        /// Take a snapshot of the current video window.
        /// If i_width AND i_height is 0, original size is used. If i_width XOR
        /// i_height is 0, original aspect-ratio is preserved.
        /// </summary>
        /// <param name="num">number of video output (typically 0 for the first/only one)</param>
        /// <param name="filePath">the path where to save the screenshot to</param>
        /// <param name="width">the snapshot's width</param>
        /// <param name="height">the snapshot's height</param>
        /// <returns>true on success</returns>
        public bool TakeSnapshot(uint num, string filePath, uint width, uint height) => 
            Native.LibVLCVideoTakeSnapshot(NativeReference, num, filePath, width, height) == 0;

        /// <summary>
        /// Enable or disable deinterlace filter
        /// </summary>
        /// <param name="mode">type of deinterlace filter, empty string to disable</param>
        public void SetDeinterlace(string mode) => Native.LibVLCVideoSetDeinterlace(NativeReference, mode);

        /// <summary>
        /// Get an integer marquee option value
        /// </summary>
        /// <param name="option">marq option to get</param>
        /// <returns></returns>
        public int MarqueeInt(VideoMarqueeOption option) => Native.LibVLCVideoGetMarqueeInt(NativeReference, option);

        /// <summary>
        /// Get a string marquee option value
        /// </summary>
        /// <param name="option">marq option to get</param>
        /// <returns></returns>
        public string MarqueeString(VideoMarqueeOption option) => Native.LibVLCVideoGetMarqueeString(NativeReference, option);

        /// <summary>
        /// Enable, disable or set an integer marquee option
        /// Setting libvlc_marquee_Enable has the side effect of enabling (arg !0)
        /// or disabling (arg 0) the marq filter.
        /// </summary>
        /// <param name="option">marq option to set</param>
        /// <param name="value">marq option value</param>
        public void SetMarqueeInt(VideoMarqueeOption option, int value) =>
            Native.LibVLCVideoSetMarqueeInt(NativeReference, option, value);

        /// <summary>
        /// Enable, disable or set an string marquee option
        /// </summary>
        /// <param name="option">marq option to set</param>
        /// <param name="value">marq option value</param>
        public void SetMarqueeString(VideoMarqueeOption option, string value) =>
            Native.LibVLCVideoSetMarqueeString(NativeReference, option, value);

        /// <summary>
        /// Get integer logo option.
        /// </summary>
        /// <param name="option">logo option to get, values of libvlc_video_logo_option_t</param>
        /// <returns></returns>
        public int LogoInt(VideoLogoOption option) => Native.LibVLCVideoGetLogoInt(NativeReference, option);

        /// <summary>
        /// Set logo option as integer. Options that take a different type value
        /// are ignored. Passing libvlc_logo_enable as option value has the side
        /// effect of starting (arg !0) or stopping (arg 0) the logo filter.
        /// </summary>
        /// <param name="option">logo option to set, values of libvlc_video_logo_option_t</param>
        /// <param name="value">logo option value</param>
        public void SetLogoInt(VideoLogoOption option, int value) => Native.LibVLCVideoSetLogoInt(NativeReference, option, value);

        /// <summary>
        /// Set logo option as string. Options that take a different type value are ignored.
        /// </summary>
        /// <param name="option">logo option to set, values of libvlc_video_logo_option_t</param>
        /// <param name="value">logo option value</param>
        public void SetLogoString(VideoLogoOption option, string value) => Native.LibVLCVideoSetLogoString(NativeReference, option, value);

        /// <summary>
        /// Get integer adjust option.
        /// </summary>
        /// <param name="option">adjust option to get, values of libvlc_video_adjust_option_t</param>
        /// <returns></returns>
        public int AdjustInt(VideoAdjustOption option) => Native.LibVLCVideoGetAdjustInt(NativeReference, option);

        /// <summary>
        /// Set adjust option as integer. Options that take a different type value
        /// are ignored. Passing libvlc_adjust_enable as option value has the side
        /// effect of starting (arg !0) or stopping (arg 0) the adjust filter.
        /// </summary>
        /// <param name="option">adust option to set, values of libvlc_video_adjust_option_t</param>
        /// <param name="value">adjust option value</param>
        public void SetAdjustInt(VideoAdjustOption option, int value) => Native.LibVLCVideoSetAdjustInt(NativeReference, option, value);

        public float AdjustFloat(VideoAdjustOption option) => Native.LibVLCVideoGetAdjustFloat(NativeReference, option);

        /// <summary>
        /// Set adjust option as float. Options that take a different type value are ignored.
        /// </summary>
        /// <param name="option">adust option to set, values of <see cref="VideoAdjustOption"/></param>
        /// <param name="value">adjust option value</param>
        public void SetAdjustFloat(VideoAdjustOption option, float value) => Native.LibVLCVideoSetAdjustFloat(NativeReference, option, value);

#if ANDROID
        /// <summary>
        /// Set the android context.
        /// </summary>
        /// <param name="aWindow">See LibVLCSharp.Android</param>
        public void SetAndroidContext(IntPtr aWindow) => Native.LibVLCMediaPlayerSetAndroidContext(NativeReference, aWindow);
#endif

        /// <summary>
        /// Add a slave to the current media player.
        /// note If the player is playing, the slave will be added directly. This call
        /// will also update the slave list of the attached VLC::Media.
        /// </summary>
        /// <param name="type">subtitle or audio</param>
        /// <param name="uri">Uri of the slave (should contain a valid scheme).</param>
        /// <param name="select">True if this slave should be selected when it's loaded</param>
        /// <returns></returns>
        [ApiVersion(3)]
        public bool AddSlave(MediaSlaveType type, string uri, bool select) =>
            Native.LibVLCMediaPlayerAddSlave(NativeReference, type, uri, select) == 0;

        /// <summary>
        /// </summary>
        /// <param name="viewpoint"></param>
        /// <param name="absolute"></param>
        /// <returns></returns>
        [ApiVersion(3)]
        public bool UpdateViewpoint(VideoViewpoint viewpoint, bool absolute) =>
            Native.LibVLCVideoUpdateViewpoint(NativeReference, viewpoint, absolute) == 0;

        [ApiVersion(3)]
        public bool SetRenderer(RendererItem rendererItem) =>
            Native.LibVLCMediaPlayerSetRenderer(NativeReference, rendererItem.NativeReference) == 0;

#region Enums

     


#endregion

#region Callbacks
        
        /// <summary>
        /// <para>A LibVLC media player plays one media (usually in a custom drawable).</para>
        /// <para>@{</para>
        /// <para></para>
        /// <para>LibVLC simple media player external API</para>
        /// </summary>
        /// <summary>Opaque equalizer handle.</summary>
        /// <remarks>Equalizer settings can be applied to a media player.</remarks>
        /// <summary>Callback prototype to allocate and lock a picture buffer.</summary>
        /// <param name="opaque">private pointer as passed to libvlc_video_set_callbacks() [IN]</param>
        /// <param name="planes">
        /// <para>start address of the pixel planes (LibVLC allocates the array</para>
        /// <para>of void pointers, this callback must initialize the array) [OUT]</para>
        /// </param>
        /// <returns>
        /// <para>a private pointer for the display and unlock callbacks to identify</para>
        /// <para>the picture buffers</para>
        /// </returns>
        /// <remarks>
        /// <para>Whenever a new video frame needs to be decoded, the lock callback is</para>
        /// <para>invoked. Depending on the video chroma, one or three pixel planes of</para>
        /// <para>adequate dimensions must be returned via the second parameter. Those</para>
        /// <para>planes must be aligned on 32-bytes boundaries.</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr LibVLCVideoLockCb(IntPtr opaque, IntPtr planes);

        /// <summary>Callback prototype to unlock a picture buffer.</summary>
        /// <param name="opaque">private pointer as passed to libvlc_video_set_callbacks() [IN]</param>
        /// <param name="picture">private pointer returned from the</param>
        /// <param name="planes">pixel planes as defined by the</param>
        /// <remarks>
        /// <para>When the video frame decoding is complete, the unlock callback is invoked.</para>
        /// <para>This callback might not be needed at all. It is only an indication that the</para>
        /// <para>application can now read the pixel values if it needs to.</para>
        /// <para>A picture buffer is unlocked after the picture is decoded,</para>
        /// <para>but before the picture is displayed.</para>
        /// <para>callback [IN]</para>
        /// <para>callback (this parameter is only for convenience) [IN]</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCVideoUnlockCb(IntPtr opaque, IntPtr picture, IntPtr planes);

        /// <summary>Callback prototype to display a picture.</summary>
        /// <param name="opaque">private pointer as passed to libvlc_video_set_callbacks() [IN]</param>
        /// <param name="picture">private pointer returned from the</param>
        /// <remarks>
        /// <para>When the video frame needs to be shown, as determined by the media playback</para>
        /// <para>clock, the display callback is invoked.</para>
        /// <para>callback [IN]</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCVideoDisplayCb(IntPtr opaque, IntPtr picture);

        /// <summary>
        /// <para>Callback prototype to configure picture buffers format.</para>
        /// <para>This callback gets the format of the video as output by the video decoder</para>
        /// <para>and the chain of video filters (if any). It can opt to change any parameter</para>
        /// <para>as it needs. In that case, LibVLC will attempt to convert the video format</para>
        /// <para>(rescaling and chroma conversion) but these operations can be CPU intensive.</para>
        /// </summary>
        /// <param name="opaque">
        /// <para>pointer to the private pointer passed to</para>
        /// <para>libvlc_video_set_callbacks() [IN/OUT]</para>
        /// </param>
        /// <param name="chroma">pointer to the 4 bytes video format identifier [IN/OUT]</param>
        /// <param name="width">pointer to the pixel width [IN/OUT]</param>
        /// <param name="height">pointer to the pixel height [IN/OUT]</param>
        /// <param name="pitches">
        /// <para>table of scanline pitches in bytes for each pixel plane</para>
        /// <para>(the table is allocated by LibVLC) [OUT]</para>
        /// </param>
        /// <param name="lines">table of scanlines count for each plane [OUT]</param>
        /// <returns>the number of picture buffers allocated, 0 indicates failure</returns>
        /// <remarks>
        /// <para>For each pixels plane, the scanline pitch must be bigger than or equal to</para>
        /// <para>the number of bytes per pixel multiplied by the pixel width.</para>
        /// <para>Similarly, the number of scanlines must be bigger than of equal to</para>
        /// <para>the pixel height.</para>
        /// <para>Furthermore, we recommend that pitches and lines be multiple of 32</para>
        /// <para>to not break assumptions that might be held by optimized code</para>
        /// <para>in the video decoders, video filters and/or video converters.</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint LibVLCVideoFormatCb(ref IntPtr userData, IntPtr chroma, ref uint width, 
            ref uint height, ref uint pitches, ref uint lines);

        /// <summary>Callback prototype to configure picture buffers format.</summary>
        /// <param name="opaque">
        /// <para>private pointer as passed to libvlc_video_set_callbacks()</para>
        /// <para>(and possibly modified by</para>
        /// </param>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCVideoCleanupCb(ref IntPtr opaque);

        /// <summary>Callback prototype to setup the audio playback.</summary>
        /// <param name="opaque">
        /// <para>pointer to the data pointer passed to</para>
        /// <para>libvlc_audio_set_callbacks() [IN/OUT]</para>
        /// </param>
        /// <param name="format">4 bytes sample format [IN/OUT]</param>
        /// <param name="rate">sample rate [IN/OUT]</param>
        /// <param name="channels">channels count [IN/OUT]</param>
        /// <returns>0 on success, anything else to skip audio playback</returns>
        /// <remarks>This is called when the media player needs to create a new audio output.</remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LibVLCAudioSetupCb(ref IntPtr data, ref IntPtr format, ref uint rate, ref uint channels);

        /// <summary>Callback prototype for audio playback cleanup.</summary>
        /// <param name="opaque">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <remarks>This is called when the media player no longer needs an audio output.</remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioCleanupCb(IntPtr data);

        /// <summary>Callback prototype for audio playback.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <param name="samples">pointer to a table of audio samples to play back [IN]</param>
        /// <param name="count">number of audio samples to play back</param>
        /// <param name="pts">expected play time stamp (see libvlc_delay())</param>
        /// <remarks>
        /// <para>The LibVLC media player decodes and post-processes the audio signal</para>
        /// <para>asynchronously (in an internal thread). Whenever audio samples are ready</para>
        /// <para>to be queued to the output, this callback is invoked.</para>
        /// <para>The number of samples provided per invocation may depend on the file format,</para>
        /// <para>the audio coding algorithm, the decoder plug-in, the post-processing</para>
        /// <para>filters and timing. Application must not assume a certain number of samples.</para>
        /// <para>The exact format of audio samples is determined by libvlc_audio_set_format()</para>
        /// <para>or libvlc_audio_set_format_callbacks() as is the channels layout.</para>
        /// <para>Note that the number of samples is per channel. For instance, if the audio</para>
        /// <para>track sampling rate is 48000&#160;Hz, then 1200&#160;samples represent 25&#160;milliseconds</para>
        /// <para>of audio signal - regardless of the number of audio channels.</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioPlayCb(IntPtr data, IntPtr samples, uint count, long pts);

        /// <summary>Callback prototype for audio pause.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <param name="pts">time stamp of the pause request (should be elapsed already)</param>
        /// <remarks>
        /// <para>LibVLC invokes this callback to pause audio playback.</para>
        /// <para>The pause callback is never called if the audio is already paused.</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioPauseCb(IntPtr data, long pts);

        /// <summary>Callback prototype for audio resumption.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <param name="pts">time stamp of the resumption request (should be elapsed already)</param>
        /// <remarks>
        /// <para>LibVLC invokes this callback to resume audio playback after it was</para>
        /// <para>previously paused.</para>
        /// <para>The resume callback is never called if the audio is not paused.</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioResumeCb(IntPtr data, long pts);

        /// <summary>Callback prototype for audio buffer flush.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <remarks>
        /// <para>LibVLC invokes this callback if it needs to discard all pending buffers and</para>
        /// <para>stop playback as soon as possible. This typically occurs when the media is</para>
        /// <para>stopped.</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioFlushCb(IntPtr data, long pts);

        /// <summary>Callback prototype for audio buffer drain.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <remarks>
        /// <para>LibVLC may invoke this callback when the decoded audio track is ending.</para>
        /// <para>There will be no further decoded samples for the track, but playback should</para>
        /// <para>nevertheless continue until all already pending buffers are rendered.</para>
        /// </remarks>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioDrainCb(IntPtr data);

        /// <summary>Callback prototype for audio volume change.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <param name="volume">software volume (1. = nominal, 0. = mute)</param>
        /// <param name="mute">muted flag</param>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCVolumeCb(IntPtr data, float volume, [MarshalAs(UnmanagedType.I1)] bool mute);

#endregion
    }

    /// <summary>Description for titles</summary>
    public enum Title
    {
        Menu = 1,
        Interactive = 2
    }

    /// <summary>Marq options definition</summary>
    public enum VideoMarqueeOption
    {
        Enable = 0,
        Text = 1,
        /// <summary>string argument</summary>
        Color = 2,
        /// <summary>string argument</summary>
        Opacity = 3,
        /// <summary>string argument</summary>
        Position = 4,
        /// <summary>string argument</summary>
        Refresh = 5,
        /// <summary>string argument</summary>
        Size = 6,
        /// <summary>string argument</summary>
        Timeout = 7,
        /// <summary>string argument</summary>
        X = 8,
        /// <summary>string argument</summary>
        Y = 9
    }

    /// <summary>Navigation mode</summary>
    public enum NavigationMode
    {
        Activate = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        Popup = 5
    }

    /// <summary>Enumeration of values used to set position (e.g. of video title).</summary>
    public enum Position
    {
        Disable = -1,
        Center = 0,
        Left = 1,
        Right = 2,
        Top = 3,
        TopLeft = 4,
        TopRight = 5,
        Bottom = 6,
        BottomLeft = 7,
        BottomRight = 8
    }

    /// <summary>
    /// <para>Enumeration of teletext keys than can be passed via</para>
    /// <para>libvlc_video_set_teletext()</para>
    /// </summary>
    public enum TeletextKey
    {
        Red = 7471104,
        Green = 6750208,
        Yellow = 7929856,
        Blue = 6422528,
        Index = 6881280
    }

    /// <summary>option values for libvlc_video_{get,set}_logo_{int,string}</summary>
    public enum VideoLogoOption
    {
        Enable = 0,
        /// <summary>string argument, &quot;file,d,t;file,d,t;...&quot;</summary>
        File = 1,
        X = 2,
        Y = 3,
        Delay = 4,
        Repeat = 5,
        Opacity = 6,
        Position = 7
    }

    /// <summary>option values for libvlc_video_{get,set}_adjust_{int,float,bool}</summary>
    public enum VideoAdjustOption
    {
        Enable = 0,
        Contrast = 1,
        Brightness = 2,
        Hue = 3,
        Saturation = 4,
        Gamma = 5
    }

    /// <summary>Audio device types</summary>
    public enum AudioOutputDeviceType
    {
        DeviceError = -1,
        DeviceMono = 1,
        DeviceStereo = 2,
        Device2F2R = 4,
        Device3F2R = 5,
        Device5_1 = 6,
        Device6_1 = 7,
        Device7_1 = 8,
        DeviceSPDIF = 10
    }

    /// <summary>Audio channels</summary>
    public enum AudioOutputChannel
    {
        Error = -1,
        Stereo = 1,
        RStereo = 2,
        Left = 3,
        Right = 4,
        Dolbys = 5
    }

    /// <summary>Media player roles.</summary>
    /// <remarks>
    /// <para>LibVLC 3.0.0 and later.</para>
    /// <para>See</para>
    /// </remarks>
    public enum MediaPlayerRole
    {
        /// <summary>Don't use a media player role</summary>
        None = 0,
        /// <summary>Music (or radio) playback</summary>
        Music = 1,
        /// <summary>Video playback</summary>
        Video = 2,
        /// <summary>Speech, real-time communication</summary>
        Communication = 3,
        /// <summary>Video game</summary>
        Game = 4,
        /// <summary>User interaction feedback</summary>
        LiblvcRoleNotification = 5,
        /// <summary>Embedded animation (e.g. in web page)</summary>
        Animation = 6,
        /// <summary>Audio editting/production</summary>
        Production = 7,
        /// <summary>Accessibility</summary>
        Accessibility = 8,
        Test = 9
    }    
}
