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
            var instance = new Instance();
            var mediaList = new MediaList(instance);
            var media = new Media(instance, Path.GetTempFileName(), Media.FromType.FromPath);
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
            var mediaList = new MediaList(new Instance());
            mediaList.Lock();
            Assert.Throws<InvalidOperationException>(() => mediaList.Lock(), "already locked");
        }

        [Test]
        public void ReleaseLockTwiceThrows()
        {
            var mediaList = new MediaList(new Instance());
            mediaList.Lock();
            mediaList.Unlock();
            Assert.Throws<InvalidOperationException>(() => mediaList.Unlock(), "not locked");
        }
    }
}
