using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaDownloaderBindingTests
    {
        [Test]
        public void DownloaderBindingsMatchHeader()
        {
            var methods = NativeBindingAssertions.NativeMethods(typeof(MediaDownloader));
            CollectionAssert.AreEquivalent(new[]
            {
                "libvlc_downloader_new", "libvlc_downloader_queue", "libvlc_downloader_cancel",
                "libvlc_downloader_set_pause", "libvlc_downloader_destroy",
                "libvlc_downloader_task_get_media", "libvlc_downloader_task_release"
            }, methods.Select(NativeBindingAssertions.DllImportEntryPoint));
            foreach (var method in methods)
                Assert.AreEqual(CallingConvention.Cdecl, method.GetCustomAttribute<DllImportAttribute>().CallingConvention);

            var cancel = NativeBindingAssertions.NativeMethod(typeof(MediaDownloader), "LibVLCDownloaderCancel");
            Assert.AreEqual(typeof(UIntPtr), cancel.ReturnType);
            var pause = NativeBindingAssertions.NativeMethod(typeof(MediaDownloader), "LibVLCDownloaderSetPause");
            Assert.AreEqual(UnmanagedType.I1, pause.GetParameters()[2].GetCustomAttribute<MarshalAsAttribute>().Value);
        }

        [Test]
        public void StructuresAndCallbacksMatchHeader()
        {
            var configuration = typeof(MediaDownloader.Configuration);
            var request = typeof(MediaDownloader.Request);
            var callbacks = typeof(MediaDownloaderCallbacks);
            var nativeCallbacks = callbacks.GetNestedType("NativeCallbacks", BindingFlags.NonPublic);
            Assert.AreEqual(8, Marshal.SizeOf(configuration));
            Assert.AreEqual(IntPtr.Size, Marshal.OffsetOf(request, "Media").ToInt32());
            Assert.AreEqual(2 * IntPtr.Size, Marshal.SizeOf(request));
            Assert.AreEqual(5 * IntPtr.Size, Marshal.SizeOf(nativeCallbacks));
            Assert.AreEqual(4 * IntPtr.Size, Marshal.OffsetOf(nativeCallbacks, "OnSlaves").ToInt32());
            var buffer = callbacks.GetNestedType("BufferCallback", BindingFlags.NonPublic).GetMethod("Invoke");
            Assert.AreEqual(typeof(IntPtr), buffer.ReturnType);
            NativeBindingAssertions.HasParameterTypes(buffer, typeof(IntPtr), typeof(IntPtr), typeof(IntPtr),
                typeof(UIntPtr), typeof(ulong), typeof(ulong));
            var slaves = callbacks.GetNestedType("SlavesCallback", BindingFlags.NonPublic).GetMethod("Invoke");
            Assert.AreEqual(typeof(UIntPtr), slaves.GetParameters()[3].ParameterType);
            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4, 5 }, Enum.GetValues<DownloadStatus>().Select(s => (int)s));
        }
    }

    [TestFixture]
    public class MediaDownloaderTests : BaseSetup
    {
        MediaDownloader _downloader;
        static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

        [SetUp]
        public void CreateDownloader() => _downloader = new MediaDownloader(_libVLC);

        [TearDown]
        public void DisposeDownloader() => _downloader?.Dispose();

        [Test]
        public async Task DownloadsLocalFileAndRetainsMedia()
        {
            using var media = new Media(LocalAudioFile);
            using var output = new MemoryStream();
            using var request = _downloader.Queue(media, (buffer, length, position, total) =>
            {
                var bytes = new byte[length];
                Marshal.Copy(buffer, bytes, 0, length);
                output.Write(bytes, 0, length);
                return length;
            });
            media.Dispose();
            Assert.AreEqual(DownloadStatus.Finished, await request.Completion.WaitAsync(Timeout));
            CollectionAssert.AreEqual(File.ReadAllBytes(LocalAudioFile), output.ToArray());
            using var retainedMedia = request.GetMedia();
            request.Dispose();
            Assert.That(retainedMedia.Mrl, Does.Contain("sample.mp3"));
        }

        [Test]
        public async Task PartialBufferPausesAndResumesWithoutLosingBytes()
        {
            var paused = NewCompletionSource<bool>();
            using var media = new Media(LocalAudioFile);
            using var output = new MemoryStream();
            bool partial = true;
            using var request = _downloader.Queue(media, (buffer, length, position, total) =>
            {
                var consumed = partial ? length / 2 : length;
                partial = false;
                var bytes = new byte[consumed];
                Marshal.Copy(buffer, bytes, 0, consumed);
                output.Write(bytes, 0, consumed);
                return consumed;
            }, status => { if (status == DownloadStatus.Paused) paused.TrySetResult(true); });
            await paused.Task.WaitAsync(Timeout);
            Assert.AreEqual(DownloadStatus.Paused, request.Status);
            request.SetPause(false);
            Assert.AreEqual(DownloadStatus.Finished, await request.Completion.WaitAsync(Timeout));
            CollectionAssert.AreEqual(File.ReadAllBytes(LocalAudioFile), output.ToArray());
        }

        [TestCase(-2, DownloadStatus.Cancelled)]
        [TestCase(-1, DownloadStatus.Error)]
        public async Task BufferCanTerminateDownload(int result, DownloadStatus expected)
        {
            using var media = new Media(LocalAudioFile);
            using var request = _downloader.Queue(media, (buffer, length, position, total) => result);
            Assert.AreEqual(expected, await request.Completion.WaitAsync(Timeout));
            Assert.Zero(request.Cancel());
            Assert.DoesNotThrow(() => request.SetPause(false));
        }

        [Test]
        public void BufferExceptionFaultsCompletion()
        {
            var failure = new IOException("Destination failed");
            using var media = new Media(LocalAudioFile);
            using var request = _downloader.Queue(media, (buffer, length, position, total) => throw failure);
            Assert.AreSame(failure, Assert.ThrowsAsync<IOException>(async () => await request.Completion.WaitAsync(Timeout)));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void InvalidBufferResultFaultsCompletion(bool negative)
        {
            using var media = new Media(LocalAudioFile);
            using var request = _downloader.Queue(media, (buffer, length, position, total) => negative ? -3 : length + 1);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await request.Completion.WaitAsync(Timeout));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task CancelsPausedRequest(bool disposeDownloader)
        {
            var paused = NewCompletionSource<bool>();
            using var media = new Media(LocalAudioFile);
            using var request = _downloader.Queue(media, (buffer, length, position, total) => 0,
                status => { if (status == DownloadStatus.Paused) paused.TrySetResult(true); });
            await paused.Task.WaitAsync(Timeout);
            if (disposeDownloader) _downloader.Dispose();
            else Assert.AreEqual(1, _downloader.CancelAll());
            Assert.AreEqual(DownloadStatus.Cancelled, await request.Completion.WaitAsync(Timeout));
        }

        [Test]
        public async Task DisposingRequestCancelsDownload()
        {
            var paused = NewCompletionSource<bool>();
            using var media = new Media(LocalAudioFile);
            using var request = _downloader.Queue(media, (buffer, length, position, total) => 0,
                status => { if (status == DownloadStatus.Paused) paused.TrySetResult(true); });
            await paused.Task.WaitAsync(Timeout);
            request.Dispose();
            Assert.AreEqual(DownloadStatus.Cancelled, await request.Completion.WaitAsync(Timeout));
            Assert.Throws<ObjectDisposedException>(() => request.SetPause(false));
        }

        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public async Task CallbackCanGetMediaDuringDisposal(bool stateCallback, bool disposeDownloader)
        {
            var paused = NewCompletionSource<bool>();
            var callbackEntered = NewCompletionSource<bool>();
            var callbackResult = NewCompletionSource<Media>();
            using var disposalStarted = new ManualResetEventSlim();
            using var media = new Media(LocalAudioFile);
            Action inspectMedia = null;
            using var request = _downloader.Queue(media, (buffer, length, position, total) =>
            {
                if (!stateCallback) Interlocked.Exchange(ref inspectMedia, null)?.Invoke();
                return 0;
            }, status =>
            {
                if (status == DownloadStatus.Paused) paused.TrySetResult(true);
                if (stateCallback && status == DownloadStatus.Running)
                    Interlocked.Exchange(ref inspectMedia, null)?.Invoke();
            });
            await paused.Task.WaitAsync(Timeout);
            Volatile.Write(ref inspectMedia, () =>
            {
                callbackEntered.TrySetResult(true);
                try
                {
                    if (!disposalStarted.Wait(Timeout))
                        throw new TimeoutException("The disposal thread did not start");

                    var gateHeld = SpinWait.SpinUntil(() =>
                    {
                        if (!Monitor.TryEnter(request.HandleGate)) return true;
                        Monitor.Exit(request.HandleGate);
                        return false;
                    }, TimeSpan.FromSeconds(1));
                    if (gateHeld)
                        throw new InvalidOperationException(
                            "Disposal holds HandleGate while waiting for the native callback; GetMedia would deadlock");

                    callbackResult.TrySetResult(request.GetMedia());
                }
                catch (Exception exception) { callbackResult.TrySetException(exception); }
            });
            request.SetPause(false);
            await callbackEntered.Task.WaitAsync(Timeout);
            var disposal = Task.Run(() =>
            {
                disposalStarted.Set();
                if (disposeDownloader) _downloader.Dispose();
                else request.Dispose();
            });

            await disposal.WaitAsync(Timeout);
            Assert.AreEqual(DownloadStatus.Cancelled, await request.Completion.WaitAsync(Timeout));
            using var retainedMedia = await callbackResult.Task.WaitAsync(Timeout);
            Assert.That(retainedMedia.Mrl, Does.Contain("sample.mp3"));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task CallbackCanRetainMedia(bool stateCallback)
        {
            var paused = NewCompletionSource<bool>();
            var callbackResult = NewCompletionSource<Media>();
            using var media = new Media(LocalAudioFile);
            Action inspectMedia = null;
            using var request = _downloader.Queue(media, (buffer, length, position, total) =>
            {
                if (!stateCallback) Interlocked.Exchange(ref inspectMedia, null)?.Invoke();
                return 0;
            }, status =>
            {
                if (status == DownloadStatus.Paused) paused.TrySetResult(true);
                if (stateCallback && status == DownloadStatus.Running)
                    Interlocked.Exchange(ref inspectMedia, null)?.Invoke();
            });
            await paused.Task.WaitAsync(Timeout);
            Volatile.Write(ref inspectMedia, () =>
            {
                try { callbackResult.TrySetResult(request.GetMedia()); }
                catch (Exception exception) { callbackResult.TrySetException(exception); }
            });
            request.SetPause(false);
            using var retainedMedia = await callbackResult.Task.WaitAsync(Timeout);
            request.Dispose();
            await request.Completion.WaitAsync(Timeout);
            media.Dispose();
            _downloader.Dispose();
            Assert.That(retainedMedia.Mrl, Does.Contain("sample.mp3"));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetMediaThrowsAfterDisposal(bool disposeDownloader)
        {
            using var media = new Media(LocalAudioFile);
            using var request = _downloader.Queue(media, (buffer, length, position, total) => length);
            await request.Completion.WaitAsync(Timeout);
            if (disposeDownloader) _downloader.Dispose();
            else request.Dispose();
            Assert.Throws<ObjectDisposedException>(() => request.GetMedia());
        }

        [Test]
        public async Task SlaveDescriptionsSurviveCallback()
        {
            var uri = new Uri(LocalAudioFileSpecialCharacter).AbsoluteUri;
            using var media = new Media(LocalAudioFile);
            Assert.True(media.AddSlave(MediaSlaveType.Audio, 4, uri));
            MediaSlave[] slaves = null;
            using var request = _downloader.Queue(media, (buffer, length, position, total) => length,
                slaves: result => slaves = result);
            Assert.AreEqual(DownloadStatus.Finished, await request.Completion.WaitAsync(Timeout));
            request.Dispose();
            Assert.NotNull(slaves);
            Assert.That(slaves.Any(slave => slave.Uri == uri && slave.Type == MediaSlaveType.Audio));
        }

        [Test]
        public async Task PlaylistReportsSubitems()
        {
            var playlist = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".m3u");
            Media subitem = null;
            try
            {
                File.WriteAllText(playlist, "#EXTM3U\n" + new Uri(LocalAudioFile).AbsoluteUri + "\n");
                using var media = new Media(playlist);
                using var request = _downloader.Queue(media, (buffer, length, position, total) => length,
                    subitems: list => subitem = list[0]);
                Assert.AreEqual(DownloadStatus.Error, await request.Completion.WaitAsync(Timeout));
                Assert.NotNull(subitem);
                Assert.AreEqual(Uri.UnescapeDataString(new Uri(LocalAudioFile).AbsoluteUri), Uri.UnescapeDataString(subitem.Mrl));
            }
            finally
            {
                subitem?.Dispose();
                File.Delete(playlist);
            }
        }

        [Test]
        public async Task MissingSourceReportsError()
        {
            using var media = new Media(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp3"));
            using var request = _downloader.Queue(media, (buffer, length, position, total) => length);
            Assert.AreEqual(DownloadStatus.Error, await request.Completion.WaitAsync(Timeout));
        }
    }
}
