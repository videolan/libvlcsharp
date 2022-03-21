using System;
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
        
        public async void DurationChanged()
        {
            var media = new Media(_libVLC, LocalAudioFile);
            var called = false;
            long duration = 0;

            media.DurationChanged += (sender, args) =>
            {
                called = true;
                duration = args.Duration;
            };

            await media.ParseAsync();

            Assert.True(called);
            Assert.NotZero(duration);
        }
    }
}
