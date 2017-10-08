/*****************************************************************************
 * Instance.hpp: Instance API
 *****************************************************************************
 * Copyright © 2015 libvlcpp authors & VideoLAN
 *
 * Authors: Alexey Sokolov <alexey+vlc@asokolov.org>
 *          Hugo Beauzée-Luyssen <hugo@beauzee.fr>
 *          Bastien Penavayre <bastienpenava@gmail.com>
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

#ifndef LIBVLC_CXX_INSTANCE_H
#define LIBVLC_CXX_INSTANCE_H

#include "common.hpp"
#include "Internal.hpp"
#include "structures.hpp"
#include "Dialog.hpp"
#include "MediaDiscoverer.hpp"

#include <string>
#include <vector>
#include <cstring>
#include <cstdio>

namespace VLC
{

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
using Question = libvlc_dialog_question_type;

namespace DialogType
{
#if !defined(_MSC_VER) || _MSC_VER >= 1900
    static constexpr Question normal = LIBVLC_DIALOG_QUESTION_NORMAL;
    static constexpr Question warning = LIBVLC_DIALOG_QUESTION_WARNING;
    static constexpr Question critical = LIBVLC_DIALOG_QUESTION_CRITICAL;
#else
    static const Question normal = LIBVLC_DIALOG_QUESTION_NORMAL;
    static const Question warning = LIBVLC_DIALOG_QUESTION_WARNING;
    static const Question critical = LIBVLC_DIALOG_QUESTION_CRITICAL;
#endif
}
#endif

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
class Instance : protected CallbackOwner<8>, public Internal<libvlc_instance_t>
#else
class Instance : protected CallbackOwner<5>, public Internal<libvlc_instance_t>
#endif
{
private:
    enum class CallbackIdx : unsigned int
    {
        Exit = 0,
        Log,
        ErrorDisplay,
#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
        LoginDisplay,
        QuestionDisplay,
        ProgressDisplay,
        CancelDialog,
        ProgressUpdate
#endif
    };

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
    std::shared_ptr<libvlc_dialog_cbs> m_callbacks_pointers;
#endif
public:
    /**
     * Create and initialize a libvlc instance. This functions accept a list
     * of "command line" arguments similar to the main(). These arguments
     * affect the LibVLC instance default configuration.
     *
     * \version Arguments are meant to be passed from the command line to
     * LibVLC, just like VLC media player does. The list of valid arguments
     * depends on the LibVLC version, the operating system and platform, and
     * set of available LibVLC plugins. Invalid or unsupported arguments will
     * cause the function to fail (i.e. return NULL). Also, some arguments
     * may alter the behaviour or otherwise interfere with other LibVLC
     * functions.
     *
     * \warning There is absolutely no warranty or promise of forward,
     * backward and cross-platform compatibility with regards to
     * Instance::Instance() arguments. We recommend that you do not use them,
     * other than when debugging.
     *
     * \param argc  the number of arguments (should be 0)
     *
     * \param argv  list of arguments (should be NULL)
     */
    Instance(int argc, const char *const * argv)
        : Internal{ libvlc_new( argc, argv ), libvlc_release }
#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
          , m_callbacks_pointers { std::make_shared<libvlc_dialog_cbs>() }
#endif
    {
    }

    /**
     * Create an empty VLC instance.
     *
     * Calling any method on such an instance is undefined.
    */
    Instance() = default;

    /**
     * Check if 2 Instance objects contain the same libvlc_instance_t.
     * \param another another Instance
     * \return true if they contain the same libvlc_instance_t
     */
    bool operator==(const Instance& another) const
    {
        return m_obj == another.m_obj;
    }


    /**
     * Try to start a user interface for the libvlc instance.
     *
     * \param name  interface name, or empty string for default
     */
    bool addIntf(const std::string& name)
    {
        return libvlc_add_intf( *this, name.length() > 0 ? name.c_str() : nullptr ) == 0;
    }

    /**
     * Registers a callback for the LibVLC exit event. This is mostly useful
     * if the VLC playlist and/or at least one interface are started with
     * libvlc_playlist_play() or Instance::addIntf() respectively. Typically,
     * this function will wake up your application main loop (from another
     * thread).
     *
     * \note This function should be called before the playlist or interface
     * are started. Otherwise, there is a small race condition: the exit
     * event could be raised before the handler is registered.
     *
     * \param cb  callback to invoke when LibVLC wants to exit, or nullptr to
     * disable the exit handler (as by default). It is expected to be a
     * std::function<void()>, or an equivalent Callable type
     */
    template <typename ExitCb>
    void setExitHandler(ExitCb&& exitCb)
    {
        static_assert(signature_match_or_nullptr<ExitCb, void()>::value, "Mismatched exit callback" );
        libvlc_set_exit_handler( *this,
            CallbackWrapper<(unsigned int)CallbackIdx::Exit, void(*)(void*)>::wrap( *m_callbacks, std::forward<ExitCb>( exitCb ) ),
            m_callbacks.get() );
    }

    /**
     * Sets the application name. LibVLC passes this as the user agent string
     * when a protocol requires it.
     *
     * \param name  human-readable application name, e.g. "FooBar player
     * 1.2.3"
     *
     * \param http  HTTP User Agent, e.g. "FooBar/1.2.3 Python/2.6.0"
     *
     * \version LibVLC 1.1.1 or later
     */
    void setUserAgent(const std::string& name, const std::string& http)
    {
        libvlc_set_user_agent( *this, name.c_str(), http.c_str() );
    }

    /**
     * Sets some meta-information about the application. See also
     * Instance::setUserAgent() .
     *
     * \param id  Java-style application identifier, e.g. "com.acme.foobar"
     *
     * \param version  application version numbers, e.g. "1.2.3"
     *
     * \param icon  application icon name, e.g. "foobar"
     *
     * \version LibVLC 2.1.0 or later.
     */
    void setAppId(const std::string& id, const std::string& version, const std::string& icon)
    {
        libvlc_set_app_id( *this, id.c_str(), version.c_str(), icon.c_str() );
    }

    /**
     * Unsets the logging callback for a LibVLC instance. This is rarely
     * needed: the callback is implicitly unset when the instance is
     * destroyed. This function will wait for any pending callbacks
     * invocation to complete (causing a deadlock if called from within the
     * callback).
     *
     * \version LibVLC 2.1.0 or later
     */
    void logUnset()
    {
        libvlc_log_unset( *this );
    }

    /**
     * Sets the logging callback for a LibVLC instance. This function is
     * thread-safe: it will wait for any pending callbacks invocation to
     * complete.
     *
     * \note Some log messages (especially debug) are emitted by LibVLC while
     * is being initialized. These messages cannot be captured with this
     * interface.
     *
     * \param logCb A std::function<void(int, const libvlc_log_t*, std::string)>
     *              or an equivalent Callable type instance.
     *
     * \warning A deadlock may occur if this function is called from the
     * callback.
     *
     * \version LibVLC 2.1.0 or later
     */
    template <typename LogCb>
    void logSet(LogCb&& logCb)
    {
        static_assert(signature_match<LogCb, void(int, const libvlc_log_t*, std::string)>::value,
                      "Mismatched log callback" );
        auto wrapper = [logCb](int level, const libvlc_log_t* ctx, const char* format, va_list va) {
            const char* psz_module;
            const char* psz_file;
            unsigned int i_line;
            libvlc_log_get_context( ctx, &psz_module, &psz_file, &i_line );

#ifndef _MSC_VER
            VaCopy vaCopy(va);
            int len = vsnprintf(nullptr, 0, format, vaCopy.va);
            if (len < 0)
                return;
            std::unique_ptr<char[]> message{ new char[len + 1] };
            char* psz_msg = message.get();
            if (vsnprintf(psz_msg, len + 1, format, va) < 0 )
                return;
            char* psz_ctx;
            if (asprintf(&psz_ctx, "[%s] (%s:%d) %s", psz_module, psz_file, i_line, psz_msg) < 0)
                return;
            std::unique_ptr<char, void(*)(void*)> ctxPtr(psz_ctx, &free);
#else
            //MSVC treats passing nullptr as 1st vsnprintf(_s) as an error
            char psz_msg[512];
            if ( vsnprintf(psz_msg, sizeof(psz_msg) - 1, format, va) < 0 )
                return;
            char psz_ctx[1024];
            sprintf_s(psz_ctx, "[%s] (%s:%d) %s", psz_module, psz_file, i_line, psz_msg);
#endif
            logCb( level, ctx, std::string{ psz_ctx } );
        };
        libvlc_log_set(*this, CallbackWrapper<(unsigned int)CallbackIdx::Log, libvlc_log_cb>::wrap( *m_callbacks, std::move(wrapper)),
            m_callbacks.get() );
    }

    /**
     * Sets up logging to a file.
     *
     * \param stream  FILE pointer opened for writing (the FILE pointer must
     * remain valid until Instance::logUnset() )
     *
     * \version LibVLC 2.1.0 or later
     */
    void logSetFile(FILE * stream)
    {
        libvlc_log_set_file( *this, stream );
    }

    /**
     * Returns a list of audio filters that are available.
     *
     * \see ModuleDescription
     */
    std::vector<ModuleDescription> audioFilterList()
    {
        std::unique_ptr<libvlc_module_description_t, decltype(&libvlc_module_description_list_release)>
                ptr( libvlc_audio_filter_list_get(*this), libvlc_module_description_list_release );
        if ( ptr == nullptr )
            return {};
        libvlc_module_description_t* p = ptr.get();
        std::vector<ModuleDescription> res;
        while ( p != NULL )
        {
            res.emplace_back( p );
            p = p->p_next;
        }
        return res;
    }


    /**
     * Returns a list of video filters that are available.
     *
     * \see ModuleDescription
     */
    std::vector<ModuleDescription> videoFilterList()
    {
        std::unique_ptr<libvlc_module_description_t, decltype(&libvlc_module_description_list_release)>
                ptr( libvlc_video_filter_list_get(*this), &libvlc_module_description_list_release );
        if ( ptr == nullptr )
            return {};
        libvlc_module_description_t* p = ptr.get();
        std::vector<ModuleDescription> res;
        while ( p != NULL )
        {
            res.emplace_back( p );
            p = p->p_next;
        }
        return res;
    }

    /**
     * Gets the list of available audio output modules.
     *
     * \see AudioOutputDescription
     */
    std::vector<AudioOutputDescription> audioOutputList()
    {
        std::unique_ptr<libvlc_audio_output_t, decltype(&libvlc_audio_output_list_release)>
                result( libvlc_audio_output_list_get(*this), libvlc_audio_output_list_release );
        if ( result == nullptr )
            return {};
        std::vector<AudioOutputDescription> res;

        libvlc_audio_output_t* p = result.get();
        while ( p != NULL )
        {
            res.emplace_back( p );
            p = p->p_next;
        }
        return res;
    }

    /**
     * Gets a list of audio output devices for a given audio output module,
     *
     * \see Audio::outputDeviceSet() .
     *
     * \note Not all audio outputs support this. In particular, an empty
     * (NULL) list of devices does imply that the specified audio output does
     * not work.
     *
     * \note The list might not be exhaustive.
     *
     * \warning Some audio output devices in the list might not actually work
     * in some circumstances. By default, it is recommended to not specify
     * any explicit audio device.
     *
     * \param psz_aout  audio output name (as returned by
     * Instance::audioOutputList() )
     *
     * \return A vector containing all audio output devices for this module
     *
     * \version LibVLC 2.1.0 or later.
     */
    std::vector<AudioOutputDeviceDescription> audioOutputDeviceList(const std::string& aout)
    {
        std::unique_ptr<libvlc_audio_output_device_t, decltype(&libvlc_audio_output_device_list_release)>
                devices(  libvlc_audio_output_device_list_get( *this, aout.c_str() ), libvlc_audio_output_device_list_release );
        if ( devices == nullptr )
            return {};
        std::vector<AudioOutputDeviceDescription> res;

        for ( auto p = devices.get(); p != nullptr; p = p->p_next )
            res.emplace_back( p );
        return res;
    }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
#if !defined(_MSC_VER) || _MSC_VER >= 1900
    /**
     * Called when an error message needs to be displayed.
     *
     * \param title title of the dialog
     * \param text text of the dialog
     */
    using ErrorCb = void(std::string &&title, std::string &&text);
    /**
     *Called when a login dialog needs to be displayed.
     *
     *You can interact with this dialog by using the postLogin method on dialog to post an answer or the dismiss method to cancel this dialog.
     *
     *\note to receive this callack, CancelCb should not be NULL.
     *\param dialog used to interact with the dialog
     *\param title title of the dialog
     *\param text text of the dialog
     *\param defaultUserName user name that should be set on the user form
     *\param askToStore if true, ask the user if he wants to save the credentials
     */
    using LoginCb = void(Dialog &&dialog, std::string &&title, std::string &&text, std::string &&defaultUserName, bool askToStore);
    /**
     * Called when a question dialog needs to be displayed
     *
     * You can interact with this dialog by using the postAction method on dialog
     * to post an answer or dismiss method to cancel this dialog.
     *
     * \note to receive this callack, CancelCb should not be
     * NULL.
     *
     * \param dialog used to interact with the dialog
     * \param title title of the diaog
     * \param text text of the dialog
     * \param qtype question type (or severity) of the dialog
     * \param cancel text of the cancel button
     * \param action1 text of the first button, if NULL, don't display this
     * button
     * \param action2 text of the second button, if NULL, don't display
     * this button
     */
    using QuestionCb = void(Dialog &&dialog, std::string &&title, std::string &&text, Question qType, std::string &&cancel, std::string &&action1, std::string &&action2);
    /**
     * Called when a progress dialog needs to be displayed
     *
     * If cancellable (cancel != NULL), you can cancel this dialog by
     * calling the dismiss method on dialog
     *
     * \note to receive this callack, CancelCb and
     * UpdtProgressCb should not be NULL.
     *
     * \param dialog used to interact with the dialog
     * \param title title of the diaog
     * \param text text of the dialog
     * \param indeterminate true if the progress dialog is indeterminate
     * \param position initial position of the progress bar (between 0.0 and
     * 1.0)
     * \param cancel text of the cancel button, if NULL the dialog is not
     * cancellable
     */
    using DspProgressCb = void(Dialog &&dialog, std::string &&title, std::string &&text, bool intermediate, float position, std::string &&cancel);
    /**
     * Called when a displayed dialog needs to be cancelled
     *
     * The implementation must call the method dismiss on dialog to really release
     * the dialog.
     *
     * \param dialog used to interact with the dialog
     */
    using CancelCb = void(Dialog &&dialog);
    /**
     * Called when a progress dialog needs to be updated
     *
     * \param dialog used to interact with the dialog
     * \param position osition of the progress bar (between 0.0 and 1.0)
     * \param text new text of the progress dialog
     */
    using UpdtProgressCb = void(Dialog &&dialog, float position, std::string &&text);
#else
    typedef void (*ErrorCb)(std::string &&title, std::string &&text);
    typedef void (*LoginCb)(Dialog &&dialog, std::string &&title, std::string &&text, std::string &&defaultUserName, bool askToStore);
    typedef void (*QuestionCb)(Dialog &&dialog, std::string &&title, std::string &&text, Question qType, std::string &&cancel, std::string &&action1, std::string &&action2);
    typedef void (*DspProgressCb)(Dialog &&dialog, std::string &&title, std::string &&text, bool intermediate, float position, std::string &&cancel);
    typedef void (*CancelCb)(Dialog &&dialog);
    typedef void (*UpdtProgressCb)(Dialog &&dialog, float position, std::string &&text);
#endif
    /**
     * Replaces all the dialog callbacks for this Instance instance
     *
     * \param error   lambda callback that will get called when an error message needs to be displayed.     \see ErrorCb
     * \param login   lambda callback that will get called when a login dialog needs to be displayed. \see LoginCb
     * \param question   lambda callback that will get called when a question dialog needs to be displayed. \see QuestionCb
     * \param dspProgress   lambda callback that will get called when a progress dialog needs to be displayed. \see DspProgressCb
     * \param cancel   lambda callback that will get called when a displayed dialog needs to be cancelled. \see CancelCb
     * \param updtProgress   lambda callback that will get called when a progress dialog needs to be updated. \see UpdtProgressCb
     */
    template <class Error, class Login, class Question, class DspProgress, class Cancel, class UpdtProgress>
    void setDialogHandlers(Error&& error, Login&& login, Question&& question, DspProgress&& dspProgress, Cancel &&cancel, UpdtProgress &&updtProgress)
    {
#if !defined(_MSC_VER) || _MSC_VER >= 1900
        static_assert(signature_match_or_nullptr<Error, ErrorCb>::value, "Mismatched error display callback prototype");
        static_assert(signature_match_or_nullptr<Login, LoginCb>::value, "Mismatched login display callback prototype");
        static_assert(signature_match_or_nullptr<Question, QuestionCb>::value, "Mismatched question display callback prototype");
        static_assert(signature_match_or_nullptr<DspProgress, DspProgressCb>::value, "Mismatched progress display callback prototype");
        static_assert(signature_match_or_nullptr<Cancel, CancelCb>::value, "Mismatched cancel callback prototype");
        static_assert(signature_match_or_nullptr<UpdtProgress, UpdtProgressCb>::value, "Mismatched update progress callback prototype");
#endif
        libvlc_dialog_cbs tmp = {
            CallbackWrapper<(unsigned)CallbackIdx::ErrorDisplay, decltype(libvlc_dialog_cbs::pf_display_error)>::wrap(*m_callbacks, std::forward<Error>(error)),
            CallbackWrapper<(unsigned)CallbackIdx::LoginDisplay, decltype(libvlc_dialog_cbs::pf_display_login)>::wrap(*m_callbacks, std::forward<Login>(login)),
            CallbackWrapper<(unsigned)CallbackIdx::QuestionDisplay, decltype(libvlc_dialog_cbs::pf_display_question)>::wrap(*m_callbacks, std::forward<Question>(question)),
            CallbackWrapper<(unsigned)CallbackIdx::ProgressDisplay, decltype(libvlc_dialog_cbs::pf_display_progress)>::wrap(*m_callbacks, std::forward<DspProgress>(dspProgress)),
            CallbackWrapper<(unsigned)CallbackIdx::CancelDialog, decltype(libvlc_dialog_cbs::pf_cancel)>::wrap(*m_callbacks, std::forward<Cancel>(cancel)),
            CallbackWrapper<(unsigned)CallbackIdx::ProgressUpdate, decltype(libvlc_dialog_cbs::pf_update_progress)>::wrap(*m_callbacks, std::forward<UpdtProgress>(updtProgress))
        };
        m_callbacks_pointers = std::make_shared<libvlc_dialog_cbs>(tmp);
        libvlc_dialog_set_callbacks(*this, m_callbacks_pointers.get(), m_callbacks.get());
    }

    /**
     * Unset all callbacks
     */
    void unsetDialogHandlers()
    {
        memset(m_callbacks_pointers.get(), 0, sizeof(libvlc_dialog_cbs));
        std::fill(m_callbacks->begin() + 2, m_callbacks->end(), nullptr);
        libvlc_dialog_set_callbacks(*this, nullptr, nullptr);
    }

#if LIBVLC_VERSION_INT >= LIBVLC_VERSION(3, 0, 0, 0)
    /**
     * Get media discoverer services by category
     *
     * \version LibVLC 3.0.0 and later.
     *
     * \param category  The category of services to fetch
     *
     * \return A vector containing the available media discoverers
     */
    std::vector<MediaDiscoverer::Description> mediaDiscoverers(MediaDiscoverer::Category category)
    {
        libvlc_media_discoverer_description_t** pp_descs;
        auto nbSd = libvlc_media_discoverer_list_get( *this, static_cast<libvlc_media_discoverer_category_t>( category ),
                                                      &pp_descs );
        if ( nbSd == 0 )
            return {};
        auto releaser = [nbSd](libvlc_media_discoverer_description_t** ptr) {
            libvlc_media_discoverer_list_release( ptr, nbSd );
        };
        std::unique_ptr<libvlc_media_discoverer_description_t*, decltype(releaser)> descPtr( pp_descs, releaser );
        std::vector<MediaDiscoverer::Description> res;
        res.reserve( nbSd );
        for ( auto i = 0u; i < nbSd; ++i )
            res.emplace_back( pp_descs[i]->psz_name, pp_descs[i]->psz_longname, pp_descs[i]->i_cat );
        return res;
    }
#endif

#endif
};

} // namespace VLC

#endif

