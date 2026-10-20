using System;
using System.IO;
using System.Linq;
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
            Assert.AreEqual(media, mediaList[0]);
            Assert.AreEqual(1, mediaList.Count);
            Assert.Zero(mediaList.IndexOf(media));
            mediaList.RemoveIndex(0);
            Assert.Zero(mediaList.Count);
        }

        [Test]
        public void ItemsAreOwnedByTheList()
        {
            var mediaList = new MediaList();
            using var first = new Media(Path.GetTempFileName());
            using var second = new Media(Path.GetTempFileName());
            mediaList.AddMedia(first);
            mediaList.AddMedia(second);

            var item = mediaList[0];
            Assert.AreSame(item, mediaList[0]);
            Assert.AreSame(item, mediaList.First());
            Assert.AreSame(mediaList[1], mediaList.Single(m => m.Mrl == second.Mrl));
            Assert.IsNull(mediaList[2]);

            using var retained = item.Retain();
            mediaList.Dispose();

            Assert.AreEqual(IntPtr.Zero, item.NativeReference);
            Assert.AreEqual(first.Mrl, retained.Mrl);
        }

        [Test]
        public void DisposedItemIsRecreated()
        {
            using var mediaList = new MediaList();
            using var media = new Media(Path.GetTempFileName());
            mediaList.AddMedia(media);

            var item = mediaList[0];
            item.Dispose();

            var again = mediaList[0];
            Assert.AreNotSame(item, again);
            Assert.AreEqual(media, again);
        }

        [Test]
        public void SubItemsAreOwnedByTheMedia()
        {
            var media = new Media(Path.GetTempFileName());
            var subItems = media.SubItems;
            Assert.AreSame(subItems, media.SubItems);

            subItems.Dispose();
            var again = media.SubItems;
            Assert.AreNotSame(subItems, again);

            media.Dispose();
            Assert.AreEqual(IntPtr.Zero, again.NativeReference);
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
