using System.Linq;
using NUnit.Framework;
using VideoLAN.LibVLCSharp;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaDiscovererTests : BaseSetup
    {
        [Test]
        public void CreateStartAndStopDiscoverer()
        {
            var instance = new Instance();
            var mds = instance.MediaDiscoverers(MediaDiscoverer.Category.Lan);
            var md = new MediaDiscoverer(instance, mds.First().Name);
            Assert.True(md.Start());
            Assert.True(md.IsRunning);
            md.Stop();
            Assert.False(md.IsRunning);
        }
    }
}