using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>The state of a LibVLC 4 download request.</summary>
    public enum DownloadStatus
    {
        /// <summary>Waiting for parsing or downloading.</summary>
        Pending = 0,
        /// <summary>Downloading data.</summary>
        Running = 1,
        /// <summary>Paused explicitly or by a partial buffer read.</summary>
        Paused = 2,
        /// <summary>The download finished.</summary>
        Finished = 3,
        /// <summary>The download was cancelled.</summary>
        Cancelled = 4,
        /// <summary>Parsing or downloading failed, or the media cannot be downloaded.</summary>
        Error = 5
    }

    /// <summary>Consumes a borrowed native buffer, valid only during the callback.</summary>
    /// <param name="buffer">Buffer to copy or consume synchronously.</param>
    /// <param name="length">Number of available bytes (currently at most about 64 KB).</param>
    /// <param name="position">Total bytes read from the source, including buffered bytes.</param>
    /// <param name="total">Current total size of the source in bytes.</param>
    /// <returns>Bytes consumed, -1 for error, or -2 for cancellation. A partial read automatically pauses the request.</returns>
    /// <remarks>
    /// GetMedia is safe to call on an undisposed request. Do not block, call downloader methods or
    /// request Cancel, SetPause or Dispose, or wait for completion inside this callback.
    /// </remarks>
    public delegate int DownloadBufferCallback(IntPtr buffer, int length, ulong position, ulong total);

    /// <summary>Downloads finite files with the LibVLC 4 downloader API.</summary>
    /// <remarks>
    /// Supports HTTP(S), FTP, file, NFS, SMB and SFTP, depending on installed plugins.
    /// Playlists and directories report subitems; live streams and unsupported sources end with an error.
    /// Dispose the downloader to cancel outstanding requests and wait for native threads to exit.
    /// </remarks>
    public sealed class MediaDownloader : Internal
    {
        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_downloader_new")]
            internal static extern IntPtr LibVLCDownloaderNew(IntPtr instance, ref Configuration configuration);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_downloader_queue")]
            internal static extern IntPtr LibVLCDownloaderQueue(IntPtr downloader, ref Request request, IntPtr callbacks, IntPtr opaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_downloader_cancel")]
            internal static extern UIntPtr LibVLCDownloaderCancel(IntPtr downloader, IntPtr task);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_downloader_set_pause")]
            internal static extern void LibVLCDownloaderSetPause(IntPtr downloader, IntPtr task, [MarshalAs(UnmanagedType.I1)] bool paused);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_downloader_destroy")]
            internal static extern void LibVLCDownloaderDestroy(IntPtr downloader);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_downloader_task_get_media")]
            internal static extern IntPtr LibVLCDownloaderTaskGetMedia(IntPtr task);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_downloader_task_release")]
            internal static extern void LibVLCDownloaderTaskRelease(IntPtr task);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Configuration
        {
            public uint Version;
            public uint MaxParserThreads;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Request
        {
            public uint Version;
            public IntPtr Media;
        }

        readonly object _gate = new object();
        readonly HashSet<DownloadRequest> _requests = new HashSet<DownloadRequest>();

        /// <summary>Create a downloader.</summary>
        /// <param name="libVLC">The LibVLC instance.</param>
        /// <param name="maxParserThreads">Maximum parser threads; zero uses the native default of one.</param>
        public MediaDownloader(LibVLC libVLC, uint maxParserThreads = 0)
            : base(() => Create(libVLC, maxParserThreads), Native.LibVLCDownloaderDestroy)
        {
        }

        static IntPtr Create(LibVLC libVLC, uint maxParserThreads)
        {
            if (libVLC == null) throw new ArgumentNullException(nameof(libVLC));
            if (libVLC.NativeReference == IntPtr.Zero) throw new ObjectDisposedException(nameof(libVLC));
            var configuration = new Configuration { Version = 0, MaxParserThreads = maxParserThreads };
            return Native.LibVLCDownloaderNew(libVLC.NativeReference, ref configuration);
        }

        /// <summary>Queue a download. Dispose the returned request when it is no longer needed.</summary>
        /// <param name="media">Source media, retained by the native request.</param>
        /// <param name="onBuffer">Required buffer consumer.</param>
        /// <param name="stateChanged">Optional state observer.</param>
        /// <param name="subitems">Optional observer of a borrowed list, disposed after the callback. Retain individual media by accessing list items.</param>
        /// <param name="slaves">Optional observer of copied audio/subtitle slave descriptions.</param>
        /// <remarks>
        /// Callbacks can run before Queue returns, on native threads. Keep them short and do not call
        /// downloader methods or request Cancel, SetPause or Dispose, or wait for other threads, inside them.
        /// Resume partial reads from another thread. GetMedia is safe to call from a callback once the
        /// request has been returned by Queue and until it is disposed.
        /// Buffer/discovery exceptions fault Completion; state observer exceptions are logged.
        /// </remarks>
        public DownloadRequest Queue(Media media, DownloadBufferCallback onBuffer,
            Action<DownloadStatus>? stateChanged = null, Action<MediaList>? subitems = null,
            Action<MediaSlave[]>? slaves = null)
        {
            if (media == null) throw new ArgumentNullException(nameof(media));
            if (onBuffer == null) throw new ArgumentNullException(nameof(onBuffer));
            lock (_gate)
            {
                CheckDisposed();
                if (media.NativeReference == IntPtr.Zero) throw new ObjectDisposedException(nameof(media));
                var state = new DownloadRequest(this, onBuffer, stateChanged, subitems, slaves);
                state.Pin = GCHandle.Alloc(state);
                var request = new Request { Version = 0, Media = media.NativeReference };
                try
                {
                    state.Handle = Native.LibVLCDownloaderQueue(NativeReference, ref request,
                        MediaDownloaderCallbacks.Pointer, GCHandle.ToIntPtr(state.Pin));
                    if (state.Handle == IntPtr.Zero) throw new VLCException("Failed to queue the download request");
                }
                catch
                {
                    if (state.TryBeginCompletion()) state.Pin.Free();
                    throw;
                }
                _requests.Add(state);
                return state;
            }
        }

        /// <summary>Cancel all pending, running or paused requests. Returns the number cancelled.</summary>
        public ulong CancelAll()
        {
            lock (_gate)
            {
                CheckDisposed();
                return Native.LibVLCDownloaderCancel(NativeReference, IntPtr.Zero).ToUInt64();
            }
        }

        internal ulong Cancel(DownloadRequest request)
        {
            lock (_gate)
            {
                CheckRequest(request);
                return request.IsTerminal ? 0 : Native.LibVLCDownloaderCancel(NativeReference, request.Handle).ToUInt64();
            }
        }

        internal void SetPause(DownloadRequest request, bool paused)
        {
            lock (_gate)
            {
                CheckRequest(request);
                if (!request.IsTerminal) Native.LibVLCDownloaderSetPause(NativeReference, request.Handle, paused);
            }
        }

        internal static Media RetainMedia(IntPtr task)
        {
            var media = Native.LibVLCDownloaderTaskGetMedia(task);
            if (media == IntPtr.Zero) throw new VLCException("The download request has no media");
            return new Media(Media.Native.LibVLCMediaRetain(media));
        }

        internal void ReleaseRequest(DownloadRequest request)
        {
            lock (_gate)
            {
                if (request.Handle == IntPtr.Zero) return;
                if (!request.IsTerminal) Native.LibVLCDownloaderCancel(NativeReference, request.Handle);
                lock (request.HandleGate)
                {
                    Native.LibVLCDownloaderTaskRelease(request.Handle);
                    request.Handle = IntPtr.Zero;
                }
                _requests.Remove(request);
            }
        }

        /// <summary>Cancel all requests and wait for callbacks to finish before releasing resources.</summary>
        /// <remarks>Blocks until the native download threads have joined. Do not call it from a callback.</remarks>
        protected override void Dispose(bool disposing)
        {
            lock (_gate)
            {
                base.Dispose(disposing);
                foreach (var request in _requests)
                {
                    lock (request.HandleGate)
                    {
                        Native.LibVLCDownloaderTaskRelease(request.Handle);
                        request.Handle = IntPtr.Zero;
                    }
                    if (request.TryBeginCompletion())
                    {
                        request.Pin.Free();
                        request.CurrentStatus = DownloadStatus.Cancelled;
#if NET45
                        System.Threading.ThreadPool.QueueUserWorkItem(_ => request.Complete(DownloadStatus.Cancelled));
#else
                        request.Complete(DownloadStatus.Cancelled);
#endif
                    }
                }
                _requests.Clear();
            }
        }

        void CheckDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(MediaDownloader));
        }

        void CheckRequest(DownloadRequest request)
        {
            CheckDisposed();
            if (request.Handle == IntPtr.Zero) throw new ObjectDisposedException(nameof(DownloadRequest));
        }
    }
}
