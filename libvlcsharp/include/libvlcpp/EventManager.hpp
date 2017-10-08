/*****************************************************************************
 * EventManager.hpp: Exposes libvlc events
 *****************************************************************************
 * Copyright © 2015 libvlcpp authors & VideoLAN
 *
 * Authors: Hugo Beauzée-Luyssen <hugo@beauzee.fr>
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

#ifndef LIBVLC_EVENTMANAGER_HPP
#define LIBVLC_EVENTMANAGER_HPP

#include <string>

#include "common.hpp"
#include "Internal.hpp"
#include "Media.hpp"

#include <algorithm>
#include <functional>
#include <vector>
#include <memory>

#define EXPECT_SIGNATURE(sig) static_assert(signature_match<decltype(f), sig>::value, "Expected a function with prototype " #sig)

namespace VLC
{

/**
 * @brief This class serves as a base for all event managers.
 *
 * All events can be handled by providing a std::function.
 * libvlcpp will take ownership (ie. the function will be moved inside an internal list)
 * If the provided std::function is a lambda, it may capture anything you desire
 */
class EventManager : public Internal<libvlc_event_manager_t>
{
protected:
    template <typename T>
    using DecayPtr = typename std::add_pointer<typename std::decay<T>::type>::type;

private:
    // Polymorphic base to hold a templated implementation
    struct EventHandlerBase
    {
        using Wrapper = std::add_pointer<void(const libvlc_event_t*, void*)>::type;
        virtual ~EventHandlerBase() = default;
        /**
         * @brief unregister Unregister this event handler.
         *
         * Calling this method makes the instance invalid.
         */
        virtual void unregister() = 0;
    };

    template <typename Func>
    class EventHandler : public EventHandlerBase
    {
    public:
        // We have to re-declare a template type, otherwise userCallback wouldn't be a forwarding reference
        // Not doing so works since EventHandler is instantiated with a perfect forwarded type, so Func type
        // will be a reference-less type if the callback was an lvalue, and an lvalue if the callback was an lvalue
        // As described here: http://stackoverflow.com/questions/24497311/is-this-a-universal-reference-does-stdforward-make-sense-here
        // this would still make sense, in a perverted kind of way.
        template <typename FuncTpl>
        EventHandler(EventManager& em, libvlc_event_e eventType, FuncTpl&& userCallback, Wrapper wrapper)
            : m_userCallback( std::forward<Func>(userCallback) )
            , m_eventManager(&em)
            , m_wrapper(wrapper)
            , m_eventType( eventType )
        {
            static_assert(std::is_same<typename std::decay<Func>::type,
                                        typename std::decay<FuncTpl>::type>::value, "");
            if (libvlc_event_attach( *m_eventManager, m_eventType, m_wrapper, &m_userCallback ) != 0)
                throw std::bad_alloc();
        }

        ~EventHandler()
        {
            // We unregister only when the object actually goes out of scope, IE. when it is
            // removed from EventManager's event handler vector
            libvlc_event_detach( *m_eventManager, m_eventType, m_wrapper, &m_userCallback );
        }

        virtual void unregister() override
        {
            m_eventManager->unregister(this);
        }

        EventHandler(const EventHandler&) = delete;
        EventHandler& operator=( const EventHandler& ) = delete;

    private:
        // Deduced type is Func& in case of lvalue; Func in case of rvalue.
        // We decay the type to ensure we either copy or take ownership.
        // Taking a reference would quite likely lead to unexpected behavior
        typename std::decay<Func>::type m_userCallback;
        // EventManager always outlive EventHandler, no need for smart pointer
        EventManager* m_eventManager;
        Wrapper m_wrapper;
        libvlc_event_e m_eventType;
    };

private:
    // variadic template recursion termination
    void unregister(){}

public:
    template <typename T, typename... Args>
    void unregister(const T e, const Args... args)
    {
        static_assert(std::is_convertible<decltype(e), const EventHandlerBase*>::value, "Expected const RegisteredEvent");

        {
            auto it = std::find_if(begin(m_lambdas), end(m_lambdas), [&e](decltype(m_lambdas)::value_type &value) {
                return e == value.get();
            });
            if (it != end(m_lambdas))
                m_lambdas.erase( it );
        }

        unregister(args...);
    }

protected:
    EventManager(InternalPtr ptr)
        : Internal{ ptr, [](InternalPtr){ /* No-op; EventManager's are handled by their respective objects */ } }
    {
    }

public:
    /**
     * @brief EventManager Wraps the same EventManager
     *
     * This will copy the underlying libvlc_event_manager_t, but will not copy
     * the already registered events.
     * This allows you to copy an event manager, register a few events for a
     * punctual task, and unregister those events by destroying the new instance.
     */
    EventManager(const EventManager& em)
        : Internal( em )
    {
        // Don't rely on the default implementation, as we don't want to copy the
        // current list of events.
    }

    /**
     * @brief operator= Wraps the same EventManager
     *
     * This will copy the underlying libvlc_event_manager_t, but will not copy
     * the already registered events.
     * This allows you to copy an event manager, register a few events for a
     * punctual task, and unregister those events by destroying the new instance.
     */
    EventManager& operator=(const EventManager& em)
    {
        // See above notes, this isn't the same as the default assignment operator
        if (this == &em)
            return *this;
        Internal::operator=(em);
        return *this;
    }

#if !defined(_MSC_VER) || _MSC_VER >= 1900
    EventManager(EventManager&&) = default;
    EventManager& operator=(EventManager&&) = default;
#else
    EventManager(EventManager&& em)
        : Internal( std::move( em ) )
        , m_lambdas(std::move( em.m_lambdas ) )
    {
    }

    EventManager& operator=(EventManager&& em)
    {
        if ( this == &em )
            return *this;
        Internal::operator=( std::move( em ) );
        m_lambdas = std::move( em.m_lambdas );
    }
#endif

    using RegisteredEvent = EventHandlerBase*;

protected:

    /**
     * @brief handle        Provides the common behavior for all event handlers
     * @param eventType     The libvlc type of event
     * @param f             The user provided std::function. This has to either match the expected parameter list
     *                      exactly, or it can take no arguments.
     * @param wrapper       Our implementation defined wrapper around the user's callback. It is expected to be able
     *                      able to decay to a regular C-style function pointer. It is currently implemented as a
     *                      captureless lambda (§5.1.2)
     * @return              A pointer to an abstract EventHandler type. It is assumed that the EventManager will
     *                      outlive this reference. When EventManager::~EventManager is called, it will destroy all
     *                      the registered event handler, this making this reference a dangling reference, which is
     *                      undefined behavior.
     *                      When calling unregister() on this object, the pointer should immediatly be considered invalid.
     */
    template <typename Func>
    RegisteredEvent handle(libvlc_event_e eventType, Func&& f, EventHandlerBase::Wrapper wrapper)
    {
        auto ptr = std::unique_ptr<EventHandlerBase>( new EventHandler<Func>(
                                *this, eventType, std::forward<Func>( f ), wrapper ) );
        auto raw = ptr.get();
        m_lambdas.push_back( std::move( ptr ) );
        return raw;
    }

    template <typename Func>
    RegisteredEvent handle(libvlc_event_e eventType, Func&& f)
    {
        EXPECT_SIGNATURE(void());
        return handle(eventType, std::forward<Func>( f ), [](const libvlc_event_t*, void* data)
        {
            auto callback = static_cast<DecayPtr<Func>>( data );
            (*callback)();
        });
    }

protected:
    // We store the EventHandlerBase's as unique_ptr in order for the function stored within
    // EventHandler<T> not to move to another memory location
    std::vector<std::unique_ptr<EventHandlerBase>> m_lambdas;
};

/**
 * @brief The MediaEventManager class allows one to register Media related events
 */
class MediaEventManager : public EventManager
{
    public:
        MediaEventManager(InternalPtr ptr) : EventManager( ptr ) {}

        /**
         * @brief onMetaChanged Registers an event called when a Media meta changes
         * @param f A std::function<void(libvlc_meta_t)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onMetaChanged( Func&& f)
        {
            EXPECT_SIGNATURE(void(libvlc_meta_t));
            return handle( libvlc_MediaMetaChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>(data);
                (*callback)( e->u.media_meta_changed.meta_type );
            });
        }

        /**
         * @brief onSubItemAdded Registers an event called when a Media gets a subitem added
         * @param f A std::function<void(MediaPtr)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onSubItemAdded( Func&& f )
        {
            EXPECT_SIGNATURE(void(MediaPtr));
            return handle(libvlc_MediaSubItemAdded, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>(data);
                auto media = e->u.media_subitem_added.new_child;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr );
            });
        }

        /**
         * \brief onDurationChanged Registers an event called when a media duration changes
         * \param f A std::function<void(int64_t)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onDurationChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(int64_t));
            return handle(libvlc_MediaDurationChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>(data);
                (*callback)( e->u.media_duration_changed.new_duration );
            });
        }

        /**
         * \brief onParsedChanged Registers an event called when the preparsed state changes
         * \param f A std::function<void(bool)> (or an equivalent Callable type)
         *          The provided boolean will be true if the media has been parsed, false otherwise.
         */
        template <typename Func>
        RegisteredEvent onParsedChanged( Func&& f )
        {
#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
            EXPECT_SIGNATURE( void(Media::ParsedStatus) );
            return handle(libvlc_MediaParsedChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>(data);
                (*callback)( static_cast<Media::ParsedStatus>( e->u.media_parsed_changed.new_status ) );
            });
#else
            EXPECT_SIGNATURE( void(bool) );
            return handle(libvlc_MediaParsedChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>(data);
                (*callback)( bool( e->u.media_parsed_changed.new_status ) );
            });
#endif
        }

        /**
         * \brief onFreed Registers an event called when the media reaches a refcount of 0
         * \param f A std::function<void(MediaPtr)> (or an equivalent Callable type)
         *          The media is being destroyed by libvlc when this event gets called.
         *          Any Media instance that would live past this call would wrap
         *          a dangling pointer.
         *
         */
        template <typename Func>
        RegisteredEvent onFreed( Func&& f)
        {
            //FIXME: Provide a read-only Media wrapper, to avoid wild dangling references to appear.
            EXPECT_SIGNATURE(void(MediaPtr));
            return handle(libvlc_MediaFreed, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>(data);
                auto media = e->u.media_freed.md;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr );
            });
        }

        /**
         * \brief onStateChanged Registers an event called when the Media state changes
         * \param f A std::function<void(libvlc_state_t)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onStateChanged( Func&& f)
        {
            //FIXME: Wrap libvlc_state_t in a class enum
            EXPECT_SIGNATURE(void(libvlc_state_t));
            return handle(libvlc_MediaStateChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>(data);
                (*callback)( e->u.media_state_changed.new_state );
            });
        }

        /**
         * \brief onSubItemTreeAdded Registers an event called when all subitem have been added.
         * \param f A std::function<void(MediaPtr)> (or an equivalent Callable type)
         *          The provided Media is the media for which this event has been registered,
         *          not a potential child media
         */
        template <typename Func>
        RegisteredEvent onSubItemTreeAdded( Func&& f)
        {
            EXPECT_SIGNATURE(void(MediaPtr));
            return handle(libvlc_MediaSubItemTreeAdded, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>(data);
                auto media = e->u.media_subitemtree_added.item;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr );
            });
        }
};

/**
 * @brief The MediaPlayerEventManager class allows one to register MediaPlayer related events
 */
class MediaPlayerEventManager : public EventManager
{
    public:
        MediaPlayerEventManager(InternalPtr ptr) : EventManager( ptr ) {}

        /**
         * \brief onMediaChanged Registers an event called when the played media changes
         * \param f A std::function<void(MediaPtr)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onMediaChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(MediaPtr));
            return handle(libvlc_MediaPlayerMediaChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                auto media = e->u.media_player_media_changed.new_media;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr );
            });
        }

        template <typename Func>
        RegisteredEvent onNothingSpecial( Func&& f )
        {
            return handle(libvlc_MediaPlayerNothingSpecial, std::forward<Func>( f ));
        }

        /**
         * \brief onOpening Registers an event called when the MediaPlayer starts initializing
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onOpening( Func&& f )
        {
            return handle(libvlc_MediaPlayerOpening, std::forward<Func>( f ) );
        }

        /**
         * \brief onBuffering Registers an event called when the buffering state changes
         * \param f A std::function<void(float)> (or an equivalent Callable type)
         *          The provided float is the current media buffering percentage
         */
        template <typename Func>
        RegisteredEvent onBuffering( Func&& f )
        {
            EXPECT_SIGNATURE(void(float));
            return handle(libvlc_MediaPlayerBuffering, std::forward<Func>(f), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_buffering.new_cache );
            });
        }

        /**
         * \brief onPlaying Registers an event called when the media player reaches playing state
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onPlaying( Func&& f )
        {
            return handle( libvlc_MediaPlayerPlaying, std::forward<Func>( f ) );
        }

        /**
         * \brief onPaused Registers an event called when the media player gets paused
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onPaused(Func&& f)
        {
            return handle( libvlc_MediaPlayerPaused, std::forward<Func>( f ) );
        }

        /**
         * \brief onStopped Registers an event called when the media player gets stopped
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onStopped(Func&& f)
        {
            return handle( libvlc_MediaPlayerStopped, std::forward<Func>( f ) );
        }

        // This never gets invoked
        template <typename Func>
        RegisteredEvent onForward(Func&& f)
        {
            return handle( libvlc_MediaPlayerForward, std::forward<Func>( f ) );
        }

        // This never gets invoked.
        template <typename Func>
        RegisteredEvent onBackward(Func&& f)
        {
            return handle( libvlc_MediaPlayerBackward, std::forward<Func>( f ) );
        }

        /**
         * \brief onEndReached Registers an event called when the media player reaches the end of a media
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onEndReached(Func&& f)
        {
            return handle( libvlc_MediaPlayerEndReached, std::forward<Func>( f ) );
        }

        /**
         * \brief onEncounteredError Registers an event called when the media player reaches encounters an error
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onEncounteredError(Func&& f)
        {
            return handle( libvlc_MediaPlayerEncounteredError, std::forward<Func>( f ) );
        }

        /**
         * \brief onTimeChanged Registers an event called periodically to indicate the current playback time
         * \param f A std::function<void(int64_t)> (or an equivalent Callable type)
         *          Time is in ms.
         */
        template <typename Func>
        RegisteredEvent onTimeChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(libvlc_time_t));
            return handle( libvlc_MediaPlayerTimeChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_time_changed.new_time );
            });
        }

        /**
         * \brief onPositionChanged Registers an event called periodically to indicate the current playback position.
         * \param f A std::function<void(float)> (or an equivalent Callable type)
         *          Provided float in a number between 0 (beginning of the media) and 1 (end of the media)
         */
        template <typename Func>
        RegisteredEvent onPositionChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(float));
            return handle( libvlc_MediaPlayerPositionChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_position_changed.new_position );
            });
        }

        /**
         * \brief onSeekableChanged Registers an event called when the seekable state changes
         * \param f A std::function<void(bool)> (or an equivalent Callable type)
         *          The provided boolean will be true if the media is seekable, false otherwise.
         */
        template <typename Func>
        RegisteredEvent onSeekableChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(bool));
            return handle( libvlc_MediaPlayerSeekableChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_seekable_changed.new_seekable != 0 );
            });
        }

        /**
         * \brief onPausableChanged Registers an event called when the pausable state changes
         * \param f A std::function<void(bool)> (or an equivalent Callable type)
         *          The provided boolean will be true if the playback can be paused, false otherwise.
         */
        template <typename Func>
        RegisteredEvent onPausableChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(bool));
            return handle( libvlc_MediaPlayerSeekableChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_seekable_changed.new_seekable != 0 );
            });
        }

        /**
         * \brief onTitleChanged Registers an event called when the current title changes
         * \param f A std::function<void(int)> (or an equivalent Callable type)
         *          The provided integer is the new current title identifier.
         *          Please note that this has nothing to do with a text title (the name of
         *          the video being played, for instance)
         */
        template <typename Func>
        RegisteredEvent onTitleChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(int));
            return handle( libvlc_MediaPlayerTitleChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_title_changed.new_title );
            });
        }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
        /**
         * \brief onChapterChanged Registers an event called when the current chapter changes
         * \param f A std::function<void(int)> (or an equivalent Callable type)
         *          The provided integer is the new current chapter identifier.
         *          Please note that this has nothing to do with a text chapter (the name of
         *          the video being played, for instance)
         */
        template <typename Func>
        RegisteredEvent onChapterChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(int));
            return handle( libvlc_MediaPlayerChapterChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_chapter_changed.new_chapter );
            });
        }
#endif

        /**
         * \brief onSnapshotTaken Registers an event called when a snapshot gets taken
         * \param f A std::function<void(std::string)> (or an equivalent Callable type)
         *          The provided string is the path of the created snapshot
         */
        template <typename Func>
        RegisteredEvent onSnapshotTaken( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string));
            return handle( libvlc_MediaPlayerSnapshotTaken, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_snapshot_taken.psz_filename );
            });
        }

        /**
         * \brief onLengthChanged Registers an event called when the length gets updated.
         * \param f A std::function<void(int64_t)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onLengthChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(libvlc_time_t));
            return handle( libvlc_MediaPlayerLengthChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_length_changed.new_length );
            });
        }

        /**
         * \brief onVout Registers an event called when the number of vout changes
         * \param f A std::function<void(int)> (or an equivalent Callable type)
         *          The provided int represents the number of currently available vout.
         */
        template <typename Func>
        RegisteredEvent onVout( Func&& f )
        {
            EXPECT_SIGNATURE(void(int));
            return handle( libvlc_MediaPlayerVout, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_vout.new_count );
            });
        }

        /**
         * \brief onScrambledChanged Registers an event called when the scrambled state changes
         * \param f A std::function<void(bool)> (or an equivalent Callable type)
         *          The provided boolean will be true if the media is scrambled, false otherwise.
         */
        template <typename Func>
        RegisteredEvent onScrambledChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(bool));
            return handle( libvlc_MediaPlayerScrambledChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_scrambled_changed.new_scrambled != 0 );
            });
        }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
        /**
         * \brief onESAdded Registers an event called when an elementary stream get added
         * \param f A std::function<void(libvlc_track_type_t, int)> (or an equivalent Callable type)
         *          libvlc_track_type_t: The new track type
         *          int: the new track index
         */
        template <typename Func>
        RegisteredEvent onESAdded( Func&& f )
        {
            //FIXME: Expose libvlc_track_type_t as an enum class
            EXPECT_SIGNATURE(void(libvlc_track_type_t, int));
            return handle( libvlc_MediaPlayerESAdded, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_es_changed.i_type, e->u.media_player_es_changed.i_id );
            });
        }

        /**
         * \brief onESDeleted Registers an event called when an elementary stream get deleted
         * \param f A std::function<void(libvlc_track_type_t, int)> (or an equivalent Callable type)
         *          libvlc_track_type_t: The track type
         *          int: the track index
         */
        template <typename Func>
        RegisteredEvent onESDeleted( Func&& f )
        {
            EXPECT_SIGNATURE(void(libvlc_track_type_t, int));
            return handle( libvlc_MediaPlayerESDeleted, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_es_changed.i_type, e->u.media_player_es_changed.i_id );
            });
        }

        /**
         * \brief onESSelected Registers an event called when an elementary stream get selected
         * \param f A std::function<void(libvlc_track_type_t, int)> (or an equivalent Callable type)
         *          libvlc_track_type_t: The track type
         *          int: the track index
         */
        template <typename Func>
        RegisteredEvent onESSelected( Func&& f )
        {
            EXPECT_SIGNATURE(void(libvlc_track_type_t, int));
            return handle( libvlc_MediaPlayerESSelected, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_es_changed.i_type, e->u.media_player_es_changed.i_id );
            });
        }

        /**
         * \brief onAudioDevice Registers an event called when the current audio output device changes
         * \param f A std::function<void(std::string)> (or an equivalent Callable type)
         *          The provided string is the new current audio device.
         */
        template <typename Func>
        RegisteredEvent onAudioDevice( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string));
            return handle( libvlc_MediaPlayerAudioDevice, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_audio_device.device );
            });
        }
#endif

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(2, 2, 2, 0)
        /**
         * \brief onCorked Registers an event called when the playback is paused automatically for a higher priority audio stream
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onCorked( Func&& f )
        {
            return handle( libvlc_MediaPlayerCorked, std::forward<Func>( f ) );
        }

        /**
         * \brief onUncorked Registers an event called when the playback is unpaused automatically after a higher priority audio stream ends
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onUncorked( Func&& f )
        {
            return handle( libvlc_MediaPlayerUncorked, std::forward<Func>( f ) );
        }

        /**
         * \brief onMuted Registers an event called when the audio is muted
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onMuted( Func&& f )
        {
            return handle( libvlc_MediaPlayerMuted, std::forward<Func>( f ) );
        }

        /**
         * \brief onUnmuted Registers an event called when the audio is unmuted
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onUnmuted( Func&& f )
        {
            return handle( libvlc_MediaPlayerUnmuted, std::forward<Func>( f ) );
        }

        /**
         * \brief onAudioVolume Registers an event called when the current audio volume changes
         * \param f A std::function<void(float)> (or an equivalent Callable type)
         *          The provided float is the new current audio volume percentage.
         */
        template <typename Func>
        RegisteredEvent onAudioVolume( Func&& f )
        {
            EXPECT_SIGNATURE(void(float));
            return handle( libvlc_MediaPlayerAudioVolume, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.media_player_audio_volume.volume );
            });
        }
#endif
};

/**
 * @brief The MediaListEventManager class allows one to register MediaList related events
 */
class MediaListEventManager : public EventManager
{
    public:
        MediaListEventManager(InternalPtr ptr) : EventManager( ptr ) {}

        /**
         * \brief onItemAdded Registers an event called when an item gets added to the media list
         * \param f A std::function<void(MediaPtr, int)> (or an equivalent Callable type)
         *          MediaPtr: The added media
         *          int: The media index in the list
         */
        template <typename Func>
        RegisteredEvent onItemAdded( Func&& f )
        {
            EXPECT_SIGNATURE(void(MediaPtr, int));
            return handle(libvlc_MediaListItemAdded, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                auto media = e->u.media_list_item_added.item;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr,
                             e->u.media_list_item_added.index );
            });
        }

        /**
         * \brief onWillAddItem Registers an event called when an item is about to be added to the media list
         * \param f A std::function<void(MediaPtr, int)> (or an equivalent Callable type)
         *          MediaPtr: The added media
         *          int: The media index in the list
         */
        template <typename Func>
        RegisteredEvent onWillAddItem( Func&& f )
        {
            EXPECT_SIGNATURE(void(MediaPtr, int));
            return handle( libvlc_MediaListWillAddItem, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                auto media = e->u.media_list_will_add_item.item;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr,
                            e->u.media_list_will_add_item.index );
            });
        }

        /**
         * \brief onItemDeleted Registers an event called when an item gets deleted to the media list
         * \param f A std::function<void(MediaPtr, int)> (or an equivalent Callable type)
         *          MediaPtr: The added media
         *          int: The media index in the list
         */
        template <typename Func>
        RegisteredEvent onItemDeleted( Func&& f )
        {
            EXPECT_SIGNATURE(void(MediaPtr, int));
            return handle(libvlc_MediaListItemDeleted, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                auto media = e->u.media_list_item_deleted.item;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr,
                             e->u.media_list_item_deleted.index );
            });
        }

        /**
         * \brief onWillDeleteItem Registers an event called when an item is about to be deleted
         * \param f A std::function<void(MediaPtr, int)> (or an equivalent Callable type)
         *          MediaPtr: The added media
         *          int: The media index in the list
         */
        template <typename Func>
        RegisteredEvent onWillDeleteItem( Func&& f )
        {
            EXPECT_SIGNATURE(void(MediaPtr, int));
            return handle(libvlc_MediaListWillDeleteItem, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                auto media = e->u.media_list_will_delete_item.item;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr,
                             e->u.media_list_will_delete_item.index );
            });
        }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
        template <typename Func>
        RegisteredEvent onEndReached( Func&& f )
        {
            return handle( libvlc_MediaListEndReached, std::forward<Func>( f ) );
        }
#endif
};

/**
 * @brief The MediaListPlayerEventManager class
 * Those events aren't sent by VLC so far.
 */
class MediaListPlayerEventManager : public EventManager
{
    public:
        MediaListPlayerEventManager(InternalPtr ptr) : EventManager( ptr ) {}

        template <typename Func>
        RegisteredEvent onPlayed(Func&& f)
        {
            return handle(libvlc_MediaListPlayerPlayed, std::forward<Func>( f ) );
        }

        template <typename Func>
        RegisteredEvent onNextItemSet( Func&& f )
        {
            EXPECT_SIGNATURE(void(MediaPtr));
            return handle(libvlc_MediaListPlayerNextItemSet, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                auto media = e->u.media_list_player_next_item_set.item;
                (*callback)( media != nullptr ? std::make_shared<Media>( media, true ) : nullptr );
            });
        }

        template <typename Func>
        RegisteredEvent onStopped( Func&& f )
        {
            return handle(libvlc_MediaListPlayerStopped, std::forward<Func>( f ) );
        }
};

#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
/**
 * @brief The MediaDiscovererEventManager class allows one to register MediaDiscoverer related events
 */
class MediaDiscovererEventManager : public EventManager
{
    public:
        MediaDiscovererEventManager(InternalPtr ptr) : EventManager( ptr ) {}

        /**
         * \brief onStarted Registers an event called when the discovery starts
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onStarted(Func&& f)
        {
            return handle(libvlc_MediaDiscovererStarted, std::forward<Func>( f ) );
        }

        /**
         * \brief onStopped Registers an event called when the discovery stops
         * \param f A std::function<void(void)> (or an equivalent Callable type)
         */
        template <typename Func>
        RegisteredEvent onStopped(Func&& f)
        {
            return handle(libvlc_MediaDiscovererEnded, std::forward<Func>( f ) );
        }
};
#endif

/**
 * @brief The VLMEventManager class allows one to register VLM related events
 */
class VLMEventManager : public EventManager
{
    public:
        VLMEventManager(InternalPtr ptr) : EventManager(ptr) {}

        /**
         * \brief onMediaAdded Registers an event called when a media gets added
         * \param f A std::function<void(std::string)> (or an equivalent Callable type)
         *          The given string is the name of the added media
         */
        template <typename Func>
        RegisteredEvent onMediaAdded( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string));
            return handle(libvlc_VlmMediaAdded, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "" );
            });
        }

        /**
         * \brief onMediaRemoved Registers an event called when a media gets removed
         * \param f A std::function<void(std::string)> (or an equivalent Callable type)
         *          The given string is the name of the removed media
         */
        template <typename Func>
        RegisteredEvent onMediaRemoved( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string));
            return handle(libvlc_VlmMediaRemoved, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "" );
            });
        }

        /**
         * \brief onMediaChanged Registers an event called when a media changes
         * \param f A std::function<void(std::string)> (or an equivalent Callable type)
         *          The given string is the name of the changed media
         */
        template <typename Func>
        RegisteredEvent onMediaChanged( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string));
            return handle(libvlc_VlmMediaChanged, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "" );
            });
        }

        /**
         * \brief onMediaInstanceStarted Registers an event called when a media instance starts
         * \param f A std::function<void(std::string, std::string)> (or an equivalent Callable type)
         *          Parameters are:
         *              - The media name
         *              - The instance name
         */
        template <typename Func>
        RegisteredEvent onMediaInstanceStarted( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string, std::string));
            return handle(libvlc_VlmMediaInstanceStarted, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "",
                             e->u.vlm_media_event.psz_instance_name ? e->u.vlm_media_event.psz_instance_name : "" );
            });
        }

        /**
         * \brief onMediaInstanceStopped Registers an event called when a media instance stops
         * \param f A std::function<void(std::string, std::string)> (or an equivalent Callable type)
         *          Parameters are:
         *              - The media name
         *              - The instance name
         */
        template <typename Func>
        RegisteredEvent onMediaInstanceStopped( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string, std::string));
            return handle(libvlc_VlmMediaInstanceStopped, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "",
                             e->u.vlm_media_event.psz_instance_name ? e->u.vlm_media_event.psz_instance_name : "" );
            });
        }

        /**
         * \brief onMediaInstanceStatusInit Registers an event called when a media instance is initializing
         * \param f A std::function<void(std::string, std::string)> (or an equivalent Callable type)
         *          Parameters are:
         *              - The media name
         *              - The instance name
         */
        template <typename Func>
        RegisteredEvent onMediaInstanceStatusInit( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string, std::string));
            return handle(libvlc_VlmMediaInstanceStatusInit, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "",
                             e->u.vlm_media_event.psz_instance_name ? e->u.vlm_media_event.psz_instance_name : "" );
            });
        }

        /**
         * \brief onMediaInstanceStatusOpening Registers an event called when a media instance is opening
         * \param f A std::function<void(std::string, std::string)> (or an equivalent Callable type)
         *          Parameters are:
         *              - The media name
         *              - The instance name
         */
        template <typename Func>
        RegisteredEvent onMediaInstanceStatusOpening( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string, std::string));
            return handle(libvlc_VlmMediaInstanceStatusOpening, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "",
                             e->u.vlm_media_event.psz_instance_name ? e->u.vlm_media_event.psz_instance_name : "" );
            });
        }

        /**
         * \brief onMediaInstanceStatusPlaying Registers an event called when a media instance reaches playing state
         * \param f A std::function<void(std::string, std::string)> (or an equivalent Callable type)
         *          Parameters are:
         *              - The media name
         *              - The instance name
         */
        template <typename Func>
        RegisteredEvent onMediaInstanceStatusPlaying( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string, std::string));
            return handle(libvlc_VlmMediaInstanceStatusPlaying, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "",
                             e->u.vlm_media_event.psz_instance_name ? e->u.vlm_media_event.psz_instance_name : "" );
            });
        }

        /**
         * \brief onMediaInstanceStatusPause Registers an event called when a media instance gets paused
         * \param f A std::function<void(std::string, std::string)> (or an equivalent Callable type)
         *          Parameters are:
         *              - The media name
         *              - The instance name
         */
        template <typename Func>
        RegisteredEvent onMediaInstanceStatusPause( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string, std::string));
            return handle(libvlc_VlmMediaInstanceStatusPause, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "",
                             e->u.vlm_media_event.psz_instance_name ? e->u.vlm_media_event.psz_instance_name : "" );
            });
        }

        /**
         * \brief onMediaInstanceStatusEnd Registers an event called when a media instance ends
         * \param f A std::function<void(std::string, std::string)> (or an equivalent Callable type)
         *          Parameters are:
         *              - The media name
         *              - The instance name
         */
        template <typename Func>
        RegisteredEvent onMediaInstanceStatusEnd( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string, std::string));
            return handle(libvlc_VlmMediaInstanceStatusEnd, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "",
                             e->u.vlm_media_event.psz_instance_name ? e->u.vlm_media_event.psz_instance_name : "" );
            });
        }

        /**
         * \brief onMediaInstanceStatusError Registers an event called when a media instance encouters an error
         * \param f A std::function<void(std::string, std::string)> (or an equivalent Callable type)
         *          Parameters are:
         *              - The media name
         *              - The instance name
         */
        template <typename Func>
        RegisteredEvent onMediaInstanceStatusError( Func&& f )
        {
            EXPECT_SIGNATURE(void(std::string, std::string));
            return handle(libvlc_VlmMediaInstanceStatusError, std::forward<Func>( f ), [](const libvlc_event_t* e, void* data)
            {
                auto callback = static_cast<DecayPtr<Func>>( data );
                (*callback)( e->u.vlm_media_event.psz_media_name ? e->u.vlm_media_event.psz_media_name : "",
                             e->u.vlm_media_event.psz_instance_name ? e->u.vlm_media_event.psz_instance_name : "" );
            });
        }
};
}

#endif // LIBVLC_EVENTMANAGER_HPP
