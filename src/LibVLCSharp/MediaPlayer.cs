using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LibVLCSharp.Helpers;

namespace LibVLCSharp
{
    /// <summary>
    /// The MediaPlayer type is used to control playback, set renderers, provide events and much more
    /// </summary>
    public class MediaPlayer : Internal
    {
        readonly struct Native
        {
#if UNITY
            [DllImport(Constants.UnityPlugin, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_unity_media_player_new")]
            internal static extern IntPtr LibVLCMediaPlayerNew(IntPtr libvlc);

            [DllImport(Constants.UnityPlugin, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_unity_media_player_release")]
            internal static extern void LibVLCMediaPlayerRelease(IntPtr mediaPlayer);

            [DllImport(Constants.UnityPlugin, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_unity_get_texture")]
            internal static extern IntPtr GetTexture(IntPtr mediaPlayer, uint width, uint height, out bool updated);
#else
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_new")]
            internal static extern IntPtr LibVLCMediaPlayerNew(IntPtr libvlc);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_release")]
            internal static extern void LibVLCMediaPlayerRelease(IntPtr mediaPlayer);
#endif
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_new_from_media")]
            internal static extern IntPtr LibVLCMediaPlayerNewFromMedia(IntPtr libvlc, IntPtr media);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_media")]
            internal static extern void LibVLCMediaPlayerSetMedia(IntPtr mediaPlayer, IntPtr media);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_media")]
            internal static extern IntPtr LibVLCMediaPlayerGetMedia(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_event_manager")]
            internal static extern IntPtr LibVLCMediaPlayerEventManager(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_is_playing")]
            internal static extern int LibVLCMediaPlayerIsPlaying(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_play")]
            internal static extern int LibVLCMediaPlayerPlay(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_pause")]
            internal static extern void LibVLCMediaPlayerSetPause(IntPtr mediaPlayer, bool pause);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_pause")]
            internal static extern void LibVLCMediaPlayerPause(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_stop_async")]
            internal static extern int LibVLCMediaPlayerStop(IntPtr mediaPlayer);

#if APPLE || DESKTOP
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_nsobject")]
            internal static extern void LibVLCMediaPlayerSetNsobject(IntPtr mediaPlayer, IntPtr drawable);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_nsobject")]
            internal static extern IntPtr LibVLCMediaPlayerGetNsobject(IntPtr mediaPlayer);
#endif
#if DESKTOP
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_xwindow")]
            internal static extern void LibVLCMediaPlayerSetXwindow(IntPtr mediaPlayer, uint drawable);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_xwindow")]
            internal static extern uint LibVLCMediaPlayerGetXwindow(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_hwnd")]
            internal static extern void LibVLCMediaPlayerSetHwnd(IntPtr mediaPlayer, IntPtr drawable);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_hwnd")]
            internal static extern IntPtr LibVLCMediaPlayerGetHwnd(IntPtr mediaPlayer);
#endif
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_length")]
            internal static extern long LibVLCMediaPlayerGetLength(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_time")]
            internal static extern long LibVLCMediaPlayerGetTime(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_time")]
            internal static extern int LibVLCMediaPlayerSetTime(IntPtr mediaPlayer, long time, bool fast);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_position")]
            internal static extern float LibVLCMediaPlayerGetPosition(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_position")]
            internal static extern int LibVLCMediaPlayerSetPosition(IntPtr mediaPlayer, float position, bool fast);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_chapter")]
            internal static extern void LibVLCMediaPlayerSetChapter(IntPtr mediaPlayer, int chapter);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_chapter")]
            internal static extern int LibVLCMediaPlayerGetChapter(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_chapter_count")]
            internal static extern int LibVLCMediaPlayerGetChapterCount(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_chapter_count_for_title")]
            internal static extern int LibVLCMediaPlayerGetChapterCountForTitle(IntPtr mediaPlayer, int title);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_title")]
            internal static extern void LibVLCMediaPlayerSetTitle(IntPtr mediaPlayer, int title);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_title")]
            internal static extern int LibVLCMediaPlayerGetTitle(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_title_count")]
            internal static extern int LibVLCMediaPlayerGetTitleCount(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_previous_chapter")]
            internal static extern void LibVLCMediaPlayerPreviousChapter(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_next_chapter")]
            internal static extern void LibVLCMediaPlayerNextChapter(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_rate")]
            internal static extern float LibVLCMediaPlayerGetRate(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_rate")]
            internal static extern int LibVLCMediaPlayerSetRate(IntPtr mediaPlayer, float rate);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_state")]
            internal static extern VLCState LibVLCMediaPlayerGetState(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_has_vout")]
            internal static extern uint LibVLCMediaPlayerHasVout(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_is_seekable")]
            internal static extern int LibVLCMediaPlayerIsSeekable(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_can_pause")]
            internal static extern int LibVLCMediaPlayerCanPause(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_program_scrambled")]
            internal static extern int LibVLCMediaPlayerProgramScrambled(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_programlist")]
            internal static extern IntPtr LibVLCMediaPlayerGetProgramList(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_program_from_id")]
            internal static extern IntPtr LibVLCMediaPlayerGetProgramFromId(IntPtr mediaPlayer, int groupId);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_selected_program")]
            internal static extern IntPtr LibVLCMediaPlayerGetSelectedProgram(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_select_program_id")]
            internal static extern void LibVLCMediaPlayerSelectProgramId(IntPtr mediaPlayer, int programId);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_player_program_delete")]
            internal static extern void LibVLCPlayerProgramDelete(IntPtr program);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_next_frame")]
            internal static extern void LibVLCMediaPlayerNextFrame(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_navigate")]
            internal static extern void LibVLCMediaPlayerNavigate(IntPtr mediaPlayer, uint navigate);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_video_title_display")]
            internal static extern void LibVLCMediaPlayerSetVideoTitleDisplay(IntPtr mediaPlayer, Position position, uint timeout);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_toggle_fullscreen")]
            internal static extern void LibVLCToggleFullscreen(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_fullscreen")]
            internal static extern void LibVLCSetFullscreen(IntPtr mediaPlayer, int fullscreen);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_fullscreen")]
            internal static extern int LibVLCGetFullscreen(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_equalizer")]
            internal static extern int LibVLCMediaPlayerSetEqualizer(IntPtr mediaPlayer, IntPtr equalizer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_callbacks")]
            internal static extern void LibVLCAudioSetCallbacks(IntPtr mediaPlayer, LibVLCAudioPlayCb play, LibVLCAudioPauseCb? pause,
                LibVLCAudioResumeCb? resume, LibVLCAudioFlushCb? flush, LibVLCAudioDrainCb? drain, IntPtr opaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_volume_callback")]
            internal static extern void LibVLCAudioSetVolumeCallback(IntPtr mediaPlayer, LibVLCVolumeCb? volumeCallback);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_format_callbacks")]
            internal static extern void LibVLCAudioSetFormatCallbacks(IntPtr mediaPlayer, LibVLCAudioSetupCb setup, LibVLCAudioCleanupCb? cleanup);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_format")]
            internal static extern void LibVLCAudioSetFormat(IntPtr mediaPlayer, IntPtr format,
                uint rate, uint channels);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_enum")]
            internal static extern IntPtr LibVLCAudioOutputDeviceEnum(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_set")]
            internal static extern int LibVLCAudioOutputDeviceSet(IntPtr mediaPlayer, IntPtr deviceId);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_get")]
            internal static extern IntPtr LibVLCAudioOutputDeviceGet(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_set")]
            internal static extern int LibVLCAudioOutputSet(IntPtr mediaPlayer, IntPtr name);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_toggle_mute")]
            internal static extern void LibVLCAudioToggleMute(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_mute")]
            internal static extern int LibVLCAudioGetMute(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_mute")]
            internal static extern void LibVLCAudioSetMute(IntPtr mediaPlayer, int status);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_volume")]
            internal static extern int LibVLCAudioGetVolume(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_volume")]
            internal static extern int LibVLCAudioSetVolume(IntPtr mediaPlayer, int volume);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_channel")]
            internal static extern AudioOutputChannel LibVLCAudioGetChannel(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_channel")]
            internal static extern int LibVLCAudioSetChannel(IntPtr mediaPlayer, AudioOutputChannel channel);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_get_delay")]
            internal static extern long LibVLCAudioGetDelay(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_set_delay")]
            internal static extern int LibVLCAudioSetDelay(IntPtr mediaPlayer, long delay);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_callbacks")]
            internal static extern void LibVLCVideoSetCallbacks(IntPtr mediaPlayer, LibVLCVideoLockCb lockCallback,
                LibVLCVideoUnlockCb? unlock, LibVLCVideoDisplayCb? display, IntPtr opaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_format")]
            internal static extern void LibVLCVideoSetFormat(IntPtr mediaPlayer, IntPtr chroma,
                uint width, uint height, uint pitch);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_format_callbacks")]
            internal static extern void LibVLCVideoSetFormatCallbacks(IntPtr mediaPlayer, LibVLCVideoFormatCb setup,
                LibVLCVideoCleanupCb? cleanup);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_key_input")]
            internal static extern void LibVLCVideoSetKeyInput(IntPtr mediaPlayer, int enable);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_mouse_input")]
            internal static extern void LibVLCVideoSetMouseInput(IntPtr mediaPlayer, int enable);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_size")]
            internal static extern int LibVLCVideoGetSize(IntPtr mediaPlayer, uint num, ref uint px, ref uint py);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_cursor")]
            internal static extern int LibVLCVideoGetCursor(IntPtr mediaPlayer, uint num, ref int px, ref int py);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_scale")]
            internal static extern float LibVLCVideoGetScale(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_scale")]
            internal static extern void LibVLCVideoSetScale(IntPtr mediaPlayer, float factor);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_aspect_ratio")]
            internal static extern IntPtr LibVLCVideoGetAspectRatio(IntPtr mediaPlayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_aspect_ratio")]
            internal static extern void LibVLCVideoSetAspectRatio(IntPtr mediaPlayer, IntPtr aspect);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_spu_delay")]
            internal static extern long LibVLCVideoGetSpuDelay(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_spu_delay")]
            internal static extern int LibVLCVideoSetSpuDelay(IntPtr mediaPlayer, long delay);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_full_title_descriptions")]
            internal static extern int LibVLCMediaPlayerGetFullTitleDescriptions(IntPtr mediaPlayer, IntPtr titles);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_title_descriptions_release")]
            internal static extern void LibVLCTitleDescriptionsRelease(IntPtr titles, uint count);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_full_chapter_descriptions")]
            internal static extern int LibVLCMediaPlayerGetFullChapterDescriptions(IntPtr mediaPlayer, int titleIndex, out IntPtr chapters);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_chapter_descriptions_release")]
            internal static extern void LibVLCChapterDescriptionsRelease(IntPtr chapters, uint count);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_teletext")]
            internal static extern int LibVLCVideoGetTeletext(IntPtr mediaPlayer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_teletext")]
            internal static extern void LibVLCVideoSetTeletext(IntPtr mediaPlayer, int page);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_take_snapshot")]
            internal static extern int LibVLCVideoTakeSnapshot(IntPtr mediaPlayer, uint num,
                IntPtr filepath, uint width, uint height);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_deinterlace")]
            internal static extern void LibVLCVideoSetDeinterlace(IntPtr mediaPlayer, int deinterlace, IntPtr deinterlaceType);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_marquee_int")]
            internal static extern int LibVLCVideoGetMarqueeInt(IntPtr mediaPlayer, VideoMarqueeOption option);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_marquee_int")]
            internal static extern void LibVLCVideoSetMarqueeInt(IntPtr mediaPlayer, VideoMarqueeOption option, int marqueeValue);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_marquee_string")]
            internal static extern void LibVLCVideoSetMarqueeString(IntPtr mediaPlayer, VideoMarqueeOption option, IntPtr marqueeValue);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_logo_int")]
            internal static extern int LibVLCVideoGetLogoInt(IntPtr mediaPlayer, VideoLogoOption option);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_logo_int")]
            internal static extern void LibVLCVideoSetLogoInt(IntPtr mediaPlayer, VideoLogoOption option, int value);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_logo_string")]
            internal static extern void LibVLCVideoSetLogoString(IntPtr mediaPlayer, VideoLogoOption option, IntPtr logoOptionValue);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_adjust_int")]
            internal static extern int LibVLCVideoGetAdjustInt(IntPtr mediaPlayer, VideoAdjustOption option);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_adjust_int")]
            internal static extern void LibVLCVideoSetAdjustInt(IntPtr mediaPlayer, VideoAdjustOption option, int value);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_adjust_float")]
            internal static extern float LibVLCVideoGetAdjustFloat(IntPtr mediaPlayer, VideoAdjustOption option);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_adjust_float")]
            internal static extern void LibVLCVideoSetAdjustFloat(IntPtr mediaPlayer, VideoAdjustOption option, float value);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_add_slave")]
            internal static extern int LibVLCMediaPlayerAddSlave(IntPtr mediaPlayer, MediaSlaveType mediaSlaveType,
                IntPtr uri, bool selectWhenloaded);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_new_viewpoint")]
            internal static extern IntPtr LibVLCVideoNewViewpoint();

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_update_viewpoint")]
            internal static extern int LibVLCVideoUpdateViewpoint(IntPtr mediaPlayer, IntPtr viewpoint, bool absolute);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_list_release")]
            internal static extern void LibVLCAudioOutputDeviceListRelease(IntPtr list);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_renderer")]
            internal static extern int LibVLCMediaPlayerSetRenderer(IntPtr mediaplayer, IntPtr renderItem);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_role")]
            internal static extern MediaPlayerRole LibVLCMediaPlayerGetRole(IntPtr mediaplayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_role")]
            internal static extern int LibVLCMediaPlayerSetRole(IntPtr mediaplayer, MediaPlayerRole role);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_retain")]
            internal static extern void LibVLCMediaPlayerRetain(IntPtr mediaplayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_selected_track")]
            internal static extern IntPtr LibVLCMediaPlayerGetSelectedTrack(IntPtr mediaplayer, TrackType type);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_tracklist")]
            internal static extern IntPtr LibVLCMediaPlayerGetTrackList(IntPtr mediaplayer, TrackType type);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_get_track_from_id")]
            internal static extern IntPtr LibVLCMediaPlayerGetTrackFromId(IntPtr mediaplayer, IntPtr id);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_select_track")]
            internal static extern void LibVLCMediaPlayerSelectTrack(IntPtr mediaplayer, IntPtr track);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_unselect_track_type")]
            internal static extern void LibVLCMediaPlayerUnselectTrackType(IntPtr mediaplayer, TrackType type);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_select_tracks")]
            internal static extern void LibVLCMediaPlayerSelectTracks(IntPtr mediaplayer, TrackType type, IntPtr tracks, UIntPtr count);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_select_tracks_by_ids")]
            internal static extern void LibVLCMediaPlayerSelectTracksByIds(IntPtr mediaplayer, TrackType type, IntPtr ids);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_crop_ratio")]
            internal static extern void LibVLCVideoSetCropRatio(IntPtr mediaplayer, uint num, uint den);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_crop_window")]
            internal static extern void LibVLCVideoSetCropWindow(IntPtr mediaplayer, uint x, uint y, uint width, uint height);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_crop_border")]
            internal static extern void LibVLCVideoSetCropBorder(IntPtr mediaplayer, uint left, uint right, uint top, uint bottom);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_spu_text_scale")]
            internal static extern void LibVLCVideoSetSpuTextScale(IntPtr mediaplayer, float scale);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_get_spu_text_scale")]
            internal static extern float LibVLCVideoGetSpuTextScale(IntPtr mediaplayer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_set_output_callbacks")]
            internal static extern bool LibVLCVideoSetOutputCallbacks(IntPtr mediaplayer, VideoEngine engine, OutputSetup? outputSetup, 
                OutputCleanup? outputCleanup, OutputSetResize? resize, UpdateOutput updateOutput, Swap swap, MakeCurrent makeCurrent, 
                GetProcAddress? getProcAddress, FrameMetadata? metadata, OutputSelectPlane? selectPlane, IntPtr opaque);
#if ANDROID
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_android_context")]
            internal static extern void LibVLCMediaPlayerSetAndroidContext(IntPtr mediaPlayer, IntPtr aWindow);
#endif
        }

        MediaPlayerEventManager? _eventManager;

        /// <summary>
        /// The GCHandle to be passed to callbacks as userData
        /// </summary>
        GCHandle _gcHandle;

        /// <summary>Create an empty Media Player object</summary>
        /// <param name="libVLC">
        /// <para>the libvlc instance in which the Media Player</para>
        /// <para>should be created.</para>
        /// </param>
        /// <returns>a new media player object, or NULL on error.</returns>
        public MediaPlayer(LibVLC libVLC)
            : base(() => Native.LibVLCMediaPlayerNew(libVLC.NativeReference), Native.LibVLCMediaPlayerRelease)
        {
            _gcHandle = GCHandle.Alloc(this);
        }

#if !UNITY
        /// <summary>Create a Media Player object from a Media</summary>
        /// <param name="libvlc">LibVLC instance to create a media player with</param>
        /// <param name="media">The media to play. Afterwards the p_md can be safely destroyed.</param>
        /// <returns>a new media player object, or NULL on error.</returns>
        public MediaPlayer(LibVLC libvlc, Media media)
            : base(() => Native.LibVLCMediaPlayerNewFromMedia(libvlc.NativeReference, media.NativeReference), Native.LibVLCMediaPlayerRelease)
        {
            _gcHandle = GCHandle.Alloc(this);
        }
#endif
        /// <summary>
        /// Get the media used by the media_player.
        /// Set the media that will be used by the media_player.
        /// If any, previous md will be released.
        /// Note: It is safe to release the Media on the C# side after it's been set on the MediaPlayer successfully
        /// </summary>
        public Media? Media
        {
            get
            {
                var mediaPtr = Native.LibVLCMediaPlayerGetMedia(NativeReference);
                return mediaPtr == IntPtr.Zero ? null : new Media(mediaPtr);
            }
            set => Native.LibVLCMediaPlayerSetMedia(NativeReference, value?.NativeReference ?? IntPtr.Zero);
        }

        /// <summary>
        /// return true if the media player is playing, false otherwise
        /// </summary>
        public bool IsPlaying => Native.LibVLCMediaPlayerIsPlaying(NativeReference) != 0;

        /// <summary>
        /// Starts playback with Media that is set
        /// If playback was already started, this method has no effect
        /// </summary>
        /// <returns>
        /// Returns true if the playback will start successfully, false otherwise. 
        /// This function returns immediately, as it sends a command to the native library
        /// but does not wait for wait for the state change. Use <see cref="PlayAsync"/>
        /// if you want to wait asynchronously for playback to start.
        /// </returns>
        /// <remarks>
        /// Playback may not actually have yet started when this function returns.
        /// </remarks>
        public bool Play()
        {
            var media = Media;
            if(media != null)
            {
                media.AddOption(Configuration);
                media.Dispose();
            }
            return Native.LibVLCMediaPlayerPlay(NativeReference) == 0;
        }

        /// <summary>
        /// Sets the Media and starts playback
        /// If playback was already started, this method has no effect
        /// </summary>
        /// <returns>
        /// Returns true if the playback will start successfully, false otherwise. 
        /// This function returns immediately, as it sends a command to the native library
        /// but does not wait for wait for the state change. Use <see cref="PlayAsync"/>
        /// if you want to wait asynchronously for playback to start.
        /// </returns>
        /// <remarks>
        /// Playback may not actually have yet started when this function returns.
        /// </remarks>
        public bool Play(Media media)
        {
            Media = media;
            return Play();
        }

        /// <summary>
        /// Starts playback with Media that is set
        /// If playback was already started, this method has no effect
        /// </summary>
        /// <returns>
        /// Returns true if the playback started successfully, false otherwise. 
        /// This function sends the command to start playback to libvlc,
        /// and waits for the Playing event to fire. This makes the function asynchonously blocking 
        /// and may take a bit of time, but you can be sure playback actually started when this method finally returns (true).
        /// </returns>
        /// <remarks>
        /// Playback has actually started when this function returns (if return value is true).
        /// </remarks>
        public Task<bool> PlayAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            void MediaPlayer_Play(object? sender, EventArgs e) => tcs.SetResult(true);

            return MarshalUtils.InternalAsync(
                nativeCall: () =>
                {
                    if (!Play())
                        tcs.SetResult(false);
                },
                sub: () => Playing += MediaPlayer_Play,
                unsub: () => Playing -= MediaPlayer_Play,
                tcs: tcs);
        }

        /// <summary>
        /// Pause or resume (no effect if there is no media).
        /// version LibVLC 1.1.1 or later
        /// </summary>
        /// <param name="pause">play/resume if true, pause if false</param>
        public void SetPause(bool pause) => Native.LibVLCMediaPlayerSetPause(NativeReference, pause);

        /// <summary>
        /// Toggle pause (no effect if there is no media)
        /// <br/>
        /// This function returns immediately, as it sends a command to the native library
        /// but does not wait for wait for the state change. 
        /// <br/>
        /// Use <see cref="PauseAsync"/> if you want to wait asynchronously for the playback to pause.
        /// </summary>
        public void Pause() => Native.LibVLCMediaPlayerPause(NativeReference);

        /// <summary>
        /// Pauses the playback and waits asynchronously for the change to be effective
        /// </summary>
        /// <returns>Task result of the async operation</returns>
        /// <remarks>
        /// Native playback pipeline has actually been paused when this function returns.
        /// </remarks>
        public Task PauseAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            void MediaPlayer_Pause(object? sender, EventArgs e) => tcs.SetResult(true);

            return MarshalUtils.InternalAsync(
                nativeCall: () => Pause(),
                sub: () => Playing += MediaPlayer_Pause,
                unsub: () => Playing -= MediaPlayer_Pause,
                tcs: tcs);
        }

        /// <summary>
        /// Stops the playback pipeline (no effect if there is no media)
        /// <br/>
        /// While the native function is asynchronous, it returns immediately, 
        /// as it sends a command to the native library but does not wait for the state change. 
        /// <br/>
        /// Use <see cref="StopAsync"/> if you want to wait asynchronously until the playback is stopped.
        /// </summary>
        /// <returns>true if the player is being stopped, false otherwise</returns>
        public bool Stop() => Native.LibVLCMediaPlayerStop(NativeReference) == 0;

        /// <summary>
        /// Stops the playback and waits asynchronously for the change to be effective
        /// </summary>
        /// <returns>Task result of the async operation</returns>
        /// <remarks>
        /// Native playback pipeline has actually been stopped when this function returns.
        /// </remarks>
        public Task<bool> StopAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            void MediaPlayer_Stopped(object? sender, EventArgs e) => tcs.SetResult(true);

            return MarshalUtils.InternalAsync(
                nativeCall: () => Stop(),
                sub: () => Stopped += MediaPlayer_Stopped,
                unsub: () => Stopped -= MediaPlayer_Stopped,
                tcs: tcs);
        }

#if APPLE || DESKTOP
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

#if DESKTOP
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
        /// Set the movie time (in ms). This has no effect if no media is being played.
        /// Not all formats and protocols support this.
        /// </summary>
        /// <param name="time">the movie time (in ms)</param>
        /// <param name="fast">prefer fast seeking or precise seeking</param>
        /// <returns>true on success, false otherwise</returns>
        public bool SetTime(long time, bool fast = false) => Native.LibVLCMediaPlayerSetTime(NativeReference, time, fast) == 0;

        /// <summary>
        /// Get the movie time (in ms), or -1 if there is no media.
        /// </summary>
        public long Time => Native.LibVLCMediaPlayerGetTime(NativeReference);

        /// <summary>
        /// Set movie position as percentage between 0.0 and 1.0.
        /// This has no effect if playback is not enabled.
        /// This might not work depending on the underlying input format and protocol.
        /// </summary>
        /// <param name="position">the position</param>
        /// <param name="fast">prefer fast seeking or precise seeking</param>
        /// <returns>true on success, false otherwise</returns>
        public bool SetPosition(float position, bool fast = false) => Native.LibVLCMediaPlayerSetPosition(NativeReference, position, fast) == 0;

        /// <summary>
        /// Get movie position as percentage between 0.0 and 1.0.
        /// Returns movie position, or -1. in case of error
        /// </summary>
        public float Position => Native.LibVLCMediaPlayerGetPosition(NativeReference);

        /// <summary>
        /// Set the movie time. This has no effect if no media is being
        /// played. Not all formats and protocols support this.
        /// </summary>
        /// <param name="time">the movie time to seek to</param>
        /// <param name="fast">prefer fast seeking or precise seeking</param>
        /// <returns>true on success, false otherwise</returns>
        public bool SeekTo(TimeSpan time, bool fast = false) => SetTime((long)time.TotalMilliseconds, fast);

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
        /// return false if an error was detected, true otherwise (but even then, it
        /// might not actually work depending on the underlying media protocol)
        /// </returns>
        public bool SetRate(float rate) => Native.LibVLCMediaPlayerSetRate(NativeReference, rate) == 0;

        /// <summary>
        /// Get the current state of the media player (playing, paused, ...)
        /// </summary>
        public VLCState State => Native.LibVLCMediaPlayerGetState(NativeReference);

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
        /// Warning, TL;DR version : Unless you know what you're doing, don't use this.
        /// Put your VideoView inside a fullscreen control instead, refer to your platform documentation.
        /// <para></para>
        /// Warning, long version :
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

        LibVLCAudioPlayCb? _audioPlayCb;
        LibVLCAudioPauseCb? _audioPauseCb;
        LibVLCAudioResumeCb? _audioResumeCb;
        LibVLCAudioFlushCb? _audioFlushCb;
        LibVLCAudioDrainCb? _audioDrainCb;
        IntPtr _audioUserData = IntPtr.Zero;

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
        public void SetAudioCallbacks(LibVLCAudioPlayCb playCb, LibVLCAudioPauseCb? pauseCb,
            LibVLCAudioResumeCb? resumeCb, LibVLCAudioFlushCb? flushCb,
            LibVLCAudioDrainCb? drainCb)
        {
            _audioPlayCb = playCb ?? throw new ArgumentNullException(nameof(playCb));
            _audioPauseCb = pauseCb;
            _audioResumeCb = resumeCb;
            _audioFlushCb = flushCb;
            _audioDrainCb = drainCb;

            Native.LibVLCAudioSetCallbacks(
                NativeReference,
                AudioPlayCallbackHandle,
                (pauseCb == null) ? null : AudioPauseCallbackHandle,
                (resumeCb == null) ? null : AudioResumeCallbackHandle,
                (flushCb == null) ? null : AudioFlushCallbackHandle,
                (drainCb == null) ? null : AudioDrainCallbackHandle,
                GCHandle.ToIntPtr(_gcHandle));
        }

        LibVLCVolumeCb? _audioVolumeCb;

        /// <summary>
        /// Set callbacks and private data for decoded audio.
        /// This only works in combination with libvlc_audio_set_callbacks().
        /// Use libvlc_audio_set_format() or libvlc_audio_set_format_callbacks() to configure the decoded audio format.
        /// </summary>
        /// <param name="volumeCb">callback to apply audio volume, or NULL to apply volume in software</param>
        public void SetVolumeCallback(LibVLCVolumeCb volumeCb)
        {
            _audioVolumeCb = volumeCb;
            Native.LibVLCAudioSetVolumeCallback(NativeReference, (volumeCb == null) ? null : AudioVolumeCallbackHandle);
        }

        LibVLCAudioSetupCb? _setupCb;
        LibVLCAudioCleanupCb? _cleanupCb;

        /// <summary>
        /// Sets decoded audio format via callbacks.
        /// This only works in combination with libvlc_audio_set_callbacks().
        /// </summary>
        /// <param name="setupCb">callback to select the audio format (cannot be NULL)</param>
        /// <param name="cleanupCb">callback to release any allocated resources (or NULL)</param>
        public void SetAudioFormatCallback(LibVLCAudioSetupCb setupCb, LibVLCAudioCleanupCb cleanupCb)
        {
            _setupCb = setupCb ?? throw new ArgumentNullException(nameof(setupCb));
            _cleanupCb = cleanupCb;
            Native.LibVLCAudioSetFormatCallbacks(NativeReference, AudioSetupCallbackHandle, (cleanupCb == null) ? null : AudioCleanupCallbackHandle);
        }

        /// <summary>
        /// Sets a fixed decoded audio format.
        /// This only works in combination with libvlc_audio_set_callbacks(), and is mutually exclusive with libvlc_audio_set_format_callbacks().
        /// The supported formats are:<para />
        /// - "S16N" for signed 16-bit PCM<para />
        /// - "S32N" for signed 32-bit PCM<para />
        /// - "FL32" for single precision IEEE 754<para />
        /// All supported formats use the native endianness. If there are more than one channel, samples are interleaved.
        /// </summary>
        /// <param name="format">a four-characters string identifying the sample format</param>
        /// <param name="rate">sample rate (expressed in Hz)</param>
        /// <param name="channels">channels count</param>
        public void SetAudioFormat(string format, uint rate, uint channels)
        {
            var formatUtf8 = format.ToUtf8();
            MarshalUtils.PerformInteropAndFree(() => Native.LibVLCAudioSetFormat(NativeReference, formatUtf8, rate, channels), formatUtf8);
        }

        /// <summary>
        /// Selects an audio output module.
        /// Note:
        /// Any change will take effect only after playback is stopped and restarted. Audio output cannot be changed while playing.
        /// </summary>
        /// <param name="name">name of audio output, use psz_name of</param>
        /// <returns>true if function succeeded, false on error</returns>
        public bool SetAudioOutput(string name)
        {
            var nameUtf8 = name.ToUtf8();
            return MarshalUtils.PerformInteropAndFree(() => Native.LibVLCAudioOutputSet(NativeReference, nameUtf8), nameUtf8) == 0;
        }

        /// <summary>
        /// Get the current audio output device identifier.
        /// This complements <see cref="SetOutputDevice"/>
        /// warning The initial value for the current audio output device identifier
        /// may not be set or may be some unknown value.A LibVLC application should
        /// compare this value against the known device identifiers (e.g.those that
        /// were previously retrieved by a call to <see cref="AudioOutputDeviceEnum"/> 
        /// to find the current audio output device.
        /// It is possible that the selected audio output device changes(an external
        /// change) without a call to <see cref="SetOutputDevice"/>.That may make this
        /// method unsuitable to use if a LibVLC application is attempting to track
        /// dynamic audio device changes as they happen.
        /// </summary>
        /// <returns>the current audio output device identifier, or NULL if no device is selected or in case of error.</returns>
        public string? OutputDevice => Native.LibVLCAudioOutputDeviceGet(NativeReference).FromUtf8(libvlcFree: true);

        /// <summary>
        /// Configures an explicit audio output device.
        /// A list of adequate potential device strings can be obtained with
        /// <see cref="AudioOutputDeviceEnum"/>
        /// </summary>
        /// <param name="deviceId">device identifier string</param>
        /// <returns>True if the change of device was requested successfully 
        /// (the actual change is asynchronous and not guaranteed to succeed). On error, this function returns false.
        /// </returns>
        public bool SetOutputDevice(string deviceId)
        {
            var deviceIdUtf8 = deviceId.ToUtf8();
            return MarshalUtils.PerformInteropAndFree(() =>
                Native.LibVLCAudioOutputDeviceSet(NativeReference, deviceIdUtf8), deviceIdUtf8) == 0;
        }

        /// <summary>
        /// Gets a list of potential audio output devices
        /// <para/> Not all audio outputs support enumerating devices. The audio output may be functional even if the list is empty (NULL).
        /// The list may not be exhaustive. Some audio output devices in the list might not actually work in some circumstances.
        /// <para/> By default, it is recommended to not specify any explicit audio device.
        /// </summary>
        public AudioOutputDevice[] AudioOutputDeviceEnum =>
           MarshalUtils.Retrieve(() => Native.LibVLCAudioOutputDeviceEnum(NativeReference),
           MarshalUtils.PtrToStructure<AudioOutputDeviceStructure>,
           s => s.Build(),
           device => device.Next,
           Native.LibVLCAudioOutputDeviceListRelease);

        /// <summary>
        /// Toggle mute status.
        /// Warning
        /// Toggling mute atomically is not always possible: On some platforms, other processes can mute the VLC audio playback
        /// stream asynchronously.
        /// Thus, there is a small race condition where toggling will not work.
        /// See also the limitations of libvlc_audio_set_mute().
        /// </summary>
        public void ToggleMute() => Native.LibVLCAudioToggleMute(NativeReference);

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
        /// Get the volume in percents (0 = mute, 100 = 0dB)
        /// </summary>
        public int Volume
        {
            get => Native.LibVLCAudioGetVolume(NativeReference);
        }

        /// <summary>
        /// Set the volume in percents (0 = mute, 100 = 0dB)
        /// </summary>
        /// <param name="volume">volume to set [0:200]</param>
        /// <returns>true if volume was set, false otherwise</returns>
        public bool SetVolume(int volume) => Native.LibVLCAudioSetVolume(NativeReference, volume) == 0;

        /// <summary>
        /// Get current audio channel.
        /// </summary>
        public AudioOutputChannel Channel => Native.LibVLCAudioGetChannel(NativeReference);

        /// <summary>
        /// Set current audio channel.
        /// </summary>
        /// <param name="channel">the audio channel</param>
        /// <returns></returns>
        public bool SetChannel(AudioOutputChannel channel) => Native.LibVLCAudioSetChannel(NativeReference, channel) == 0;

        /// <summary>
        /// Equals override based on the native instance reference
        /// </summary>
        /// <param name="obj">the mediaplayer instance to compare this to</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return obj is MediaPlayer player &&
                   EqualityComparer<IntPtr>.Default.Equals(NativeReference, player.NativeReference);
        }

        /// <summary>
        /// Custom hascode implemenation for this MediaPlayer instance
        /// </summary>
        /// <returns>the hashcode for this MediaPlayer instance</returns>
        public override int GetHashCode()
        {
            return NativeReference.GetHashCode();
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
        /// Set/unset the video crop ratio.
        /// This function forces a crop ratio on any and all video tracks rendered by
        /// the media player. If the display aspect ratio of a video does not match the
        /// crop ratio, either the top and bottom, or the left and right of the video
        /// will be cut out to fit the crop ratio.
        /// For instance, a ratio of 1:1 will force the video to a square shape.
        /// To disable video crop, set a crop ratio with zero as denominator.
        /// A call to this function overrides any previous call to any of
        /// libvlc_video_set_crop_ratio(), libvlc_video_set_crop_border() and/or
        /// libvlc_video_set_crop_window().
        /// </summary>
        /// <param name="num">crop ratio numerator (ignored if denominator is 0)</param>
        /// <param name="den">crop ratio denominator (or 0 to unset the crop ratio)</param>
        public void CropRatio(uint num, uint den) => Native.LibVLCVideoSetCropRatio(NativeReference, num, den);

        /// <summary>
        /// Set the video crop window.
        /// This function selects a sub-rectangle of video to show. Any pixels outside
        /// the rectangle will not be shown.
        /// To unset the video crop window, use libvlc_video_set_crop_ratio() or
        /// libvlc_video_set_crop_border().
        /// A call to this function overrides any previous call to any of
        /// libvlc_video_set_crop_ratio(), libvlc_video_set_crop_border() and/or
        /// libvlc_video_set_crop_window().
        /// </summary>
        /// <param name="x">abscissa (i.e. leftmost sample column offset) of the crop window</param>
        /// <param name="y">ordinate (i.e. topmost sample row offset) of the crop window</param>
        /// <param name="width">sample width of the crop window (cannot be zero)</param>
        /// <param name="height">sample height of the crop window (cannot be zero)</param>
        public void CropWindow(uint x, uint y, uint width, uint height)
            => Native.LibVLCVideoSetCropWindow(NativeReference, x, y, width, height);

        /// <summary>
        /// Set the video crop borders.
        /// This function selects the size of video edges to be cropped out.
        /// To unset the video crop borders, set all borders to zero.
        /// A call to this function overrides any previous call to any of
        /// libvlc_video_set_crop_ratio(), libvlc_video_set_crop_border() and/or
        /// libvlc_video_set_crop_window().
        /// </summary>
        /// <param name="left">number of sample columns to crop on the left</param>
        /// <param name="right">number of sample columns to crop on the right</param>
        /// <param name="top">number of sample rows to crop on the top</param>
        /// <param name="bottom">number of sample rows to corp on the bottom</param>
        public void CropBorder(uint left, uint right, uint top, uint bottom)
            => Native.LibVLCVideoSetCropBorder(NativeReference, left, right, top, bottom);

        LibVLCVideoLockCb? _videoLockCb;
        LibVLCVideoUnlockCb? _videoUnlockCb;
        LibVLCVideoDisplayCb? _videoDisplayCb;


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
        public void SetVideoCallbacks(LibVLCVideoLockCb lockCb, LibVLCVideoUnlockCb? unlockCb,
            LibVLCVideoDisplayCb? displayCb)
        {
            _videoLockCb = lockCb ?? throw new ArgumentNullException(nameof(lockCb));
            _videoUnlockCb = unlockCb;
            _videoDisplayCb = displayCb;
            Native.LibVLCVideoSetCallbacks(NativeReference,
                                           VideoLockCallbackHandle,
                                           (unlockCb == null)? null : VideoUnlockCallbackHandle,
                                           (displayCb == null)? null : VideoDisplayCallbackHandle,
                                           GCHandle.ToIntPtr(_gcHandle));
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
            var chromaUtf8 = chroma.ToUtf8();

            MarshalUtils.PerformInteropAndFree(() =>
                Native.LibVLCVideoSetFormat(NativeReference, chromaUtf8, width, height, pitch),
                chromaUtf8);
        }

        LibVLCVideoFormatCb? _videoFormatCb;
        LibVLCVideoCleanupCb? _videoCleanupCb;
        IntPtr _videoUserData = IntPtr.Zero;

        /// <summary>
        /// Set decoded video chroma and dimensions.
        /// This only works in combination with libvlc_video_set_callbacks().
        /// </summary>
        /// <param name="formatCb">callback to select the video format (cannot be NULL)</param>
        /// <param name="cleanupCb">callback to release any allocated resources (or NULL)</param>
        public void SetVideoFormatCallbacks(LibVLCVideoFormatCb formatCb, LibVLCVideoCleanupCb? cleanupCb)
        {
            _videoFormatCb = formatCb ?? throw new ArgumentNullException(nameof(formatCb));
            _videoCleanupCb = cleanupCb;
            Native.LibVLCVideoSetFormatCallbacks(NativeReference, VideoFormatCallbackHandle,
                (cleanupCb == null)? null : VideoCleanupCallbackHandle);
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
            return Native.LibVLCVideoGetSize(NativeReference, num, ref px, ref py) == 0;
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
            return Native.LibVLCVideoGetCursor(NativeReference, num, ref px, ref py) == 0;
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
        /// Get/Set the subtitle text scale.
        /// The scale factor is expressed as a percentage of the default size, where 1.0 represents 100 percent.
        /// A value of 0.5 would result in text half the normal size, and a value of 2.0 would result in text twice the normal size.
        /// The minimum acceptable value for the scale factor is 0.1.
        /// The maximum is 5.0 (five times normal size).
        /// LibVLC 4.0.0 or later
        /// </summary>
        public float SpuTextScale
        {
            get => Native.LibVLCVideoGetSpuTextScale(NativeReference);
            set => Native.LibVLCVideoSetSpuTextScale(NativeReference, value);
        }

        /// <summary>
        /// Get/set current video aspect ratio.
        /// Set to null to reset to default
        /// Invalid aspect ratios are ignored.
        /// </summary>
        public string? AspectRatio
        {
            get => Native.LibVLCVideoGetAspectRatio(NativeReference).FromUtf8(libvlcFree: true);
            set
            {
                var aspectRatioUtf8 = value.ToUtf8();
                MarshalUtils.PerformInteropAndFree(() => Native.LibVLCVideoSetAspectRatio(NativeReference, aspectRatioUtf8), aspectRatioUtf8);
            }
        }

        /// <summary>
        /// Get the current subtitle delay.
        /// </summary>
        public long SpuDelay => Native.LibVLCVideoGetSpuDelay(NativeReference);

        /// <summary>
        /// Set the subtitle delay.
        /// This affects the timing of when the subtitle will be displayed.
        /// Positive values result in subtitles being displayed later, while negative values will result in subtitles being displayed earlier.
        /// The subtitle delay will be reset to zero each time the media changes.
        /// </summary>
        /// <param name="delay">time (in microseconds) the display of subtitles should be delayed</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool SetSpuDelay(long delay) => Native.LibVLCVideoSetSpuDelay(NativeReference, delay) == 0;

        /// <summary>
        /// Get the full description of available chapters.
        /// </summary>
        /// <param name="titleIndex">Index of the title to query for chapters (uses current title if set to -1)</param>
        /// <returns>Array of chapter descriptions.</returns>
        public ChapterDescription[] FullChapterDescriptions(int titleIndex = -1) => MarshalUtils.Retrieve(NativeReference,
            (IntPtr nativeRef, out IntPtr array) =>
            {
                var count = Native.LibVLCMediaPlayerGetFullChapterDescriptions(nativeRef, titleIndex, out array);
                // the number of chapters (-1 on error)
                return count < 0 ? 0 : (uint)count;
            },
            MarshalUtils.PtrToStructure<ChapterDescriptionStructure>,
            t => t.Build(),
            Native.LibVLCChapterDescriptionsRelease);

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
        /// Take a snapshot of the current video window.
        /// If width AND height is 0, original size is used. If width XOR
        /// height is 0, original aspect-ratio is preserved.
        /// <br/>
        /// While the native function is asynchronous, it returns immediately, 
        /// as it sends a command to the native library but does not wait for the state change.
        /// <br/>
        /// Use <see cref="TakeSnapshotAsync"/> if you want to wait asynchronously for the snapshot to be taken.
        /// </summary>
        /// <param name="num">number of video output (typically 0 for the first/only one)</param>
        /// <param name="filePath">the path where to save the screenshot to</param>
        /// <param name="width">the snapshot's width</param>
        /// <param name="height">the snapshot's height</param>
        /// <returns>true on success</returns>
        public bool TakeSnapshot(uint num, string? filePath, uint width, uint height)
        {
            var filePathUtf8 = filePath.ToUtf8();
            return MarshalUtils.PerformInteropAndFree(() =>
                Native.LibVLCVideoTakeSnapshot(NativeReference, num, filePathUtf8, width, height) == 0,
                filePathUtf8);
        }

        /// <summary>
        /// Take a snapshot of the current video window and waits asynchronously for the snapshot to be saved.
        /// If width AND height is 0, original size is used. If width XOR
        /// height is 0, original aspect-ratio is preserved.
        /// <br/>
        /// Snapshot has actually been taken when this function returns.
        /// </summary>
        /// <param name="num">number of video output (typically 0 for the first/only one)</param>
        /// <param name="filePath">the path where to save the screenshot to</param>
        /// <param name="width">the snapshot's width</param>
        /// <param name="height">the snapshot's height</param>
        /// <returns>Task result of the async operation</returns>
        public Task<bool> TakeSnapshotAsync(uint num, string? filePath, uint width, uint height)
        {
            var tcs = new TaskCompletionSource<bool>();

            void MediaPlayer_SnapshotTaken(object? sender, MediaPlayerSnapshotTakenEventArgs e) => tcs.SetResult(true);

            return MarshalUtils.InternalAsync(
                nativeCall: () => TakeSnapshot(num, filePath, width, height),
                sub: () => SnapshotTaken += MediaPlayer_SnapshotTaken,
                unsub: () => SnapshotTaken -= MediaPlayer_SnapshotTaken,
                tcs: tcs);
        }

        /// <summary>
        /// Enable or disable deinterlace filter
        /// </summary>
        /// <param name="deinterlace">deinterlace state -1: auto (default), 0: disabled, 1: enabled</param>
        /// <param name="deinterlaceType">type of deinterlace filter, empty string to disable</param>
        public void SetDeinterlace(int deinterlace, string deinterlaceType = "")
        {
            var deinterlaceTypeUtf8 = deinterlaceType.ToUtf8();

            MarshalUtils.PerformInteropAndFree(() =>
                Native.LibVLCVideoSetDeinterlace(NativeReference, deinterlace, deinterlaceTypeUtf8),
                deinterlaceTypeUtf8);
        }

        /// <summary>
        /// Get an integer marquee option value
        /// </summary>
        /// <param name="option">marq option to get</param>
        /// <returns></returns>
        public int MarqueeInt(VideoMarqueeOption option) => Native.LibVLCVideoGetMarqueeInt(NativeReference, option);

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
        /// <param name="marqueeValue">marq option value</param>
        public void SetMarqueeString(VideoMarqueeOption option, string? marqueeValue)
        {
            var marqueeValueUtf8 = marqueeValue.ToUtf8();
            MarshalUtils.PerformInteropAndFree(() =>
                Native.LibVLCVideoSetMarqueeString(NativeReference, option, marqueeValueUtf8),
                marqueeValueUtf8);
        }


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
        /// <param name="logoValue">logo option value</param>
        public void SetLogoString(VideoLogoOption option, string? logoValue)
        {
            var logoValueUtf8 = logoValue.ToUtf8();

            MarshalUtils.PerformInteropAndFree(() =>
                Native.LibVLCVideoSetLogoString(NativeReference, option, logoValueUtf8),
                logoValueUtf8);
        }

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

        /// <summary>
        /// Get adjust option float value
        /// </summary>
        /// <param name="option">The option for which to get the value</param>
        /// <returns>the float value for a given option</returns>
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
        public bool AddSlave(MediaSlaveType type, string uri, bool select)
        {
            var uriUtf8 = uri.ToUtf8();
            return MarshalUtils.PerformInteropAndFree(() =>
                Native.LibVLCMediaPlayerAddSlave(NativeReference, type, uriUtf8, select) == 0,
                uriUtf8);
        }

        /// <summary>
        /// Current 360 viewpoint of this mediaplayer.
        /// <para/>Update with <see cref="UpdateViewpoint"/>
        /// </summary>
        public VideoViewpoint Viewpoint { get; private set; }

        /// <summary>
        /// Update the video viewpoint information.
        /// The values are set asynchronously, it will be used by the next frame displayed.
        /// It is safe to call this function before the media player is started.
        /// LibVLC 3.0.0 and later
        /// </summary>
        /// <param name="yaw">view point yaw in degrees  ]-180;180]</param>
        /// <param name="pitch">view point pitch in degrees  ]-90;90]</param>
        /// <param name="roll">view point roll in degrees ]-180;180]</param>
        /// <param name="fov">field of view in degrees ]0;180[ (default 80.)</param>
        /// <param name="absolute">if true replace the old viewpoint with the new one. If false, increase/decrease it.</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool UpdateViewpoint(float yaw, float pitch, float roll, float fov, bool absolute = true)
        {
            var vpPtr = Native.LibVLCVideoNewViewpoint();
            if (vpPtr == IntPtr.Zero) return false;

            Viewpoint = new VideoViewpoint(yaw, pitch, roll, fov);
            Marshal.StructureToPtr(Viewpoint, vpPtr, false);

            var result = Native.LibVLCVideoUpdateViewpoint(NativeReference, vpPtr, absolute) == 0;
            MarshalUtils.LibVLCFree(ref vpPtr);

            return result;
        }

        /// <summary>
        /// Set a renderer to the media player.
        /// </summary>
        /// <param name="rendererItem">discovered renderer item or null to fallback on local rendering</param>
        /// <returns>true on success, false otherwise</returns>
        public bool SetRenderer(RendererItem? rendererItem) =>
            Native.LibVLCMediaPlayerSetRenderer(NativeReference, rendererItem?.NativeReference ?? IntPtr.Zero) == 0;

        /// <summary>Gets the media role.
        /// <para/> version LibVLC 3.0.0 and later.
        /// </summary>
        public MediaPlayerRole Role => Native.LibVLCMediaPlayerGetRole(NativeReference);

        /// <summary>Sets the media role.
        /// <para/> version LibVLC 3.0.0 and later.
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        public bool SetRole(MediaPlayerRole role) => Native.LibVLCMediaPlayerSetRole(NativeReference, role) == 0;

        /// <summary>Increments the native reference counter for this mediaplayer instance</summary>
        internal void Retain() => Native.LibVLCMediaPlayerRetain(NativeReference);

        /// <summary>
        /// Enable/disable hardware decoding in a crossplatform way.
        /// </summary>
        public bool EnableHardwareDecoding
        {
            get => Configuration.EnableHardwareDecoding;
            set => Configuration.EnableHardwareDecoding = value;
        }

        /// <summary>
        /// Caching value for local files, in milliseconds [0 .. 60000ms]
        /// </summary>
        public uint FileCaching
        {
            get => Configuration.FileCaching;
            set => Configuration.FileCaching = value;
        }

        /// <summary>
        /// Caching value for network resources, in milliseconds [0 .. 60000ms]
        /// </summary>
        public uint NetworkCaching
        {
            get => Configuration.NetworkCaching;
            set => Configuration.NetworkCaching = value;
        }

        /// <summary>
        /// Get the track list for one type
        /// LibVLC 4.0.0 and later.
        ///
        ///<br/> You need to call libvlc_media_parse_request() or play the media
        ///at least once before calling this function. Not doing this will result in
        ///an empty list.
        ///
        ///<br/> This track list is a snapshot of the current tracks when this function
        ///is called.If a track is updated after this call, the user will need to call
        ///this function again to get the updated track.
        ///
        ///
        ///The track list can be used to get track informations and to select specific
        ///tracks.
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MediaTrackList? Tracks(TrackType type)
        {
            var ptr = Native.LibVLCMediaPlayerGetTrackList(NativeReference, type);
            if (ptr == IntPtr.Zero)
                return null;
            return new MediaTrackList(ptr);
        }

        /// <summary>
        /// Get the selected track for one type
        /// LibVLC 4.0.0 and later.
        ///
        /// <br/>More than one tracks can be selected for one type. In that case,
        ///libvlc_media_player_get_tracklist() should be used.
        /// </summary>
        /// <param name="type">type of track</param>
        /// <returns>a valid track or NULL if there is no selected tracks for this type. Remember to dispose of it when done</returns>
        public MediaTrack? SelectedTrack(TrackType type)
        {
            var ptr = Native.LibVLCMediaPlayerGetSelectedTrack(NativeReference, type);
            if (ptr == IntPtr.Zero)
                return null;
            return new MediaTrack(ptr);
        }

        /// <summary>
        /// Get a track from a track id
        /// version LibVLC 4.0.0 and later.
        /// This function can be used to get the last updated informations of a track.
        /// </summary>
        /// <param name="id">valid string representing a track id</param>
        /// <returns>a valid track or NULL if there is currently no tracks identified by
        /// the string id, don't forget to dispose of it.</returns>
        public MediaTrack? TrackFromId(string id)
        {
            if(string.IsNullOrEmpty(id)) return null;

            var idPtr = id.ToUtf8();
            var ptr = MarshalUtils.PerformInteropAndFree(() => Native.LibVLCMediaPlayerGetTrackFromId(NativeReference, idPtr), idPtr);

            if(ptr == IntPtr.Zero) return null;
            return new MediaTrack(ptr);
        }

        /// <summary>
        /// Select a track. This will unselected the current track.
        /// LibVLC 4.0.0 and later.
        /// </summary>
        /// <param name="track">track to select</param>
        public void Select(MediaTrack track) => Native.LibVLCMediaPlayerSelectTrack(NativeReference, track.NativeReference);

        /// <summary>
        /// Unselect all tracks for a given type
        /// LibVLC 4.0.0 and later.
        /// </summary>
        /// <param name="trackType">type to unselect</param>
        public void Unselect(TrackType trackType) => Native.LibVLCMediaPlayerUnselectTrackType(NativeReference, trackType);

        /// <summary>
        /// Select multiple tracks. All tracks must be of the same TrackType, otherwise this method does nothing.
        /// LibVLC 4.0.0 and later.
        /// The internal track list can change between the calls of
        /// TrackList(TrackType type) and
        /// libvlc_media_player_set_tracks(). If a track selection change but the
        /// track is not present anymore, the player will just ignore it.
        /// selecting multiple audio tracks is currently not supported.
        /// </summary>
        /// <param name="tracks">tracks to select</param>
        public void Select(MediaTrack[] tracks)
        {
            if (tracks == null || tracks.Length == 0 || tracks.Any(t => t.TrackType != tracks[0].TrackType))
                return;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Select tracks by their string identifier
        /// version LibVLC 4.0.0 and later.
        /// This function can be used pre-select a list of tracks before starting the
        /// player. It has only effect for the current media. It can also be used when
        /// the player is already started.
        /// 'ids' can contain more than one track id, delimited with ','. "" or any
        /// invalid track id will cause the player to unselect all tracks of that
        /// category. NULL will disable the preference for newer tracks without
        /// unselecting any current tracks.
        /// Example:
        /// - (libvlc_track_video, "video/1,video/2") will select these 2 video tracks.
        /// If there is only one video track with the id "video/0", no tracks will be
        /// selected.
        /// - (libvlc_track_type_t, "${slave_url_md5sum}/spu/0) will select one spu
        /// added by an input slave with the corresponding url.
        ///
        /// <remarks>The string identifier of a track can be found via psz_id from \ref
        /// libvlc_media_track_t</remarks>
        ///
        /// <remarks>selecting multiple audio tracks is currently not supported.</remarks>
        /// </summary>
        /// <param name="type">track type to select</param>
        /// <param name="ids">'ids' can contain more than one track id, delimited with ','</param>
        public void Select(TrackType type, string ids) => Native.LibVLCMediaPlayerSelectTracksByIds(NativeReference, type, ids.ToUtf8());

        /// <summary>
        /// Get the program list
        /// version LibVLC 4.0.0 and later.
        ///
        /// note: This program list is a snapshot of the current programs when this
        /// function is called. If a program is updated after this call, the user will
        /// need to call this function again to get the updated program.
        ///
        /// The program list can be used to get program informations and to select
        /// specific programs.
        ///
        /// Return a valid ProgramList or NULL in case of error or empty list,
        /// </summary>
        public ProgramList? ProgramList
        {
            get
            {
                var programList = Native.LibVLCMediaPlayerGetProgramList(NativeReference);
                return programList == IntPtr.Zero ? null : new ProgramList(programList);
            }
        }

        /// <summary>
        /// Get a program from a program id
        ///
        /// version LibVLC 4.0.0 or later
        ///
        /// </summary>
        /// <param name="groupId">program id</param>
        /// <returns>a valid program or NULL if the program id is not found.</returns>
        public Program? Program(int groupId)
        {
            var program = Native.LibVLCMediaPlayerGetProgramFromId(NativeReference, groupId);
            return MarshalExtensions.BuildProgram(program, Native.LibVLCPlayerProgramDelete);
        }

        /// <summary>
        /// Get the selected program
        /// version LibVLC 4.0.0 or later
        ///
        /// return a valid program struct or NULL if no programs are selected.
        /// </summary>
        public Program? SelectedProgram
        {
            get
            {
                var program = Native.LibVLCMediaPlayerGetSelectedProgram(NativeReference);
                return MarshalExtensions.BuildProgram(program, Native.LibVLCPlayerProgramDelete);
            }
        }

        /// <summary>
        /// Select program with a given program id.
        ///
        /// note program ids are sent via the ProgramAdded event or
        /// can be fetch via ProgramList property
        /// </summary>
        /// <param name="programId">program id</param>
        public void SelectProgram(int programId) => Native.LibVLCMediaPlayerSelectProgramId(NativeReference, programId);


        /// <summary>
        /// Set callbacks and data to render decoded video to a custom texture
        /// warning: VLC will perform video rendering in its own thread and at its own rate,
        /// You need to provide your own synchronisation mechanism.
        /// <para/>
        /// note that <see cref="OutputSetup"/> and <see cref="OutputCleanup"/> may be called more than once per playback.
        /// <para/>
        /// version LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="engine">the GPU engine to use</param>
        /// <param name="outputSetup">callback called to initialize user data</param>
        /// <param name="outputCleanup">callback called to clean up user data</param>
        /// <param name="resize">callback to set the resize callback</param>
        /// <param name="updateOutput">callback to get the rendering format of the host (cannot be NULL)</param>
        /// <param name="swap">called after rendering a video frame (cannot be NULL)</param>
        /// <param name="makeCurrent">callback called to enter/leave the rendering context (cannot be NULL)</param>
        /// <param name="getProcAddress">opengl function loading callback (cannot be NULL when either <see cref="VideoEngine.OpenGL"/> 
        /// or <see cref="VideoEngine.OpenGLES2"/> is selected)</param>
        /// <param name="metadata">callback to provide frame metadata (D3D11 only)</param>
        /// <param name="selectPlane">callback to select different D3D11 rendering targets</param>
        /// <returns>true engine selected and callbacks set, or false if engine type unknown or that the callbacks are not set</returns>
        public bool SetOutputCallbacks(VideoEngine engine, OutputSetup? outputSetup, OutputCleanup? outputCleanup, OutputSetResize? resize,
            UpdateOutput updateOutput, Swap swap, MakeCurrent makeCurrent, GetProcAddress? getProcAddress, FrameMetadata? metadata,
            OutputSelectPlane? selectPlane)
        {
            _outputSetup = outputSetup;
            _outputCleanup = outputCleanup;
            _outputResize = resize;
            _updateOutput = updateOutput ?? throw new ArgumentNullException(nameof(updateOutput));
            _swap = swap ?? throw new ArgumentNullException(nameof(swap));
            _makeCurrent = makeCurrent ?? throw new ArgumentNullException(nameof(makeCurrent));
            _getProcAddress = getProcAddress == null && (engine == VideoEngine.OpenGL || engine == VideoEngine.OpenGLES2) 
                ? throw new ArgumentNullException(nameof(getProcAddress)) 
                : getProcAddress;
            _frameMetadata = metadata;
            _outputSelectPlane = selectPlane;

            return Native.LibVLCVideoSetOutputCallbacks(NativeReference, engine,
                _outputSetup == null ? null : OutputSetupHandle,
                _outputCleanup == null ? null : OutputCleanupHandle,
                _outputResize == null ? null : OutputSetResizeHandle,
                UpdateOutputHandle,
                SwapHandle,
                MakeCurrentHandle,
                _getProcAddress == null ? null : GetProcAddressHandle,
                _frameMetadata == null ? null : FrameMetadataHandle,
                _outputSelectPlane == null ? null : OutputSelectPlaneHandle,
                GCHandle.ToIntPtr(_gcHandle));
        }

        OutputSetup? _outputSetup;
        OutputCleanup? _outputCleanup;
        OutputSetResize? _outputResize;
        UpdateOutput? _updateOutput;
        Swap? _swap;
        MakeCurrent? _makeCurrent;
        GetProcAddress? _getProcAddress;
        FrameMetadata? _frameMetadata;
        OutputSelectPlane? _outputSelectPlane;

        readonly MediaConfiguration Configuration = new MediaConfiguration();

#if UNITY
        /// <summary>
        /// Retrieve a video frame from the Unity plugin as a texture.
        /// </summary>
        /// <param name="updated">True if the video frame has been updated</param>
        /// <param name="width">Width size request in pixel, will be applied to the next frame if changed</param>
        /// <param name="height">Height size request in pixel, will be applied to the next frame if changed</param>
        /// <returns>A decoded texture</returns>
        public IntPtr GetTexture(uint width, uint height, out bool updated)
        {
            var texture = Native.GetTexture(NativeReference, width, height, out bool isUpdated);
            updated = isUpdated;
            return texture;
        }
#endif

        #region Callbacks

        static unsafe readonly OutputSetup OutputSetupHandle = OutputSetupCallback;
        static readonly OutputCleanup OutputCleanupHandle = OutputCleanupCallback;
        static readonly OutputSetResize OutputSetResizeHandle = OutputSetResizeCallback;
        static unsafe readonly UpdateOutput UpdateOutputHandle = UpdateOutputCallback;
        static readonly Swap SwapHandle = SwapCallback;
        static readonly MakeCurrent MakeCurrentHandle = MakeCurrentCallback;
        static readonly GetProcAddress GetProcAddressHandle = GetProcAddressCallback;
        static unsafe readonly FrameMetadata FrameMetadataHandle = FrameMetadataCallback;
        static unsafe readonly OutputSelectPlane OutputSelectPlaneHandle = OutputSelectPlaneCallback;

        [MonoPInvokeCallback(typeof(OutputSetup))]
        private static unsafe bool OutputSetupCallback(ref IntPtr opaque, SetupDeviceConfig* config, ref SetupDeviceInfo setup)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._outputSetup != null)
            {
                return mediaPlayer._outputSetup(ref mediaPlayer._videoUserData, config, ref setup);
            }
            return false;
        }

        [MonoPInvokeCallback(typeof(OutputCleanup))]
        private static void OutputCleanupCallback(IntPtr opaque)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._outputCleanup != null)
            {
                mediaPlayer._outputCleanup(mediaPlayer._videoUserData);
            }
        }

        [MonoPInvokeCallback(typeof(OutputSetResize))]
        private static void OutputSetResizeCallback(IntPtr opaque, ReportSizeChange report_size_change, IntPtr report_opaque)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._outputResize != null)
            {
                mediaPlayer._outputResize(mediaPlayer._videoUserData, report_size_change, report_opaque);
            }
        }

        [MonoPInvokeCallback(typeof(UpdateOutput))]
        private unsafe static bool UpdateOutputCallback(IntPtr opaque, RenderConfig* config, ref OutputConfig output)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._updateOutput != null)
            {
                return mediaPlayer._updateOutput(mediaPlayer._videoUserData, config, ref output);
            }
            return false;
        }

        [MonoPInvokeCallback(typeof(Swap))]
        private static void SwapCallback(IntPtr opaque)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._swap != null)
            {
                mediaPlayer._swap(mediaPlayer._videoUserData);
            }
        }

        [MonoPInvokeCallback(typeof(MakeCurrent))]
        private static bool MakeCurrentCallback(IntPtr opaque, bool enter)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._makeCurrent != null)
            {
                return mediaPlayer._makeCurrent(mediaPlayer._videoUserData, enter);
            }
            return false;
        }

        [MonoPInvokeCallback(typeof(GetProcAddress))]
        private static IntPtr GetProcAddressCallback(IntPtr opaque, IntPtr functionName)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._getProcAddress != null)
            {
                return mediaPlayer._getProcAddress(mediaPlayer._videoUserData, functionName);
            }
            return IntPtr.Zero;
        }

        [MonoPInvokeCallback(typeof(FrameMetadata))]
        private unsafe static void FrameMetadataCallback(IntPtr opaque, FrameMetadataType type, void* metadata)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._frameMetadata != null)
            {
                mediaPlayer._frameMetadata(mediaPlayer._videoUserData, type, metadata);
            }
        }

        [MonoPInvokeCallback(typeof(OutputSelectPlane))]
        private unsafe static bool OutputSelectPlaneCallback(IntPtr opaque, UIntPtr plane, void* output)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._outputSelectPlane != null)
            {
                return mediaPlayer._outputSelectPlane(mediaPlayer._videoUserData, plane, output);
            }
            return false;
        }

        static readonly LibVLCVideoLockCb VideoLockCallbackHandle = VideoLockCallback;
        static readonly LibVLCVideoUnlockCb VideoUnlockCallbackHandle = VideoUnlockCallback;
        static readonly LibVLCVideoDisplayCb VideoDisplayCallbackHandle = VideoDisplayCallback;
        static readonly LibVLCVideoFormatCb VideoFormatCallbackHandle = VideoFormatCallback;
        static readonly LibVLCVideoCleanupCb VideoCleanupCallbackHandle = VideoCleanupCallback;

        [MonoPInvokeCallback(typeof(LibVLCVideoLockCb))]
        private static IntPtr VideoLockCallback(IntPtr opaque, IntPtr planes)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._videoLockCb != null)
            {
                return mediaPlayer._videoLockCb(mediaPlayer._videoUserData, planes);
            }
            return IntPtr.Zero;
        }

        [MonoPInvokeCallback(typeof(LibVLCVideoUnlockCb))]
        private static void VideoUnlockCallback(IntPtr opaque, IntPtr picture, IntPtr planes)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._videoUnlockCb != null)
            {
                mediaPlayer._videoUnlockCb(mediaPlayer._videoUserData, picture, planes);
            }
        }

        [MonoPInvokeCallback(typeof(LibVLCVideoDisplayCb))]
        private static void VideoDisplayCallback(IntPtr opaque, IntPtr picture)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._videoDisplayCb != null)
            {
                mediaPlayer._videoDisplayCb(mediaPlayer._videoUserData, picture);
            }
        }

        [MonoPInvokeCallback(typeof(LibVLCVideoFormatCb))]
        private static uint VideoFormatCallback(ref IntPtr opaque, IntPtr chroma, ref uint width, ref uint height, ref uint pitches, ref uint lines)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._videoFormatCb != null)
            {
                return mediaPlayer._videoFormatCb(ref mediaPlayer._videoUserData, chroma, ref width, ref height, ref pitches, ref lines);
            }

            return 0;
        }

        [MonoPInvokeCallback(typeof(LibVLCVideoCleanupCb))]
        private static void VideoCleanupCallback(ref IntPtr opaque)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._videoCleanupCb != null)
            {
                mediaPlayer._videoCleanupCb(ref mediaPlayer._videoUserData);
            }
        }

        static readonly LibVLCAudioPlayCb AudioPlayCallbackHandle = AudioPlayCallback;
        static readonly LibVLCAudioPauseCb AudioPauseCallbackHandle = AudioPauseCallback;
        static readonly LibVLCAudioResumeCb AudioResumeCallbackHandle = AudioResumeCallback;
        static readonly LibVLCAudioFlushCb AudioFlushCallbackHandle = AudioFlushCallback;
        static readonly LibVLCAudioDrainCb AudioDrainCallbackHandle = AudioDrainCallback;
        static readonly LibVLCVolumeCb AudioVolumeCallbackHandle = AudioVolumeCallback;
        static readonly LibVLCAudioSetupCb AudioSetupCallbackHandle = AudioSetupCallback;
        static readonly LibVLCAudioCleanupCb AudioCleanupCallbackHandle = AudioCleanupCallback;

        [MonoPInvokeCallback(typeof(LibVLCAudioPlayCb))]
        private static void AudioPlayCallback(IntPtr data, IntPtr samples, uint count, long pts)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(data);
            if (mediaPlayer?._audioPlayCb != null)
            {
                mediaPlayer._audioPlayCb(mediaPlayer._audioUserData, samples, count, pts);
            }
        }

        [MonoPInvokeCallback(typeof(LibVLCAudioPauseCb))]
        private static void AudioPauseCallback(IntPtr data, long pts)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(data);
            if (mediaPlayer?._audioPauseCb != null)
            {
                mediaPlayer._audioPauseCb(mediaPlayer._audioUserData, pts);
            }
        }

        [MonoPInvokeCallback(typeof(LibVLCAudioResumeCb))]
        private static void AudioResumeCallback(IntPtr data, long pts)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(data);
            if (mediaPlayer?._audioResumeCb != null)
            {
                mediaPlayer._audioResumeCb(mediaPlayer._audioUserData, pts);
            }
        }

        [MonoPInvokeCallback(typeof(LibVLCAudioFlushCb))]
        private static void AudioFlushCallback(IntPtr data, long pts)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(data);
            if (mediaPlayer?._audioFlushCb != null)
            {
                mediaPlayer._audioFlushCb(mediaPlayer._audioUserData, pts);
            }
        }

        [MonoPInvokeCallback(typeof(LibVLCAudioDrainCb))]
        private static void AudioDrainCallback(IntPtr data)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(data);
            if (mediaPlayer?._audioDrainCb != null)
            {
                mediaPlayer._audioDrainCb(mediaPlayer._audioUserData);
            }
        }

        [MonoPInvokeCallback(typeof(LibVLCVolumeCb))]
        private static void AudioVolumeCallback(IntPtr data, float volume, bool mute)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(data);
            if (mediaPlayer?._audioVolumeCb != null)
            {
                mediaPlayer._audioVolumeCb(mediaPlayer._audioUserData, volume, mute);
            }
        }

        [MonoPInvokeCallback(typeof(LibVLCAudioSetupCb))]
        private static int AudioSetupCallback(ref IntPtr opaque, ref IntPtr format, ref uint rate, ref uint channels)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._setupCb != null)
            {
                return mediaPlayer._setupCb(ref mediaPlayer._audioUserData, ref format, ref rate, ref channels);
            }

            return -1;
        }

        [MonoPInvokeCallback(typeof(LibVLCAudioCleanupCb))]
        private static void AudioCleanupCallback(IntPtr opaque)
        {
            var mediaPlayer = MarshalUtils.GetInstance<MediaPlayer>(opaque);
            if (mediaPlayer?._cleanupCb != null)
            {
                mediaPlayer._cleanupCb(mediaPlayer._audioUserData);
            }
        }

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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCVideoUnlockCb(IntPtr opaque, IntPtr picture, IntPtr planes);

        /// <summary>Callback prototype to display a picture.</summary>
        /// <param name="opaque">private pointer as passed to libvlc_video_set_callbacks() [IN]</param>
        /// <param name="picture">private pointer returned from the</param>
        /// <remarks>
        /// <para>When the video frame needs to be shown, as determined by the media playback</para>
        /// <para>clock, the display callback is invoked.</para>
        /// <para>callback [IN]</para>
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint LibVLCVideoFormatCb(ref IntPtr opaque, IntPtr chroma, ref uint width,
            ref uint height, ref uint pitches, ref uint lines);

        /// <summary>Callback prototype to configure picture buffers format.</summary>
        /// <param name="opaque">
        /// <para>private pointer as passed to libvlc_video_set_callbacks()</para>
        /// <para>(and possibly modified by</para>
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LibVLCAudioSetupCb(ref IntPtr opaque, ref IntPtr format, ref uint rate, ref uint channels);

        /// <summary>Callback prototype for audio playback cleanup.</summary>
        /// <param name="opaque">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <remarks>This is called when the media player no longer needs an audio output.</remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioCleanupCb(IntPtr opaque);

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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioPlayCb(IntPtr data, IntPtr samples, uint count, long pts);

        /// <summary>Callback prototype for audio pause.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <param name="pts">time stamp of the pause request (should be elapsed already)</param>
        /// <remarks>
        /// <para>LibVLC invokes this callback to pause audio playback.</para>
        /// <para>The pause callback is never called if the audio is already paused.</para>
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioPauseCb(IntPtr data, long pts);

        /// <summary>Callback prototype for audio resumption.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <param name="pts">time stamp of the resumption request (should be elapsed already)</param>
        /// <remarks>
        /// <para>LibVLC invokes this callback to resume audio playback after it was</para>
        /// <para>previously paused.</para>
        /// <para>The resume callback is never called if the audio is not paused.</para>
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioResumeCb(IntPtr data, long pts);

        /// <summary>Callback prototype for audio buffer flush.
        /// <para>LibVLC invokes this callback if it needs to discard all pending buffers and</para>
        /// <para>stop playback as soon as possible. This typically occurs when the media is stopped.</para>
        /// </summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <param name="pts">current presentation timestamp</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioFlushCb(IntPtr data, long pts);

        /// <summary>Callback prototype for audio buffer drain.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <remarks>
        /// <para>LibVLC may invoke this callback when the decoded audio track is ending.</para>
        /// <para>There will be no further decoded samples for the track, but playback should</para>
        /// <para>nevertheless continue until all already pending buffers are rendered.</para>
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCAudioDrainCb(IntPtr data);

        /// <summary>Callback prototype for audio volume change.</summary>
        /// <param name="data">data pointer as passed to libvlc_audio_set_callbacks() [IN]</param>
        /// <param name="volume">software volume (1. = nominal, 0. = mute)</param>
        /// <param name="mute">muted flag</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibVLCVolumeCb(IntPtr data, float volume, [MarshalAs(UnmanagedType.I1)] bool mute);

        /// <summary>
        /// Callback prototype called to initialize user data. Setup the rendering environment.
        /// <para/>
        /// Note: LibVLC 4.0.0 or later
        /// <para>
        /// For libvlc_video_engine_d3d9 the output must be a IDirect3D9*.
        /// A reference to this object is held until the LIBVLC_VIDEO_DEVICE_CLEANUP is called.
        /// the device must be created with D3DPRESENT_PARAMETERS.hDeviceWindow set to 0.
        /// </para>
        /// <para>
        /// For libvlc_video_engine_d3d11 the output must be a ID3D11DeviceContext*.
        /// A reference to this object is held until the \ref LIBVLC_VIDEO_DEVICE_CLEANUP is called.
        /// The ID3D11Device used to create ID3D11DeviceContext must have multithreading enabled.
        /// </para>
        /// <para>
        /// If the ID3D11DeviceContext is used outside of the callbacks called by libvlc, the host
        /// MUST use a mutex to protect the access to the ID3D11DeviceContext of libvlc. This mutex
        /// value is set on <see cref="SetupDeviceInfoD3D11.ContextMutex"/>. If the ID3D11DeviceContext is not used outside of
        /// the callbacks, the mutex <see cref="SetupDeviceInfoD3D11.ContextMutex"/> may be NULL.
        /// </para>
        /// </summary>
        /// <param name="opaque">private pointer passed to the @a libvlc_video_set_output_callbacks()
        /// on input.The callback can change this value on output to be
        /// passed to all the other callbacks set on @a libvlc_video_set_output_callbacks().
        /// [IN/OUT]</param>
        /// <param name="config">requested configuration of the video device [IN]</param>
        /// <param name="setup">libvlc_video_setup_device_info_t* to fill [OUT]</param>
        /// <returns>true on success</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public unsafe delegate bool OutputSetup(ref IntPtr opaque, SetupDeviceConfig* config, ref SetupDeviceInfo setup);

        /// <summary>
        /// Callback prototype called to release user data.
        /// <para/>
        /// Note: version LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="opaque">private pointer set on the opaque parameter of <see cref="OutputSetup"/></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OutputCleanup(IntPtr opaque);

        /// <summary>
        /// Set the callback to call when the host app resizes the rendering area.
        /// <para/>
        /// This allows text rendering and aspect ratio to be handled properly when the host
        /// rendering size changes.
        /// <para/>
        /// <para/>
        /// version LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="opaque">opaque pointer</param>
        /// <param name="report_size_change">callback which must be called when the host size changes.
        /// <para/>The callback is valid until another call to Resize/>
        /// is done. This may be called from any thread.</param>
        /// <param name="report_opaque">private pointer to pass to the <see cref="ReportSizeChange"/> callback</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OutputSetResize(IntPtr opaque, ReportSizeChange report_size_change, IntPtr report_opaque);

        /// <summary>
        /// Callback which must be called when the host size changes. Set with <see cref="OutputSetResize"/>
        /// <para/> LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="report_opaque">opaque pointer</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReportSizeChange(IntPtr report_opaque, uint width, uint height);

        /// <summary>
        /// Update the rendering output setup. <para/>
        /// Note: the configuration device for Direct3D9 is the IDirect3DDevice9 that VLC
        /// uses to render. The host must set a Render target and call Present()
        /// when it needs the drawing from VLC to be done.This object is not valid
        /// anymore after Cleanup is called. <para/>
        /// Tone mapping, range and color conversion will be done depending on the values set in the output structure.
        /// <para/> LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="opaque">private pointer passed to the <see cref="SetOutputCallbacks"/>[IN]</param>
        /// <param name="config">configuration of the video that will be rendered [IN]</param>
        /// <param name="output">output configuration describing with how the rendering is setup [OUT]</param>
        /// <returns>true on success</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public unsafe delegate bool UpdateOutput(IntPtr opaque, RenderConfig* config, ref OutputConfig output);

        /// <summary>
        /// Callback prototype called after performing drawing calls.
        /// This callback is called outside of libvlc_video_makeCurrent_cb current/not-current calls.
        /// <para/> LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="opaque">private pointer passed to the 
        /// <see cref="SetOutputCallbacks(VideoEngine, OutputSetup?, OutputCleanup?, OutputSetResize?, 
        /// UpdateOutput, Swap,  MakeCurrent, GetProcAddress, FrameMetadata?, OutputSelectPlane?)"/></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Swap(IntPtr opaque);

        /// <summary>
        /// Callback prototype to set up the OpenGL context for rendering. Tell the host the rendering is about to start/has finished.
        /// <para/> LibVLC 4.0.0 or later <para/>
        /// On Direct3D11 the following may change on the provided ID3D11DeviceContext*
        /// between \ref enter being true and \ref enter being false: <para/> 
        /// - IASetPrimitiveTopology() <para/> 
        /// - IASetInputLayout() <para/> 
        /// - IASetVertexBuffers() <para/> 
        /// - IASetIndexBuffer() <para/> 
        /// - VSSetConstantBuffers() <para/> 
        /// - VSSetShader() <para/> 
        /// - PSSetSamplers() <para/> 
        /// - PSSetConstantBuffers() <para/> 
        /// - PSSetShaderResources() <para/>
        /// - PSSetShader() <para/>
        /// - RSSetViewports() <para/>
        /// - DrawIndexed() <para/>
        /// <para/> LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="opaque">private pointer passed to the 
        /// <see cref="SetOutputCallbacks(VideoEngine, OutputSetup?, OutputCleanup?, OutputSetResize?, 
        /// UpdateOutput, Swap, MakeCurrent, GetProcAddress, FrameMetadata?, OutputSelectPlane?)"/></param>
        /// <param name="enter">true to set the context as current, false to unset it</param>
        /// <returns>true on success</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool MakeCurrent(IntPtr opaque, bool enter);

        /// <summary>
        /// Callback prototype to load opengl functions
        /// <para/> LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="opaque">private pointer passed to the 
        /// <see cref="SetOutputCallbacks(VideoEngine, OutputSetup?, OutputCleanup?, OutputSetResize?, 
        /// UpdateOutput, Swap, MakeCurrent, GetProcAddress, FrameMetadata?, OutputSelectPlane?)"/></param>
        /// <param name="functionName">name of the opengl function to load</param>
        /// <returns>a pointer to the named OpenGL function, null otherwise</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetProcAddress(IntPtr opaque, IntPtr functionName);

        /// <summary>
        /// Callback prototype for frame metadata information
        /// <para/> LibVLC 4.0.0 or later
        /// </summary>
        /// <param name="opaque">private pointer passed to the 
        /// <see cref="SetOutputCallbacks(VideoEngine, OutputSetup?, OutputCleanup?, OutputSetResize?, 
        /// UpdateOutput, Swap, MakeCurrent, GetProcAddress, FrameMetadata?, OutputSelectPlane?)"/></param>
        /// <param name="type">the type of frame metadata</param>
        /// <param name="metadata">the actual data, typed as one of <see cref="FrameMetadataType"/></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void FrameMetadata(IntPtr opaque, FrameMetadataType type, void* metadata);

        /// <summary>
        /// Tell the host the rendering for the given plane is about to start
        /// <para/> LibVLC 4.0.0 or later
        /// <para/> note: This is only used with <see cref="VideoEngine.D3D11"/> <para/>
        /// The output parameter receives the ID3D11RenderTargetView* to use for rendering
        /// the plane.
        /// If this callback is not used (set to NULL in <see cref="SetOutputCallbacks"/>)
        /// OMSetRenderTargets has to be set during <see cref="MakeCurrent"/>
        /// <para/>
        /// The number of planes depend on the DXGI_FORMAT returned during the
        /// <see cref="UpdateOutput"/> call.It's usually one plane except for
        /// semi-planar formats like DXGI_FORMAT_NV12 or DXGI_FORMAT_P010.
        /// entering call.
        /// <para/>
        /// This callback is called between <see cref="MakeCurrent"/> current/not-current calls.
        /// </summary>
        /// <param name="opaque">private pointer passed to the 
        /// <see cref="SetOutputCallbacks(VideoEngine, OutputSetup?, OutputCleanup?, OutputSetResize?, 
        /// UpdateOutput, Swap, MakeCurrent, GetProcAddress, FrameMetadata?, OutputSelectPlane?)"/></param>
        /// <param name="plane">number of the rendering plane to select</param>
        /// <param name="output">handle of the rendering output for the given plane</param>
        /// <returns>true on success</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate bool OutputSelectPlane(IntPtr opaque, UIntPtr plane, void* output);
        #endregion

        /// <summary>
        /// Get the Event Manager from which the media player send event.
        /// </summary>
        MediaPlayerEventManager EventManager
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


        #region events

        /// <summary>
        /// The media of this mediaplayer changed
        /// </summary>
        public event EventHandler<MediaPlayerMediaChangedEventArgs> MediaChanged
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerMediaChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerMediaChanged, value);
        }

        /// <summary>
        /// Nothing special to report
        /// </summary>
        public event EventHandler<EventArgs> NothingSpecial
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerNothingSpecial, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerNothingSpecial, value);
        }

        /// <summary>
        /// The mediaplayer is opening a media
        /// </summary>
        public event EventHandler<EventArgs> Opening
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerOpening, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerOpening, value);
        }

        /// <summary>
        /// The mediaplayer is buffering
        /// </summary>
        public event EventHandler<MediaPlayerBufferingEventArgs> Buffering
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerBuffering, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerBuffering, value);
        }

        /// <summary>
        /// The mediaplayer started playing a media
        /// </summary>
        public event EventHandler<EventArgs> Playing
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerPlaying, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerPlaying, value);
        }

        /// <summary>
        /// The mediaplayer paused playback
        /// </summary>
        public event EventHandler<EventArgs> Paused
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerPaused, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerPaused, value);
        }

        /// <summary>
        /// The mediaplayer stopped playback
        /// </summary>
        public event EventHandler<EventArgs> Stopped
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerStopped, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerStopped, value);
        }

        /// <summary>
        /// The mediaplayer went forward in the playback
        /// </summary>
        public event EventHandler<EventArgs> Forward
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerForward, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerForward, value);
        }

        /// <summary>
        /// The mediaplayer went backward in the playback
        /// </summary>
        public event EventHandler<EventArgs> Backward
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerBackward, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerBackward, value);
        }

        /// <summary>
        /// The mediaplayer is reaching the end of the playback
        /// </summary>
        public event EventHandler<EventArgs> Stopping
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerStopping, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerStopping, value);
        }

        /// <summary>
        /// The mediaplayer encountered an error during playback
        /// </summary>
        public event EventHandler<EventArgs> EncounteredError
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerEncounteredError, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerEncounteredError, value);
        }

        /// <summary>
        /// The mediaplayer's playback time changed
        /// </summary>
        public event EventHandler<MediaPlayerTimeChangedEventArgs> TimeChanged
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerTimeChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerTimeChanged, value);
        }

        /// <summary>
        /// The mediaplayer's position changed
        /// </summary>
        public event EventHandler<MediaPlayerPositionChangedEventArgs> PositionChanged
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerPositionChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerPositionChanged, value);
        }

        /// <summary>
        /// The mediaplayer's seek capability changed
        /// </summary>
        public event EventHandler<MediaPlayerSeekableChangedEventArgs> SeekableChanged
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerSeekableChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerSeekableChanged, value);
        }

        /// <summary>
        /// The mediaplayer's pause capability changed
        /// </summary>
        public event EventHandler<MediaPlayerPausableChangedEventArgs> PausableChanged
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerPausableChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerPausableChanged, value);
        }

        /// <summary>
        /// The mediaplayer changed the chapter of a media
        /// </summary>
        public event EventHandler<MediaPlayerChapterChangedEventArgs> ChapterChanged
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerChapterChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerChapterChanged, value);
        }

        /// <summary>
        /// The mediaplayer took a snapshot
        /// </summary>
        public event EventHandler<MediaPlayerSnapshotTakenEventArgs> SnapshotTaken
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerSnapshotTaken, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerSnapshotTaken, value);
        }

        /// <summary>
        /// The length of a playback changed
        /// </summary>
        public event EventHandler<MediaPlayerLengthChangedEventArgs> LengthChanged
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerLengthChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerLengthChanged, value);
        }

        /// <summary>
        /// The Video Output count of the MediaPlayer changed
        /// </summary>
        public event EventHandler<MediaPlayerVoutEventArgs> Vout
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerVout, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerVout, value);
        }

        /// <summary>
        /// The mediaplayer has a new Elementary Stream (ES)
        /// </summary>
        public event EventHandler<MediaPlayerESAddedEventArgs> ESAdded
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerESAdded, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerESAdded, value);
        }

        /// <summary>
        /// The mediaplayer has one less Elementary Stream (ES)
        /// </summary>
        public event EventHandler<MediaPlayerESDeletedEventArgs> ESDeleted
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerESDeleted, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerESDeleted, value);
        }

        /// <summary>
        /// An Elementary Stream (ES) was selected
        /// </summary>
        public event EventHandler<MediaPlayerESSelectedEventArgs> ESSelected
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerESSelected, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerESSelected, value);
        }

        /// <summary>
        /// The mediaplayer's audio device changed
        /// </summary>
        public event EventHandler<MediaPlayerAudioDeviceEventArgs> AudioDevice
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerAudioDevice, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerAudioDevice, value);
        }

        /// <summary>
        /// The mediaplayer is corked
        /// </summary>
        public event EventHandler<EventArgs> Corked
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerCorked, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerCorked, value);
        }

        /// <summary>
        /// The mediaplayer is uncorked
        /// </summary>
        public event EventHandler<EventArgs> Uncorked
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerUncorked, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerUncorked, value);
        }

        /// <summary>
        /// The mediaplayer is muted
        /// </summary>
        public event EventHandler<EventArgs> Muted
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerMuted, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerMuted, value);
        }

        /// <summary>
        /// The mediaplayer is unmuted
        /// </summary>
        public event EventHandler<EventArgs> Unmuted
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerUnmuted, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerUnmuted, value);
        }

        /// <summary>
        /// The mediaplayer's volume changed
        /// </summary>
        public event EventHandler<MediaPlayerVolumeChangedEventArgs> VolumeChanged
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerAudioVolume, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerAudioVolume, value);
        }

        /// <summary>
        /// The new program detected by the mediaplayer
        /// </summary>
        public event EventHandler<MediaPlayerProgramAddedEventArgs> ProgramAdded
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerProgramAdded, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerProgramAdded, value);
        }

        /// <summary>
        /// The deleted program detected by the mediaplayer
        /// </summary>
        public event EventHandler<MediaPlayerProgramDeletedEventArgs> ProgramDeleted
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerProgramDeleted, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerProgramDeleted, value);
        }

        /// <summary>
        /// The updated program detected by the mediaplayer
        /// </summary>
        public event EventHandler<MediaPlayerProgramUpdatedEventArgs> ProgramUpdated
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerProgramUpdated, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerProgramUpdated, value);
        }

        /// <summary>
        /// The selected/unselected program detected by the mediaplayer
        /// </summary>
        public event EventHandler<MediaPlayerProgramSelectedEventArgs> ProgramSelected
        {
            add => EventManager.AttachEvent(EventType.MediaPlayerProgramSelected, value);
            remove => EventManager.DetachEvent(EventType.MediaPlayerProgramSelected, value);
        }
        #endregion

        /// <summary>
        /// Dispose override
        /// Effectively stops playback and disposes a media if any
        /// </summary>
        /// <param name="disposing">release any unmanaged resources</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (_gcHandle.IsAllocated)
                    _gcHandle.Free();
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>Description for titles</summary>
    public enum Title
    {
        /// <summary>
        /// Menu title
        /// </summary>
        Menu = 1,

        /// <summary>
        /// Interactive title
        /// </summary>
        Interactive = 2
    }

    /// <summary>Marq options definition</summary>
    public enum VideoMarqueeOption
    {
        /// <summary>
        /// Enable marquee
        /// </summary>
        Enable = 0,

        /// <summary>
        /// Text marquee
        /// </summary>
        Text = 1,

        /// <summary>
        /// Color marquee
        /// </summary>
        Color = 2,

        /// <summary>
        /// Opacity marquee
        /// </summary>
        Opacity = 3,

        /// <summary>
        /// Position marquee
        /// </summary>
        Position = 4,

        /// <summary>
        /// Refresh marquee
        /// </summary>
        Refresh = 5,

        /// <summary>
        /// Size marquee
        /// </summary>
        Size = 6,

        /// <summary>
        /// Timeout marquee
        /// </summary>
        Timeout = 7,

        /// <summary>
        /// X marquee
        /// </summary>
        X = 8,

        /// <summary>
        /// Y marquee
        /// </summary>
        Y = 9
    }

    /// <summary>Navigation mode</summary>
    public enum NavigationMode
    {
        /// <summary>
        /// Activate
        /// </summary>
        Activate = 0,

        /// <summary>
        /// Navigation up
        /// </summary>
        Up = 1,

        /// <summary>
        /// Navigation down
        /// </summary>
        Down = 2,

        /// <summary>
        /// Navigation left
        /// </summary>
        Left = 3,

        /// <summary>
        /// Navigation right
        /// </summary>
        Right = 4,

        /// <summary>
        /// Navigation popup
        /// </summary>
        Popup = 5
    }

    /// <summary>Enumeration of values used to set position (e.g. of video title).</summary>
    public enum Position
    {
        /// <summary>
        /// Disable
        /// </summary>
        Disable = -1,

        /// <summary>
        /// Center video title
        /// </summary>
        Center = 0,

        /// <summary>
        /// Left video title
        /// </summary>
        Left = 1,

        /// <summary>
        /// Right video title
        /// </summary>
        Right = 2,

        /// <summary>
        /// Top video title
        /// </summary>
        Top = 3,

        /// <summary>
        /// TopLeft video title
        /// </summary>
        TopLeft = 4,

        /// <summary>
        /// TopRight video title
        /// </summary>
        TopRight = 5,

        /// <summary>
        /// Bottom video title
        /// </summary>
        Bottom = 6,

        /// <summary>
        /// BottomLeft video title
        /// </summary>
        BottomLeft = 7,

        /// <summary>
        /// BottomRight video title
        /// </summary>
        BottomRight = 8
    }

    /// <summary>
    /// <para>Enumeration of teletext keys than can be passed via</para>
    /// <para>libvlc_video_set_teletext()</para>
    /// </summary>
    public enum TeletextKey
    {
        /// <summary>
        /// Red
        /// </summary>
        Red = 7471104,

        /// <summary>
        /// Green
        /// </summary>
        Green = 6750208,

        /// <summary>
        /// Yellow
        /// </summary>
        Yellow = 7929856,

        /// <summary>
        /// Blue
        /// </summary>
        Blue = 6422528,

        /// <summary>
        /// Index
        /// </summary>
        Index = 6881280
    }

    /// <summary>
    /// option values for libvlc_video_{get,set}_logo_{int,string}
    /// </summary>
    public enum VideoLogoOption
    {
        /// <summary>
        /// Enable
        /// </summary>
        Enable = 0,

        /// <summary>
        /// string argument, &quot;file,d,t;file,d,t;...&quot;
        /// </summary>
        File = 1,

        /// <summary>
        /// X
        /// </summary>
        X = 2,

        /// <summary>
        /// Y
        /// </summary>
        Y = 3,

        /// <summary>
        /// Delay
        /// </summary>
        Delay = 4,

        /// <summary>
        /// Repeat
        /// </summary>
        Repeat = 5,

        /// <summary>
        /// Opacity
        /// </summary>
        Opacity = 6,

        /// <summary>
        /// Position
        /// </summary>
        Position = 7
    }

    /// <summary>
    /// option values for libvlc_video_{get,set}_adjust_{int,float,bool}
    /// </summary>
    public enum VideoAdjustOption
    {
        /// <summary>
        /// Enable
        /// </summary>
        Enable = 0,

        /// <summary>
        /// Contrast
        /// </summary>
        Contrast = 1,

        /// <summary>
        /// Brightness
        /// </summary>
        Brightness = 2,

        /// <summary>
        /// Hue
        /// </summary>
        Hue = 3,

        /// <summary>
        /// Saturation
        /// </summary>
        Saturation = 4,

        /// <summary>
        /// Gamma
        /// </summary>
        Gamma = 5
    }

    /// <summary>
    /// Audio channels
    /// </summary>
    public enum AudioOutputChannel
    {
        /// <summary>
        /// Error
        /// </summary>
        Error = -1,

        /// <summary>
        /// Stereo mode
        /// </summary>
        Stereo = 1,

        /// <summary>
        /// RStereo mode
        /// </summary>
        RStereo = 2,

        /// <summary>
        /// Left mode
        /// </summary>
        Left = 3,

        /// <summary>
        /// Right mode
        /// </summary>
        Right = 4,

        /// <summary>
        /// Dolbys mode
        /// </summary>
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
        /// <summary>Audio editing/production</summary>
        Production = 7,
        /// <summary>Accessibility</summary>
        Accessibility = 8,
        /// <summary>Testing</summary>
        Test = 9
    }
}
