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
        public async Task DiscoverItems()
        {
            Core.Initialize();

            var libVLC = new LibVLC();

            var mp = new MediaPlayer(libVLC)
            {
                Media = new Media(libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                    Media.FromType.FromLocation)
            };

            Assert.True(mp.Play());

            var rendererList = libVLC.RendererList;
            Assert.IsNotEmpty(rendererList);

            var rendererDiscoverer = new RendererDiscoverer(libVLC);
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
            var rendererDiscoverer = new RendererDiscoverer(new LibVLC());
            rendererDiscoverer.Start();
            rendererDiscoverer.Dispose();
            Assert.AreEqual(IntPtr.Zero, rendererDiscoverer.NativeReference);
        }
    }
}