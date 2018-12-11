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
            var libVLC = new LibVLC();

            var media = new Media(libVLC, Path.GetTempFileName());

            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFail()
        {
            Assert.Throws<ArgumentNullException>(() => new Media(null, Path.GetTempFileName()));
            Assert.Throws<ArgumentNullException>(() => new Media(new LibVLC(), string.Empty));
        }

        [Test]
        public void ReleaseMedia()
        {
            var media = new Media(new LibVLC(), Path.GetTempFileName());

            media.Dispose();

            Assert.AreEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromStream()
        {
            var media = new Media(new LibVLC(), new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void AddOption()
        {
            var media = new Media(new LibVLC(), new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            media.AddOption("-sout-all");
        }

        [Test]
        public async Task CreateRealMedia()
        {
            using (var libVLC = new LibVLC())
            {
                using (var media = new Media(libVLC, RealStreamMediaPath, Media.FromType.FromLocation))
                {
                    Assert.False(media.IsParsed);
                    media.Parse();

                    Assert.True(media.IsParsed);
                    Assert.AreEqual(Media.MediaParsedStatus.Done, media.ParsedStatus);
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
        }

        [Test]
        public void Duplicate()
        {
            var media = new Media(new LibVLC(), new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            var duplicate = media.Duplicate();
            Assert.AreNotEqual(duplicate.NativeReference, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromFileStream()
        {
            var media = new Media(new LibVLC(), new FileStream(RealMp3Path, FileMode.Open, FileAccess.Read, FileShare.Read));
            media.Parse();
            Assert.NotZero(media.Tracks.First().Data.Audio.Channels);
        }

        [Test]
        public void SetMetadata()
        {
            var media = new Media(new LibVLC(), Path.GetTempFileName());
            const string test = "test";
            media.SetMeta(Media.MetadataType.ShowName, test);
            Assert.True(media.SaveMeta());
            Assert.AreEqual(test, media.Meta(Media.MetadataType.ShowName));
        }

        [Test]
        public void GetTracks()
        {
            var media = new Media(new LibVLC(), RealMp3Path);
            media.Parse();
            Assert.AreEqual(1, media.Tracks);
        }

        [Test]
        public async Task CreateRealMediaSpecialCharacters()
        {
            using (var libVLC = new LibVLC())
            {
                libVLC.Log += LibVLC_Log;
                using (var media = new Media(libVLC, RealMp3PathSpecialCharacter, Media.FromType.FromPath))
                {
                    Assert.False(media.IsParsed);

                    media.Parse();
                    await Task.Delay(5000);
                    Assert.True(media.IsParsed);
                    Assert.AreEqual(Media.MediaParsedStatus.Done, media.ParsedStatus);
                    using (var mp = new MediaPlayer(media))
                    {
                        Assert.True(mp.Play());
                        await Task.Delay(10000);
                        mp.Stop();
                    }
                }
                libVLC.Log -= LibVLC_Log;
            }
        }

        [Test]
        public async Task CreateMediaFromStreamMultiplePlay()
        {
            using (var libVLC = new LibVLC())
            using(var mp = new MediaPlayer(libVLC))
            {
                var media = new Media(libVLC, await GetStreamFromUrl("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4"));
                mp.Play(media);

                await Task.Delay(1000);

                mp.Time = 60000;

                await Task.Delay(10000); // end reached, rewind stream

                mp.Play(media);
            }
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