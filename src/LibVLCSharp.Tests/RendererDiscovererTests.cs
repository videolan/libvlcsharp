using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using NUnit.Framework;
using static System.Diagnostics.Debug;
using Assert = NUnit.Framework.Assert;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class RendererDiscovererTests : BaseSetup
    {
        // This test depends on both accepting the network access request made by the test runner 
        // and having a chromecast on the same local network.
        [Test]
        [Ignore("requires network calls that may fail when run from CI")]
        public async Task DiscoverItems()
        {
            var mp = new MediaPlayer(_libVLC)
            {
                Media = new Media(_libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                    FromType.FromLocation)
            };

            Assert.True(mp.Play());

            var rendererList = _libVLC.RendererList;
            Assert.IsNotEmpty(rendererList);

            var rendererDiscoverer = new RendererDiscoverer(_libVLC, _libVLC.RendererList.LastOrDefault().Name);
            var rendererItems = new List<RendererItem>();
            var tcs = new TaskCompletionSource<bool>();

            rendererDiscoverer.ItemAdded += (sender, args) =>
            {
                WriteLine($"New item discovered: {args.RendererItem.Name} of type {args.RendererItem.Type}");
                if (args.RendererItem.CanRenderVideo)
                    WriteLine("Can render video");
                if (args.RendererItem.CanRenderAudio)
                    WriteLine("Can render audio");

                rendererItems.Add(args.RendererItem);
                
                tcs.SetResult(true);
            };


            Assert.True(rendererDiscoverer.Start());

            await tcs.Task;
            Assert.True(tcs.Task.Result);
            Assert.IsNotEmpty(rendererItems);
            Assert.True(mp.SetRenderer(rendererItems.First()));

            await Task.Delay(10000);
        }

        [Test]
        public void DisposeRendererDiscoverer()
        {
            var rendererDiscoverer = new RendererDiscoverer(_libVLC, _libVLC.RendererList.LastOrDefault().Name);
            rendererDiscoverer.Dispose();
            Assert.AreEqual(IntPtr.Zero, rendererDiscoverer.NativeReference);
        }
    }
}