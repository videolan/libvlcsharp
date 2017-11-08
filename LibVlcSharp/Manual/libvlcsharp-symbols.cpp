#include <vlc.h>

extern "C" { void VideoLAN.LibVLC_symbols1(void* instance) { new (instance) libvlc_module_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols2(void* instance, const libvlc_module_description_t& _0) { new (instance) libvlc_module_description_t(_0); } }
libvlc_module_description_t& (libvlc_module_description_t::*VideoLAN.LibVLC_symbols3)(const libvlc_module_description_t&) = &libvlc_module_description_t::operator=;
libvlc_module_description_t& (libvlc_module_description_t::*VideoLAN.LibVLC_symbols4)(libvlc_module_description_t&&) = &libvlc_module_description_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols5(libvlc_module_description_t* instance) { instance->~libvlc_module_description_t(); } }
::int64_t (*VideoLAN.LibVLC_symbols6)(::int64_t) = &libvlc_delay;
extern "C" { void VideoLAN.LibVLC_symbols7(void* instance) { new (instance) libvlc_rd_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols8(void* instance, const libvlc_rd_description_t& _0) { new (instance) libvlc_rd_description_t(_0); } }
libvlc_rd_description_t& (libvlc_rd_description_t::*VideoLAN.LibVLC_symbols9)(const libvlc_rd_description_t&) = &libvlc_rd_description_t::operator=;
libvlc_rd_description_t& (libvlc_rd_description_t::*VideoLAN.LibVLC_symbols10)(libvlc_rd_description_t&&) = &libvlc_rd_description_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols11(libvlc_rd_description_t* instance) { instance->~libvlc_rd_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols12(void* instance) { new (instance) libvlc_media_stats_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols13(void* instance, const libvlc_media_stats_t& _0) { new (instance) libvlc_media_stats_t(_0); } }
libvlc_media_stats_t& (libvlc_media_stats_t::*VideoLAN.LibVLC_symbols14)(const libvlc_media_stats_t&) = &libvlc_media_stats_t::operator=;
libvlc_media_stats_t& (libvlc_media_stats_t::*VideoLAN.LibVLC_symbols15)(libvlc_media_stats_t&&) = &libvlc_media_stats_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols16(libvlc_media_stats_t* instance) { instance->~libvlc_media_stats_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols17(void* instance) { new (instance) libvlc_media_track_info_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols18(void* instance, const libvlc_media_track_info_t& _0) { new (instance) libvlc_media_track_info_t(_0); } }
libvlc_media_track_info_t& (libvlc_media_track_info_t::*VideoLAN.LibVLC_symbols19)(const libvlc_media_track_info_t&) = &libvlc_media_track_info_t::operator=;
libvlc_media_track_info_t& (libvlc_media_track_info_t::*VideoLAN.LibVLC_symbols20)(libvlc_media_track_info_t&&) = &libvlc_media_track_info_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols21(libvlc_media_track_info_t* instance) { instance->~libvlc_media_track_info_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols22(void* instance) { new (instance) libvlc_audio_track_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols23(void* instance, const libvlc_audio_track_t& _0) { new (instance) libvlc_audio_track_t(_0); } }
libvlc_audio_track_t& (libvlc_audio_track_t::*VideoLAN.LibVLC_symbols24)(const libvlc_audio_track_t&) = &libvlc_audio_track_t::operator=;
libvlc_audio_track_t& (libvlc_audio_track_t::*VideoLAN.LibVLC_symbols25)(libvlc_audio_track_t&&) = &libvlc_audio_track_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols26(libvlc_audio_track_t* instance) { instance->~libvlc_audio_track_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols27(void* instance) { new (instance) libvlc_video_track_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols28(void* instance, const libvlc_video_track_t& _0) { new (instance) libvlc_video_track_t(_0); } }
libvlc_video_track_t& (libvlc_video_track_t::*VideoLAN.LibVLC_symbols29)(const libvlc_video_track_t&) = &libvlc_video_track_t::operator=;
libvlc_video_track_t& (libvlc_video_track_t::*VideoLAN.LibVLC_symbols30)(libvlc_video_track_t&&) = &libvlc_video_track_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols31(libvlc_video_track_t* instance) { instance->~libvlc_video_track_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols32(void* instance) { new (instance) libvlc_subtitle_track_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols33(void* instance, const libvlc_subtitle_track_t& _0) { new (instance) libvlc_subtitle_track_t(_0); } }
libvlc_subtitle_track_t& (libvlc_subtitle_track_t::*VideoLAN.LibVLC_symbols34)(const libvlc_subtitle_track_t&) = &libvlc_subtitle_track_t::operator=;
libvlc_subtitle_track_t& (libvlc_subtitle_track_t::*VideoLAN.LibVLC_symbols35)(libvlc_subtitle_track_t&&) = &libvlc_subtitle_track_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols36(libvlc_subtitle_track_t* instance) { instance->~libvlc_subtitle_track_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols37(void* instance) { new (instance) libvlc_media_track_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols38(void* instance, const libvlc_media_track_t& _0) { new (instance) libvlc_media_track_t(_0); } }
libvlc_media_track_t& (libvlc_media_track_t::*VideoLAN.LibVLC_symbols39)(const libvlc_media_track_t&) = &libvlc_media_track_t::operator=;
libvlc_media_track_t& (libvlc_media_track_t::*VideoLAN.LibVLC_symbols40)(libvlc_media_track_t&&) = &libvlc_media_track_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols41(libvlc_media_track_t* instance) { instance->~libvlc_media_track_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols42(void* instance) { new (instance) libvlc_media_slave_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols43(void* instance, const libvlc_media_slave_t& _0) { new (instance) libvlc_media_slave_t(_0); } }
libvlc_media_slave_t& (libvlc_media_slave_t::*VideoLAN.LibVLC_symbols44)(const libvlc_media_slave_t&) = &libvlc_media_slave_t::operator=;
libvlc_media_slave_t& (libvlc_media_slave_t::*VideoLAN.LibVLC_symbols45)(libvlc_media_slave_t&&) = &libvlc_media_slave_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols46(libvlc_media_slave_t* instance) { instance->~libvlc_media_slave_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols47(void* instance) { new (instance) libvlc_track_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols48(void* instance, const libvlc_track_description_t& _0) { new (instance) libvlc_track_description_t(_0); } }
libvlc_track_description_t& (libvlc_track_description_t::*VideoLAN.LibVLC_symbols49)(const libvlc_track_description_t&) = &libvlc_track_description_t::operator=;
libvlc_track_description_t& (libvlc_track_description_t::*VideoLAN.LibVLC_symbols50)(libvlc_track_description_t&&) = &libvlc_track_description_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols51(libvlc_track_description_t* instance) { instance->~libvlc_track_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols52(void* instance) { new (instance) libvlc_title_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols53(void* instance, const libvlc_title_description_t& _0) { new (instance) libvlc_title_description_t(_0); } }
libvlc_title_description_t& (libvlc_title_description_t::*VideoLAN.LibVLC_symbols54)(const libvlc_title_description_t&) = &libvlc_title_description_t::operator=;
libvlc_title_description_t& (libvlc_title_description_t::*VideoLAN.LibVLC_symbols55)(libvlc_title_description_t&&) = &libvlc_title_description_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols56(libvlc_title_description_t* instance) { instance->~libvlc_title_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols57(void* instance) { new (instance) libvlc_chapter_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols58(void* instance, const libvlc_chapter_description_t& _0) { new (instance) libvlc_chapter_description_t(_0); } }
libvlc_chapter_description_t& (libvlc_chapter_description_t::*VideoLAN.LibVLC_symbols59)(const libvlc_chapter_description_t&) = &libvlc_chapter_description_t::operator=;
libvlc_chapter_description_t& (libvlc_chapter_description_t::*VideoLAN.LibVLC_symbols60)(libvlc_chapter_description_t&&) = &libvlc_chapter_description_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols61(libvlc_chapter_description_t* instance) { instance->~libvlc_chapter_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols62(void* instance) { new (instance) libvlc_audio_output_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols63(void* instance, const libvlc_audio_output_t& _0) { new (instance) libvlc_audio_output_t(_0); } }
libvlc_audio_output_t& (libvlc_audio_output_t::*VideoLAN.LibVLC_symbols64)(const libvlc_audio_output_t&) = &libvlc_audio_output_t::operator=;
libvlc_audio_output_t& (libvlc_audio_output_t::*VideoLAN.LibVLC_symbols65)(libvlc_audio_output_t&&) = &libvlc_audio_output_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols66(libvlc_audio_output_t* instance) { instance->~libvlc_audio_output_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols67(void* instance) { new (instance) libvlc_audio_output_device_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols68(void* instance, const libvlc_audio_output_device_t& _0) { new (instance) libvlc_audio_output_device_t(_0); } }
libvlc_audio_output_device_t& (libvlc_audio_output_device_t::*VideoLAN.LibVLC_symbols69)(const libvlc_audio_output_device_t&) = &libvlc_audio_output_device_t::operator=;
libvlc_audio_output_device_t& (libvlc_audio_output_device_t::*VideoLAN.LibVLC_symbols70)(libvlc_audio_output_device_t&&) = &libvlc_audio_output_device_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols71(libvlc_audio_output_device_t* instance) { instance->~libvlc_audio_output_device_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols72(void* instance) { new (instance) libvlc_video_viewpoint_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols73(void* instance, const libvlc_video_viewpoint_t& _0) { new (instance) libvlc_video_viewpoint_t(_0); } }
libvlc_video_viewpoint_t& (libvlc_video_viewpoint_t::*VideoLAN.LibVLC_symbols74)(const libvlc_video_viewpoint_t&) = &libvlc_video_viewpoint_t::operator=;
libvlc_video_viewpoint_t& (libvlc_video_viewpoint_t::*VideoLAN.LibVLC_symbols75)(libvlc_video_viewpoint_t&&) = &libvlc_video_viewpoint_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols76(libvlc_video_viewpoint_t* instance) { instance->~libvlc_video_viewpoint_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols77(void* instance) { new (instance) libvlc_media_discoverer_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols78(void* instance, const libvlc_media_discoverer_description_t& _0) { new (instance) libvlc_media_discoverer_description_t(_0); } }
libvlc_media_discoverer_description_t& (libvlc_media_discoverer_description_t::*VideoLAN.LibVLC_symbols79)(const libvlc_media_discoverer_description_t&) = &libvlc_media_discoverer_description_t::operator=;
libvlc_media_discoverer_description_t& (libvlc_media_discoverer_description_t::*VideoLAN.LibVLC_symbols80)(libvlc_media_discoverer_description_t&&) = &libvlc_media_discoverer_description_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols81(libvlc_media_discoverer_description_t* instance) { instance->~libvlc_media_discoverer_description_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols82(void* instance) { new (instance) libvlc_event_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols83(void* instance, const libvlc_event_t& _0) { new (instance) libvlc_event_t(_0); } }
libvlc_event_t& (libvlc_event_t::*VideoLAN.LibVLC_symbols84)(const libvlc_event_t&) = &libvlc_event_t::operator=;
libvlc_event_t& (libvlc_event_t::*VideoLAN.LibVLC_symbols85)(libvlc_event_t&&) = &libvlc_event_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols86(libvlc_event_t* instance) { instance->~libvlc_event_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols87(void* instance) { new (instance) libvlc_dialog_cbs(); } }
extern "C" { void VideoLAN.LibVLC_symbols88(void* instance, const libvlc_dialog_cbs& _0) { new (instance) libvlc_dialog_cbs(_0); } }
libvlc_dialog_cbs& (libvlc_dialog_cbs::*VideoLAN.LibVLC_symbols89)(const libvlc_dialog_cbs&) = &libvlc_dialog_cbs::operator=;
libvlc_dialog_cbs& (libvlc_dialog_cbs::*VideoLAN.LibVLC_symbols90)(libvlc_dialog_cbs&&) = &libvlc_dialog_cbs::operator=;
extern "C" { void VideoLAN.LibVLC_symbols91(libvlc_dialog_cbs* instance) { instance->~libvlc_dialog_cbs(); } }
extern "C" { void VideoLAN.LibVLC_symbols92(void* instance) { new (instance) libvlc_log_message_t(); } }
extern "C" { void VideoLAN.LibVLC_symbols93(void* instance, const libvlc_log_message_t& _0) { new (instance) libvlc_log_message_t(_0); } }
libvlc_log_message_t& (libvlc_log_message_t::*VideoLAN.LibVLC_symbols94)(const libvlc_log_message_t&) = &libvlc_log_message_t::operator=;
libvlc_log_message_t& (libvlc_log_message_t::*VideoLAN.LibVLC_symbols95)(libvlc_log_message_t&&) = &libvlc_log_message_t::operator=;
extern "C" { void VideoLAN.LibVLC_symbols96(libvlc_log_message_t* instance) { instance->~libvlc_log_message_t(); } }
