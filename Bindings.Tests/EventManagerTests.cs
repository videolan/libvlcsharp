using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using VideoLAN.LibVLC;

namespace Bindings.Tests
{
    [TestFixture]
    public class EventManagerTests
    {
        [Test]
        public void MetaChangedEventSubscribe()
        {
            var media = new Media(new Instance(), Path.GetTempFileName(), Media.FromType.FromPath);
            var eventManager = media.EventManager;
            var eventHandlerCalled = false;
            const Media.MetadataType description = Media.MetadataType.Description;
            eventManager.MetaChanged += (sender, args) =>
            {
                Assert.AreEqual(description, args.MetadataType);
                eventHandlerCalled = true;
            };
            media.SetMeta(Media.MetadataType.Description, "test");
            Assert.True(eventHandlerCalled);
        }

        [Test]
        public void SubItemAdded()
        {
            var media = new Media(new Instance(), Path.GetTempFileName(), Media.FromType.FromPath);
            //media.SubItems.
            //TODO: Implement MediaList to test this.
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
        public void DurationChanged()
        {
            var media = new Media(new Instance(), RealMediaPath, Media.FromType.FromPath);
            var called = false;
            long duration = 0;

            media.EventManager.DurationChanged += (sender, args) =>
            {
                called = true;
                duration = args.Duration;
            };

            media.Parse();

            Assert.True(called);
            Assert.NotZero(duration);
        }

        [Test]
        public void FreedMedia()
        {
            var media = new Media(new Instance(), RealMediaPath, Media.FromType.FromPath);
            var eventCalled = false;
            media.EventManager.MediaFreed += (sender, args) =>
            {
                eventCalled = true;
            };

            media.Dispose();

            Assert.True(eventCalled);
        }

        [Test]
        public void StateChanged()
        {
            var media = new Media(new Instance(), RealMediaPath, Media.FromType.FromPath);
            var called = false;
            media.EventManager.StateChanged += (sender, args) => called = true;
            //TODO: implement MediaPlayer.cs
            //mediaPlayer.SetMedia(media);
            Assert.True(called);
        }


        [Test]
        public void SubItemTreeAdded()
        {
            var media = new Media(new Instance(), RealMediaPath, Media.FromType.FromPath);
            //TODO: Implement MediaList.cs
            Assert.Fail();
        }
    }
}