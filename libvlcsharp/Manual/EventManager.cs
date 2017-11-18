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

        protected virtual void Unregister(EventHandlerBase eventHandler) { }

        protected EventHandler Handle(EventType eventType, EventCallback eventCallback, LibVLCEvent libVLCEvent)
        {
            var eventHandler = new EventHandler(this, eventType, eventCallback, libVLCEvent);
            Lambdas.Add(eventHandler);
            return eventHandler;
        }

        //protected EventHandler Handle(EventType eventType, EventCallback eventCallback)
        //{
        //return Handle(eventType, eventCallback, )
        //}

        public abstract class EventHandlerBase
        {
            public LibVLCEvent LibVLCEvent { get; protected set; }
            protected abstract void Unregister();
        }

        public class EventHandler : EventHandlerBase
        {
            readonly IntPtr _eventCallbackPtr;
            readonly EventType _eventType;
            readonly EventManager _eventManager;
            readonly IntPtr _eventStructPtr;

            public EventHandler(EventManager eventManager, EventType eventType, EventCallback eventCallback, LibVLCEvent libVLCEvent)
            {
                _eventStructPtr = new IntPtr();
                Marshal.StructureToPtr(libVLCEvent, _eventStructPtr, true);
                _eventCallbackPtr = Marshal.GetFunctionPointerForDelegate(eventCallback);

                if (Internal.LibVLCEventAttach(eventManager.NativeReference, eventType, _eventStructPtr, _eventCallbackPtr) != 0)
                    throw new VLCException();

                _eventManager = eventManager;
                _eventType = eventType;
                LibVLCEvent = libVLCEvent;
            }

            ~EventHandler()
            {
                Internal.LibVLCEventDetach(_eventManager.NativeReference, _eventType, _eventStructPtr, _eventCallbackPtr);
            }

            protected override void Unregister()
            {
                _eventManager.Unregister(this);
            }
        }
    }

    public class MediaEventManager : EventManager
    {
        public MediaEventManager(IntPtr ptr) : base(ptr)
        {
        }

        public EventHandler OnMetaChanged(EventCallback cb)
        {
            return Handle(EventType.MediaMetaChanged, cb, new LibVLCEvent
            {
                Type = EventType.MediaMetaChanged,
                //Union = new LibVLCEvent.EventUnion
                //{
                //    MediaMetaChanged = new LibVLCEvent.MediaMetaChanged
                //    {
                //    }
                //}
            });
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

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventCallback(IntPtr args);

}
