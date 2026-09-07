using LibVLCSharp.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LibVLCSharp
{
    /// <summary>A queued download, owned by its downloader. Dispose to cancel and release its native handle.</summary>
    /// <remarks>Disposing a request initiates cancellation; await Completion before disposing resources used by callbacks.</remarks>
    public sealed class DownloadRequest : IDisposable
    {
        readonly MediaDownloader _owner;
        readonly TaskCompletionSource<DownloadStatus> _completion = MarshalUtils.NewCompletionSource<DownloadStatus>();
        internal readonly DownloadBufferCallback OnBuffer;
        internal readonly Action<DownloadStatus>? StateChanged;
        internal readonly Action<MediaList>? Subitems;
        internal readonly Action<MediaSlave[]>? Slaves;
        internal readonly object HandleGate = new object();
        internal IntPtr Handle;
        internal GCHandle Pin;
        internal volatile DownloadStatus CurrentStatus;
        internal volatile Exception? Failure;
        int _completing;
        internal bool IsTerminal => CurrentStatus >= DownloadStatus.Finished;

        internal DownloadRequest(MediaDownloader owner, DownloadBufferCallback onBuffer,
            Action<DownloadStatus>? stateChanged, Action<MediaList>? subitems, Action<MediaSlave[]>? slaves)
        {
            _owner = owner;
            OnBuffer = onBuffer;
            StateChanged = stateChanged;
            Subitems = subitems;
            Slaves = slaves;
        }

        /// <summary>The most recently reported native state.</summary>
        public DownloadStatus Status => CurrentStatus;

        /// <summary>Completes with the terminal native state, or faults if a buffer/discovery callback throws.</summary>
        public Task<DownloadStatus> Completion => _completion.Task;

        /// <summary>Get an independently retained media reference. The caller must dispose it.</summary>
        public Media GetMedia()
        {
            lock (HandleGate)
            {
                if (Handle == IntPtr.Zero) throw new ObjectDisposedException(nameof(DownloadRequest));
                return MediaDownloader.RetainMedia(Handle);
            }
        }

        /// <summary>Cancel this request. Returns zero if already terminal, otherwise the number cancelled.</summary>
        public ulong Cancel() => _owner.Cancel(this);

        /// <summary>Pause or resume from outside a callback. Has no effect on a terminal request.</summary>
        public void SetPause(bool paused) => _owner.SetPause(this, paused);

        /// <summary>Cancel if active and release the native task. May be called more than once.</summary>
        public void Dispose() => _owner.ReleaseRequest(this);

        internal bool TryBeginCompletion() => Interlocked.Exchange(ref _completing, 1) == 0;

        internal void Complete(DownloadStatus status)
        {
            if (Failure != null) _completion.TrySetException(Failure);
            else _completion.TrySetResult(status);
        }
    }
}
