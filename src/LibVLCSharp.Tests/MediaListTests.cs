using System;
using System.IO;
using System.Linq;
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
            Assert.That(media.NativeReference == mediaList.First().NativeReference);
            Assert.That(1 == mediaList.Count);
            Assert.That(itemAdded);
            Assert.That(mediaList.IndexOf(media), Is.Zero);
            mediaList.RemoveIndex(0);
            Assert.That(mediaList.Count, Is.Zero);
            Assert.That(itemDeleted);
        }

        [Test]
        public void DisposeMediaList()
        {
            var mediaList = new MediaList(_libVLC);
            mediaList.Dispose();
            Assert.That(IntPtr.Zero == mediaList.NativeReference);
        }
    }
}
