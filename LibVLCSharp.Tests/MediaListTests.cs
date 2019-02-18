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
            var mediaList = new MediaList(_libVLC);
            var media = new Media(_libVLC, Path.GetTempFileName());
            var itemAdded = false;
            var itemDeleted = false;
            mediaList.ItemAdded += (sender, args) => itemAdded = true;
            mediaList.ItemDeleted += (sender, args) => itemDeleted = true;
            mediaList.AddMedia(media);
            Assert.AreEqual(media, mediaList[0]);
            Assert.AreEqual(1, mediaList.Count);
            Assert.True(itemAdded);
            Assert.Zero(mediaList.IndexOf(media));
            mediaList.RemoveIndex(0);
            Assert.Zero(mediaList.Count);
            Assert.True(itemDeleted);
        }

        [Test]
        public void DisposeMediaList()
        {
            var mediaList = new MediaList(_libVLC);
            mediaList.Dispose();
            Assert.AreEqual(IntPtr.Zero, mediaList.NativeReference);
        }
    }
}
