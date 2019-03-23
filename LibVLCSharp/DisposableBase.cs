using System;

namespace LibVLCSharp.Shared
{
    public interface IDisposableObservable : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this object has been disposed.
        /// </summary>
        bool IsDisposed { get; }
    }

    /// <summary>
    /// Implements the IDisposable pattern
    /// </summary>
    public abstract class DisposableBase : IDisposableObservable
    {
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableBase"/> class.
        /// </summary>
        ~DisposableBase()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposing">True if called explicitly, false if called through the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeResources();
            }

            IsDisposed = true;
        }

        /// <summary>
        /// Release resources. base.Dispose() should not be called from this method. 
        /// </summary>
        protected abstract void DisposeResources();
    }
}
