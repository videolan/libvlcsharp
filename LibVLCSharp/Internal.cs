using System;

namespace VideoLAN.LibVLC
{
    public abstract class Internal : IDisposable
    {
        public IntPtr NativeReference { get; private set; }

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
            Release(NativeReference);
            NativeReference = IntPtr.Zero;
        }
    }
}