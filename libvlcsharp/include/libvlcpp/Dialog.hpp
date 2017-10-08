/*****************************************************************************
 * Dialog.hpp:  libvlcpp dialog API
 *****************************************************************************
 * Copyright © 2016 VLC authors and VideoLAN
 *
 * Authors:         Bastien Penavayre <bastienPenava@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston MA 02110-1301, USA.
 *****************************************************************************/

#ifndef LIBVLC_CXX_DIALOG_H
#define LIBVLC_CXX_DIALOG_H

#include <string>

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
namespace VLC
{
    ///
    /// \brief The Dialog class exposes libvlc_dialog_id functionalities
    ///
    class Dialog
    {
    private:
        libvlc_dialog_id *m_id;

        Dialog() = delete;
        Dialog(const Dialog &) = delete;
        Dialog &operator=(const Dialog &) = delete;

        template <size_t, typename ...>
        friend struct CallbackWrapper;

        /**
         * The standard constructor.
         *
         * Used only by CallbackWrapper when setting up the callbacks.
         * \param id   identifier for the current real dialog
         */
        Dialog(libvlc_dialog_id *id) : m_id(id) 
        {
            if (!m_id)
                throw std::runtime_error("The required id is NULL, Dialog inoperable");
        }

    public:
        /**
         * Move constructor, steals the id of the instance given in parameter
         * \param other rvalue reference instance of this class
         */
        Dialog(Dialog &&other) : m_id(other.m_id)
        {
            other.m_id = nullptr;
        }

        ~Dialog()
        {
            if (m_id)
                dismiss();
        }

        /**
         * Post a login answer.
         *
         * After this call, the instance won't be valid anymore
         *
         * \param username   valid non-empty string
         * \param password   valid string
         * \param store      if true stores the credentials
         * \return true if success, false otherwise
         */
        bool postLogin(const std::string &username, const std::string &password, bool store)
        {
            if (!m_id)
                throw std::runtime_error("Calling method on dismissed Dialog instance");
            bool ret =  libvlc_dialog_post_login(m_id, username.c_str(), password.c_str(), store) == 0;
            m_id = nullptr;
            return ret;
        }

        /**
         * Post a question answer.
         *
         * After this call, this instance won't be valid anymore
         *
         * \see QuestionCb
         * \param actionIndex 1 for action1, 2 for action2
         * \return true on success, false otherwise
         */
        bool postAction(int actionIndex)
        {
            if (!m_id)
                throw std::runtime_error("Calling method on dismissed Dialog instance");
            bool ret =  libvlc_dialog_post_action(m_id, actionIndex) == 0;
            m_id = nullptr;
            return ret;
        }

        /**
         * Dismiss a dialog.
         *
         * After this call, this instance won't be valid anymore
         *
         * \see CancelCb
         */
        bool dismiss()
        {
            if (!m_id)
                throw std::runtime_error("Calling method on dismissed Dialog instance");
            bool ret = libvlc_dialog_dismiss(m_id) == 0;
            m_id = nullptr;
            return ret;
        }
    };
} // namespace VLC
#endif

#endif /*LIBVLC_CXX_DIALOG_H*/
