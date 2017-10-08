/*****************************************************************************
 * common.hpp: Basic shared types & helpers
 *****************************************************************************
 * Copyright © 2015 libvlcpp authors & VideoLAN
 *
 * Authors: Hugo Beauzée-Luyssen <hugo@beauzee.fr>
 *          Jonathan Calmels <exxo@videolabs.io>
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

#ifndef LIBVLC_CXX_COMMON_H
#define LIBVLC_CXX_COMMON_H

#ifdef _MSC_VER
using ssize_t = long int;
#endif

#include "vlc/vlc.h"
#include <array>
#include <cassert>
#include <memory>

namespace VLC
{
    class Media;
    using MediaPtr = std::shared_ptr<Media>;

    // Work around cross class dependencies
    // Class A needs to access B's internal pointer
    // Class B needs to access A's internal pointer
    // By using a template function to do this, we're delegating
    // the access to both classes' guts to a later point, when the compiler
    // already knows everything it needs.
    // The only drawback is that we can't return decltype(ptr->get()), since when
    // the compiler checks for the prototype, it hasn't parsed all the declarations yet.
    template <typename TYPE, typename REF>
    TYPE* getInternalPtr(const REF& ref)
    {
        return ref.get();
    }

    inline std::unique_ptr<char, void (*)(void*)> wrapCStr(char* str)
    {
        return std::unique_ptr<char, void(*)(void*)>( str, [](void* ptr) { libvlc_free(ptr); } );
    }

#if !defined(_MSC_VER)
    // Kudos to 3xxO for the signature_match helper
    template <typename, typename, typename = void>
    struct signature_match : std::false_type {};

    template <typename Func, typename Ret, typename... Args>
    struct signature_match<Func, Ret(Args...),
        typename std::enable_if<
            std::is_convertible<
                decltype(std::declval<Func>()(std::declval<Args>()...)),
                Ret
            >::value // true or false
        >::type // void or SFINAE
    > : std::true_type {};
#else
    template <typename... Args>
    struct signature_match : std::false_type {};

    template <typename Func, typename Ret, class... Args>
    struct signature_match<Func, Ret(Args...)>
        : std::integral_constant < bool,
            std::is_convertible < decltype(std::declval<Func>()(std::declval<Args>()...)), Ret
        > ::value
    > {};
#endif

    template <typename Func, typename Ret, typename... Args>
    struct signature_match_or_nullptr : std::integral_constant<bool,
        signature_match<Func, Ret, Args...>::value
        >
    {
    };

    template <typename Func>
    struct signature_match_or_nullptr<std::nullptr_t, Func> : std::true_type
    {
    };

    struct CallbackHandlerBase
    {
        virtual ~CallbackHandlerBase() = default;
    };

    template <typename Func>
    struct CallbackHandler : public CallbackHandlerBase
    {
        template <typename FuncFwd>
        CallbackHandler(FuncFwd&& f) : func( std::forward<Func>( f ) ) {}
        Func func;
    };

    template <size_t NbEvent>
    using CallbackArray = std::array<std::unique_ptr<CallbackHandlerBase>, NbEvent>;

    ///
    /// Utility class that contains a shared pointer to a callback array.
    /// We use a shared_ptr to allow multiple instances to share the same callback array
    /// This must be inherited before the Internal<T> type, to ensure it gets deleted
    /// after the wrapped libvlc object.
    ///
    template <size_t NbEvent>
    class CallbackOwner
    {
    protected:
        CallbackOwner()
            : m_callbacks( std::make_shared<CallbackArray<NbEvent>>() )
        {
        }
        std::shared_ptr<CallbackArray<NbEvent>> m_callbacks;
    };

    template <size_t, typename>
    struct FromOpaque;

    template <size_t NbEvents>
    struct FromOpaque<NbEvents, void*>
    {
        static CallbackArray<NbEvents>& get( void* opaque )
        {
            return *reinterpret_cast<CallbackArray<NbEvents>*>( opaque );
        }
    };

    template <size_t NbEvents>
    struct FromOpaque<NbEvents, void**>
    {
        static CallbackArray<NbEvents>& get(void** opaque)
        {
            return *reinterpret_cast<CallbackArray<NbEvents>*>( *opaque );
        }
    };

    namespace detail
    {
        template <typename ArgType>
        ArgType &&converterForNullToString(ArgType &&arg)
        {
            return std::forward<ArgType>(arg);
        }

        template <>
        inline const char * &&converterForNullToString(const char * &&str)
        {
            static const char* empty = "";
            return std::forward<const char *>(str == nullptr ? empty : str);
        }
    }

    template <size_t Idx, typename... Args>
    struct CallbackWrapper;

    // We assume that any callback will take a void*/void** opaque as its first parameter.
    // We intercept this parameter, and use it to fetch the list of user provided
    // functions. Once we know what function to call, we forward the rest of the
    // parameters.
    // Using partial specialization also allows us to get the list of the expected
    // callback parameters automatically, rather than having to specify them.
    template <size_t Idx, typename Ret, typename Opaque, typename... Args>
    struct CallbackWrapper<Idx, Ret(*)(Opaque, Args...)>
    {
        using Wrapped = Ret(*)(Opaque, Args...);

        template <size_t NbEvents, typename Func>
        static Wrapped wrap(CallbackArray<NbEvents>& callbacks, Func&& func)
        {
            callbacks[Idx] = std::unique_ptr<CallbackHandler<Func>>( new CallbackHandler<Func>( std::forward<Func>( func ) ) );
            return [](Opaque opaque, Args... args) -> Ret {
                auto& callbacks = FromOpaque<NbEvents, Opaque>::get( opaque );
                assert(callbacks[Idx] != nullptr);
                auto cbHandler = static_cast<CallbackHandler<Func>*>( callbacks[Idx].get() );
                return cbHandler->func( detail::converterForNullToString<Args>(std::forward<Args>(args))... );
            };
        }

        // Overload to handle null callbacks at build time.
        // We could try to compare any "Func" against nullptr at runtime, though
        // since Func is a template type, which roughly has to satisfy the "Callable" concept,
        // it could be an instance of a function object, which doesn't compare nicely against nullptr.
        // Using the specialization at build time is easier and performs better.
        template <size_t NbEvents>
        static std::nullptr_t wrap(CallbackArray<NbEvents>&, std::nullptr_t)
        {
            return nullptr;
        }
    };

    struct VaCopy
    {
        VaCopy(va_list va_)
        {
            va_copy( va, va_ );
        }

        ~VaCopy()
        {
            va_end( va );
        }

        VaCopy( const VaCopy& ) = delete;
        VaCopy& operator=(const VaCopy& ) = delete;
        VaCopy( VaCopy&& ) = delete;
        VaCopy& operator=( VaCopy&& ) = delete;

        va_list va;
    };

    namespace imem
    {
        // libvlc_media_new_callbacks is a bit different from other libvlc's callbacks.
        // Instead of always receiving the initialy provided void* opaque, the open
        // callback is responsible for creating another opaque value, that will be
        // passed to read/seek/close.
        // Since we use the opaque value to pass a CallbackArray instance, we need
        // a way to keep the user's opaque value, and replace it by our own.
        // The Opaque type is a small boxing helper, that will be used for this purpose.
        // In case the OpenCallback is a nullptr, there is no need for boxing, since
        // the opaque value will be the inial opaque, provided when calling
        // libvlc_media_new_callbacks.
        template <int NbEvent>
        struct Opaque
        {
            CallbackArray<NbEvent>* callbacks;
            void* userOpaque;
        };

        enum class BoxingStrategy
        {
            /// No boxing required.
            NoBoxing,
            /// Used to create the Opaque wrapper and setup pointers
            Setup,
            /// Unbox CallbackArray/user callback pointers
            Unbox,
            /// Releases the Opaque, created during Setup
            Cleanup,
        };

        /// Base case: this is an identity function.
        template <typename OpenCb, BoxingStrategy Strategy_>
        struct GuessBoxingStrategy
        {
#if !defined(_MSC_VER) || _MSC_VER >= 1900
            static constexpr BoxingStrategy Strategy = Strategy_;
#else
            static const BoxingStrategy Strategy = Strategy_;
#endif
        };

        // In case the user provides a nullptr open callback, there's nothing
        // to box, as we get the original opaque (which is our CallbackArray*)
        template <BoxingStrategy Strategy_>
        struct GuessBoxingStrategy<std::nullptr_t, Strategy_>
        {
#if !defined(_MSC_VER) || _MSC_VER >= 1900
            static constexpr BoxingStrategy Strategy = BoxingStrategy::NoBoxing;
#else
            static const BoxingStrategy Strategy = BoxingStrategy::NoBoxing;
#endif
        };

        template <size_t NbEvents, BoxingStrategy Strategy>
        struct BoxOpaque;

        // This specialization is our base case. It unboxes an Opaque pointer
        // to a CallbackArray, of a user provided opaque.
        template <size_t NbEvents>
        struct BoxOpaque<NbEvents, BoxingStrategy::Unbox>
        {
            template <typename... Args>
            BoxOpaque(void* ptr, Args...) : m_ptr( reinterpret_cast<Opaque<NbEvents>*>( ptr ) ) {}
            // Assume the cast operator will be used when calling the callback.
            // In this case, decay the boxed type to the user provided value
            operator void*() { return m_ptr->userOpaque; }
            CallbackArray<NbEvents>& callbacks() { return *(m_ptr->callbacks); }
            Opaque<NbEvents>* m_ptr;
        };

        // This specialization is used to hook the user provided opaque variable,
        // and store it in an Opaque wrapper.
        // We use RAII like code to prepare the hooking before the actual callback
        // call, and finilize the hooking once the callback has been called,
        // and has provided us with a value, store in the address stored in m_userOpaque.
        // This makes us receive all parameters so we can extract the void** param.
        // As a drawback, all other overloads have to receive those parameters, and
        // ignore them.
        template <size_t NbEvents>
        struct BoxOpaque<NbEvents, BoxingStrategy::Setup>
                : public BoxOpaque<NbEvents, BoxingStrategy::Unbox>
        {
            using Base = BoxOpaque<NbEvents, BoxingStrategy::Unbox>;
            template <typename... Args>
            BoxOpaque(void* ptr, void** userOpaque, Args...)
                : Base( new Opaque<NbEvents> )
                , m_userOpaque( userOpaque )
            {
                Base::m_ptr->callbacks = reinterpret_cast<CallbackArray<NbEvents>*>( ptr );
            }
            operator void*() { return Base::m_ptr; }

            ~BoxOpaque()
            {
                // Store the user provided callback
                Base::m_ptr->userOpaque = *m_userOpaque;
                // And replace it with our boxed type
                *m_userOpaque = Base::m_ptr;
            }

            void** m_userOpaque;
        };

        // This is a special case which is only used to delete the Opaque type,
        // created by the BoxingStrategy::Setup specialization
        template <size_t NbEvents>
        struct BoxOpaque<NbEvents, BoxingStrategy::Cleanup>
                : public BoxOpaque<NbEvents, BoxingStrategy::Unbox>
        {
            using Base = BoxOpaque<NbEvents, BoxingStrategy::Unbox>;
            template <typename... Args>
            BoxOpaque(void* ptr, Args...) : BoxOpaque<NbEvents, BoxingStrategy::Unbox>( ptr ) {}
            ~BoxOpaque() { delete Base::m_ptr; }
        };

        // When no boxing is required, enfore the user provided callback as a nullptr.
        // Otherwise, we assume there is no Opaque wrapper, and therefore the
        // pointer we receive already is our CallbackOwner instance.
        template <size_t NbEvents>
        struct BoxOpaque<NbEvents, BoxingStrategy::NoBoxing>
        {
            template <typename... Args>
            BoxOpaque(void* ptr, Args...) : m_ptr( reinterpret_cast<CallbackArray<NbEvents>*>( ptr ) ) {}
            operator void*() { return nullptr; }
            CallbackArray<NbEvents>& callbacks() { return *m_ptr; }

            CallbackArray<NbEvents>* m_ptr;
        };

        template <size_t Idx, typename... Args>
        struct CallbackWrapper;

        // Reimplement the general case CallbackWrapper for imem specific purpose.
        // This could be refactored away, if we delegate the actual call to the BoxOpaque
        // wrapper.
        // For this CallbackWrapper implementation, the BoxOpaque is always passed,
        // and will decay to a void* using a type conversion operator.
        // General-case doesn't need this, since we do not forward the opaque values.
        // This difference leads to a different call, and the need for 2 implementations.
        // Delegating the call to the Opaque class could remove this requirement,
        // and forward the user provided opaque only when it's actually used
        // However, this would mean that depending on the OpenCb type, the expected
        // prototype of Read/Seek/Close cb would change, which might not be ideal.
        // Using CallbackWrapper type is preferable to wrapping user provided callbacks
        // with lambdas, as we would lose the ability to detect when nullptr is provided,
        // so we'd have VLC call a no-op function, while the nullptr wrap() specialization
        // takes care of this.
        template <size_t Idx, typename Ret, typename... Args>
        struct CallbackWrapper<Idx, Ret(*)(void*, Args...)>
        {
            using Wrapped = Ret(*)(void*, Args...);

            template <BoxingStrategy Strategy, size_t NbEvents, typename Func>
            static Wrapped wrap(CallbackArray<NbEvents>& callbacks, Func&& func)
            {
                callbacks[Idx] = std::unique_ptr<CallbackHandler<Func>>( new CallbackHandler<Func>( std::forward<Func>( func ) ) );
                return [](void* opaque, Args... args) -> Ret {
                    auto boxed = BoxOpaque<NbEvents, Strategy>( opaque, std::forward<Args>( args )... );
                    assert(boxed.callbacks()[Idx] != nullptr );
                    auto cbHandler = static_cast<CallbackHandler<Func>*>( boxed.callbacks()[Idx].get() );
                    return cbHandler->func( boxed, std::forward<Args>(args)... );
                };
            }

            template <BoxingStrategy Strategy, size_t NbEvents>
            static std::nullptr_t wrap(CallbackArray<NbEvents>&, std::nullptr_t)
            {
                return nullptr;
            }
        };
    } //namespace imem
}

#endif
