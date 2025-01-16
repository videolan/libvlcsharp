﻿using System;
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
            var media = new Media(new Uri(LocalAudioFile));
            var mp = new MediaPlayer(_libVLC, media);
            var called = false;
            mp.Playing += (sender, args) =>
            {
                called = true;
            };
            mp.Play();
            await Task.Delay(5000);
            Assert.True(called);
        }

        int callCountRegisterOne = 0;
        int callCountRegisterTwo = 0;

        [Test]
        public async Task EventFireOnceForeachRegistration()
        {
            try
            {
                var media = new Media(new Uri(LocalAudioFile));
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

            mp.Play(new Media(new Uri("https://streams.videolan.org/streams/360/eagle_360.mp4"), ":no-video"));

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

            Assert.True(mp.SetRole(MediaPlayerRole.Music));
            Assert.AreEqual(MediaPlayerRole.Music, mp.Role);
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

            await Task.Delay(5000);

            Assert.True(mp.SetABLoop(ABLoop.A));

            await Task.Delay(3000);

            Assert.True(mp.SetABLoop(ABLoop.B));

            var abloop = mp.GetABLoop(out var atime, out var aposition, out var btime, out var bposition);

            Assert.AreEqual(ABLoop.B, abloop);

            Assert.Greater(btime, atime);
            Assert.Greater(bposition, aposition);

            Assert.True(mp.SetABLoop(ABLoop.None));
            Assert.AreEqual(ABLoop.None, mp.GetABLoop(out atime, out aposition, out btime, out bposition));
        }
    }
}
