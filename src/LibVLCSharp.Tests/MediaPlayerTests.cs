using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaPlayerTests : BaseSetup
    {
        [Test]
        public void CreateAndDestroy()
        {
            var mp = new MediaPlayer(_libVLC);
            mp.Dispose();
            Assert.AreEqual(IntPtr.Zero, mp.NativeReference);
        }

        [Test]
        public void OutputDeviceEnum()
        {
            var mp = new MediaPlayer(_libVLC);
            var t = mp.AudioOutputDeviceEnum;
            Debug.WriteLine(t);
        }
        
        [Test]
        public async Task TrackDescription()
        {
            var mp = new MediaPlayer(_libVLC);
            var media = new Media(_libVLC, new Uri(RealStreamMediaPath));
            var tcs = new TaskCompletionSource<bool>();
            
            mp.Media = media;
            mp.Play();
            mp.Playing += (sender, args) =>
            {
                var description = mp.AudioTrackDescription;
                Assert.True(mp.SetAudioTrack(description.First().Id));
                Assert.IsNotEmpty(description);
                tcs.SetResult(true);
            };
            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        public async Task ChapterDescriptions()
        {
            var mp = new MediaPlayer(_libVLC);
            var media = new Media(_libVLC, "https://auphonic.com/media/blog/auphonic_chapters_demo.m4a", FromType.FromLocation);
            var tcs = new TaskCompletionSource<bool>();

            mp.Media = media;
            mp.Play();
            mp.Playing += (sender, args) =>
            {
                var chapters = mp.FullChapterDescriptions(-1);
                Assert.IsNotEmpty(chapters);
                Assert.AreEqual(chapters.Length, mp.ChapterCount);
                tcs.SetResult(true);
            };
            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        public async Task Play()
        {
            var media = new Media(_libVLC, new Uri(RealStreamMediaPath));
            var mp = new MediaPlayer(media);
            var called = false;
            mp.Playing += (sender, args) =>
            {
                called = true;
            };
            mp.Play();
            await Task.Delay(5000);
            Assert.True(called);
            //Assert.True(mp.IsPlaying);
        }

        int callCountRegisterOne = 0;
        int callCountRegisterTwo = 0;

        [Test]
        public async Task EventFireOnceForeachRegistration()
        {
            try
            {
                var media = new Media(_libVLC, new Uri(RealStreamMediaPath));
                var mp = new MediaPlayer(media);
                
                mp.Playing += Mp_Playing;
                mp.Playing += Mp_Playing1;

                Debug.WriteLine("first play");

                mp.Play();
                await Task.Delay(2000);
                Assert.AreEqual(callCountRegisterOne, 1);
                Assert.AreEqual(callCountRegisterTwo, 1);
            
                callCountRegisterOne = 0;
                callCountRegisterTwo = 0;

                mp.Stop();

                mp.Playing -= Mp_Playing;

            
                Debug.WriteLine("second play");

                mp.Play();
                await Task.Delay(2000);

                Assert.AreEqual(callCountRegisterOne, 0);
                Assert.AreEqual(callCountRegisterTwo, 1);

              //  mp.Stop();

                mp.Playing -= Mp_Playing1; // native crash in detach?



                callCountRegisterOne = 0;
                callCountRegisterTwo = 0;


                Debug.WriteLine("third play");

                mp.Play();
                await Task.Delay(500);

                Assert.AreEqual(callCountRegisterOne, 0);
                Assert.AreEqual(callCountRegisterTwo, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        void Mp_Playing1(object sender, EventArgs e)
        {
            callCountRegisterTwo++;
            Debug.WriteLine($"Mp_Playing1 called with {callCountRegisterTwo}");

        }

        void Mp_Playing(object sender, EventArgs e)
        {
            callCountRegisterOne++;
            Debug.WriteLine($"Mp_Playing called with {callCountRegisterOne}");
        }

        [Test]
        public async Task DisposeMediaPlayer()
        {
            var mp = new MediaPlayer(_libVLC);

            mp.Play(new Media(_libVLC, new Uri(RealStreamMediaPath)));

            await Task.Delay(1000);

            mp.Dispose();

            Assert.AreEqual(IntPtr.Zero, mp.NativeReference);
        }

        [Test]
        public async Task UpdateViewpoint()
        {
            var mp = new MediaPlayer(_libVLC);

            mp.Play(new Media(_libVLC, "https://streams.videolan.org/streams/360/eagle_360.mp4", FromType.FromLocation));

            await Task.Delay(1000);

            var result = mp.UpdateViewpoint(yaw: 0, pitch: 90, roll: 0, fov: 0);

            Assert.IsTrue(result);

            await Task.Delay(1000);
            
            mp.Dispose();

            Assert.AreEqual(IntPtr.Zero, mp.NativeReference);
        }

        [Test]
        public void GetMediaPlayerRole()
        {
            var mp = new MediaPlayer(_libVLC);
            Assert.AreEqual(MediaPlayerRole.Video, mp.Role);
        }

        [Test]
        public void SetMediaPlayerRole()
        {
            var mp = new MediaPlayer(_libVLC);
            Assert.AreEqual(MediaPlayerRole.Video, mp.Role);

            Assert.True(mp.SetRole(MediaPlayerRole.None));
            Assert.AreEqual(MediaPlayerRole.None, mp.Role);
        }
    }
}
