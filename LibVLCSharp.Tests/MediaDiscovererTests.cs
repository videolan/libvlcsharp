using System;
using System.Linq;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaDiscovererTests : BaseSetup
    {
        [Test]
        public void CreateStartAndStopDiscoverer()
        {
            var mds = _libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md = new MediaDiscoverer(_libVLC, mds.First().Name);
            Assert.True(md.Start());
            Assert.True(md.IsRunning);
            md.Stop();
            Assert.False(md.IsRunning);
        }

        [Test]
        public void DisposeMediaDiscoverer()
        {
            var mds = _libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md = new MediaDiscoverer(_libVLC, mds.First().Name);
            Assert.True(md.Start());
            Assert.True(md.IsRunning);
            Assert.NotNull(md.MediaList);
            md.Dispose();
            Assert.IsNull(md.MediaList);
            Assert.False(md.IsRunning);
            Assert.AreEqual(IntPtr.Zero, md.NativeReference);
        }
    }
}