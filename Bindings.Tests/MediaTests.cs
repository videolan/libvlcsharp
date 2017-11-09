using System;
using System.IO;
using NUnit.Framework;
using VideoLAN.LibVLC;

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
    }
}
