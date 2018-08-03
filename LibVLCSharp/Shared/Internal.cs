using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace LibVLCSharp.Shared
{
    public abstract class Internal : IDisposable
    {
        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "libvlc_event_attach")]
            internal static extern int LibVLCEventAttach(IntPtr eventManager, EventType eventType, EventCallback eventCallback,
                IntPtr userData);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "libvlc_event_detach")]
            internal static extern void LibVLCEventDetach(IntPtr eventManager, EventType eventType, EventCallback eventCallback,
                IntPtr userData);
        }

        /// <summary>
        /// The pointer to the native code representation of this object
        /// </summary>
        public IntPtr NativeReference { get; private set; }
       
        /// <summary>
        /// The pointer to the native code representation of the event manager of this object
        /// </summary>
        protected IntPtr NativeEventManagerReference { get; private set; }

        /// <summary>
        /// Release native resources by calling this C function
        /// </summary>
        protected readonly Action<IntPtr> Release;

        protected Internal(Func<IntPtr> create, Action<IntPtr> release, Func<IntPtr, IntPtr> retrieveEventManager = null)
        {
            Release = release;

            NativeReference = create();
            if(NativeReference == IntPtr.Zero)
                throw new VLCException("failed to create instance in native code");
            
            if (retrieveEventManager != null)
            {
                NativeEventManagerReference = retrieveEventManager(NativeReference);
                if(NativeEventManagerReference == IntPtr.Zero)
                    throw new VLCException("failed to retrieve event manager");
            }
        }

        public virtual void Dispose()
        {
            if (NativeReference == IntPtr.Zero) return;

            Release(NativeReference);
            NativeReference = IntPtr.Zero;
            NativeEventManagerReference = IntPtr.Zero;
        }

        readonly List<EventCallback> _eventCallbacks = new List<EventCallback>();

        protected void AttachEvent(EventType eventType, EventCallback eventCallback)
        {
            _eventCallbacks.Add(eventCallback);
            if (Native.LibVLCEventAttach(NativeEventManagerReference, eventType, eventCallback, IntPtr.Zero) != 0)
            {
                _eventCallbacks.Remove(eventCallback);
                throw new VLCException($"Could not attach event {eventType}");
            }
        }

        protected void DetachEvent(EventType eventType, EventCallback eventCallback)
        {
            _eventCallbacks.Remove(eventCallback);

            Native.LibVLCEventDetach(NativeEventManagerReference, eventType, eventCallback, IntPtr.Zero);
        }

        static protected LibVLCEvent RetrieveEvent(IntPtr eventPtr) => Marshal.PtrToStructure<LibVLCEvent>(eventPtr);
     }

    public class VLCException : Exception
    {
        public readonly string Reason;

        public VLCException(string reason = "")
        {
            Reason = reason;
        }
    }

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventCallback(IntPtr args);
}