﻿using System;
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
            using var media = new Media(_libVLC, Path.GetTempFileName());
            Assert.That(media.NativeReference, Is.Not.EqualTo(IntPtr.Zero));
        }

        [Test]
        public void CreateMediaFromUri()
        {
            var media = new Media(_libVLC, new Uri(RealStreamMediaPath, UriKind.Absolute));
            Assert.That(media.NativeReference, Is.Not.EqualTo(IntPtr.Zero));
        }

        [Test]
        public void CreateMediaFail()
        {
            Assert.Throws<ArgumentNullException>(() => new Media(null!, Path.GetTempFileName()));
            Assert.Throws<ArgumentNullException>(() => new Media(_libVLC, string.Empty));
            Assert.Throws<InvalidOperationException>(() => new Media(_libVLC, new Uri("/hello.mp4", UriKind.Relative)));
            Assert.Throws<ArgumentNullException>(() => new Media(_libVLC, uri: null!));
        }

        [Test]
        public void ReleaseMedia()
        {
            var media = new Media(_libVLC, Path.GetTempFileName());

            media.Dispose();

            Assert.That(IntPtr.Zero == media.NativeReference);
        }

        [Test]
        public void CreateMediaFromStream()
        {
            using var stream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(_libVLC, input);
            Assert.That(media.NativeReference, Is.Not.EqualTo(IntPtr.Zero));
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
            using (var media = new Media(_libVLC, RealStreamMediaPath, FromType.FromLocation))
            {
                Assert.That(media.Duration, Is.Not.Zero);
                using (var mp = new MediaPlayer(media))
                {
                    Assert.That(mp.Play());
                    await Task.Delay(4000); // have to wait a bit for statistics to populate
                    Assert.That(media.Statistics.DemuxBitrate, Is.GreaterThan(0));
                    mp.Stop();
                }
            }
        }

        [Test]
        public async Task CreateRealMediaFromUri()
        {
            using (var media = new Media(_libVLC, new Uri(RealStreamMediaPath, UriKind.Absolute)))
            {
                Assert.That(media.Duration, Is.Not.Zero);
                using (var mp = new MediaPlayer(media))
                {
                    Assert.That(mp.Play());
                    await Task.Delay(4000); // have to wait a bit for statistics to populate
                    Assert.That(media.Statistics.DemuxBitrate, Is.GreaterThan(0));
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
            Assert.That(media.NativeReference, Is.Not.EqualTo(duplicate.NativeReference));
        }

        [Test]
        public void CreateMediaFromFileStream()
        {
            using var stream = new FileStream(RealMp3Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var input = new StreamMediaInput(stream);
            using var media = new Media(_libVLC, input);
            Assert.That(media.NativeReference, Is.Not.EqualTo(IntPtr.Zero));
        }

        [Test]
        public void SetMetadata()
        {
            using var media = new Media(_libVLC, Path.GetTempFileName());
            const string test = "test";
            media.SetMeta(MetadataType.ShowName, test);
            Assert.That(media.SaveMeta());
            Assert.That(test == media.Meta(MetadataType.ShowName));
        }

        [Test]
        [Ignore("Crashes test runner on CI")]
        public async Task GetTracks()
        {
            using var media = new Media(_libVLC, RealMp3Path);
            await media.Parse();
            Assert.That(media.Tracks.Single().Data.Audio.Channels == 2);
            Assert.That(media.Tracks.Single().Data.Audio.Rate == 44100);
        }

        [Test]
        public async Task CreateRealMediaSpecialCharacters()
        {
            using (var media = new Media(_libVLC, RealMp3PathSpecialCharacter, FromType.FromPath))
            {
                Assert.That(media.IsParsed, Is.False);

                await media.Parse();
                await Task.Delay(5000);
                Assert.That(media.IsParsed);
                Assert.That(MediaParsedStatus.Done == media.ParsedStatus);
                using (var mp = new MediaPlayer(media))
                {
                    Assert.That(mp.Play());
                    await Task.Delay(1000);
                    mp.Stop();
                }
            }
        }

        [Test]
        public async Task CreateMediaFromStreamMultiplePlay()
        {
            using var mp = new MediaPlayer(_libVLC);
            using var stream = await GetStreamFromUrl(RealStreamMediaPath);
            using var mediaInput = new StreamMediaInput(stream);
            using var media = new Media(_libVLC, mediaInput);
            mp.Play(media);

            await Task.Delay(1000);

            mp.Time = 60000;

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

            using var s1 = await GetStreamFromUrl(RealStreamMediaPath);
            using var s2 = await GetStreamFromUrl(RealStreamMediaPath);

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
            using var media = new Media(_libVLC, RealMp3Path);
            var cancellationToken = new CancellationToken(canceled: true);
            Assert.ThrowsAsync<OperationCanceledException>(async () => await media.Parse(cancellationToken: cancellationToken));
        }

        [Test]
        [Ignore("Crashes test runner on CI")]
        public async Task ParseShouldTimeoutWith1MillisecondLimit()
        {
            using var media = new Media(_libVLC, RealMp3Path);
            var parseResult = await media.Parse(timeout: 1);
            Assert.That(MediaParsedStatus.Timeout == parseResult);
        }

        [Test]
        [Ignore("Crashes test runner on CI")]
        public async Task ParseShouldSucceed()
        {
            using var media = new Media(_libVLC, RealMp3Path);
            var parseResult = await media.Parse();
            Assert.That(MediaParsedStatus.Done == parseResult);
        }

        [Test]
        [Ignore("Crashes test runner on CI")]
        public async Task ParseShouldFailIfNotMediaFile()
        {
            using var media = new Media(_libVLC, Path.GetTempFileName());
            var parseResult = await media.Parse();
            Assert.That(MediaParsedStatus.Failed == parseResult);
        }

        [Test]
        public async Task ParseShouldBeSkippedIfLocalParseSpecifiedAndRemoteUrlProvided()
        {
            using var media = new Media(_libVLC, RealStreamMediaPath, FromType.FromLocation);
            var parseResult = await media.Parse(MediaParseOptions.ParseLocal);
            Assert.That(MediaParsedStatus.Skipped == parseResult);
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
