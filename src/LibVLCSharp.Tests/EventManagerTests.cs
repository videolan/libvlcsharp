﻿using System;
using System.IO;
using System.Threading.Tasks;
using LibVLCSharp;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class EventManagerTests : BaseSetup
    {
        [Test]
        public void MetaChangedEventSubscribe()
        {
            var media = new Media(_libVLC, Path.GetTempFileName());
            var eventHandlerCalled = false;
            const MetadataType description = MetadataType.Description;
            media.MetaChanged += (sender, args) =>
            {
                Assert.AreEqual(description, args.MetadataType);
                eventHandlerCalled = true;
            };
            media.SetMeta(MetadataType.Description, "test");
            Assert.True(eventHandlerCalled);
        }

        public void DurationChanged()
        {
            var media = new Media(_libVLC, LocalAudioFile);
            var called = false;
            long duration = 0;

            media.DurationChanged += (sender, args) =>
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
            var media = new Media(_libVLC, LocalAudioFile);
            var eventCalled = false;
            media.MediaFreed += (sender, args) =>
            {
                eventCalled = true;
            };

            media.Dispose();

            Assert.True(eventCalled);
        }

        [Test]
        public async Task OpeningStateChanged()
        {
            var media = new Media(_libVLC, LocalAudioFile);
            var tcs = new TaskCompletionSource<bool>();
            var openingCalled = false;
            media.StateChanged += (sender, args) =>
            {
                if (media.State == VLCState.Opening)
                {
                    openingCalled = true;
                    tcs.SetResult(true);
                    return;
                }
            };

            var mp = new MediaPlayer(media);
            mp.Play();
            await Task.Delay(1000);
            Assert.True(openingCalled);
        }
    }
}
