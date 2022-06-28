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
        [Test]
        public void CreateMedia()
        {
            using var media = new Media(_libVLC, Path.GetTempFileName());

            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromUri()
        {
            var media = new Media(_libVLC, new Uri(RemoteAudioStream, UriKind.Absolute));
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFail()
        {
            Assert.Throws<ArgumentNullException>(() => new Media(null, Path.GetTempFileName()));
            Assert.Throws<ArgumentNullException>(() => new Media(_libVLC, string.Empty));
            Assert.Throws<InvalidOperationException>(() => new Media(_libVLC, new Uri("/hello.mp4", UriKind.Relative)));
            Assert.Throws<ArgumentNullException>(() => new Media(_libVLC, uri: null!));
        }

        [Test]
        public void ReleaseMedia()
        {
            var media = new Media(_libVLC, Path.GetTempFileName());

            media.Dispose();

            Assert.AreEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromStream()
        {
            using var stream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(_libVLC, input);
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void AddOption()
        {
            using var stream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(_libVLC, input);
            media.AddOption("-sout-all");
        }

        [Test]
        public async Task CreateRealMedia()
        {
            using (var media = new Media(_libVLC, RemoteAudioStream, FromType.FromLocation))
            {
                Assert.NotZero(media.Duration);
                using (var mp = new MediaPlayer(media))
                {
                    Assert.True(mp.Play());
                    await Task.Delay(4000); // have to wait a bit for statistics to populate
                    Assert.Greater(media.Statistics.DemuxBitrate, 0);
                    mp.Stop();
                }
            }
        }

        [Test]
        public async Task CreateRealMediaFromUri()
        {
            using (var media = new Media(_libVLC, new Uri(RemoteAudioStream, UriKind.Absolute)))
            {
                Assert.NotZero(media.Duration);
                using (var mp = new MediaPlayer(media))
                {
                    Assert.True(mp.Play());
                    await Task.Delay(4000); // have to wait a bit for statistics to populate
                    Assert.Greater(media.Statistics.DemuxBitrate, 0);
                    mp.Stop();
                }
            }
        }

        [Test]
        public void Duplicate()
        {
            using var stream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(_libVLC, input);
            var duplicate = media.Duplicate();
            Assert.AreNotEqual(duplicate.NativeReference, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromFileStream()
        {
            using var stream = new FileStream(LocalAudioFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(_libVLC, input);
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void SetMetadata()
        {
            using var media = new Media(_libVLC, Path.GetTempFileName());
            const string test = "test";
            media.SetMeta(MetadataType.ShowName, test);
            Assert.True(media.SaveMeta());
            Assert.AreEqual(test, media.Meta(MetadataType.ShowName));
        }

        [Test]
        public async Task GetTracks()
        {
            using var media = new Media(_libVLC, LocalAudioFile);
            using var mp = new MediaPlayer(media);
            await media.ParseAsync();
            await mp.PlayAsync();
            using var audioTracks = mp.Tracks(TrackType.Audio);
            using var track = audioTracks?[0];
            Assert.AreEqual(track?.Data.Audio.Channels, 2);
            Assert.AreEqual(track?.Data.Audio.Rate, 44100);
        }

        [Test]
        public async Task CreateRealMediaSpecialCharacters()
        {
            using (var media = new Media(_libVLC, LocalAudioFileSpecialCharacter, FromType.FromPath))
            {
                await media.ParseAsync();
                Assert.AreEqual(MediaParsedStatus.Done, media.ParsedStatus);
            }
        }

        [Test]
        public async Task CreateMediaFromStreamMultiplePlay()
        {
            using var mp = new MediaPlayer(_libVLC);
            using var stream = await GetStreamFromUrl(RemoteVideoStream);
            using var mediaInput = new StreamMediaInput(stream);
            using var media = new Media(_libVLC, mediaInput);
            mp.Play(media);

            await Task.Delay(1000);

            mp.SetTime(60000);

            await Task.Delay(10000); // end reached, rewind stream

            mp.Play(media);
        }

        [Test]
        public async Task CreateMultipleMediaFromStreamPlay()
        {
            var libVLC1 = new LibVLC("--no-audio", "--no-video");
            var libVLC2 = new LibVLC("--no-audio", "--no-video");

            var mp1 = new MediaPlayer(libVLC1);
            var mp2 = new MediaPlayer(libVLC2);

            using var s1 = await GetStreamFromUrl(RemoteVideoStream);
            using var s2 = await GetStreamFromUrl("https://streams.videolan.org/streams/mp3/05-Mr.%20Zebra.mp3");

            using var i1 = new StreamMediaInput(s1);
            using var i2 = new StreamMediaInput(s2);

            var m1 = new Media(libVLC1, i1);
            var m2 = new Media(libVLC2, i2);

            mp1.Play(m1);
            m1.Dispose();
            mp2.Play(m2);
            m2.Dispose();

            await Task.Delay(10000);
        }

        [Test]
        public void ParseShouldThrowIfCancelledOperation()
        {
            using var media = new Media(_libVLC, LocalAudioFile);
            var cancellationToken = new CancellationToken(canceled: true);
            Assert.ThrowsAsync<OperationCanceledException>(async () => await media.ParseAsync(cancellationToken: cancellationToken));
        }

        [Test]
        public async Task ParseShouldTimeoutWith1MillisecondLimit()
        {
            using var media = new Media(_libVLC, LocalAudioFile);
            var parseResult = await media.ParseAsync(timeout: 1);
            Assert.AreEqual(MediaParsedStatus.Timeout, parseResult);
        }

        [Test]
        public async Task ParseShouldSucceed()
        {
            using var media = new Media(_libVLC, LocalAudioFile);
            var parseResult = await media.ParseAsync();
            Assert.AreEqual(MediaParsedStatus.Done, parseResult);
        }

        [Test]
        public async Task ParseShouldFailIfNotMediaFile()
        {
            using var media = new Media(_libVLC, Path.GetTempFileName());
            var parseResult = await media.ParseAsync();
            Assert.AreEqual(MediaParsedStatus.Failed, parseResult);
        }

        [Test]
        public async Task ParseShouldBeSkippedIfLocalParseSpecifiedAndRemoteUrlProvided()
        {
            using var media = new Media(_libVLC, RemoteAudioStream, FromType.FromLocation);
            var parseResult = await media.ParseAsync(MediaParseOptions.ParseLocal);
            Assert.AreEqual(MediaParsedStatus.Skipped, parseResult);
        }

        [Test]
        public async Task MediaPictureTest()
        {
            using var media = new Media(_libVLC, RemoteVideoStream, FromType.FromLocation);
            await media.ParseAsync(MediaParseOptions.ParseNetwork);

            using var thumbnail = await media.GenerateThumbnailAsync(_libVLC, media.Duration / 2, ThumbnailerSeekSpeed.Precise, 200, 200, false, PictureType.Png);
            using var thumbnail2 = await media.GenerateThumbnailAsync(_libVLC, 5.0f, ThumbnailerSeekSpeed.Precise, 200, 200, false, PictureType.Png);
        }

        [Test]
        public async Task MediaFileStat()
        {
            using var media = new Media(_libVLC, new Uri(Directory.GetParent(typeof(MediaTests).Assembly.Location).FullName));
            await media.ParseAsync();

            var sample = media.SubItems.Single(m => m.Mrl.EndsWith("sample.mp3"));
            sample.FileStat(FileStat.Mtime, out var mtime);
            Assert.AreEqual(1618993056, mtime);

            sample.FileStat(FileStat.Size, out var size);
            Assert.AreEqual(24450, size);
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
