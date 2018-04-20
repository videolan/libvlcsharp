using System;
using System.IO;
using System.Linq;
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
            var instance = new Instance();

            var media = new Media(instance, Path.GetTempFileName(), Media.FromType.FromPath);

            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFail()
        {
            Assert.Throws<ArgumentNullException>(() => new Media(null, Path.GetTempFileName(), Media.FromType.FromPath));
            Assert.Throws<ArgumentNullException>(() => new Media(new Instance(), string.Empty, Media.FromType.FromPath));
        }

        [Test]
        public void ReleaseMedia()
        {
            var media = new Media(new Instance(), Path.GetTempFileName(), Media.FromType.FromPath);

            media.Dispose();

            Assert.AreEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromStream()
        {
            var media = new Media(new Instance(), new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            Assert.AreNotEqual(IntPtr.Zero, media.NativeReference);
        }

        [Test]
        public void AddOption()
        {
            var media = new Media(new Instance(), new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            media.AddOption("-sout-all");
        }

        [Test]
        public async Task CreateRealMedia()
        {
            using (var instance = new Instance())
            {
                using (var media = new Media(instance, RealStreamMediaPath, Media.FromType.FromLocation))
                {
                    Assert.False(media.IsParsed);
                    media.Parse();

                    Assert.True(media.IsParsed);
                    Assert.NotZero(media.Duration);
                    using (var mp = new MediaPlayer(media))
                    {
                        Assert.True(mp.Play());
                        await Task.Delay(3000); // have to wait a bit for statistics to populate
                        Assert.Greater(media.Statistics.DemuxBitrate, 0);
                        mp.Stop();
                    }
                }
            }
        }

        [Test]
        public void Duplicate()
        {
            var media = new Media(new Instance(), new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate));
            var duplicate = media.Duplicate();
            Assert.AreNotEqual(duplicate.NativeReference, media.NativeReference);
        }

        [Test]
        public void CreateMediaFromFileStream()
        {
            Assert.Fail();
            // TODO: fix this.
            var media = new Media(new Instance(), new FileStream("realFileUri", FileMode.Open, FileAccess.Read, FileShare.Read));
            media.Parse();
            Assert.NotZero(media.Tracks.First().Data.Audio.Channels);
        }

        [Test]
        public void SetMetadata()
        {
            var media = new Media(new Instance(), Path.GetTempFileName(), Media.FromType.FromPath);
            const string test = "test";
            media.SetMeta(Media.MetadataType.ShowName, test);
            Assert.True(media.SaveMeta());
            Assert.AreEqual(test, media.Meta(Media.MetadataType.ShowName));
        }

        [Test]
        public void GetTracks()
        {
            var media = new Media(new Instance(), RealMp3Path);
            media.Parse();
            var tracks = media.Tracks;

        }
    }
}
