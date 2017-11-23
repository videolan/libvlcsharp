using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using VideoLAN.LibVLC;
using VideoLAN.LibVLC.Manual;

namespace Bindings.Tests
{
    [TestFixture]
    public class MediaTests
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
        public void CreateRealMedia()
        {
            var instance = new Instance();
            var media = new Media(instance, RealMediaPath, Media.FromType.FromPath);
            
            Assert.False(media.IsParsed);
            media.Parse();

            //await media.ParseAsync();
            Assert.True(media.IsParsed);
            Assert.NotZero(media.Duration);
            Assert.NotZero(media.Tracks.First().Data.Audio.Channels);
            Assert.AreEqual(Media.MediaParsedStatus.Done, media.ParsedStatus);
            Assert.AreEqual(Media.MediaType.File, media.Type);
        }

        string RealMediaPath
        {
            get
            {
                var dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                var binDir = Path.Combine(dir, "..\\..\\..\\");
                var files = Directory.GetFiles(binDir);
                return files.First();
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
            // TODO: fix this.
            var media = new Media(new Instance(), new FileStream(RealMediaPath, FileMode.Open, FileAccess.Read, FileShare.Read));
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
    }
}
