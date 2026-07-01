using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaTests : BaseSetup
    {
        const int GetTracksOperationTimeoutMilliseconds = 10000;

        [Test]
        public void CreateMedia()
        {
            using var media = new Media(Path.GetTempFileName());

            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromUri()
        {
            using var media = new Media(new Uri(LocalAudioFile));
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFail()
        {
            Assert.Throws<ArgumentNullException>(() => new Media(string.Empty));
            Assert.Throws<InvalidOperationException>(() => new Media(new Uri("/hello.mp4", UriKind.Relative)));
            Assert.Throws<ArgumentNullException>(() => new Media(uri: null!));
        }

        [Test]
        public void ReleaseMedia()
        {
            var media = new Media(Path.GetTempFileName());

            media.Dispose();

            Assert.AreEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromStream()
        {
            using var stream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(input);
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void MediaNewCallbacksUsesVersionedCallbacksStruct()
        {
            var method = NativeBindingAssertions.NativeMethod(typeof(Media), "LibVLCMediaNewCallbacks");

            NativeBindingAssertions.HasDllImport(method, "libvlc_media_new_callbacks");
            NativeBindingAssertions.HasParameterTypes(method, typeof(IntPtr), typeof(IntPtr));
        }

        [Test]
        public void AddOption()
        {
            using var stream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(input);
            media.AddOption("-sout-all");
        }

        [Test]
        public async Task CreateRealMedia()
        {
            using (var media = new Media(LocalAudioFile, FromType.FromPath))
            {
                var status = await media.ParseAsync(_libVLC);
                Assert.AreEqual(MediaParsedStatus.Done, status);
                Assert.NotZero(media.Duration);
                using (var mp = new MediaPlayer(_libVLC, media))
                {
                    Assert.True(mp.Play());
                    await Task.Delay(1000);
                    mp.Stop();
                }
            }
        }

        [Test]
        public async Task CreateRealMediaFromUri()
        {
            using (var media = new Media(new Uri(LocalAudioFile)))
            {
                var status = await media.ParseAsync(_libVLC);
                Assert.AreEqual(MediaParsedStatus.Done, status);
                Assert.NotZero(media.Duration);
                using (var mp = new MediaPlayer(_libVLC, media))
                {
                    Assert.True(mp.Play());
                    await Task.Delay(1000);
                    mp.Stop();
                }
            }
        }

        [Test]
        public void Duplicate()
        {
            using var stream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(input);
            var duplicate = media.Duplicate();
            Assert.AreNotEqual(duplicate.NativeReference, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromFileStream()
        {
            using var stream = new FileStream(LocalAudioFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(input);
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void SetMetadata()
        {
            using var media = new Media(Path.GetTempFileName());
            const string test = "test";
            media.SetMeta(MetadataType.ShowName, test);
            Assert.AreEqual(test, media.Meta(MetadataType.ShowName));
        }

        [Test]
        public async Task GetTracks()
        {
            ResetGetTracksLog();
            LogGetTracks($"using local audio file '{LocalAudioFile}'");

            Media media = null;
            MediaPlayer mp = null;
            var disposeOnExit = false;

            try
            {
                media = new Media(LocalAudioFile);
                LogGetTracks($"media created, native reference {media.NativeReference}");
                mp = new MediaPlayer(_libVLC, media);
                LogGetTracks($"media player created, native reference {mp.NativeReference}");

                var parseResult = await AwaitGetTracksOperation("media.ParseAsync", media.ParseAsync(_libVLC, timeout: GetTracksOperationTimeoutMilliseconds));
                Assert.AreEqual(MediaParsedStatus.Done, parseResult);

                var playResult = await AwaitGetTracksOperation("mp.PlayAsync", mp.PlayAsync());
                Assert.True(playResult);

                using var audioTracks = await AwaitGetTracksOperation("mp.Tracks(Audio)", Task.Run(() => mp.Tracks(TrackType.Audio)));
                LogGetTracks($"audio track count is {audioTracks?.Count}");
                using var track = audioTracks?[0];
                LogGetTracks($"selected track channels={track?.Data.Audio.Channels}, rate={track?.Data.Audio.Rate}");
                Assert.AreEqual(track?.Data.Audio.Channels, 2);
                Assert.AreEqual(track?.Data.Audio.Rate, 44100);
                Assert.True(await StopWithTimeout(mp));
                disposeOnExit = true;
            }
            finally
            {
                if (disposeOnExit)
                {
                    LogGetTracks("disposing media player and media");
                    mp?.Dispose();
                    media?.Dispose();
                }
                else
                {
                    LogGetTracks("skipping native disposal after failed diagnostic run");
                }
            }
        }

        static async Task<T> AwaitGetTracksOperation<T>(string operation, Task<T> task)
        {
            LogGetTracks($"waiting for {operation} with {GetTracksOperationTimeoutMilliseconds} ms timeout");

            var completedTask = await Task.WhenAny(task, Task.Delay(GetTracksOperationTimeoutMilliseconds)).ConfigureAwait(false);
            if (completedTask != task)
            {
                LogGetTracks($"timed out while waiting for {operation}");
                throw new AssertionException($"GetTracks timed out after {GetTracksOperationTimeoutMilliseconds} ms while waiting for {operation}.");
            }

            try
            {
                var result = await task.ConfigureAwait(false);
                LogGetTracks($"{operation} completed with result '{result}'");
                return result;
            }
            catch (Exception ex)
            {
                LogGetTracks($"{operation} failed with {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }

        static void ResetGetTracksLog()
        {
            var logPath = GetTracksLogPath;
            if (File.Exists(logPath))
                File.Delete(logPath);
        }

        static void LogGetTracks(string message)
        {
            var line = $"{DateTime.UtcNow:O} GetTracks: {message}";
            TestContext.Progress.WriteLine(line);
            File.AppendAllText(GetTracksLogPath, line + Environment.NewLine);
        }

        static string GetTracksLogPath => Path.Combine(TestContext.CurrentContext.WorkDirectory, "GetTracks.log");

        [Test]
        public async Task CreateRealMediaSpecialCharacters()
        {
            using (var media = new Media(LocalAudioFileSpecialCharacter, FromType.FromPath))
            {
                var status = await media.ParseAsync(_libVLC);
                Assert.AreEqual(MediaParsedStatus.Done, status);
                Assert.True(media.IsParsed);
            }
        }

        [Test]
        public async Task CreateMediaFromStreamMultiplePlay()
        {
            using var mp = new MediaPlayer(_libVLC);
            using var stream = new FileStream(LocalAudioFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var mediaInput = new StreamMediaInput(stream);
            using var media = new Media(mediaInput);
            mp.Play(media);

            await Task.Delay(1000);

            mp.SetTime(1000);

            await Task.Delay(1000);

            mp.Play(media);

            Assert.True(await StopWithTimeout(mp));
        }

        [Test]
        public async Task CreateMultipleMediaFromStreamPlay()
        {
            using var libVLC1 = new LibVLC("--aout=dummy", "--vout=dummy");
            using var libVLC2 = new LibVLC("--aout=dummy", "--vout=dummy");

            using var mp1 = new MediaPlayer(libVLC1);
            using var mp2 = new MediaPlayer(libVLC2);

            using var s1 = new FileStream(LocalAudioFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var s2 = new FileStream(LocalAudioFileSpecialCharacter, FileMode.Open, FileAccess.Read, FileShare.Read);

            using var i1 = new StreamMediaInput(s1);
            using var i2 = new StreamMediaInput(s2);

            var m1 = new Media(i1);
            var m2 = new Media(i2);

            mp1.Play(m1);
            m1.Dispose();
            mp2.Play(m2);
            m2.Dispose();

            await Task.Delay(1000);
            Assert.True(await StopWithTimeout(mp1));
            Assert.True(await StopWithTimeout(mp2));
        }

        [Test]
        public void ParseShouldThrowIfCancelledOperation()
        {
            using var media = new Media(LocalAudioFile);
            var cancellationToken = new CancellationToken(canceled: true);
            Assert.ThrowsAsync<OperationCanceledException>(async () => await media.ParseAsync(_libVLC, cancellationToken: cancellationToken));
        }

        [Test]
        public async Task ParseShouldTimeoutWith1MillisecondLimit()
        {
            using var media = new Media(LocalAudioFile);
            var parseResult = await media.ParseAsync(_libVLC, timeout: 1);
            Assert.AreEqual(MediaParsedStatus.Timeout, parseResult);
        }

        [Test]
        public async Task ParseShouldSucceed()
        {
            using var media = new Media(LocalAudioFile);
            var parseResult = await media.ParseAsync(_libVLC);
            Assert.AreEqual(MediaParsedStatus.Done, parseResult);
        }

        [Test]
        public async Task ParseShouldFailIfNotMediaFile()
        {
            using var media = new Media(Path.GetTempFileName());
            var parseResult = await media.ParseAsync(_libVLC);
            Assert.AreEqual(MediaParsedStatus.Failed, parseResult);
        }

        [Test]
        public async Task ParseShouldBeSkippedIfLocalParseSpecifiedAndRemoteUrlProvided()
        {
            using var media = new Media(RemoteAudioStream, FromType.FromLocation);
            var parseResult = await media.ParseAsync(_libVLC, MediaParseOptions.ParseLocal);
            Assert.AreEqual(MediaParsedStatus.Done, parseResult);
        }

        [Test]
        public async Task MediaPictureTest()
        {
            using var media = new Media(RemoteVideoStream, FromType.FromLocation);
            await media.ParseAsync(_libVLC, MediaParseOptions.ParseNetwork);

            using var thumbnail = await media.GenerateThumbnailAsync(_libVLC, media.Duration / 2, ThumbnailerSeekSpeed.Precise, 200, 200, false, PictureType.Png);
            using var thumbnail2 = await media.GenerateThumbnailAsync(_libVLC, 5.0f, ThumbnailerSeekSpeed.Precise, 200, 200, false, PictureType.Png);
        }

        [Test]
        public async Task MediaFileStat()
        {
            using var media = new Media(new Uri(Directory.GetParent(typeof(MediaTests).Assembly.Location).FullName));
            await media.ParseAsync(_libVLC);

            using var subItems = media.SubItems;
            using var sample = subItems.Single(m => m.Mrl.EndsWith("sample.mp3"));
            sample.FileStat(FileStat.Mtime, out var mtime);
            var expectedMtime = new DateTimeOffset(File.GetLastWriteTimeUtc(LocalAudioFile)).ToUnixTimeSeconds();
            Assert.AreEqual((ulong)expectedMtime, mtime);

            sample.FileStat(FileStat.Size, out var size);
            var expectedSize = new FileInfo(LocalAudioFile).Length;
            Assert.AreEqual((ulong)expectedSize, size);
        }

        private async Task<Stream> GetStreamFromUrl(string url)
        {
            byte[] imageData;

            using (var client = new System.Net.Http.HttpClient())
                imageData = await client.GetByteArrayAsync(url);

            return new MemoryStream(imageData);
        }

        private void LibVLC_Log(object sender, LogEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }
}
