using System;

namespace LibVLCSharp.Shared
{
    public abstract class Internal : IDisposable
    {
        /// <summary>
        /// The pointer to the native code representation of this object
        /// </summary>
        public IntPtr NativeReference { get; private set; }
       
        /// <summary>
        /// Release native resources by calling this C function
        /// </summary>
        protected readonly Action<IntPtr> Release;

        /// <summary>
        /// Indicates whether this object has already been disposed
        /// </summary>
        protected bool IsDisposed;

        protected Internal(Func<IntPtr> create, Action<IntPtr> release)
        {
            Release = release;
            var nativeRef = create();
            if(nativeRef == IntPtr.Zero)
                throw new VLCException("Failed to perform instanciation on the native side. " +
                    "Make sure you installed the correct VideoLAN.LibVLC.[YourPlatform] package in your platform specific project");
            NativeReference = nativeRef;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed || NativeReference == IntPtr.Zero)
                return;

            // dispose unmanaged resources
            Release(NativeReference);
            NativeReference = IntPtr.Zero;
            IsDisposed = true;
        }

        ~Internal()
        {
            Dispose(false);
        }
    }
}