/*****************************************************************************
 * Media.hpp: Media API
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

#ifndef LIBVLC_CXX_MEDIA_H
#define LIBVLC_CXX_MEDIA_H

#include "common.hpp"

#include <vector>
#include <stdexcept>

namespace VLC
{

class MediaEventManager;
class Instance;
class MediaList;

class Media : protected CallbackOwner<4>, public Internal<libvlc_media_t>
{
private:
    enum class CallbackIdx : unsigned int
    {
        Open,
        Read,
        Seek,
        Close,
    };
#if !defined(_MSC_VER) || _MSC_VER >= 1900
    static constexpr unsigned int NbEvents = 4;
#else
    static const unsigned int NbEvents = 4;
#endif

public:
    ///
    /// \brief The FromType enum is used to drive the media creation.
    /// A media is usually created using a string, which can represent one of 3 things:
    ///
    enum class FromType
    {
        /**
         * Create a media for a certain file path.
         */
        FromPath,
        /**
         * Create a media with a certain given media resource location,
         * for instance a valid URL.
         *
         * \note To refer to a local file with this function,
         * the file://... URI syntax <b>must</b> be used (see IETF RFC3986).
         * We recommend using FromPath instead when dealing with
         * local files.
         */
        FromLocation,
        /**
         * Create a media as an empty node with a given name.
         */
        AsNode,
    };
    // To be able to write Media::FromLocation
#if !defined(_MSC_VER) || _MSC_VER >= 1900
    constexpr static FromType FromPath = FromType::FromPath;
    constexpr static FromType FromLocation = FromType::FromLocation;
    constexpr static FromType AsNode = FromType::AsNode;
#else
    static const FromType FromPath = FromType::FromPath;
    static const FromType FromLocation = FromType::FromLocation;
    static const FromType AsNode = FromType::AsNode;
#endif

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
    enum class ParseFlags
    {
        /**
         * Parse media if it's a local file
         */
        Local = libvlc_media_parse_local,
        /**
         * Parse media even if it's a network file
         */
        Network = libvlc_media_parse_network,
        /**
         * Fetch meta and covert art using local resources
         */
        FetchLocal = libvlc_media_fetch_local,
        /**
         * Fetch meta and covert art using network resources
         */
        FetchNetwork = libvlc_media_fetch_network,
        /**
         * Interact with the user (via libvlc_dialog_cbs) when preparsing this item
         * (and not its sub items). Set this flag in order to receive a callback
         * when the input is asking for credentials.
         */
        Interact = libvlc_media_do_interact,
    };

    enum class ParsedStatus
    {
        Skipped = libvlc_media_parsed_status_skipped,
        Failed = libvlc_media_parsed_status_failed,
        Done = libvlc_media_parsed_status_done,
        Timeout = libvlc_media_parsed_status_timeout,
    };

    enum class Type
    {
        Unknown = libvlc_media_type_unknown,
        File = libvlc_media_type_file,
        Directory = libvlc_media_type_directory,
        Disc = libvlc_media_type_disc,
        Stream = libvlc_media_type_stream,
        Playlist = libvlc_media_type_playlist,
    };
#endif

    /**
     * @brief Media Constructs a libvlc Media instance
     * @param instance  A libvlc instance
     * @param mrl       A path, location, or node name, depending on the 3rd parameter
     * @param type      The type of the 2nd argument. \sa{FromType}
     */
    Media(Instance& instance, const std::string& mrl, FromType type)
        : Internal{ libvlc_media_release }
    {
        InternalPtr ptr = nullptr;
        switch (type)
        {
        case FromLocation:
            ptr = libvlc_media_new_location( getInternalPtr<libvlc_instance_t>( instance ), mrl.c_str() );
            break;
        case FromPath:
            ptr = libvlc_media_new_path( getInternalPtr<libvlc_instance_t>( instance ), mrl.c_str() );
            break;
        case AsNode:
            ptr = libvlc_media_new_as_node( getInternalPtr<libvlc_instance_t>( instance ), mrl.c_str() );
            break;
        default:
            break;
        }
        if ( ptr == nullptr )
            throw std::runtime_error("Failed to construct a media");
        m_obj.reset( ptr, libvlc_media_release );
    }

    /**
     * Create a media for an already open file descriptor.
     * The file descriptor shall be open for reading (or reading and writing).
     *
     * Regular file descriptors, pipe read descriptors and character device
     * descriptors (including TTYs) are supported on all platforms.
     * Block device descriptors are supported where available.
     * Directory descriptors are supported on systems that provide fdopendir().
     * Sockets are supported on all platforms where they are file descriptors,
     * i.e. all except Windows.
     *
     * \note This library will <b>not</b> automatically close the file descriptor
     * under any circumstance. Nevertheless, a file descriptor can usually only be
     * rendered once in a media player. To render it a second time, the file
     * descriptor should probably be rewound to the beginning with lseek().
     *
     * \param instance the instance
     * \param fd open file descriptor
     * \return the newly created media
     */
    Media(Instance& instance, int fd)
        : Internal { libvlc_media_new_fd( getInternalPtr<libvlc_instance_t>( instance ), fd ),
                     libvlc_media_release }
    {
    }

    /**
     * Get media instance from this media list instance. This action will increase
     * the refcount on the media instance.
     * The libvlc_media_list_lock should NOT be held upon entering this function.
     *
     * \param list a media list instance
     * \return media instance
     */
    Media(MediaList& list)
        : Internal{ libvlc_media_list_media( getInternalPtr<libvlc_media_list_t>( list ) ),
                    libvlc_media_release }
    {
    }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Callback prototype to open a custom bitstream input media.
     *
     * The same media item can be opened multiple times. Each time, this callback
     * is invoked. It should allocate and initialize any instance-specific
     * resources, then store them in *datap. The instance resources can be freed
     * in the @ref libvlc_close_cb callback.
     *
     * \param opaque private pointer as passed to libvlc_media_new_callbacks()
     * \param datap storage space for a private data pointer [OUT]
     * \param sizep byte length of the bitstream or 0 if unknown [OUT]
     *
     * \note For convenience, *datap is initially NULL and *sizep is initially 0.
     *
     * \return 0 on success, non-zero on error. In case of failure, the other
     * callbacks will not be invoked and any value stored in *datap and *sizep is
     * discarded.
     */
    using ExpectedMediaOpenCb = int(void*, void** datap, uint64_t* sizep);

    /**
     * Callback prototype to read data from a custom bitstream input media.
     *
     * \param opaque private pointer as set by the @ref libvlc_media_open_cb
     *               callback
     * \param buf start address of the buffer to read data into
     * \param len bytes length of the buffer
     *
     * \return strictly positive number of bytes read, 0 on end-of-stream,
     *         or -1 on non-recoverable error
     *
     * \note If no data is immediately available, then the callback should sleep.
     * \warning The application is responsible for avoiding deadlock situations.
     * In particular, the callback should return an error if playback is stopped;
     * if it does not return, then libvlc_media_player_stop() will never return.
     */
    using ExpectedMediaReadCb = ssize_t(void* opaque, unsigned char* buf, size_t len);

    /**
     * Callback prototype to seek a custom bitstream input media.
     *
     * \param opaque private pointer as set by the @ref libvlc_media_open_cb
     *               callback
     * \param offset absolute byte offset to seek to
     * \return 0 on success, -1 on error.
     */
    using ExpectedMediaSeekCb = int(void* opaque, uint64_t);

    /**
     * Callback prototype to close a custom bitstream input media.
     * \param opaque private pointer as set by the @ref libvlc_media_open_cb
     *               callback
     */
    using ExpectedMediaCloseCb = void(void* opaque);

    /**
     * Create a media with custom callbacks to read the data from.
     *
     * \param instance LibVLC instance
     * \param open_cb callback to open the custom bitstream input media
     * \param read_cb callback to read data (must not be nullptr)
     * \param seek_cb callback to seek, or nullptr if seeking is not supported
     * \param close_cb callback to close the media, or nullptr if unnecessary
     *
     * \return the newly created media.
     *
     * \throw std::runtime_error if the media creation fails
     *
     * \note If open_cb is NULL, the opaque pointer will be passed to read_cb,
     * seek_cb and close_cb, and the stream size will be treated as unknown.
     *
     * \note The callbacks may be called asynchronously (from another thread).
     * A single stream instance need not be reentrant. However the open_cb needs to
     * be reentrant if the media is used by multiple player instances.
     *
     * \warning The callbacks may be used until all or any player instances
     * that were supplied the media item are stopped.
     *
     * \see ExpectedMediaOpenCb
     * \see ExpectedMediaReadCb
     * \see ExpectedMediaSeekCb
     * \see ExpectedMediaCloseCb
     *
     * \version LibVLC 3.0.0 and later.
     */

    template <typename OpenCb, typename ReadCb, typename SeekCb, typename CloseCb>
    Media( Instance& instance, OpenCb&& openCb, ReadCb&& readCb, SeekCb&& seekCb, CloseCb&& closeCb )
    {
        static_assert( signature_match_or_nullptr<OpenCb, ExpectedMediaOpenCb>::value, "Mismatched Open callback prototype" );
        static_assert( signature_match_or_nullptr<SeekCb, ExpectedMediaSeekCb>::value, "Mismatched Seek callback prototype" );
        static_assert( signature_match_or_nullptr<CloseCb, ExpectedMediaCloseCb>::value, "Mismatched Close callback prototype" );
        static_assert( signature_match<ReadCb, ExpectedMediaReadCb>::value, "Mismatched Read callback prototype" );

        auto ptr = libvlc_media_new_callbacks( instance,
            imem::CallbackWrapper<(unsigned int)CallbackIdx::Open, libvlc_media_open_cb>::
                wrap<imem::GuessBoxingStrategy<OpenCb, imem::BoxingStrategy::Setup>::Strategy>(
                    *m_callbacks, std::forward<OpenCb>( openCb ) ),
            imem::CallbackWrapper<(unsigned int)CallbackIdx::Read, libvlc_media_read_cb>::
                wrap<imem::GuessBoxingStrategy<OpenCb, imem::BoxingStrategy::Unbox>::Strategy>(
                    *m_callbacks, std::forward<ReadCb>( readCb ) ),
            imem::CallbackWrapper<(unsigned int)CallbackIdx::Seek, libvlc_media_seek_cb>::
                wrap<imem::GuessBoxingStrategy<OpenCb, imem::BoxingStrategy::Unbox>::Strategy>(
                    *m_callbacks, std::forward<SeekCb>( seekCb ) ),
            imem::CallbackWrapper<(unsigned int)CallbackIdx::Close, libvlc_media_close_cb>::
                wrap<imem::GuessBoxingStrategy<OpenCb, imem::BoxingStrategy::Cleanup>::Strategy>(
                    *m_callbacks, std::forward<CloseCb>( closeCb ) ),
            m_callbacks.get()
        );
        if ( ptr == nullptr )
            throw std::runtime_error( "Failed to create media" );
        m_obj.reset( ptr, libvlc_media_release );
    }

#endif

    explicit Media( Internal::InternalPtr ptr, bool incrementRefCount)
        : Internal{ ptr, libvlc_media_release }
    {
        if ( incrementRefCount && ptr != nullptr )
            retain();
    }

    /**
     * Create an empty VLC Media instance.
     *
     * Calling any method on such an instance is undefined.
    */
    Media() = default;

    /**
     * Check if 2 Media objects contain the same libvlc_media_t.
     * \param another another Media
     * \return true if they contain the same libvlc_media_t
     */
    bool operator==(const Media& another) const
    {
        return m_obj == another.m_obj;
    }

    /**
     * Add an option to the media.
     *
     * This option will be used to determine how the media_player will read
     * the media. This allows to use VLC's advanced reading/streaming options
     * on a per-media basis.
     *
     * \note The options are listed in 'vlc long-help' from the command line,
     * e.g. "-sout-all". Keep in mind that available options and their
     * semantics vary across LibVLC versions and builds.
     *
     * \warning Not all options affects libvlc_media_t objects: Specifically,
     * due to architectural issues most audio and video options, such as text
     * renderer options, have no effects on an individual media. These
     * options must be set through Instance::Instance() instead.
     *
     * \param psz_options  the options (as a string)
     */
    void addOption(const std::string& psz_options)
    {
        libvlc_media_add_option(*this,psz_options.c_str());
    }

    /**
     * Add an option to the media with configurable flags.
     *
     * This option will be used to determine how the media_player will read
     * the media. This allows to use VLC's advanced reading/streaming options
     * on a per-media basis.
     *
     * The options are detailed in vlc long-help, for instance "--sout-all".
     * Note that all options are not usable on medias: specifically, due to
     * architectural issues, video-related options such as text renderer
     * options cannot be set on a single media. They must be set on the whole
     * libvlc instance instead.
     *
     * \param psz_options  the options (as a string)
     *
     * \param i_flags  the flags for this option
     */
    void addOptionFlag(const std::string& psz_options, unsigned i_flags)
    {
        libvlc_media_add_option_flag(*this,psz_options.c_str(), i_flags);
    }

    /**
     * Get the media resource locator (mrl) from a media descriptor object
     *
     * \return string with mrl of media descriptor object
     */
    std::string mrl()
    {
        auto str = wrapCStr( libvlc_media_get_mrl(*this) );
        if ( str == nullptr )
            return {};
        return str.get();
    }

    /**
     * Duplicate a media descriptor object.
     */
    Media duplicate()
    {
        auto obj = libvlc_media_duplicate(*this);
        // Assume failure to duplicate is due to VLC_ENOMEM.
        // libvlc_media_duplicate(nullptr) would also return nullptr, but
        // we consider the use of an empty libvlcpp instance undefined.
        if ( obj == nullptr )
            throw std::bad_alloc();
        return Media( obj, false );
    }

    /**
     * Read the meta of the media.
     *
     * If the media has not yet been parsed this will return NULL.
     *
     * This methods automatically calls parseAsync() , so after
     * calling it you may receive a libvlc_MediaMetaChanged event. If you
     * prefer a synchronous version ensure that you call parse()
     * before get_meta().
     *
     * \see parse()
     *
     * \see parseAsync()
     *
     * \see libvlc_MediaMetaChanged
     *
     * \param e_meta  the meta to read
     *
     * \return the media's meta
     */
    std::string meta(libvlc_meta_t e_meta)
    {
        auto str = wrapCStr(libvlc_media_get_meta(*this, e_meta) );
        if ( str == nullptr )
            return {};
        return str.get();
    }

    /**
     * Set the meta of the media (this function will not save the meta, call
     * libvlc_media_save_meta in order to save the meta)
     *
     * \param e_meta  the meta to write
     *
     * \param psz_value  the media's meta
     */
    void setMeta(libvlc_meta_t e_meta, const std::string& psz_value)
    {
        libvlc_media_set_meta(*this, e_meta, psz_value.c_str());
    }


    /**
     * Save the meta previously set
     *
     * \return true if the write operation was successful
     */
    bool saveMeta()
    {
        return libvlc_media_save_meta(*this) != 0;
    }

    /**
     * Get current state of media descriptor object. Possible media states
     * are defined in libvlc_structures.c ( libvlc_NothingSpecial=0,
     * libvlc_Opening, libvlc_Buffering, libvlc_Playing, libvlc_Paused,
     * libvlc_Stopped, libvlc_Ended, libvlc_Error).
     *
     * \see libvlc_state_t
     *
     * \return state of media descriptor object
     */
    libvlc_state_t state()
    {
        return libvlc_media_get_state(*this);
    }

    /**
     * Get the current statistics about the media
     *
     * \param p_stats  structure that contain the statistics about the media
     * (this structure must be allocated by the caller)
     *
     * \return true if the statistics are available, false otherwise
     */
    bool stats(libvlc_media_stats_t * p_stats)
    {
        return libvlc_media_get_stats(*this, p_stats) != 0;
    }

    /**
     * Get event manager from media descriptor object. NOTE: this function
     * doesn't increment reference counting.
     *
     * \return event manager object
     */
    MediaEventManager& eventManager()
    {
        if ( m_eventManager == nullptr )
        {
            libvlc_event_manager_t* obj = libvlc_media_event_manager(*this);
            m_eventManager = std::make_shared<MediaEventManager>( obj );
        }
        return *m_eventManager;
    }

    /**
     * Get duration (in ms) of media descriptor object item.
     *
     * \return duration of media item or -1 on error
     */
    libvlc_time_t duration()
    {
        return libvlc_media_get_duration(*this);
    }

#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Parse a media.
     *
     * This fetches (local) meta data and tracks information. The method is
     * synchronous.
     *
     * \see parseAsync()
     *
     * \see meta()
     *
     * \see tracksInfo()
     */
    void parse()
    {
        libvlc_media_parse(*this);
    }

    /**
     * Parse a media.
     *
     * This fetches (local) meta data and tracks information. The method is
     * the asynchronous of parse() .
     *
     * To track when this is over you can listen to libvlc_MediaParsedChanged
     * event. However if the media was already parsed you will not receive
     * this event.
     *
     * \see parse()
     *
     * \see libvlc_MediaParsedChanged
     *
     * \see meta()
     *
     * \see tracks()
     */
    void parseAsync()
    {
        libvlc_media_parse_async(*this);
    }

    /**
     * Get Parsed status for media descriptor object.
     *
     * \see libvlc_MediaParsedChanged
     *
     * \return true if media object has been parsed otherwise it returns
     * false
     */
    bool isParsed()
    {
        return libvlc_media_is_parsed(*this) != 0;
    }
#else
    /**
     * Parse the media asynchronously with options.
     *
     * This fetches (local or network) art, meta data and/or tracks information.
     * This method is the extended version of libvlc_media_parse_async().
     *
     * To track when this is over you can listen to libvlc_MediaParsedStatus
     * event. However if this functions returns an error, you will not receive any
     * events.
     *
     * It uses a flag to specify parse options (see libvlc_media_parse_flag_t). All
     * these flags can be combined. By default, media is parsed if it's a local
     * file.
     *
     * \see ParsedStatus
     * \see meta()
     * \see tracks()
     * \see parsedStatus
     * \see ParseFlag
     *
     * \return true on success, false otherwise
     * \param flags parse options
     * \param timeout maximum time allowed to preparse the media. If -1, the
     *      default "preparse-timeout" option will be used as a timeout. If 0, it will
     *      wait indefinitely. If > 0, the timeout will be used (in milliseconds).
     * \version LibVLC 3.0.0 or later
     */
    bool parseWithOptions( ParseFlags flags, int timeout )
    {
        return libvlc_media_parse_with_options( *this, static_cast<libvlc_media_parse_flag_t>( flags ), timeout ) == 0;
    }

    ParsedStatus parsedStatus()
    {
        return static_cast<ParsedStatus>( libvlc_media_get_parsed_status( *this ) );
    }
#endif

    /**
     * Sets media descriptor's user_data. user_data is specialized data
     * accessed by the host application, VLC.framework uses it as a pointer
     * to an native object that references a libvlc_media_t pointer
     *
     * \param p_new_user_data  pointer to user data
     */
    void setUserData(void * p_new_user_data)
    {
        libvlc_media_set_user_data(*this, p_new_user_data);
    }

    /**
     * Get media descriptor's user_data. user_data is specialized data
     * accessed by the host application, VLC.framework uses it as a pointer
     * to an native object that references a libvlc_media_t pointer
     */
    void* userData()
    {
        return libvlc_media_get_user_data(*this);
    }

    /**
     * Get media descriptor's elementary streams description
     *
     * Note, you need to call parse() or play the media at least once
     * before calling this function. Not doing this will result in an empty
     * list.
     *
     * \version LibVLC 2.1.0 and later.
     *
     * \return a vector containing all tracks
     */
    std::vector<MediaTrack> tracks()
    {
        libvlc_media_track_t**  tracks;
        uint32_t                nbTracks = libvlc_media_tracks_get(*this, &tracks);
        std::vector<MediaTrack> res;

        if ( nbTracks == 0 )
            return res;

        for ( uint32_t i = 0; i < nbTracks; ++i )
            res.emplace_back( tracks[i] );
        libvlc_media_tracks_release( tracks, nbTracks );
        return res;
    }

    std::shared_ptr<MediaList> subitems()
    {
        auto p = libvlc_media_subitems( *this );
        if ( p == nullptr )
            return nullptr;
        return std::make_shared<MediaList>( p );
    }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
    Type type()
    {
        return static_cast<Type>( libvlc_media_get_type( *this ) );
    }

    /**
     * Add a slave to the current media.
     *
     * A slave is an external input source that may contains an additional subtitle
     * track (like a .srt) or an additional audio track (like a .ac3).
     *
     * \note This function must be called before the media is parsed (via parseWithOptions())
     *  or before the media is played (via MediaPlayer::play())
     *
     * \version LibVLC 3.0.0 and later.
     *
     * \param uri Uri of the slave (should contain a valid scheme).
     * \param type subtitle or audio
     * \param priority from 0 (low priority) to 4 (high priority)
     *
     * \return true on success, false on error.
     */
    bool addSlave(MediaSlave::Type type, unsigned priority, std::string const &uri)
    {
        return libvlc_media_slaves_add(*this, (libvlc_media_slave_type_t)type, priority, uri.c_str()) == 0;
    }

    /**
     * Clear all slaves previously added by addSlave() or
     * internally.
     *
     * \version LibVLC 3.0.0 and later.
     */
    void slavesClear()
    {
        libvlc_media_slaves_clear(*this);
    }

    /**
     * Get a media descriptor's slaves in a vector
     *
     * The list will contain slaves parsed by VLC or previously added by
     * addSlave(). The typical use case of this function is to save
     * a list of slave in a database for a later use.
     *
     * \version LibVLC 3.0.0 and later.
     *
     * \see addSlave()
     *
     * \return a vector of MediaSlave
     */
    std::vector<MediaSlave> slaves() const
    {
        libvlc_media_slave_t **list = nullptr;

        auto length = libvlc_media_slaves_get(*this, &list);
        if (length == 0)
            return {};
        auto deletor = [length](libvlc_media_slave_t **p_list) {
            libvlc_media_slaves_release(p_list, length);
        };
        std::unique_ptr<libvlc_media_slave_t*, decltype(deletor)> scope_gard(list, deletor);
        std::vector<MediaSlave> res(list, list + length);
        return res;
    }
#endif

private:

    /**
     * Retain a reference to a media descriptor object (libvlc_media_t). Use
     * release() to decrement the reference count of a media
     * descriptor object.
     */
    void retain()
    {
        if ( isValid() )
            libvlc_media_retain(*this);
    }


private:
    std::shared_ptr<MediaEventManager> m_eventManager;
};

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
inline VLC::Media::ParseFlags operator|(Media::ParseFlags l, Media::ParseFlags r)
{
#if !defined(_MSC_VER) || _MSC_VER >= 1900
    using T = typename std::underlying_type<Media::ParseFlags>::type;
#else
    using T = std::underlying_type<Media::ParseFlags>::type;
#endif
    return static_cast<Media::ParseFlags>( static_cast<T>( l ) | static_cast<T>( r ) );
}
#endif

} // namespace VLC

#endif

