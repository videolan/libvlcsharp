/*****************************************************************************
 * MediaPlayer.hpp: MediaPlayer API
 *****************************************************************************
 * Copyright © 2015 libvlcpp authors & VideoLAN
 *
 * Authors: Alexey Sokolov <alexey+vlc@asokolov.org>
 *          Hugo Beauzée-Luyssen <hugo@beauzee.fr>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston MA 02110-1301, USA.
 *****************************************************************************/

#ifndef LIBVLC_CXX_MEDIAPLAYER_H
#define LIBVLC_CXX_MEDIAPLAYER_H

#include <array>
#include <string>
#include <vector>
#include <memory>

#include "common.hpp"

namespace VLC
{

class AudioOutputDeviceDescription;
class Equalizer;
class Instance;
class Media;
class MediaPlayerEventManager;
class TrackDescription;

///
/// \brief The MediaPlayer class exposes libvlc_media_player_t functionnalities
///
class MediaPlayer : private CallbackOwner<13>, public Internal<libvlc_media_player_t>
{
private:
    enum class CallbackIdx : unsigned int
    {
        AudioPlay,
        AudioPause,
        AudioResume,
        AudioFlush,
        AudioDrain,
        AudioVolume,
        AudioSetup,
        AudioCleanup,

        VideoLock,
        VideoUnlock,
        VideoDisplay,
        VideoFormat,
        VideoCleanup,
    };
public:
    /**
     * Check if 2 MediaPlayer objects contain the same libvlc_media_player_t.
     * \param another another MediaPlayer
     * \return true if they contain the same libvlc_media_player_t
     */
    bool operator==(const MediaPlayer& another) const
    {
        return m_obj == another.m_obj;
    }

    /**
     * Create an empty Media Player object
     *
     * \param p_libvlc_instance  the libvlc instance in which the Media
     * Player should be created.
     */
    MediaPlayer( Instance& instance )
        : Internal{ libvlc_media_player_new( instance ), libvlc_media_player_release }
    {
    }

    /**
     * Create a Media Player object from a Media
     *
     * \param p_md  the media. Afterwards the p_md can be safely destroyed.
     */
    MediaPlayer( Media& md )
        : Internal{ libvlc_media_player_new_from_media(
                        getInternalPtr<libvlc_media_t>( md ) ),
                    libvlc_media_player_release }
    {
    }

/**
     * Create an empty VLC MediaPlayer instance.
     *
     * Calling any method on such an instance is undefined.
    */
    MediaPlayer() = default;

    /**
     * Set the media that will be used by the media_player. If any, previous
     * md will be released.
     *
     * \param p_md  the Media. Afterwards the p_md can be safely destroyed.
     */
    void setMedia(Media& md)
    {
        libvlc_media_player_set_media( *this, getInternalPtr<libvlc_media_t>( md ) );
    }

    /**
     * Get the media used by the media_player.
     *
     * \return the media associated with p_mi, or NULL if no media is
     * associated
     */
    MediaPtr media()
    {
        auto media = libvlc_media_player_get_media(*this);
        if ( media == nullptr )
            return nullptr;
        return std::make_shared<Media>( media, true );
    }

    /**
     * Get the Event Manager from which the media player send event.
     *
     * \return the event manager associated with p_mi
     */
    MediaPlayerEventManager& eventManager()
    {
        if ( m_eventManager == nullptr )
        {
            libvlc_event_manager_t* obj = libvlc_media_player_event_manager( *this );
            m_eventManager = std::make_shared<MediaPlayerEventManager>( obj );
        }
        return *m_eventManager;
    }

    /**
     * \return true if the media player is playing, 0 otherwise
     */
    bool isPlaying()
    {
        return libvlc_media_player_is_playing(*this) != 0;
    }

    /**
     * @brief play Start playback
     *
     * If playback was already started, this method has no effect
     */
    bool play()
    {
        return libvlc_media_player_play(*this) == 0;
    }

    /**
     * Pause or resume (no effect if there is no media)
     *
     * \param do_pause  play/resume if true, pause if false
     *
     * \version LibVLC 1.1.1 or later
     */
    void setPause(bool pause)
    {
        libvlc_media_player_set_pause(*this, pause);
    }

    /**
     * @brief pause Toggle pause (no effect if there is no media)
     */
    void pause()
    {
        libvlc_media_player_pause(*this);
    }

    /**
     * @brief stop Stop the playback (no effect if there is no media)
     *
     * \warning This is synchronous, and will block until all VLC threads have
     *          been joined.
     *          Calling this from a VLC callback is a bound to cause a deadlock.
     */
    void stop()
    {
        libvlc_media_player_stop(*this);
    }

    /**
     * Set the NSView handler where the media player should render its video
     * output.
     *
     * Use the vout called "macosx".
     *
     * The drawable is an NSObject that follow the
     * VLCOpenGLVideoViewEmbedding protocol: VLCOpenGLVideoViewEmbedding <NSObject>
     *
     * Or it can be an NSView object.
     *
     * If you want to use it along with Qt4 see the QMacCocoaViewContainer.
     * Then the following code should work:  { NSView *video = [[NSView
     * alloc] init]; QMacCocoaViewContainer *container = new
     * QMacCocoaViewContainer(video, parent);
     * libvlc_media_player_set_nsobject(mp, video); [video release]; }
     *
     * You can find a live example in VLCVideoView in VLCKit.framework.
     *
     * \param drawable  the drawable that is either an NSView or an object
     * following the VLCOpenGLVideoViewEmbedding protocol.
     */
    void setNsobject(void* drawable)
    {
        libvlc_media_player_set_nsobject(*this, drawable);
    }

    /**
     * Get the NSView handler previously set with MediaPlayer::setNsobject()
     * .
     *
     * \return the NSView handler or 0 if none where set
     */
    void* nsobject()
    {
        return libvlc_media_player_get_nsobject(*this);
    }

    /**
     * Set an X Window System drawable where the media player should render
     * its video output. If LibVLC was built without X11 output support, then
     * this has no effects.
     *
     * The specified identifier must correspond to an existing Input/Output
     * class X11 window. Pixmaps are supported. The caller shall ensure that
     * the X11 server is the same as the one the VLC instance has been
     * configured with. This function must be called before video playback is
     * started; otherwise it will only take effect after playback stop and
     * restart.
     *
     * \param drawable  the ID of the X window
     */
    void setXwindow(uint32_t drawable)
    {
        libvlc_media_player_set_xwindow(*this, drawable);
    }

    /**
     * Get the X Window System window identifier previously set with
     * MediaPlayer::setXwindow() . Note that this will return the identifier
     * even if VLC is not currently using it (for instance if it is playing
     * an audio-only input).
     *
     * \return an X window ID, or 0 if none where set.
     */
    uint32_t xwindow()
    {
        return libvlc_media_player_get_xwindow(*this);
    }

    /**
     * Set a Win32/Win64 API window handle (HWND) where the media player
     * should render its video output. If LibVLC was built without
     * Win32/Win64 API output support, then this has no effects.
     *
     * \param drawable  windows handle of the drawable
     */
    void setHwnd(void * drawable)
    {
        libvlc_media_player_set_hwnd(*this, drawable);
    }

    /**
     * Get the Windows API window handle (HWND) previously set with
     * MediaPlayer::setHwnd() . The handle will be returned even if LibVLC is
     * not currently outputting any video to it.
     *
     * \return a window handle or nullptr if there are none.
     */
    void* hwnd()
    {
        return libvlc_media_player_get_hwnd(*this);
    }

    /**
     * Get the current movie length (in ms).
     *
     * \return the movie length (in ms), or -1 if there is no media.
     */
    libvlc_time_t length()
    {
        return libvlc_media_player_get_length(*this);
    }

    /**
     * Get the current movie time (in ms).
     *
     * \return the movie time (in ms), or -1 if there is no media.
     */
    libvlc_time_t time()
    {
        return  libvlc_media_player_get_time(*this);
    }

    /**
     * Set the movie time (in ms). This has no effect if no media is being
     * played. Not all formats and protocols support this.
     *
     * \param i_time  the movie time (in ms).
     */
    void setTime(libvlc_time_t i_time)
    {
        libvlc_media_player_set_time(*this, i_time);
    }

    /**
     * Get movie position as percentage between 0.0 and 1.0.
     *
     * \return movie position, or -1. in case of error
     */
    float position()
    {
        return libvlc_media_player_get_position(*this);
    }

    /**
     * Set movie position as percentage between 0.0 and 1.0. This has no
     * effect if playback is not enabled. This might not work depending on
     * the underlying input format and protocol.
     *
     * \param f_pos  the position
     */
    void setPosition(float f_pos)
    {
        libvlc_media_player_set_position(*this, f_pos);
    }

    /**
     * Set movie chapter (if applicable).
     *
     * \param i_chapter  chapter number to play
     */
    void setChapter(int i_chapter)
    {
        libvlc_media_player_set_chapter(*this, i_chapter);
    }

    /**
     * Get movie chapter.
     *
     * \return chapter number currently playing, or -1 if there is no media.
     */
    int chapter()
    {
        return libvlc_media_player_get_chapter(*this);
    }

    /**
     * Get movie chapter count
     *
     * \return number of chapters in movie, or -1.
     */
    int chapterCount()
    {
        return libvlc_media_player_get_chapter_count(*this);
    }

    /**
     * Is the player able to play
     *
     * \return boolean
     */
    bool willPlay()
    {
        return libvlc_media_player_will_play(*this) != 0;
    }

    /**
     * Get title chapter count
     *
     * \param i_title  title
     *
     * \return number of chapters in title, or -1
     */
    int chapterCountForTitle(int i_title)
    {
        return libvlc_media_player_get_chapter_count_for_title(*this, i_title);
    }

    /**
     * Set movie title
     *
     * \param i_title  title number to play
     */
    void setTitle(int i_title)
    {
        libvlc_media_player_set_title(*this, i_title);
    }

    /**
     * Get movie title
     *
     * \return title number currently playing, or -1
     */
    int title()
    {
        return libvlc_media_player_get_title(*this);
    }

    /**
     * Get movie title count
     *
     * \return title number count, or -1
     */
    int titleCount()
    {
        return libvlc_media_player_get_title_count(*this);
    }

    /**
     * Set previous chapter (if applicable)
     */
    void previousChapter()
    {
        libvlc_media_player_previous_chapter(*this);
    }

    /**
     * Set next chapter (if applicable)
     */
    void nextChapter()
    {
        libvlc_media_player_next_chapter(*this);
    }

    /**
     * Get the requested movie play rate.
     *
     * \warning Depending on the underlying media, the requested rate may be
     * different from the real playback rate.
     *
     * \return movie play rate
     */
    float rate()
    {
        return libvlc_media_player_get_rate(*this);
    }

    /**
     * Set movie play rate
     *
     * \param rate  movie play rate to set
     *
     * \return -1 if an error was detected, 0 otherwise (but even then, it
     * might not actually work depending on the underlying media protocol)
     */
    int setRate(float rate)
    {
        return libvlc_media_player_set_rate(*this, rate);
    }

    /**
     * Get current movie state
     *
     * \return the current state of the media player (playing, paused, ...)
     *
     * \see libvlc_state_t
     */
    libvlc_state_t state()
    {
        return libvlc_media_player_get_state(*this);
    }

#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Get movie fps rate
     *
     * \return frames per second (fps) for this playing movie, or 0 if
     * unspecified
     */
    float fps()
    {
        return libvlc_media_player_get_fps(*this);
    }
#endif

    /**
     * Get the amount of video outputs this media player has?
     *
     * \return the number of video outputs
     */
    uint32_t hasVout()
    {
        return libvlc_media_player_has_vout(*this);
    }

    /**
     * Is this media player seekable?
     *
     * \return true if the media player can seek
     */
    bool isSeekable()
    {
        return libvlc_media_player_is_seekable(*this) != 0;
    }

    /**
     * Can this media player be paused?
     *
     * \return true if the media player can pause
     */
    bool canPause()
    {
        return libvlc_media_player_can_pause(*this) != 0;
    }

    /**
     * Check if the current program is scrambled
     *
     * \return true if the current program is scrambled
     *
     * \version LibVLC 2.2.0 or later
     */
    bool programScrambled()
    {
        return libvlc_media_player_program_scrambled(*this) != 0;
    }

    /**
     * Display the next frame (if supported)
     */
    void nextFrame()
    {
        libvlc_media_player_next_frame(*this);
    }

    /**
     * Navigate through DVD Menu
     *
     * \param navigate  the Navigation mode
     *
     * \version libVLC 2.0.0 or later
     */
    void navigate(unsigned navigate)
    {
        libvlc_media_player_navigate(*this, navigate);
    }

    /**
     * Set if, and how, the video title will be shown when media is played.
     *
     * \param position  position at which to display the title, or
     * libvlc_position_disable to prevent the title from being displayed
     *
     * \param timeout  title display timeout in milliseconds (ignored if
     * libvlc_position_disable)
     *
     * \version libVLC 2.1.0 or later
     */
    void setVideoTitleDisplay(libvlc_position_t position, unsigned int timeout)
    {
        libvlc_media_player_set_video_title_display(*this, position, timeout);
    }

    /**
     * Toggle fullscreen status on non-embedded video outputs.
     *
     * \warning The same limitations applies to this function as to
     * MediaPlayer::setFullscreen() .
     */
    void toggleFullscreen()
    {
        libvlc_toggle_fullscreen(*this);
    }

    /**
     * Enable or disable fullscreen.
     *
     * \warning With most window managers, only a top-level windows can be in
     * full-screen mode. Hence, this function will not operate properly if
     * MediaPlayer::setXwindow() was used to embed the video in a non-top-
     * level window. In that case, the embedding window must be reparented to
     * the root window fullscreen mode is enabled. You will want to reparent
     * it back to its normal parent when disabling fullscreen.
     *
     * \param b_fullscreen  boolean for fullscreen status
     */
    void setFullscreen(bool fullscreen)
    {
        libvlc_set_fullscreen( *this, fullscreen );
    }

    /**
     * Get current fullscreen status.
     *
     * \return the fullscreen status (boolean)
     */
    bool fullscreen()
    {
        return libvlc_get_fullscreen(*this) != 0;
    }

#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Toggle teletext transparent status on video output.
     */
    void toggleTeletext()
    {
        libvlc_toggle_teletext(*this);
    }
#endif

    /**
     * Apply new equalizer settings to a media player.
     *
     * The equalizer is first created by invoking
     * libvlc_audio_equalizer_new() or
     * libvlc_audio_equalizer_new_from_preset() .
     *
     * It is possible to apply new equalizer settings to a media player
     * whether the media player is currently playing media or not.
     *
     * Invoking this method will immediately apply the new equalizer settings
     * to the audio output of the currently playing media if there is any.
     *
     * If there is no currently playing media, the new equalizer settings
     * will be applied later if and when new media is played.
     *
     * Equalizer settings will automatically be applied to subsequently
     * played media.
     *
     * To disable the equalizer for a media player invoke this method passing
     * NULL for the p_equalizer parameter.
     *
     * The media player does not keep a reference to the supplied equalizer
     * so it is safe for an application to release the equalizer reference
     * any time after this method returns.
     *
     * \param equalizer  The equalizer to be used by this media player
     *
     * \return true on success, false otherwise
     *
     * \version LibVLC 2.2.0 or later
     */
    bool setEqualizer(Equalizer& equalizer)
    {
        return libvlc_media_player_set_equalizer( *this, equalizer ) == 0;
    }

    ///
    /// \brief unsetEqualizer disable equalizer for this media player
    ///
    /// \return true on success, false otherwise.
    ///
    bool unsetEqualizer()
    {
        return libvlc_media_player_set_equalizer( *this, nullptr ) == 0;
    }

    /**
     * Set callbacks and private data for decoded audio. Use
     * MediaPlayer::setFormat() or MediaPlayer::setFormatCallbacks() to configure the
     * decoded audio format.
     *
     * \param play  callback to play audio samples (must not be nullptr)
     *              Required prototype is: void(const void *samples, unsigned count, int64_t pts)
     *
     * \param pause  callback to pause playback (or nullptr to ignore)
     *               Required prototype is void(int64_t pts);
     *
     * \param resume callback to resume playback (or nullptr to ignore)
     *               Required prototype is void(int64_t pts);
     *
     * \param flush  callback to flush audio buffers (or nullptr to ignore)
     *               Required prototype is void(int64_t pts);
     *
     * \param drain  callback to drain audio buffers (or nullptr to ignore)
     * *             Required prototype is void();
     *
     * \version LibVLC 2.0.0 or later
     */
    template <typename PlayCb, typename PauseCb, typename ResumeCb, typename FlushCb, typename DrainCb>
    void setAudioCallbacks(PlayCb&& play, PauseCb&& pause, ResumeCb&& resume, FlushCb&& flush, DrainCb&& drain)
    {
        static_assert(signature_match<PlayCb, void(const void*, unsigned int, int64_t)>::value, "Mismatched play callback prototype");
        static_assert(signature_match_or_nullptr<PauseCb, void(int64_t)>::value, "Mismatched pause callback prototype");
        static_assert(signature_match_or_nullptr<ResumeCb, void(int64_t)>::value, "Mismatched resume callback prototype");
        static_assert(signature_match_or_nullptr<FlushCb, void(int64_t pts)>::value, "Mismatched flush callback prototype");
        static_assert(signature_match_or_nullptr<DrainCb, void()>::value, "Mismatched drain callback prototype");

        libvlc_audio_set_callbacks( *this,
            CallbackWrapper<(unsigned int)CallbackIdx::AudioPlay,   libvlc_audio_play_cb>::wrap(   *m_callbacks, std::forward<PlayCb>( play ) ),
            CallbackWrapper<(unsigned int)CallbackIdx::AudioPause,  libvlc_audio_pause_cb>::wrap(  *m_callbacks, std::forward<PauseCb>( pause ) ),
            CallbackWrapper<(unsigned int)CallbackIdx::AudioResume, libvlc_audio_resume_cb>::wrap( *m_callbacks, std::forward<ResumeCb>( resume ) ),
            CallbackWrapper<(unsigned int)CallbackIdx::AudioFlush,  libvlc_audio_flush_cb>::wrap(  *m_callbacks, std::forward<FlushCb>( flush ) ),
            CallbackWrapper<(unsigned int)CallbackIdx::AudioDrain,  libvlc_audio_drain_cb>::wrap(  *m_callbacks, std::forward<DrainCb>( drain ) ),
            // We will receive the pointer as a void*, we need to offset the value *now*, otherwise we'd get
            // a shifted value, resulting in an invalid callback array.
            m_callbacks.get() );
    }

    /**
     * Set callbacks and private data for decoded audio. Use
     * MediaPlayer::setFormat() or MediaPlayer::setFormatCallbacks() to configure the
     * decoded audio format.
     *
     * \param set_volume  callback to apply audio volume, or nullptr to apply
     *                    volume in software
     *                    Expected prototype is void(float volume, bool mute)
     *
     * \version LibVLC 2.0.0 or later
     */
    template <typename VolumeCb>
    void setVolumeCallback(VolumeCb&& func)
    {
        static_assert(signature_match_or_nullptr<VolumeCb, void(float, bool)>::value, "Mismatched set volume callback");
        libvlc_audio_set_volume_callback(*this,
            CallbackWrapper<(unsigned int)CallbackIdx::AudioVolume, libvlc_audio_set_volume_cb>::wrap( this, std::forward<VolumeCb>( func ) ) );
    }

    /**
     * Set decoded audio format. This only works in combination with
     * MediaPlayer::setCallbacks() .
     *
     * \param setup  callback to select the audio format (cannot be a nullptr)
     *               Expected prototype is int(char *format, unsigned *rate, unsigned *channels)
     *               Where the return value is 0 for success, anything else to skip audio playback
     *
     * \param cleanup  callback to release any allocated resources (or nullptr)
     *                 Expected prototype is void()
     *
     * \version LibVLC 2.0.0 or later
     */
    template <typename SetupCb, typename CleanupCb>
    void setAudioFormatCallbacks(SetupCb&& setup, CleanupCb&& cleanup)
    {
        static_assert(signature_match<SetupCb, int(char*, uint32_t*, uint32_t*)>::value, "Mismatched Setup callback");
        static_assert(signature_match_or_nullptr<CleanupCb, void()>::value, "Mismatched cleanup callback");

        libvlc_audio_set_format_callbacks(*this,
            CallbackWrapper<(unsigned int)CallbackIdx::AudioSetup, libvlc_audio_setup_cb>::wrap( this, std::forward<SetupCb>( setup ) ),
            CallbackWrapper<(unsigned int)CallbackIdx::AudioCleanup, libvlc_audio_cleanup_cb>::wrap( this, std::forward<CleanupCb>( cleanup ) ) );
    }

    /**
     * Set decoded audio format. This only works in combination with
     * MediaPlayer::setCallbacks() , and is mutually exclusive with
     * MediaPlayer::setFormatCallbacks() .
     *
     * \param format  a four-characters string identifying the sample format
     * (e.g. "S16N" or "FL32")
     *
     * \param rate  sample rate (expressed in Hz)
     *
     * \param channels  channels count
     *
     * \version LibVLC 2.0.0 or later
     */
    void setAudioFormat(const std::string& format, unsigned rate, unsigned channels)
    {
        libvlc_audio_set_format(*this, format.c_str(), rate, channels);
    }

    /**
     * Selects an audio output module.
     *
     * \note Any change will take be effect only after playback is stopped
     * and restarted. Audio output cannot be changed while playing.
     *
     * \param name  name of audio output, use psz_name of
     *
     * \see AudioOutputDescription
     *
     * \return 0 if function succeded, -1 on error
     */
    int setAudioOutput(const std::string& name)
    {
        return libvlc_audio_output_set(*this, name.c_str());
    }

    /**
     * Gets a list of potential audio output devices,
     *
     * \see MediaPlayer::outputDeviceSet() .
     *
     * \note Not all audio outputs support enumerating devices. The audio
     * output may be functional even if the list is empty (NULL).
     *
     * \note The list may not be exhaustive.
     *
     * \warning Some audio output devices in the list might not actually work
     * in some circumstances. By default, it is recommended to not specify
     * any explicit audio device.
     *
     * \version LibVLC 2.2.0 or later.
     */
    std::vector<AudioOutputDeviceDescription> outputDeviceEnum()
    {
        auto devices = libvlc_audio_output_device_enum(*this);
        if ( devices == nullptr )
            return {};
        std::vector<AudioOutputDeviceDescription> res;
        std::unique_ptr<libvlc_audio_output_device_t, decltype(&libvlc_audio_output_device_list_release)>
                devicesPtr( devices, libvlc_audio_output_device_list_release);
        for ( auto* p = devices; p != NULL; p = p->p_next )
            res.emplace_back( p );
        return res;
    }

    /**
     * Configures an explicit audio output device.
     *
     * If the module paramater is NULL, audio output will be moved to the
     * device specified by the device identifier string immediately. This is
     * the recommended usage.
     *
     * A list of adequate potential device strings can be obtained with
     * MediaPlayer::outputDeviceEnum() .
     *
     * However passing NULL is supported in LibVLC version 2.2.0 and later
     * only; in earlier versions, this function would have no effects when
     * the module parameter was NULL.
     *
     * If the module parameter is not NULL, the device parameter of the
     * corresponding audio output, if it exists, will be set to the specified
     * string. Note that some audio output modules do not have such a
     * parameter (notably MMDevice and PulseAudio).
     *
     * A list of adequate potential device strings can be obtained with
     * Instance::audioOutputDeviceList() .
     *
     * \note This function does not select the specified audio output plugin.
     * MediaPlayer::outputSet() is used for that purpose.
     *
     * \warning The syntax for the device parameter depends on the audio
     * output. Some audio output modules require further parameters (e.g. a
     * channels map in the case of ALSA).
     *
     * \param module  If NULL, current audio output module. if non-NULL, name
     * of audio output module (
     *
     * \see AudioOutputDescription )
     *
     * \param device_id  device identifier string
     *
     * \return Nothing. Errors are ignored (this is a design bug).
     */
    void outputDeviceSet(const std::string& module, const std::string& device_id)
    {
        libvlc_audio_output_device_set(*this, module.c_str(), device_id.c_str());
    }

    void outputDeviceSet(const std::string& device_id)
    {
        libvlc_audio_output_device_set(*this, nullptr, device_id.c_str());
    }

    /**
     * Toggle mute status.
     *
     * \warning Toggling mute atomically is not always possible: On some
     * platforms, other processes can mute the VLC audio playback stream
     * asynchronously. Thus, there is a small race condition where toggling
     * will not work. See also the limitations of MediaPlayer::setMute() .
     */
    void toggleMute()
    {
        libvlc_audio_toggle_mute(*this);
    }

    /**
     * Get current mute status.
     */
    bool mute()
    {
        return libvlc_audio_get_mute( *this ) == 1;
    }

    /**
     * Set mute status.
     *
     * \param status  If status is true then mute, otherwise unmute
     *
     * \warning This function does not always work. If there are no active
     * audio playback stream, the mute status might not be available. If
     * digital pass-through (S/PDIF, HDMI...) is in use, muting may be
     * unapplicable. Also some audio output plugins do not support muting at
     * all.
     *
     * \note To force silent playback, disable all audio tracks. This is more
     * efficient and reliable than mute.
     */
    void setMute(bool mute)
    {
        libvlc_audio_set_mute( *this, (int)mute );
    }

    /**
     * Get current software audio volume.
     *
     * \return the software volume in percents (0 = mute, 100 = nominal /
     * 0dB)
     */
    int volume()
    {
        return libvlc_audio_get_volume(*this);
    }

    /**
     * Set current software audio volume.
     *
     * \param i_volume  the volume in percents (0 = mute, 100 = 0dB)
     */
    bool setVolume(int i_volume)
    {
        return libvlc_audio_set_volume(*this, i_volume) == 0;
    }

    /**
     * Get number of available audio tracks.
     *
     * \return the number of available audio tracks (int), or -1 if
     * unavailable
     */
    int audioTrackCount()
    {
        return libvlc_audio_get_track_count(*this);
    }

    /**
     * Get the description of available audio tracks.
     *
     * \return list with description of available audio tracks
     */
    std::vector<TrackDescription> audioTrackDescription()
    {
        libvlc_track_description_t* result = libvlc_audio_get_track_description( *this );
        return getTracksDescription( result );
    }

    /**
     * Get current audio track.
     *
     * \return the audio track ID or -1 if no active input.
     */
    int audioTrack()
    {
        return libvlc_audio_get_track(*this);
    }

    /**
     * Set current audio track.
     *
     * \param i_track  the track ID (i_id field from track description)
     */
    bool setAudioTrack(int i_track)
    {
        return libvlc_audio_set_track(*this, i_track) == 0;
    }

    /**
     * Get current audio channel.
     *
     * \return the audio channel
     *
     * \see libvlc_audio_output_channel_t
     */
    int channel()
    {
        return libvlc_audio_get_channel(*this);
    }

    /**
     * Set current audio channel.
     *
     * \param channel  the audio channel,
     *
     * \see libvlc_audio_output_channel_t
     */
    bool setChannel(int channel)
    {
        return libvlc_audio_set_channel(*this, channel) == 0;
    }

    /**
     * Get current audio delay.
     *
     * \return the audio delay (microseconds)
     *
     * \version LibVLC 1.1.1 or later
     */
    int64_t audioDelay()
    {
        return libvlc_audio_get_delay(*this);
    }

    /**
     * Set current audio delay. The audio delay will be reset to zero each
     * time the media changes.
     *
     * \param i_delay  the audio delay (microseconds)
     *
     * \version LibVLC 1.1.1 or later
     */
    bool setAudioDelay(int64_t i_delay)
    {
        return libvlc_audio_set_delay(*this, i_delay) == 0;
    }

    /**
     * Set callbacks and private data to render decoded video to a custom
     * area in memory. Use MediaPlayer::setFormat() or MediaPlayer::setFormatCallbacks()
     * to configure the decoded format.
     *
     * \param lock  callback to lock video memory (must not be nullptr)
     *              Expected prototype is void*(void** planes)
     *              planes is a pointer to an array of planes, that must be provided with some allocated buffers
     *              Return value is a picture identifier, to be passed to unlock & display callbacks
     *
     * \param unlock  callback to unlock video memory (or nullptr if not needed)
     *                Expected prototype is void(void* pictureId, void*const* planes);
     *                pictureId is the value returned by the lock callback.
     *                planes is the planes provided by the lock callback
     *
     * \param display callback to display video (or nullptr if not needed)
     *                Expected prototype is void(void* pictureId);
     *                pictureId is the value returned by the lock callback.
     *
     * \version LibVLC 1.1.1 or later
     */
    template <typename LockCb, typename UnlockCb, typename DisplayCb>
    void setVideoCallbacks(LockCb&& lock, UnlockCb&& unlock, DisplayCb&& display)
    {
        static_assert(signature_match<LockCb, void*(void**)>::value, "Mismatched lock callback signature");
        static_assert(signature_match_or_nullptr<UnlockCb, void(void*, void *const *)>::value, "Mismatched unlock callback signature");
        static_assert(signature_match_or_nullptr<DisplayCb, void(void*)>::value, "Mismatched lock callback signature");

        libvlc_video_set_callbacks(*this,
                CallbackWrapper<(unsigned int)CallbackIdx::VideoLock, libvlc_video_lock_cb>::wrap( *m_callbacks, std::forward<LockCb>( lock ) ),
                CallbackWrapper<(unsigned int)CallbackIdx::VideoUnlock, libvlc_video_unlock_cb>::wrap( *m_callbacks, std::forward<UnlockCb>( unlock ) ),
                CallbackWrapper<(unsigned int)CallbackIdx::VideoDisplay, libvlc_video_display_cb>::wrap( *m_callbacks, std::forward<DisplayCb>( display ) ),
                // We will receive the pointer as a void*, we need to offset the value *now*, otherwise we'd get
                // a shifted value, resulting in an empty callback array.
                m_callbacks.get() );
    }

    /**
     * Set decoded video chroma and dimensions. This only works in
     * combination with MediaPlayer::setCallbacks() , and is mutually exclusive
     * with MediaPlayer::setFormatCallbacks() .
     *
     * \param chroma  a four-characters string identifying the chroma (e.g.
     * "RV32" or "YUYV")
     *
     * \param width  pixel width
     *
     * \param height  pixel height
     *
     * \param pitch  line pitch (in bytes)
     *
     * \version LibVLC 1.1.1 or later
     */
    void setVideoFormat(const std::string& chroma, unsigned width, unsigned height, unsigned pitch)
    {
        libvlc_video_set_format(*this, chroma.c_str(), width, height, pitch);
    }

    /**
     * Set decoded video chroma and dimensions. This only works in
     * combination with MediaPlayer::setCallbacks() .
     *
     * \param setup  callback to lock video memory (must not be nullptr)
     *              Expected prototype is uint32_t(char *chroma,       // Must be filled with the required fourcc
     *                                              unsigned *width,   // Must be filled with the required width
     *                                              unsigned *height,  // Must be filled with the required height
     *                                              unsigned *pitches, // Must be filled with the required pitch for each plane
     *                                              unsigned *lines);  // Must be filled with the required number of lines for each plane
     *              The return value reprensent the amount of pictures to create in a pool. 0 to indicate an error
     *
     * \param cleanup  callback to release any allocated resources (or nullptr)
     *                 Expected prototype is void()
     *
     * \version LibVLC 2.0.0 or later
     */
    template <typename FormatCb, typename CleanupCb>
    void setVideoFormatCallbacks(FormatCb&& setup, CleanupCb&& cleanup)
    {
        static_assert(signature_match<FormatCb, uint32_t(char*, uint32_t*, uint32_t*, uint32_t*, uint32_t*)>::value,
                      "Unmatched prototype for format callback" );
        static_assert(signature_match_or_nullptr<CleanupCb, void()>::value, "Unmatched prototype for cleanup callback");

        libvlc_video_set_format_callbacks(*this,
                CallbackWrapper<(unsigned int)CallbackIdx::VideoFormat, libvlc_video_format_cb>::wrap( *m_callbacks, std::forward<FormatCb>( setup ) ),
                CallbackWrapper<(unsigned int)CallbackIdx::VideoCleanup, libvlc_video_cleanup_cb>::wrap( *m_callbacks, std::forward<CleanupCb>( cleanup ) ) );
    }

    /**
     * Enable or disable key press events handling, according to the LibVLC
     * hotkeys configuration. By default and for historical reasons, keyboard
     * events are handled by the LibVLC video widget.
     *
     * \note On X11, there can be only one subscriber for key press and mouse
     * click events per window. If your application has subscribed to those
     * events for the X window ID of the video widget, then LibVLC will not
     * be able to handle key presses and mouse clicks in any case.
     *
     * \warning This function is only implemented for X11 and Win32 at the
     * moment.
     *
     * \param on  true to handle key press events, false to ignore them.
     */
    void setKeyInput(bool enable)
    {
        libvlc_video_set_key_input(*this, (int)enable);
    }

    /**
     * Enable or disable mouse click events handling. By default, those
     * events are handled. This is needed for DVD menus to work, as well as a
     * few video filters such as "puzzle".
     *
     * \see MediaPlayer::setKeyInput() .
     *
     * \warning This function is only implemented for X11 and Win32 at the
     * moment.
     *
     * \param on  true to handle mouse click events, false to ignore them.
     */
    void setMouseInput(bool enable)
    {
        libvlc_video_set_mouse_input(*this, (int)enable);
    }

    /**
     * Get the pixel dimensions of a video.
     *
     * \param num  number of the video (starting from, and most commonly 0)
     *
     * \param px  pointer to get the pixel width [OUT]
     *
     * \param py  pointer to get the pixel height [OUT]
     */
    bool size(unsigned num, unsigned * px, unsigned * py)
    {
        return libvlc_video_get_size( *this, num, px, py ) == 0;
    }

    /**
     * Get the mouse pointer coordinates over a video. Coordinates are
     * expressed in terms of the decoded video resolution, in terms of pixels
     * on the screen/viewport (to get the latter, you can query your
     * windowing system directly).
     *
     * Either of the coordinates may be negative or larger than the
     * corresponding dimension of the video, if the cursor is outside the
     * rendering area.
     *
     * \warning The coordinates may be out-of-date if the pointer is not
     * located on the video rendering area. LibVLC does not track the pointer
     * if it is outside of the video widget.
     *
     * \note LibVLC does not support multiple pointers (it does of course
     * support multiple input devices sharing the same pointer) at the
     * moment.
     *
     * \param num  number of the video (starting from, and most commonly 0)
     *
     * \param px  pointer to get the abscissa [OUT]
     *
     * \param py  pointer to get the ordinate [OUT]
     */
    bool cursor(unsigned num, int * px, int * py)
    {
        return libvlc_video_get_cursor( *this, num, px, py ) == 0;
    }

    /**
     * Get the current video scaling factor. See also MediaPlayer::setScale() .
     *
     * \return the currently configured zoom factor, or 0. if the video is
     * set to fit to the output window/drawable automatically.
     */
    float scale()
    {
        return libvlc_video_get_scale(*this);
    }

    /**
     * Set the video scaling factor. That is the ratio of the number of
     * pixels on screen to the number of pixels in the original decoded video
     * in each dimension. Zero is a special value; it will adjust the video
     * to the output window/drawable (in windowed mode) or the entire screen.
     *
     * Note that not all video outputs support scaling.
     *
     * \param f_factor  the scaling factor, or zero
     */
    void setScale(float f_factor)
    {
        libvlc_video_set_scale(*this, f_factor);
    }

    /**
     * Get current video aspect ratio.
     *
     * \return the video aspect ratio or an empty string if unspecified.
     */
    std::string aspectRatio()
    {
        auto str = wrapCStr( libvlc_video_get_aspect_ratio(*this) );
        if ( str == nullptr )
            return {};
        return str.get();
    }

    /**
     * Set new video aspect ratio.
     *
     * \param psz_aspect  new video aspect-ratio or empty string to reset to default
     *
     * \note Invalid aspect ratios are ignored.
     */
    void setAspectRatio( const std::string& ar )
    {
        libvlc_video_set_aspect_ratio( *this, ar.size() > 0 ? ar.c_str() : nullptr );
    }

    /**
     * Get current video subtitle.
     *
     * \return the video subtitle selected, or -1 if none
     */
    int spu()
    {
        return libvlc_video_get_spu(*this);
    }

    /**
     * Get the number of available video subtitles.
     *
     * \return the number of available video subtitles
     */
    int spuCount()
    {
        return libvlc_video_get_spu_count(*this);
    }

    /**
     * Get the description of available video subtitles.
     *
     * \return list containing description of available video subtitles
     */
    std::vector<TrackDescription> spuDescription()
    {
        libvlc_track_description_t* result = libvlc_video_get_spu_description( *this );
        return getTracksDescription( result );
    }

    /**
     * Set new video subtitle.
     *
     * \param i_spu  video subtitle track to select (i_id from track
     * description)
     *
     * \return 0 on success, -1 if out of range
     */
    int setSpu(int i_spu)
    {
        return libvlc_video_set_spu(*this, i_spu);
    }

#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Set new video subtitle file.
     *
     * \param psz_subtitle  new video subtitle file
     */
    bool setSubtitleFile(const std::string& psz_subtitle)
    {
        return libvlc_video_set_subtitle_file(*this, psz_subtitle.c_str()) != 0;
    }
#endif

    /**
     * Get the current subtitle delay. Positive values means subtitles are
     * being displayed later, negative values earlier.
     *
     * \return time (in microseconds) the display of subtitles is being
     * delayed
     *
     * \version LibVLC 2.0.0 or later
     */
    int64_t spuDelay()
    {
        return libvlc_video_get_spu_delay(*this);
    }

    /**
     * Set the subtitle delay. This affects the timing of when the subtitle
     * will be displayed. Positive values result in subtitles being displayed
     * later, while negative values will result in subtitles being displayed
     * earlier.
     *
     * The subtitle delay will be reset to zero each time the media changes.
     *
     * \param i_delay  time (in microseconds) the display of subtitles should
     * be delayed
     *
     * \return 0 on success, -1 on error
     *
     * \version LibVLC 2.0.0 or later
     */
    int setSpuDelay(int64_t i_delay)
    {
        return libvlc_video_set_spu_delay(*this, i_delay);
    }

    /**
     * Get the description of available titles.
     *
     * \return list containing description of available titles
     */
#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
    std::vector<TrackDescription> titleDescription()
    {
        libvlc_track_description_t* result = libvlc_video_get_title_description( *this );
        return getTracksDescription( result );
    }
#else
    std::vector<TitleDescription> titleDescription()
    {
        libvlc_title_description_t **titles;
        int nbTitles = libvlc_media_player_get_full_title_descriptions( *this, &titles);
        auto cleanupCb = [nbTitles]( libvlc_title_description_t** ts) {
            libvlc_title_descriptions_release( ts, nbTitles );
        };

        std::vector<TitleDescription> res;

        if ( nbTitles < 1 )
            return res;

        std::unique_ptr<libvlc_title_description_t*[], decltype(cleanupCb)> ptr( titles, cleanupCb );

        for ( int i = 0; i < nbTitles; ++i )
            res.emplace_back( ptr[i] );
        return res;
    }
#endif

    /**
     * Get the description of available chapters for specific title.
     *
     * \param i_title  selected title
     *
     * \return list containing description of available chapters for title
     * i_title
     */
#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
    std::vector<TrackDescription> chapterDescription(int i_title)
    {
        libvlc_track_description_t* result = libvlc_video_get_chapter_description( *this, i_title );
        return getTracksDescription( result );
    }
#else
    std::vector<ChapterDescription> chapterDescription(int i_title)
    {
        libvlc_chapter_description_t **chapters;
        int nbChapters = libvlc_media_player_get_full_chapter_descriptions( *this, i_title, &chapters );
        auto cleanupCb = [nbChapters](libvlc_chapter_description_t** cs) {
            libvlc_chapter_descriptions_release( cs, nbChapters );
        };

        std::vector<ChapterDescription> res;

        if ( nbChapters < 1 )
            return res;

        std::unique_ptr<libvlc_chapter_description_t*[], decltype(cleanupCb)> ptr( chapters, cleanupCb );

        for ( int i = 0; i < nbChapters; ++i )
            res.emplace_back( ptr[i] );
        return res;
    }
#endif

    /**
     * Get current crop filter geometry.
     *
     * \return the crop filter geometry or an empty string if unset
     */
    std::string cropGeometry()
    {
        auto str = wrapCStr( libvlc_video_get_crop_geometry(*this) );
        if ( str == nullptr )
            return {};
        return str.get();
    }

    /**
     * Set new crop filter geometry.
     *
     * \param psz_geometry new crop filter geometry (empty string to unset)
     */
    void setCropGeometry(const std::string& geometry)
    {
        libvlc_video_set_crop_geometry( *this, geometry.size() > 0 ? geometry.c_str() : nullptr );
    }

    /**
     * Get current teletext page requested.
     *
     * \return the current teletext page requested.
     */
    int teletext()
    {
        return libvlc_video_get_teletext(*this);
    }

    /**
     * Set new teletext page to retrieve.
     *
     * \param i_page  teletex page number requested
     */
    void setTeletext(int i_page)
    {
        libvlc_video_set_teletext(*this, i_page);
    }

    /**
     * Get number of available video tracks.
     *
     * \return the number of available video tracks (int)
     */
    int videoTrackCount()
    {
        return libvlc_video_get_track_count(*this);
    }

    /**
     * Get the description of available video tracks.
     *
     * \return list with description of available video tracks
     */
    std::vector<TrackDescription> videoTrackDescription()
    {
        libvlc_track_description_t* result = libvlc_video_get_track_description( *this );
        return getTracksDescription( result );
    }

    /**
     * Get current video track.
     *
     * \return the video track ID (int) or -1 if no active input
     */
    int videoTrack()
    {
        return libvlc_video_get_track(*this);
    }

    /**
     * Set video track.
     *
     * \param i_track  the track ID (i_id field from track description)
     *
     * \return 0 on success, -1 if out of range
     */
    int setVideoTrack(int i_track)
    {
        return libvlc_video_set_track(*this, i_track);
    }

    /**
     * Take a snapshot of the current video window.
     *
     * If i_width AND i_height is 0, original size is used. If i_width XOR
     * i_height is 0, original aspect-ratio is preserved.
     *
     * \param num  number of video output (typically 0 for the first/only
     * one)
     *
     * \param filepath  the path where to save the screenshot to
     *
     * \param i_width  the snapshot's width
     *
     * \param i_height  the snapshot's height
     */
    bool takeSnapshot(unsigned num, const std::string& filepath, unsigned int i_width, unsigned int i_height)
    {
        return libvlc_video_take_snapshot(*this, num, filepath.c_str(), i_width, i_height) == 0;
    }

    /**
     * Enable or disable deinterlace filter
     *
     * \param psz_mode  type of deinterlace filter, empty string to disable
     */
    void setDeinterlace(const std::string& mode)
    {
        libvlc_video_set_deinterlace(*this,
                                     mode.empty() ? NULL : mode.c_str());
    }

    /**
     * Get an integer marquee option value
     *
     * \param option  marq option to get
     *
     * \see libvlc_video_marquee_int_option_t
     */
    int marqueeInt(unsigned option)
    {
        return libvlc_video_get_marquee_int(*this, option);
    }

    /**
     * Get a string marquee option value
     *
     * \param option  marq option to get
     *
     * \see libvlc_video_marquee_string_option_t
     */
    std::string marqueeString(unsigned option)
    {
        auto str = wrapCStr( libvlc_video_get_marquee_string(*this, option) );
        if ( str == nullptr )
            return {};
        return str.get();
    }

    /**
     * Enable, disable or set an integer marquee option
     *
     * Setting libvlc_marquee_Enable has the side effect of enabling (arg !0)
     * or disabling (arg 0) the marq filter.
     *
     * \param option  marq option to set
     *
     * \see libvlc_video_marquee_int_option_t
     *
     * \param i_val  marq option value
     */
    void setMarqueeInt(unsigned option, int i_val)
    {
        libvlc_video_set_marquee_int(*this, option, i_val);
    }

    /**
     * Set a marquee string option
     *
     * \param option  marq option to set
     *
     * \see libvlc_video_marquee_string_option_t
     *
     * \param text  marq option value
     */
    void setMarqueeString(unsigned option, const std::string& text)
    {
        libvlc_video_set_marquee_string(*this, option, text.c_str());
    }

    /**
     * Get integer logo option.
     *
     * \param option  logo option to get, values of
     * libvlc_video_logo_option_t
     */
    int logoInt(unsigned option)
    {
        return libvlc_video_get_logo_int(*this, option);
    }

    /**
     * Set logo option as integer. Options that take a different type value
     * are ignored. Passing libvlc_logo_enable as option value has the side
     * effect of starting (arg !0) or stopping (arg 0) the logo filter.
     *
     * \param option  logo option to set, values of
     * libvlc_video_logo_option_t
     *
     * \param value  logo option value
     */
    void setLogoInt(unsigned option, int value)
    {
        libvlc_video_set_logo_int(*this, option, value);
    }

    /**
     * Set logo option as string. Options that take a different type value
     * are ignored.
     *
     * \param option  logo option to set, values of
     * libvlc_video_logo_option_t
     *
     * \param psz_value  logo option value
     */
    void setLogoString(unsigned option, const std::string& value)
    {
        libvlc_video_set_logo_string(*this, option, value.c_str());
    }

    /**
     * Get integer adjust option.
     *
     * \param option  adjust option to get, values of
     * libvlc_video_adjust_option_t
     *
     * \version LibVLC 1.1.1 and later.
     */
    int adjustInt(unsigned option)
    {
        return libvlc_video_get_adjust_int(*this, option);
    }

    /**
     * Set adjust option as integer. Options that take a different type value
     * are ignored. Passing libvlc_adjust_enable as option value has the side
     * effect of starting (arg !0) or stopping (arg 0) the adjust filter.
     *
     * \param option  adust option to set, values of
     * libvlc_video_adjust_option_t
     *
     * \param value  adjust option value
     *
     * \version LibVLC 1.1.1 and later.
     */
    void setAdjustInt(unsigned option, int value)
    {
        libvlc_video_set_adjust_int(*this, option, value);
    }

    /**
     * Get float adjust option.
     *
     * \param option  adjust option to get, values of
     * libvlc_video_adjust_option_t
     *
     * \version LibVLC 1.1.1 and later.
     */
    float adjustFloat(unsigned option)
    {
        return libvlc_video_get_adjust_float(*this, option);
    }

    /**
     * Set adjust option as float. Options that take a different type value
     * are ignored.
     *
     * \param option  adust option to set, values of
     * libvlc_video_adjust_option_t
     *
     * \param value  adjust option value
     *
     * \version LibVLC 1.1.1 and later.
     */
    void setAdjustFloat(unsigned option, float value)
    {
        libvlc_video_set_adjust_float(*this, option, value);
    }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Add a slave to the current media player.
     *
     * \note If the player is playing, the slave will be added directly. This call
     * will also update the slave list of the attached VLC::Media.
     *
     * \version LibVLC 3.0.0 and later.
     *
     * \see Media::addSlave
     *
     * \param type subtitle or audio
     * \param uri Uri of the slave (should contain a valid scheme).
     * \param select True if this slave should be selected when it's loaded
     *
     * \return true on success, false on error.
     */
    bool addSlave( MediaSlave::Type type, const std::string& uri, bool select )
    {
        return libvlc_media_player_add_slave( *this,
                        static_cast<libvlc_media_slave_type_t>( type ), uri.c_str(), select ) == 0;
    }

    bool updateViewpoint( const VideoViewpoint& viewpoint, bool b_absolute )
    {
        return libvlc_video_update_viewpoint( *this, 
            static_cast<const libvlc_video_viewpoint_t*>( &viewpoint ), b_absolute ) == 0;
    }

#endif

private:
    std::vector<TrackDescription> getTracksDescription( libvlc_track_description_t* tracks ) const
    {
        if ( tracks == nullptr )
            return {};
        std::vector<TrackDescription> result;
        auto p = tracks;
        std::unique_ptr<libvlc_track_description_t, decltype(&libvlc_track_description_list_release)>
                devicePtr( tracks, libvlc_track_description_list_release );
        while ( p != nullptr )
        {
            result.emplace_back( p );
            p = p->p_next;
        }
        return result;
    }

private:
    std::shared_ptr<MediaPlayerEventManager> m_eventManager;
};

} // namespace VLC

#endif
