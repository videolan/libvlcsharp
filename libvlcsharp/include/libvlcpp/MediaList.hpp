/*****************************************************************************
 * MediaList.hpp: MediaList API
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

#ifndef LIBVLC_CXX_MEDIALIST_H
#define LIBVLC_CXX_MEDIALIST_H

#include "common.hpp"

#include <mutex>

namespace VLC
{

class Media;
class MediaListEventManager;
class MediaDiscoverer;
class MediaLibrary;

class MediaList : public Internal<libvlc_media_list_t>
{
public:
    ///
    /// @brief A convenience RAII type to handle MediaList's lock
    ///
    using Lock = std::lock_guard<MediaList>;

    /**
     * Check if 2 MediaList objects contain the same libvlc_media_list_t.
     * \param another another MediaList
     * \return true if they contain the same libvlc_media_list_t
     */
    bool operator==(const MediaList& another) const
    {
        return m_obj == another.m_obj;
    }

    /**
     * Get subitems of media descriptor object.
     */
    MediaList(Media& md)
        : Internal{ libvlc_media_subitems( getInternalPtr<libvlc_media_t>( md ) ), libvlc_media_list_release }
    {
    }

    /**
     * Get media service discover media list.
     *
     * \param p_mdis  media service discover object
     */
    MediaList(MediaDiscoverer& mdis)
        : Internal{ libvlc_media_discoverer_media_list( getInternalPtr<libvlc_media_discoverer_t>( mdis ) ),
                    libvlc_media_list_release }
    {
    }

    /**
     * Get media library subitems.
     *
     * \param p_mlib  media library object
     */
    MediaList(MediaLibrary& mlib )
        : Internal{ libvlc_media_library_media_list( getInternalPtr<libvlc_media_library_t>( mlib ) ), libvlc_media_list_release }
    {
    }


    /**
     * Create an empty media list.
     *
     * \param p_instance  libvlc instance
     */
    MediaList(Instance& instance)
        : Internal{ libvlc_media_list_new( getInternalPtr<libvlc_instance_t>( instance ) ),
                                           libvlc_media_list_release }
    {
    }

    MediaList( Internal::InternalPtr mediaList )
        : Internal{ mediaList, libvlc_media_list_release }
    {
    }

    /**
     * Create an empty VLC MediaList instance.
     *
     * Calling any method on such an instance is undefined.
    */
    MediaList() = default;

    /**
     * Associate media instance with this media list instance. If another
     * media instance was present it will be released. The
     * MediaList lock should NOT be held upon entering this function.
     *
     * \param p_md  media instance to add
     */
    void setMedia(Media& md)
    {
        libvlc_media_list_set_media( *this, getInternalPtr<libvlc_media_t>( md ) );
    }

    /**
     * Add media instance to media list The MediaList lock should be
     * held upon entering this function.
     *
     * \param p_md  a media instance
     */
    bool addMedia(Media& md)
    {
        return libvlc_media_list_add_media( *this, getInternalPtr<libvlc_media_t>( md ) ) == 0;
    }

    /**
     * Insert media instance in media list on a position The
     * MediaList lock should be held upon entering this function.
     *
     * \param p_md  a media instance
     *
     * \param i_pos  position in array where to insert
     */
    bool insertMedia(Media& md, int pos)
    {
        return libvlc_media_list_insert_media( *this, getInternalPtr<libvlc_media_t>( md ), pos ) == 0;
    }

    /**
     * Remove media instance from media list on a position The
     * MediaList lock should be held upon entering this function.
     *
     * \param i_pos  position in array where to insert
     */
    bool removeIndex(int i_pos)
    {
        return libvlc_media_list_remove_index( *this, i_pos ) == 0;
    }

    /**
     * Get count on media list items The MediaList lock should be
     * held upon entering this function.
     *
     * \return number of items in media list
     */
    int count()
    {
        return libvlc_media_list_count( *this );
    }

    /**
     * List media instance in media list at a position The
     * MediaList lock should be held upon entering this function.
     *
     * \param i_pos  position in array where to insert
     *
     * \return media instance at position i_pos, or nullptr if not found.
     */
    MediaPtr itemAtIndex(int i_pos)
    {
        auto ptr = libvlc_media_list_item_at_index(*this,i_pos);
        if ( ptr == nullptr )
            return nullptr;
        return std::make_shared<Media>( ptr, false );
    }

    /**
     * Find index position of List media instance in media list. Warning: the
     * function will return the first matched position. The
     * MediaList lock should be held upon entering this function.
     *
     * \param p_md  media instance
     *
     * \return position of media instance or -1 if media not found
     */
    int indexOfItem(Media& md)
    {
        return libvlc_media_list_index_of_item( *this, getInternalPtr<libvlc_media_t>( md ) );
    }

    /**
     * This indicates if this media list is read-only from a user point of
     * view
     *
     * \return true if readonly, false otherwise
     */
    bool isReadonly()
    {
        return libvlc_media_list_is_readonly(*this) == 1;
    }

    /**
     * Get lock on media list items
     */
    void lock()
    {
        libvlc_media_list_lock( *this );
    }

    /**
     * Release lock on media list items The MediaList lock should be
     * held upon entering this function.
     */
    void unlock()
    {
        libvlc_media_list_unlock( *this );
    }

    /**
     * Get libvlc_event_manager from this media list instance. The
     * p_event_manager is immutable, so you don't have to hold the lock
     *
     * \return libvlc_event_manager
     */
    MediaListEventManager& eventManager()
    {
        if ( m_eventManager == nullptr )
        {
            libvlc_event_manager_t* obj = libvlc_media_list_event_manager( *this );
            m_eventManager = std::make_shared<MediaListEventManager>( obj );
        }
        return *m_eventManager;
    }

private:
    std::shared_ptr<MediaListEventManager> m_eventManager;
};

} // namespace VLC

#endif

