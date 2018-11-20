using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace LibVLCSharp.Shared
{
    internal abstract class EventManager
    {
        internal struct Internal
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "libvlc_event_attach")]
            internal static extern int LibVLCEventAttach(IntPtr eventManager, EventType eventType, EventCallback eventCallback,
                IntPtr userData);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "libvlc_event_detach")]
            internal static extern void LibVLCEventDetach(IntPtr eventManager, EventType eventType, EventCallback eventCallback,
                IntPtr userData);
        }

        internal IntPtr NativeReference;
        readonly List<EventCallback> _callbacks = new List<EventCallback>();

        internal protected EventManager(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new NullReferenceException(nameof(ptr));

            NativeReference = ptr;
        }

        private void AttachNativeEvent(EventType eventType, EventCallback eventCallback)
        {
            _callbacks.Add(eventCallback);
            if (Internal.LibVLCEventAttach(NativeReference, eventType, eventCallback, IntPtr.Zero) != 0)
            {
                _callbacks.Remove(eventCallback);
                throw new VLCException($"Could not attach event {eventType}");
            }
        }

        private void DetachNativeEvent(EventType eventType, EventCallback eventCallback)
        {
            _callbacks.Remove(eventCallback);

            Internal.LibVLCEventDetach(NativeReference, eventType, eventCallback, IntPtr.Zero);
        }

        protected void Attach(EventType eventType, ref int registrationCount, Action managedSubscribe, Func<EventCallback> setCallback)
        {
            managedSubscribe();

            if (registrationCount == 0)
            {
                AttachNativeEvent(eventType, setCallback());
            }
            registrationCount++;
        }

        protected void Detach(EventType eventType, ref int registrationCount, Action managedUnsubscribe, ref EventCallback eventCallback)
        {
            if (registrationCount == 0) return;
            registrationCount--;

            managedUnsubscribe();

            if (registrationCount == 0)
            {
                Debug.Assert(eventCallback != null);
                DetachNativeEvent(eventType, eventCallback);
                eventCallback = null;
            }
        }

#if IOS
        internal protected static LibVLCEvent RetrieveEvent(IntPtr eventPtr) => MarshalUtils.PtrToStructure<LibVLCEvent>(eventPtr);
#else
        internal protected LibVLCEvent RetrieveEvent(IntPtr eventPtr) => MarshalUtils.PtrToStructure<LibVLCEvent>(eventPtr);
#endif

        internal protected void OnEventUnhandled(object sender, EventType eventType)
            => throw new InvalidOperationException($"eventType {nameof(eventType)} unhandled by type {sender.GetType().Name}");
        
        internal protected abstract void AttachEvent<T>(EventType eventType, EventHandler<T> eventHandler) where T : EventArgs;
        internal protected abstract void DetachEvent<T>(EventType eventType, EventHandler<T> eventHandler) where T : EventArgs;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventCallback(IntPtr args);
}