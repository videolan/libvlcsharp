using NUnit.Framework;
using VideoLAN.LibVLC;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class VersionCheckTests
    {
        [Test]
        public void ShouldThrowIfDllVersionNotHighEnough()
        {
            Assert.Throws<VLCException>(UnavailableAPIMethod);
        }

        [LibVLC(10)]
        void UnavailableAPIMethod() => Assert.Fail("should not reach here");
    }
}
