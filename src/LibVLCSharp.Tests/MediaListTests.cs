using System;
using System.IO;
using LibVLCSharp;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaListTests : BaseSetup
    {
        [Test]
        public void AddAndRemoveMediaFromMediaList()
        {
            var mediaList = new MediaList();
            var media = new Media(Path.GetTempFileName());
            mediaList.AddMedia(media);
            using (var item = mediaList.ItemAt(0))
                Assert.AreEqual(media, item);
            Assert.AreEqual(1, mediaList.Count);
            Assert.Zero(mediaList.IndexOf(media));
            mediaList.RemoveIndex(0);
            Assert.Zero(mediaList.Count);
        }

        [Test]
        public void ItemAtReturnsIndependentReference()
        {
            using var mediaList = new MediaList();
            using var media = new Media(Path.GetTempFileName());
            mediaList.AddMedia(media);

            for (var i = 0; i < 3; i++)
            {
                using var item = mediaList.ItemAt(0);
                Assert.NotNull(item);
            }

            using var last = mediaList.ItemAt(0);
            Assert.AreEqual(media.Mrl, last.Mrl);
            Assert.IsNull(mediaList.ItemAt(1));
        }

        [Test]
        public void GetAssociatedMedia()
        {
            using var mediaList = new MediaList();
            Assert.IsNull(mediaList.GetAssociatedMedia());

            using var media = new Media(Path.GetTempFileName());
            mediaList.SetMedia(media);
            using var associated = mediaList.GetAssociatedMedia();
            Assert.AreEqual(media, associated);
        }

        [Test]
        public void DisposeMediaList()
        {
            var mediaList = new MediaList();
            mediaList.Dispose();
            Assert.AreEqual(IntPtr.Zero, mediaList.NativeReference);
        }
    }
}
