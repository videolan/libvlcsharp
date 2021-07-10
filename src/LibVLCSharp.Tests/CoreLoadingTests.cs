using LibVLCSharp.Shared;
using NUnit.Framework;
using System.IO;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class CoreLoadingTests
    {
        [Test]
        public void LoadLibVLCFromSpecificPath()
        {
            var dirPath = Path.GetDirectoryName(typeof(CoreLoadingTests).Assembly.Location)!;
            var finalPath = Path.Combine(dirPath, "libvlc", "win-x86");

            Assert.DoesNotThrow(() => Core.Initialize(finalPath), "fail to load libVLC dll at specified location");
            var libVLC = new LibVLC("--no-audio", "--no-video");
        }

        [Test]
        public void LoadLibVLCFromInferredPath()
        {
            Assert.DoesNotThrow(() => Core.Initialize(), "fail to load libVLC dll at specified location");
            var libVLC = new LibVLC("--no-audio", "--no-video");
        }
    }
}
