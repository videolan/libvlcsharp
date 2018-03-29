using NUnit.Framework;
using VideoLAN.LibVLC;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class VersionCheckTests : BaseSetup
    {
        [Test]
        public void ShouldThrowIfDllVersionNotHighEnough()
        {
            Assert.Throws<VLCException>(UnavailableAPIMethod);
        }

        [LibVLC(int.MaxValue)]
        void UnavailableAPIMethod() => Assert.Fail("should not reach here");

        bool _result;
        [LibVLC(major: 0, minor: 0, min: false, strict: true)]
        public bool UnavailableAPIProperty
        {
            get { Assert.Fail(); return _result; }
            
        }

        [Test]
        public void ShouldThrowIfDllVersionTooHigh()
        {
            Assert.Throws<VLCException>(() =>
            {
                var r = UnavailableAPIProperty;
            });
        }
    }
}