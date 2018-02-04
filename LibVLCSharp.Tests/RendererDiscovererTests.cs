using System.Threading.Tasks;
using NUnit.Framework;
using VideoLAN.LibVLC;
using static System.Diagnostics.Debug;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class RendererDiscovererTests
    {
        [Test]
        public async Task T()
        {
            var instance = new Instance();

            var rendererDiscoverer = new RendererDiscoverer(instance, "microdns");
            var em = rendererDiscoverer.EventManager;

            em.ItemAdded += (sender, args) =>
            {
                WriteLine($"New item discovered: {args.RendererItem.Name} of type {args.RendererItem.Type}");
                if(args.RendererItem.CanRenderVideo)
                    WriteLine("Can render video");
                if(args.RendererItem.CanRenderAudio)
                    WriteLine("Can render audio");
            };

            if(!rendererDiscoverer.Start())
                NUnit.Framework.Assert.Fail();

            await Task.Delay(1000);
        }
    }
}
