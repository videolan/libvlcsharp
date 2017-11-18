using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using VideoLAN.LibVLC;

namespace VideoLAN.LibVLC
{
    public abstract class EventManager
    {
        public struct Internal
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_event_attach")]
            internal static extern int LibVLCEventAttach(IntPtr eventManager, EventType eventType, IntPtr callback,
                IntPtr userData);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_event_detach")]
            internal static extern void LibVLCEventDetach(IntPtr eventManager, EventType eventType, IntPtr callback,
                IntPtr userData);


        }

        public IntPtr NativeReference;
        protected List<EventHandlerBase> Lambdas = new List<EventHandlerBase>();

        protected EventManager(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new NullReferenceException(nameof(ptr));

            NativeReference = ptr;
        }

        protected virtual void Unregister(EventHandlerBase eventHandler) {}

        public abstract class EventHandlerBase
        {
            protected abstract void Unregister();
        }

        class EventHandler : EventHandlerBase
        {
            readonly IntPtr _eventCallback;
            readonly EventType _eventType;
            readonly EventManager _eventManager;
            readonly LibVLCEvent _libVLCEvent;

            public EventHandler(EventManager eventManager, EventType eventType, IntPtr eventCallback,
                LibVLCEvent libVLCEvent)
            {
                if (Internal.LibVLCEventAttach(eventManager.NativeReference, eventType, eventCallback, IntPtr.Zero) !=
                    0)
                    throw new VLCException();

                _eventManager = eventManager;
                _eventType = eventType;
                _eventCallback = eventCallback;
                _libVLCEvent = libVLCEvent;
            }

            ~EventHandler()
            {
                Internal.LibVLCEventDetach(_eventManager.NativeReference, _eventType, _eventCallback, IntPtr.Zero);
            }

            protected override void Unregister()
            {
                _eventManager.Unregister(this);
            }
        }

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate void EventCallback(IntPtr args);

        //[SuppressUnmanagedCodeSecurity]
        //[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
        //    EntryPoint = "libvlc_callback_t")]
        //public static extern void EventCallback(IntPtr args);
    }

    public class MediaEventManager : EventManager
    {
        public MediaEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class MediaPlayerEventManager : EventManager
    {
        public MediaPlayerEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class MediaListEventManager : EventManager
    {
        public MediaListEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class MediaListPlayerEventManager : EventManager
    {
        public MediaListPlayerEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class MediaDiscovererEventManager : EventManager
    {
        public MediaDiscovererEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class VLMEventManager : EventManager
    {
        public VLMEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class VLCException : Exception
    {
    }
}
