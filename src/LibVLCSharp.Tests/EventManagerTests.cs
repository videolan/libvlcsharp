﻿using System;
using System.IO;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
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
                Assert.That(description == args.MetadataType);
                eventHandlerCalled = true;
            };
            media.SetMeta(MetadataType.Description, "test");
            Assert.That(eventHandlerCalled);
        }
        
        public async void DurationChanged()
        {
            var media = new Media(_libVLC, RealMp3Path);
            var called = false;
            long duration = 0;

            media.DurationChanged += (sender, args) =>
            {
                called = true;
                duration = args.Duration;
            };

            await media.Parse();

            Assert.That(called);
            Assert.That(duration, Is.Not.Zero);
        }

        [Test]
        public void FreedMedia()
        {
            var media = new Media(_libVLC, RealMp3Path);
            var eventCalled = false;
            media.MediaFreed += (sender, args) =>
            {
                eventCalled = true;
            };

            media.Dispose();

            Assert.That(eventCalled);
        }

        [Test]
        public async Task OpeningStateChanged()
        {
            var media = new Media(_libVLC, RealMp3Path);
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
            await tcs.Task;
            Assert.That(tcs.Task.Result);
            Assert.That(openingCalled);
        }
    }
}
