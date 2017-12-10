using System;
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

        [LibVLC(int.MaxValue)]
        void UnavailableAPIMethod() => Assert.Fail("should not reach here");

        [LibVLC(major: 0, minor: 0, min: false, strict: true)]
        public bool UnavailableAPIProperty { get; set; }

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
