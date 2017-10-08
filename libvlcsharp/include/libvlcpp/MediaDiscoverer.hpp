/*****************************************************************************
 * MediaDiscoverer.hpp: MediaDiscoverer API
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

#ifndef LIBVLC_CXX_MEDIADISCOVERER_H
#define LIBVLC_CXX_MEDIADISCOVERER_H

#include "common.hpp"

#include <string>
#include <vlc/libvlc_version.h>

namespace VLC
{

class Instance;
class MediaList;
#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
class MediaDiscovererEventManager;
#endif

class MediaDiscoverer : public Internal<libvlc_media_discoverer_t>
{
public:
#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
    enum class Category
    {
        Devices = libvlc_media_discoverer_devices,
        Lan = libvlc_media_discoverer_lan,
        Podcasts = libvlc_media_discoverer_podcasts,
        Localdirs = libvlc_media_discoverer_localdirs,
    };

    class Description
    {
    public:
        explicit Description( const std::string& name, const std::string& longName, libvlc_media_discoverer_category_t cat )
            : m_name( name )
            , m_longName( longName )
            , m_category( static_cast<Category>( cat ) )
        {
        }

        const std::string& name() const
        {
            return m_name;
        }

        const std::string& longName() const
        {
            return m_longName;
        }

        Category category() const
        {
            return m_category;
        }

    private:
        std::string m_name;
        std::string m_longName;
        Category m_category;
    };
#endif
    /**
     * Discover media service by name.
     *
     * \param p_inst  libvlc instance
     *
     * \param psz_name  service name
     */
    MediaDiscoverer(Instance& inst, const std::string& name)
#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
        : Internal{ libvlc_media_discoverer_new(getInternalPtr<libvlc_instance_t>( inst ), name.c_str()),
#else
        : Internal{ libvlc_media_discoverer_new_from_name(getInternalPtr<libvlc_instance_t>( inst ), name.c_str()),
#endif
                    libvlc_media_discoverer_release }
    {
    }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Start media discovery.
     *
     * To stop it, call MediaDiscover::stop() or
     * destroy the object directly.
     *
     * \see MediaDiscover::stop
     *
     * \return false in case of error, true otherwise
     */
    bool start()
    {
        return libvlc_media_discoverer_start( *this ) == 0;
    }

    /**
     * Stop media discovery.
     *
     * \see MediaDiscoverer::start()
     */
    void stop()
    {
        libvlc_media_discoverer_stop( *this );
    }
#endif

#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Get media service discover object its localized name.
     *
     * \return localized name
     */
    std::string localizedName()
    {
        auto str = wrapCStr( libvlc_media_discoverer_localized_name(*this) );
        if ( str == nullptr )
            return {};
        return str.get();
    }

    /**
     * Get event manager from media service discover object.
     *
     * \return event manager object.
     */
    MediaDiscovererEventManager& eventManager()
    {
        if ( m_eventManager == nullptr )
        {
            libvlc_event_manager_t* obj = libvlc_media_discoverer_event_manager( *this );
            m_eventManager = std::make_shared<MediaDiscovererEventManager>( obj );
        }
        return *m_eventManager;
    }
#endif

    /**
     * Query if media service discover object is running.
     *
     * \return true if running, false if not
     */
    bool isRunning()
    {
        return libvlc_media_discoverer_is_running(*this) != 0;
    }

    std::shared_ptr<MediaList> mediaList()
    {
        if ( m_mediaList == nullptr )
        {
            auto mlist = libvlc_media_discoverer_media_list( *this );
            if ( mlist == nullptr )
                return nullptr;
            m_mediaList = std::make_shared<MediaList>( mlist );
        }
        return m_mediaList;
    }

private:
#if LIBVLC_VERSION_INT < LIBVLC_VERSION(3, 0, 0, 0)
    std::shared_ptr<MediaDiscovererEventManager> m_eventManager;
#endif
    std::shared_ptr<MediaList> m_mediaList;
};

} // namespace VLC

#endif

