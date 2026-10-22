using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
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
            Assert.That(IntPtr.Zero == mp.NativeReference);
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
                Assert.That(mp.SetAudioTrack(description.First().Id));
                Assert.That(description, Is.Not.Empty);
                tcs.SetResult(true);
            };
            await tcs.Task;
            Assert.That(tcs.Task.Result);
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
                Assert.That(chapters, Is.Not.Empty);
                Assert.That(chapters.Length == mp.ChapterCount);
                tcs.SetResult(true);
            };
            await tcs.Task;
            Assert.That(tcs.Task.Result);
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
            Assert.That(called);
            //Assert.That(mp.IsPlaying);
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
                Assert.That(callCountRegisterOne == 1);
                Assert.That(callCountRegisterTwo == 1);
            
                callCountRegisterOne = 0;
                callCountRegisterTwo = 0;

                mp.Stop();

                mp.Playing -= Mp_Playing;

            
                Debug.WriteLine("second play");

                mp.Play();
                await Task.Delay(2000);

                Assert.That(callCountRegisterOne == 0);
                Assert.That(callCountRegisterTwo == 1);

              //  mp.Stop();

                mp.Playing -= Mp_Playing1; // native crash in detach?



                callCountRegisterOne = 0;
                callCountRegisterTwo = 0;


                Debug.WriteLine("third play");

                mp.Play();
                await Task.Delay(500);

                Assert.That(callCountRegisterOne == 0);
                Assert.That(callCountRegisterTwo == 0);
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

            Assert.That(IntPtr.Zero == mp.NativeReference);
        }

        [Test]
        public async Task UpdateViewpoint()
        {
            var mp = new MediaPlayer(_libVLC);

            mp.Play(new Media(_libVLC, "https://streams.videolan.org/streams/360/eagle_360.mp4", FromType.FromLocation));

            await Task.Delay(1000);

            var result = mp.UpdateViewpoint(yaw: 0, pitch: 90, roll: 0, fov: 0);

            Assert.That(result, Is.True);

            await Task.Delay(1000);
            
            mp.Dispose();

            Assert.That(IntPtr.Zero == mp.NativeReference);
        }

        [Test]
        public async Task VideoFormatCleanupCallbackReceivesUserData()
        {
            const uint width = 64;
            const uint height = 64;
            var userData = new IntPtr(0x1234);
            var buffer = Marshal.AllocHGlobal((int)(width * height * 4));
            // continuations must not run on the libvlc thread raising the callback, or Stop() would deadlock
            var firstFrame = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var cleanupUserData = new TaskCompletionSource<IntPtr>(TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                // the visualization renders video from the local audio file
                using var libVLC = new LibVLC("--aout=adummy", "--audio-visual=visual");
                using var mp = new MediaPlayer(libVLC);
                using var media = new Media(libVLC, RealMp3Path);

                mp.SetVideoCallbacks((opaque, planes) =>
                {
                    Marshal.WriteIntPtr(planes, buffer);
                    firstFrame.TrySetResult(true);
                    return IntPtr.Zero;
                }, null, null);
                mp.SetVideoFormatCallbacks((ref IntPtr opaque, IntPtr chroma, ref uint w, ref uint h, ref uint pitches, ref uint lines) =>
                {
                    Marshal.Copy(Encoding.ASCII.GetBytes("RV32"), 0, chroma, 4);
                    w = width;
                    h = height;
                    pitches = width * 4;
                    lines = height;
                    opaque = userData;
                    return 1;
                }, (ref IntPtr opaque) => cleanupUserData.TrySetResult(opaque));

                Assert.That(mp.Play(media));
                Assert.That(await Task.WhenAny(firstFrame.Task, Task.Delay(10000)), Is.SameAs(firstFrame.Task), "no video frame was rendered");

                // closing the video output calls the cleanup callback
                mp.Stop();

                Assert.That(await Task.WhenAny(cleanupUserData.Task, Task.Delay(10000)), Is.SameAs(cleanupUserData.Task), "cleanup callback was not called");
                Assert.That(cleanupUserData.Task.Result, Is.EqualTo(userData));
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        [Test]
        public void GetMediaPlayerRole()
        {
            var mp = new MediaPlayer(_libVLC);
            Assert.That(MediaPlayerRole.Video == mp.Role);
        }

        [Test]
        public void SetMediaPlayerRole()
        {
            var mp = new MediaPlayer(_libVLC);
            Assert.That(MediaPlayerRole.Video == mp.Role);

            Assert.That(mp.SetRole(MediaPlayerRole.None));
            Assert.That(MediaPlayerRole.None == mp.Role);
        }
    }
}
