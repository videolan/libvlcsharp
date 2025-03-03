using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            var md = new MediaDiscoverer(_libVLC, mds.First().Name!);
            Assert.That(md.Start());
            Assert.That(md.IsRunning);
            md.Stop();
            Assert.That(md.IsRunning, Is.False);
        }

        [Test]
        public async Task DisposeMediaDiscoverer()
        {
            var mds = _libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md = new MediaDiscoverer(_libVLC, mds.First().Name!);
            Assert.That(md.Start());
            Assert.That(md.IsRunning);
            Assert.That(md.MediaList, Is.Not.Null);
            await Task.Delay(1000);
            foreach(var media in md.MediaList!)
            {
                Debug.WriteLine(media.Mrl);
            }
            md.Dispose();
            Assert.That(md.MediaList, Is.Null);
            Assert.That(md.IsRunning, Is.False);
            Assert.That(IntPtr.Zero == md.NativeReference);
        }
    }
}
