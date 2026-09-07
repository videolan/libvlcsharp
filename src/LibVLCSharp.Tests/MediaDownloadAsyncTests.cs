using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaDownloadAsyncTests : BaseSetup
    {
        static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);
        MediaDownloader _downloader;
        string _source;
        string _destination;
        byte[] _bytes;

        [SetUp]
        public void CreateDownloader()
        {
            _downloader = new MediaDownloader(_libVLC);
            _source = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp3");
            _destination = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp3");
            // Exceed the buffer capacity so slow destinations exercise native pause/resume.
            var sample = File.ReadAllBytes(LocalAudioFile);
            using var content = new MemoryStream();
            while (content.Length < 2 * 1024 * 1024) content.Write(sample, 0, sample.Length);
            _bytes = content.ToArray();
            File.WriteAllBytes(_source, _bytes);
        }

        [TearDown]
        public void Cleanup()
        {
            _downloader?.Dispose();
            File.Delete(_source);
            File.Delete(_destination);
        }

        [Test]
        public async Task SpanCallbackSupportsPartialConsumption()
        {
            var paused = NewCompletionSource<bool>();
            using var media = new Media(LocalAudioFile);
            using var output = new MemoryStream();
            var partial = true;
            using var request = _downloader.Queue(media, (bytes, position, total) =>
            {
                var consumed = partial ? bytes.Length / 2 : bytes.Length;
                partial = false;
                output.Write(bytes.Slice(0, consumed));
                return consumed;
            }, state => { if (state == DownloadStatus.Paused) paused.TrySetResult(true); });
            await paused.Task.WaitAsync(Timeout);
            request.SetPause(false);
            Assert.AreEqual(DownloadStatus.Finished, await request.Completion.WaitAsync(Timeout));
            CollectionAssert.AreEqual(File.ReadAllBytes(LocalAudioFile), output.ToArray());
        }

        [Test]
        public void SpanCallbackExceptionFaultsCompletion()
        {
            var failure = new IOException("Consumer failed");
            using var media = new Media(LocalAudioFile);
            using var request = _downloader.Queue(media, (bytes, position, total) => throw failure);
            Assert.AreSame(failure, Assert.ThrowsAsync<IOException>(async () => await request.Completion.WaitAsync(Timeout)));
        }

        [Test]
        public async Task SlowStreamDrainsBuffersReportsWrittenBytesAndStaysOpen()
        {
            using var media = new Media(_source);
            using var output = new ControlledStream();
            output.WriteByte(42);
            var reports = new List<DownloadProgress>();
            var progress = new InlineProgress(value =>
            {
                Assert.AreEqual((ulong)output.Length - 1, value.BytesWritten);
                reports.Add(value);
            });
            var download = _downloader.DownloadAsync(media, output, progress);
            await output.Writing.Task.WaitAsync(Timeout);
            Assert.False(download.IsCompleted);
            output.AllowWrites.TrySetResult(true);
            await output.Flushing.Task.WaitAsync(Timeout);
            Assert.False(download.IsCompleted, "Completion must wait for the final flush");
            output.AllowFlush.TrySetResult(true);
            await download.WaitAsync(Timeout);
            Assert.True(output.CanWrite);
            Assert.AreEqual(42, output.GetBuffer()[0]);
            CollectionAssert.AreEqual(_bytes, output.ToArray().AsSpan(1).ToArray());
            Assert.That(reports.Count, Is.GreaterThan(1));
            Assert.AreEqual((ulong)_bytes.Length, reports[reports.Count - 1].BytesWritten);
            Assert.AreEqual((ulong)_bytes.Length, reports[reports.Count - 1].TotalBytes);
        }

        [Test]
        public async Task DownloadsToFileWithoutOverwritingExistingOutput()
        {
            using var media = new Media(_source);
            await _downloader.DownloadAsync(media, _destination).WaitAsync(Timeout);
            CollectionAssert.AreEqual(_bytes, File.ReadAllBytes(_destination));
            Assert.ThrowsAsync<IOException>(async () =>
                await _downloader.DownloadAsync(media, _destination).WaitAsync(Timeout));
            CollectionAssert.AreEqual(_bytes, File.ReadAllBytes(_destination));
        }

        [Test]
        public async Task CancellationInterruptsPendingWriteAndLeavesStreamOpen()
        {
            using var media = new Media(_source);
            using var output = new ControlledStream();
            using var cancellation = new CancellationTokenSource();
            var download = _downloader.DownloadAsync(media, output, cancellationToken: cancellation.Token);
            await output.Writing.Task.WaitAsync(Timeout);
            cancellation.Cancel();
            Assert.CatchAsync<OperationCanceledException>(async () => await download.WaitAsync(Timeout));
            Assert.True(download.IsCanceled);
            Assert.True(output.CanWrite);
            Assert.Zero(output.Length);
            Assert.Zero(_downloader.CancelAll(), "The helper must finish cancelling before returning");
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task ExternalCancellationCompletesHelper(bool disposeDownloader)
        {
            using var media = new Media(_source);
            using var output = new ControlledStream();
            var download = _downloader.DownloadAsync(media, output);
            await output.Writing.Task.WaitAsync(Timeout);
            if (disposeDownloader) _downloader.Dispose();
            else Assert.AreEqual(1, _downloader.CancelAll());
            output.AllowWrites.TrySetResult(true);
            Assert.CatchAsync<OperationCanceledException>(async () => await download.WaitAsync(Timeout));
            Assert.True(download.IsCanceled);
            Assert.True(output.CanWrite);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task DestinationFailurePropagatesAndStopsDownload(bool failFlush)
        {
            var failure = new IOException("Destination failed");
            using var media = new Media(_source);
            using var output = new ControlledStream();
            if (failFlush)
            {
                output.AllowWrites.TrySetResult(true);
                output.AllowFlush.TrySetException(failure);
            }
            else output.AllowWrites.TrySetException(failure);
            var download = _downloader.DownloadAsync(media, output);
            Assert.AreSame(failure, Assert.ThrowsAsync<IOException>(async () => await download.WaitAsync(Timeout)));
            Assert.True(output.CanWrite);
            Assert.Zero(_downloader.CancelAll());
            // Ensure this downloader is still usable after a destination failure.
            using var retry = new MemoryStream();
            await _downloader.DownloadAsync(media, retry).WaitAsync(Timeout);
            CollectionAssert.AreEqual(_bytes, retry.ToArray());
        }

        [Test]
        public void MissingSourceThrowsIOException()
        {
            using var media = new Media(_destination);
            using var output = new MemoryStream();
            Assert.ThrowsAsync<IOException>(async () =>
                await _downloader.DownloadAsync(media, output).WaitAsync(Timeout));
            Assert.True(output.CanWrite);
        }

        [Test]
        public void PreCancelledDownloadDoesNotCreateFile()
        {
            using var media = new Media(_source);
            var cancellation = new CancellationToken(true);
            Assert.CatchAsync<OperationCanceledException>(async () =>
                await _downloader.DownloadAsync(media, _destination, cancellationToken: cancellation));
            Assert.False(File.Exists(_destination));
        }

        [Test]
        public void ReadOnlyDestinationIsRejected()
        {
            using var media = new Media(_source);
            using var output = new MemoryStream(_bytes, writable: false);
            Assert.ThrowsAsync<ArgumentException>(async () => await _downloader.DownloadAsync(media, output));
        }

        sealed class InlineProgress : IProgress<DownloadProgress>
        {
            readonly Action<DownloadProgress> _report;
            internal InlineProgress(Action<DownloadProgress> report) => _report = report;
            public void Report(DownloadProgress value) => _report(value);
        }

        sealed class ControlledStream : MemoryStream
        {
            internal readonly TaskCompletionSource<bool> Writing = NewCompletionSource<bool>();
            internal readonly TaskCompletionSource<bool> AllowWrites = NewCompletionSource<bool>();
            internal readonly TaskCompletionSource<bool> Flushing = NewCompletionSource<bool>();
            internal readonly TaskCompletionSource<bool> AllowFlush = NewCompletionSource<bool>();

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                Writing.TrySetResult(true);
                await AllowWrites.Task.WaitAsync(cancellationToken);
                await base.WriteAsync(buffer, offset, count, cancellationToken);
            }

            public override async Task FlushAsync(CancellationToken cancellationToken)
            {
                Flushing.TrySetResult(true);
                await AllowFlush.Task.WaitAsync(cancellationToken);
                await base.FlushAsync(cancellationToken);
            }
        }
    }
}
