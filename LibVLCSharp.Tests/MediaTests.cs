using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaTests : BaseSetup
    {
        [Test]
        public void CreateMedia()
        {
            var media = new Media(_libVLC, Path.GetTempFileName());

            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFail()
        {
            Assert.Throws<ArgumentNullException>(() => new Media(null, Path.GetTempFileName()));
            Assert.Throws<ArgumentNullException>(() => new Media(_libVLC, string.Empty));
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
            var media = new Media(_libVLC, new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void AddOption()
        {
            var media = new Media(_libVLC, new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            media.AddOption("-sout-all");
        }

        [Test]
        public async Task CreateRealMedia()
        {
            using (var media = new Media(_libVLC, RealStreamMediaPath, FromType.FromLocation))
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
            var media = new Media(_libVLC, new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            var duplicate = media.Duplicate();
            Assert.AreNotEqual(duplicate.NativeReference, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromFileStream()
        {
            var media = new Media(_libVLC, new FileStream(RealMp3Path, FileMode.Open, FileAccess.Read, FileShare.Read));
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void SetMetadata()
        {
            var media = new Media(_libVLC, Path.GetTempFileName());
            const string test = "test";
            media.SetMeta(MetadataType.ShowName, test);
            Assert.True(media.SaveMeta());
            Assert.AreEqual(test, media.Meta(MetadataType.ShowName));
        }

        [Test]
        public async Task GetTracks()
        {
            var media = new Media(_libVLC, RealMp3Path);
            await media.Parse();
            Assert.AreEqual(media.Tracks.Single().Data.Audio.Channels, 2);
            Assert.AreEqual(media.Tracks.Single().Data.Audio.Rate, 44100);
        }

        [Test]
        public async Task CreateRealMediaSpecialCharacters()
        {
            using (var media = new Media(_libVLC, RealMp3PathSpecialCharacter, FromType.FromPath))
            {
                Assert.False(media.IsParsed);

                await media.Parse();
                await Task.Delay(5000);
                Assert.True(media.IsParsed);
                Assert.AreEqual(MediaParsedStatus.Done, media.ParsedStatus);
                using (var mp = new MediaPlayer(media))
                {
                    Assert.True(mp.Play());
                    await Task.Delay(10000);
                    mp.Stop();
                }
            }
        }

        [Test]
        public async Task CreateMediaFromStreamMultiplePlay()
        {
            using(var mp = new MediaPlayer(_libVLC))
            {
                var media = new Media(_libVLC, await GetStreamFromUrl("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4"));
                mp.Play(media);

                await Task.Delay(1000);

                mp.Time = 60000;

                await Task.Delay(10000); // end reached, rewind stream

                mp.Play(media);
            }
        }

        [Test]
        public async Task CreateMultipleMediaFromStreamPlay()
        {
            var libVLC1 = new LibVLC("--no-audio", "--no-video");
            var libVLC2 = new LibVLC("--no-audio", "--no-video");

            var mp1 = new MediaPlayer(libVLC1);
            var mp2 = new MediaPlayer(libVLC2);

            mp1.Play(new Media(libVLC1, await GetStreamFromUrl("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4")));
            mp2.Play(new Media(libVLC2, await GetStreamFromUrl("https://streams.videolan.org/streams/mp3/05-Mr.%20Zebra.mp3")));

            await Task.Delay(10000);
        }

        [Test]
        public void ParseShouldThrowIfCancelledOperation()
        {
            var media = new Media(_libVLC, RealMp3Path);
            var cancellationToken = new CancellationToken(canceled: true);
            Assert.ThrowsAsync<TaskCanceledException>(async () => await media.Parse(cancellationToken: cancellationToken));
        }

        [Test]
        public async Task ParseShouldTimeoutWith1MillisecondLimit()
        {
            var media = new Media(_libVLC, RealMp3Path);
            var parseResult = await media.Parse(timeout: 1);
            Assert.AreEqual(MediaParsedStatus.Timeout, parseResult);
        }

        [Test]
        public async Task ParseShouldSucceed()
        {
            var media = new Media(_libVLC, RealMp3Path);
            var parseResult = await media.Parse();
            Assert.AreEqual(MediaParsedStatus.Done, parseResult);
        }

        [Test]
        public async Task ParseShouldFailIfNotMediaFile()
        {
            var media = new Media(_libVLC, Path.GetTempFileName());
            var parseResult = await media.Parse();
            Assert.AreEqual(MediaParsedStatus.Failed, parseResult);
        }

        [Test]
        public async Task ParseShouldBeSkippedIfLocalParseSpecifiedAndRemoteUrlProvided()
        {
            var media = new Media(_libVLC, RealStreamMediaPath, FromType.FromLocation);
            var parseResult = await media.Parse(MediaParseOptions.ParseLocal);
            Assert.AreEqual(MediaParsedStatus.Skipped, parseResult);
        }

        private async Task<Stream> GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

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