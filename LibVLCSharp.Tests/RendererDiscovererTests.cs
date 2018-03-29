using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using VideoLAN.LibVLC;
using static System.Diagnostics.Debug;
using Assert = NUnit.Framework.Assert;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class RendererDiscovererTests : BaseSetup
    {
        [Test]
        public async Task DiscoverItems()
        {
            Core.Initialize();

            var instance = new Instance(new []{"--verbose=2"});
            instance.Log += (sender, args) =>
            {
                WriteLine(args.Message);
            };
            var mp = new MediaPlayer(instance)
            {
                Media = new Media(instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                    Media.FromType.FromLocation)
            };

           // Assert.True(mp.Play());

            var rendererList = instance.RendererList;
            Assert.IsNotEmpty(rendererList);

            var rendererDiscoverer = new RendererDiscoverer(instance, /*"microdns"*/ rendererList[0].Name);
            var rendererItems = new List<RendererItem>();
            var tcs = new TaskCompletionSource<bool>();

            rendererDiscoverer.EventManager.ItemAdded += (sender, args) =>
            {
                WriteLine($"New item discovered: {args.RendererItem.Name} of type {args.RendererItem.Type}");
                if(args.RendererItem.CanRenderVideo)
                    WriteLine("Can render video");
                if(args.RendererItem.CanRenderAudio)
                    WriteLine("Can render audio");
                tcs.SetResult(true);

                rendererItems.Add(args.RendererItem);
            };


            Assert.True(rendererDiscoverer.Start());

            //await Task.Delay(10000);
            await tcs.Task;
            Assert.True(tcs.Task.Result);
            Assert.IsNotEmpty(rendererItems);
            Assert.True(mp.SetRenderer(rendererItems.First()));

            Console.ReadKey();
        }
    }
}
