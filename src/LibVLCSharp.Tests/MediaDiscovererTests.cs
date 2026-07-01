using System;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaDiscovererTests : BaseSetup
    {
        [Test]
        public async Task CreateStartAndStopDiscoverer()
        {
            var mds = _libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md = new MediaDiscoverer(_libVLC, mds.First().Name!);
            Assert.True(md.Start());
            Assert.True(md.IsRunning);
            md.Stop();
            for (var i = 0; i < 30 && md.IsRunning; i++)
            {
                await Task.Delay(100);
            }

            Assert.False(md.IsRunning);
        }

        [Test]
        public async Task DisposeMediaDiscoverer()
        {
            var mds = _libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md = new MediaDiscoverer(_libVLC, mds.First().Name!);
            var addedCount = 0;
            var removedCount = 0;
            md.MediaAdded += (_, _) => addedCount++;
            md.MediaRemoved += (_, _) => removedCount++;

            Assert.True(md.Start());
            Assert.True(md.IsRunning);
            await Task.Delay(1000);

            Assert.GreaterOrEqual(addedCount, 0);
            Assert.GreaterOrEqual(removedCount, 0);

            md.Dispose();
            Assert.False(md.IsRunning);
            Assert.AreEqual(IntPtr.Zero, md.NativeReference);
        }
    }
}
