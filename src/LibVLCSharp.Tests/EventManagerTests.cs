using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class EventManagerTests : BaseSetup
    {
        [Test]
        [Ignore("event does not fire in unit test")]
        public void MetaChangedEventSubscribe()
        {
            var media = new Media(Path.GetTempFileName());
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
            var media = new Media(LocalAudioFile);
            var called = false;
            long duration = 0;

            media.DurationChanged += (sender, args) =>
            {
                called = true;
                duration = args.Duration;
            };

            await media.ParseAsync(_libVLC);

            Assert.True(called);
            Assert.NotZero(duration);
        }

        [Test]
        public void MetaExtraTest()
        {
            var key = "key";
            var value = "value";

            var media = new Media(LocalAudioFile);

            media.SetMetaExtra(key, value);

            Assert.AreEqual(value, media.MetaExtra(key));
            Assert.AreEqual(key, media.MetaExtraNames.Single());

            media.SetMetaExtra(key, null);

            Assert.AreEqual(null, media.MetaExtra(key));
            Assert.IsEmpty(media.MetaExtraNames);
        }
    }
}
