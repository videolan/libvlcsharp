using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LibVLCSharp
{
    /// <summary>Transfers borrowed native buffers to an asynchronous stream writer with bounded memory.</summary>
    internal sealed class DownloadWriter : IDisposable
    {
        const int Capacity = 8;
        const int BufferSize = 65536;
        readonly object _gate = new object();
        readonly Queue<Buffer> _buffers = new Queue<Buffer>();
        readonly SemaphoreSlim _available = new SemaphoreSlim(0, 1);
        bool _paused;
        bool _completed;

        readonly struct Buffer
        {
            internal readonly byte[] Bytes;
            internal readonly int Length;
            internal readonly ulong Total;

            internal Buffer(byte[] bytes, int length, ulong total)
            {
                Bytes = bytes;
                Length = length;
                Total = total;
            }
        }

        internal int OnBuffer(ReadOnlySpan<byte> bytes, ulong position, ulong total)
        {
            lock (_gate)
            {
                if (_buffers.Count == Capacity) return 0;
                var length = Math.Min(bytes.Length, BufferSize);
                var rented = ArrayPool<byte>.Shared.Rent(length);
                try
                {
                    bytes.Slice(0, length).CopyTo(rented);
                    _buffers.Enqueue(new Buffer(rented, length, total));
                }
                catch
                {
                    ArrayPool<byte>.Shared.Return(rented);
                    throw;
                }
                Signal();
                return length;
            }
        }

        internal void OnStateChanged(DownloadStatus status)
        {
            if (status != DownloadStatus.Paused) return;
            lock (_gate)
            {
                _paused = true;
                Signal();
            }
        }

        internal async Task ObserveCompletionAsync(DownloadRequest request)
        {
            try { await request.Completion.ConfigureAwait(false); }
            catch { } // The caller observes the result; this also runs when native disposal completes the request.
            finally
            {
                lock (_gate)
                {
                    _completed = true;
                    Signal();
                }
            }
        }

        internal async Task WriteAsync(DownloadRequest request, Stream output, IProgress<DownloadProgress>? progress,
            CancellationToken cancellationToken)
        {
            ulong written = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Buffer buffer = default;
                bool resume;
                lock (_gate)
                {
                    if (_completed && (_buffers.Count == 0 || request.Status != DownloadStatus.Finished)) return;
                    if (_buffers.Count != 0) buffer = _buffers.Dequeue();
                    resume = _paused && !_completed;
                    if (resume) _paused = false;
                }

                // Never call native methods while holding the gate used by callbacks.
                try
                {
                    if (resume)
                    {
                        try { request.SetPause(false); }
                        catch (ObjectDisposedException) { } // Completion will report downloader disposal.
                    }
                    if (buffer.Bytes != null)
                    {
                        await output.WriteAsync(buffer.Bytes, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                        written += (ulong)buffer.Length;
                        progress?.Report(new DownloadProgress(written, buffer.Total));
                    }
                    else await _available.WaitAsync(cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    if (buffer.Bytes != null) ArrayPool<byte>.Shared.Return(buffer.Bytes);
                }
            }
        }

        // Called under _gate. Coalesce notifications; the reader always rechecks the queue and state.
        void Signal()
        {
            if (_available.CurrentCount == 0) _available.Release();
        }

        public void Dispose()
        {
            // The owner awaits native completion and its observer before disposing this writer.
            while (_buffers.Count != 0) ArrayPool<byte>.Shared.Return(_buffers.Dequeue().Bytes);
            _available.Dispose();
        }
    }
}
