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

        protected Internal(Func<IntPtr> create, Action<IntPtr> release)
        {
            Release = release;
            var nativeRef = create();
            if(nativeRef == IntPtr.Zero)
                throw new VLCException();
            NativeReference = nativeRef;
        }

        public virtual void Dispose()
        {
            if (NativeReference == IntPtr.Zero) return;

            Release(NativeReference);
            NativeReference = IntPtr.Zero;
        }
    }
}