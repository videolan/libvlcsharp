using System;
using System.IO;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaListTests : BaseSetup
    {
        [Test]
        public void AddAndRemoveMediaFromMediaList()
        {
            var libVLC = new LibVLC();
            var mediaList = new MediaList(libVLC);
            var media = new Media(libVLC, Path.GetTempFileName());
            var itemAdded = false;
            var itemDeleted = false;
            mediaList.EventManager.ItemAdded += (sender, args) => itemAdded = true;
            mediaList.EventManager.ItemDeleted += (sender, args) => itemDeleted = true;
            mediaList.Lock();
            mediaList.AddMedia(media);
            Assert.AreEqual(media, mediaList[0]);
            Assert.AreEqual(1, mediaList.Count);
            Assert.True(itemAdded);
            Assert.Zero(mediaList.IndexOf(media));
            mediaList.RemoveIndex(0);
            Assert.Zero(mediaList.Count);
            Assert.True(itemDeleted);
            mediaList.Unlock();
        }

        [Test]
        public void AcquireLockTwiceThrows()
        {
            var mediaList = new MediaList(new LibVLC());
            mediaList.Lock();
            Assert.Throws<InvalidOperationException>(() => mediaList.Lock(), "already locked");
        }

        [Test]
        public void ReleaseLockTwiceThrows()
        {
            var mediaList = new MediaList(new LibVLC());
            mediaList.Lock();
            mediaList.Unlock();
            Assert.Throws<InvalidOperationException>(() => mediaList.Unlock(), "not locked");
        }
    }
}
