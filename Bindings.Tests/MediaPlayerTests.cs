using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using VideoLAN.LibVLC;
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
        public void OutputDeviceEnum()
        {
            var mp = new MediaPlayer(new Instance());
            var t = mp.OutputDeviceEnum;
            Debug.WriteLine(t);
        }

        string RealMediaPath
        {
            get
            {
                var dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
           //     var binDir = Path.Combine(dir, "..\\..\\..\\");
                var files = Directory.GetFiles(dir);
                return files.First();
            }
        }

        [Test]
        public void TrackDescription()
        {
            //FIX ME
            var instance = new Instance();
            var mp = new MediaPlayer(instance);
            var media = new Media(instance, RealMediaPath, Media.FromType.FromPath);            
            mp.Media = media;
            var track = mp.AudioTrack;
            Assert.True(mp.SetAudioTrack(track));
            Assert.IsNotEmpty(mp.AudioTrackDescription);
        }

        [Test]
        public async Task Play()
        {
            var instance = new Instance();
            var media = new Media(instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
            var mp = new MediaPlayer(media);
            var called = false;
            mp.EventManager.Playing += (sender, args) =>
            {
                called = true;
            };
            mp.Play();
            await Task.Delay(5000);
            Assert.True(called);
            Assert.True(mp.IsPlaying);
        }
    }
}
