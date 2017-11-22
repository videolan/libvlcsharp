using System;
using NUnit.Framework;
using VideoLAN.LibVLC.Manual;
using MediaPlayer = VideoLAN.LibVLC.Manual.MediaPlayer;

namespace Bindings.Tests
{
    [TestFixture]
    public class MediaPlayerTests
    {
        [Test]
        public void CreateAndDestroy()
        {
            var mp = new MediaPlayer(new Instance());
            mp.Dispose();
            Assert.AreEqual(IntPtr.Zero, mp.NativeReference);
        }

        [Test]
        public void T()
        {
            var mp = new MediaPlayer(new Instance());
            
        }

    }
}
