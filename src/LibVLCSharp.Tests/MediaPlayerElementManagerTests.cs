using System;
using System.Reflection;
using LibVLCSharp;
using LibVLCSharp.MediaPlayerElement;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaPlayerElementManagerTests : BaseSetup
    {
        [Test]
        public void DisposeUnsubscribesExactlyOnce()
        {
            using var mediaPlayer = new MediaPlayer(_libVLC);
            using var manager = new CountingManager { MediaPlayer = mediaPlayer };

            manager.Dispose();
            manager.Dispose();

            Assert.That(manager.UnsubscribeCount, Is.EqualTo(1));
        }

        [Test]
        public void AutoHideNotifierUnsubscribesOpeningHandler()
        {
            using var mediaPlayer = new MediaPlayer(_libVLC);
            using var manager = new AutoHideNotifier(null) { MediaPlayer = mediaPlayer };

            Assert.That(GetOpeningHandlers(mediaPlayer), Is.Not.Null);
            manager.Dispose();

            Assert.That(GetOpeningHandlers(mediaPlayer), Is.Null);
        }

        [Test]
        public void DeviceAwakeningManagerUnsubscribesOpeningHandler()
        {
            using var mediaPlayer = new MediaPlayer(_libVLC);
            using var manager = new DeviceAwakeningManager(null, new DisplayRequest()) { MediaPlayer = mediaPlayer };

            Assert.That(GetOpeningHandlers(mediaPlayer), Is.Not.Null);
            manager.Dispose();

            Assert.That(GetOpeningHandlers(mediaPlayer), Is.Null);
        }

        static Delegate GetOpeningHandlers(MediaPlayer mediaPlayer)
        {
            var eventManager = typeof(MediaPlayer)
                .GetField("_eventManager", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(mediaPlayer);

            return (Delegate)eventManager?.GetType()
                .GetField("Opening", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(eventManager);
        }

        sealed class CountingManager : MediaPlayerElementManagerBase
        {
            internal int UnsubscribeCount { get; private set; }

            internal CountingManager() : base(null)
            {
            }

            protected override void UnsubscribeEvents(MediaPlayer mediaPlayer)
            {
                UnsubscribeCount++;
                base.UnsubscribeEvents(mediaPlayer);
            }
        }

        sealed class DisplayRequest : IDisplayRequest
        {
            public void RequestActive()
            {
            }

            public void RequestRelease()
            {
            }
        }
    }
}
