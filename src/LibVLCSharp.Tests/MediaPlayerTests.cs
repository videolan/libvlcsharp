using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp;
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
            foreach(var audioOutputDevice in mp.AudioOutputDeviceEnum)
            {
                Debug.WriteLine(audioOutputDevice);
            }
        }

        [Test]
        public async Task TrackDescription()
        {
            var mp = new MediaPlayer(_libVLC);
            var media = new Media("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4", FromType.FromLocation);
            var tcs = new TaskCompletionSource<bool>();

            mp.Media = media;
            mp.Playing += (sender, args) =>
            {
                using var audioTracks = mp.Tracks(TrackType.Audio);
                mp.Select(audioTracks.First());
                Assert.AreEqual(mp.SelectedTrack(TrackType.Audio)?.Id, audioTracks.First().Id);
                tcs.SetResult(true);
            };
            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        public async Task ChapterDescriptions()
        {
            var mp = new MediaPlayer(_libVLC);
            var media = new Media("https://auphonic.com/media/blog/auphonic_chapters_demo.m4a", FromType.FromLocation);
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
            var media = new Media("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", FromType.FromLocation);
            var mp = new MediaPlayer(_libVLC, media);
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
                var media = new Media("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", FromType.FromLocation);
                var mp = new MediaPlayer(_libVLC, media);


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

            mp.Play(new Media("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", FromType.FromLocation));

            await Task.Delay(1000);

            mp.Dispose();

            Assert.AreEqual(IntPtr.Zero, mp.NativeReference);
        }

        [Test]
        public async Task UpdateViewpoint()
        {
            var mp = new MediaPlayer(_libVLC);

            mp.Play(new Media("https://streams.videolan.org/streams/360/eagle_360.mp4", FromType.FromLocation));

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
            Assert.AreEqual(MediaPlayerRole.None, mp.Role);
        }

        [Test]
        public void SetMediaPlayerRole()
        {
            var mp = new MediaPlayer(_libVLC);
            Assert.AreEqual(MediaPlayerRole.None, mp.Role);

            Assert.True(mp.SetRole(MediaPlayerRole.Video));
            Assert.AreEqual(MediaPlayerRole.Video, mp.Role);
        }

        [Test]
        public async Task MultiTrackSelection()
        {
            var msub = "https://streams.videolan.org/samples/Matroska/subtitles/multiple_sub_sample.mkv";
            var mp = new MediaPlayer(_libVLC)
            {
                Media = new Media(new Uri(msub)),
                Mute = true
            };

            var tcs = new TaskCompletionSource<bool>();

            var trackList = default(MediaTrackList);
            mp.Playing += (s, e) => Task.Run(() =>
            {
                trackList = mp.Tracks(TrackType.Text);
                tcs.SetResult(true);
            });

            mp.Play();
            await tcs.Task;

            Assert.AreEqual(7, trackList?.Count);

            mp.Select(trackList.ToArray());

            await Task.Delay(10000);
        }

        [Test]
        public async Task JumpTime()
        {
            var mp = new MediaPlayer(_libVLC);
            mp.TimeChanged += (s, e) => Debug.WriteLine(e.Time);
            mp.Play(new Media(new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")));

            await Task.Delay(4000);
            
            Assert.True(mp.JumpTime(5000));
        }

        [Test]
        public async Task SetABTest()
        {
            var mp = new MediaPlayer(_libVLC)
            {
                Media = new Media(new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"))
            };

            await mp.PlayAsync();

            long aTime = 3000;
            long bTime = 6000;

            Assert.True(mp.SetABLoopTime(aTime, bTime));

            var abloop = mp.GetABLoop(out var atime, out _, out var btime, out _);

            Assert.AreEqual(ABLoop.B, abloop);

            Assert.AreEqual(aTime, atime);
            Assert.AreEqual(bTime, btime);

            Assert.True(mp.ResetABLoop());
            Assert.AreEqual(ABLoop.None, mp.GetABLoop(out _, out _, out _, out _));

            var aPosition = 0.3;
            var bPosition = 0.6;

            Assert.True(mp.SetABLoopPosition(aPosition, bPosition));

            abloop = mp.GetABLoop(out _, out var aposition, out _, out var bposition);
            Assert.AreEqual(ABLoop.B, abloop);

            Assert.AreEqual(aPosition, aposition);
            Assert.AreEqual(bPosition, bposition);
        }
    }
}
