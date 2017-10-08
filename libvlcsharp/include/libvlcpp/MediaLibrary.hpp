/*****************************************************************************
 * MediaLibrary.hpp: MediaLibrary API
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

#ifndef LIBVLC_CXX_MEDIALIBRARY_H
#define LIBVLC_CXX_MEDIALIBRARY_H

#include "common.hpp"
#include "Internal.hpp"

namespace VLC
{

class MediaLibrary : public Internal<libvlc_media_library_t>
{
public:
    /**
     * Create an new Media Library object
     *
     * \param instance  the libvlc instance
     */
    MediaLibrary(Instance& instance)
        : Internal{ libvlc_media_library_new( getInternalPtr<libvlc_instance_t>( instance ) ),
                    libvlc_media_library_release }
    {
    }

    /**
     * Create an empty VLC MediaLibrary instance.
     *
     * Calling any method on such an instance is undefined.
    */
    MediaLibrary() = default;

    /**
     * Check if 2 MediaLibrary objects contain the same libvlc_media_library_t.
     * \param another another MediaLibrary
     * \return true if they contain the same libvlc_media_library_t
     */
    bool operator==(const MediaLibrary& another) const
    {
        return m_obj == another.m_obj;
    }

    /**
     * Load media library.
     */
    bool load()
    {
        return libvlc_media_library_load(*this) == 0;
    }
};

} // namespace VLC

#endif

