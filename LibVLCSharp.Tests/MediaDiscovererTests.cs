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
            var libVLC = new LibVLC();
            var mds = libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md = new MediaDiscoverer(libVLC, mds.First().Name);
            Assert.True(md.Start());
            Assert.True(md.IsRunning);
            md.Stop();
            Assert.False(md.IsRunning);
        }

        [Test]
        public void DisposeMediaDiscoverer()
        {
            var libVLC = new LibVLC();
            var mds = libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md = new MediaDiscoverer(libVLC, mds.First().Name);
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