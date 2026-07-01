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
        [SetUp]
        public void SetUpMediaPlayerTests()
        {
            _libVLC.Dispose();
            _libVLC = new LibVLC("--aout=dummy", "--vout=dummy", "--verbose=2");
        }

        [Test]
        public void CreateAndDestroy()
        {
            var mp = new MediaPlayer(_libVLC);
            mp.Dispose();
            Assert.AreEqual(IntPtr.Zero, mp.NativeReference);
        }

        [Test]
        public void WatchTimeUsesVersionedCallbacksStructAndPointerTimePoints()
        {
            var watchTime = NativeBindingAssertions.NativeMethod(typeof(MediaPlayer), "LibVLCMediaPlayerWatchTime");
            NativeBindingAssertions.HasDllImport(watchTime, "libvlc_media_player_watch_time");
            NativeBindingAssertions.HasParameterTypes(watchTime, typeof(IntPtr), typeof(long), typeof(IntPtr), typeof(IntPtr));

            var interpolate = NativeBindingAssertions.NativeMethod(typeof(MediaPlayer), "LibVLCMediaPlayerTimePointInterpolate");
            Assert.True(interpolate.GetParameters()[0].ParameterType.IsByRef);

            var nextDate = NativeBindingAssertions.NativeMethod(typeof(MediaPlayer), "LibVLCMediaPlayerTimePointGetNextDate");
            Assert.True(nextDate.GetParameters()[0].ParameterType.IsByRef);
        }

        [Test]
        public void NewLibVLC4MediaPlayerFunctionsAreBound()
        {
            NativeBindingAssertions.HasDllImport(typeof(MediaPlayer), "LibVLCMediaPlayerSetNextMedia", "libvlc_media_player_set_next_media");
            NativeBindingAssertions.HasDllImport(typeof(MediaPlayer), "LibVLCMediaPlayerGetNextMedia", "libvlc_media_player_get_next_media");
            NativeBindingAssertions.HasDllImport(typeof(MediaPlayer), "LibVLCMediaPlayerLock", "libvlc_media_player_lock");
            NativeBindingAssertions.HasDllImport(typeof(MediaPlayer), "LibVLCMediaPlayerUnlock", "libvlc_media_player_unlock");
            NativeBindingAssertions.HasDllImport(typeof(MediaPlayer), "LibVLCMediaPlayerWait", "libvlc_media_player_wait");
            NativeBindingAssertions.HasDllImport(typeof(MediaPlayer), "LibVLCMediaPlayerSignal", "libvlc_media_player_signal");
        }

        [Test]
        public void FullscreenUsesNativeBoolParameter()
        {
            var method = NativeBindingAssertions.NativeMethod(typeof(MediaPlayer), "LibVLCSetFullscreen");

            NativeBindingAssertions.HasDllImport(method, "libvlc_set_fullscreen");
            Assert.AreEqual(typeof(bool), method.GetParameters()[1].ParameterType);
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
            using var mp = new MediaPlayer(_libVLC);
            using var media = new Media(LocalAudioFile);

            mp.Media = media;
            Assert.True(await mp.PlayAsync());
            // Pause as soon as playback starts: the sample is short and would otherwise reach EOF and
            // stop before we can query track selection, leaving the player with no selected tracks.
            mp.SetPause(true);

            using var audioTracks = await WaitForAudioTracks(mp);
            var firstTrack = audioTracks.First();
            mp.Select(firstTrack);
            using var selectedTrack = await WaitForSelectedAudioTrack(mp);
            Assert.AreEqual(firstTrack.Id, selectedTrack.Id);
            Assert.True(await StopWithTimeout(mp));
        }

        [Test]
        [Ignore("requires remote chaptered media")]
        public async Task ChapterDescriptions()
        {
            var mp = new MediaPlayer(_libVLC);
            var media = new Media("https://auphonic.com/media/blog/auphonic_chapters_demo.m4a", FromType.FromLocation);
            var tcs = NewCompletionSource<bool>();

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
            using var media = new Media(LocalAudioFile);
            using var mp = new MediaPlayer(_libVLC, media);
            var tcs = NewCompletionSource<bool>();
            mp.Playing += (sender, args) =>
            {
                tcs.TrySetResult(true);
            };
            Assert.True(mp.Play());
            Assert.AreSame(tcs.Task, await Task.WhenAny(tcs.Task, Task.Delay(3000)));
            Assert.True(await tcs.Task);
            Assert.True(await StopWithTimeout(mp));
            //Assert.True(mp.IsPlaying);
        }

        int callCountRegisterOne = 0;
        int callCountRegisterTwo = 0;

        [Test]
        public async Task EventFireOnceForeachRegistration()
        {
            using var media = new Media(LocalAudioFile);
            using var mp = new MediaPlayer(_libVLC, media);

            mp.Playing += Mp_Playing;
            mp.Playing += Mp_Playing1;

            Debug.WriteLine("first play");

            Assert.True(await mp.PlayAsync());
            Assert.AreEqual(callCountRegisterOne, 1);
            Assert.AreEqual(callCountRegisterTwo, 1);

            callCountRegisterOne = 0;
            callCountRegisterTwo = 0;

            Assert.True(await StopWithTimeout(mp));

            mp.Playing -= Mp_Playing;

            Debug.WriteLine("second play");

            Assert.True(await mp.PlayAsync());

            Assert.AreEqual(callCountRegisterOne, 0);
            Assert.AreEqual(callCountRegisterTwo, 1);

            Assert.True(await StopWithTimeout(mp));
            mp.Playing -= Mp_Playing1;
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

            mp.Play(new Media(LocalAudioFile));

            await Task.Delay(1000);

            mp.Dispose();

            Assert.AreEqual(IntPtr.Zero, mp.NativeReference);
        }

        [Test]
        [Ignore("requires remote 360 video media")]
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
        [Ignore("requires remote media with multiple text tracks")]
        public async Task MultiTrackSelection()
        {
            var msub = "https://streams.videolan.org/samples/Matroska/subtitles/multiple_sub_sample.mkv";
            var mp = new MediaPlayer(_libVLC)
            {
                Media = new Media(new Uri(msub)),
                Mute = true
            };

            var tcs = NewCompletionSource<bool>();

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
            using var mp = new MediaPlayer(_libVLC);
            mp.TimeChanged += (s, e) => Debug.WriteLine(e.Time);
            using var media = new Media(LocalAudioFile);
            Assert.True(await mp.PlayAsync(media));

            await WaitForSeekable(mp);
            
            Assert.True(mp.JumpTime(100000));
            Assert.True(await StopWithTimeout(mp));
        }

        [Test]
        public async Task SetABTest()
        {
            using var media = new Media(LocalAudioFile);
            using var mp = new MediaPlayer(_libVLC)
            {
                Media = media
            };

            await mp.PlayAsync();

            long aTime = 100;
            long bTime = 500;

            Assert.True(mp.SetABLoopTime(aTime, bTime));

            var abloop = mp.GetABLoop(out var atime, out _, out var btime, out _);

            Assert.AreEqual(ABLoop.B, abloop);

            Assert.AreEqual(aTime, atime, 1);
            Assert.AreEqual(bTime, btime, 1);

            Assert.True(mp.ResetABLoop());
            Assert.AreEqual(ABLoop.None, mp.GetABLoop(out _, out _, out _, out _));

            var aPosition = 0.3;
            var bPosition = 0.6;

            Assert.True(mp.SetABLoopPosition(aPosition, bPosition));

            abloop = mp.GetABLoop(out _, out var aposition, out _, out var bposition);
            Assert.AreEqual(ABLoop.B, abloop);

            Assert.AreEqual(aPosition, aposition);
            Assert.AreEqual(bPosition, bposition);
            Assert.True(await StopWithTimeout(mp));
        }

        static async Task<MediaTrackList> WaitForAudioTracks(MediaPlayer mp)
        {
            for (var i = 0; i < 30; i++)
            {
                var tracks = mp.Tracks(TrackType.Audio);
                if (tracks.Count > 0)
                    return tracks;
                tracks.Dispose();
                await Task.Delay(100);
            }

            Assert.Fail("Timed out waiting for audio tracks.");
            throw new InvalidOperationException();
        }

        static async Task WaitForSeekable(MediaPlayer mp)
        {
            for (var i = 0; i < 30; i++)
            {
                if (mp.IsSeekable)
                    return;
                await Task.Delay(100);
            }

            Assert.Fail("Timed out waiting for media player to become seekable.");
        }

        static async Task<MediaTrack> WaitForSelectedAudioTrack(MediaPlayer mp)
        {
            for (var i = 0; i < 30; i++)
            {
                var track = mp.SelectedTrack(TrackType.Audio);
                if (track != null)
                    return track;
                await Task.Delay(100);
            }

            Assert.Fail("Timed out waiting for selected audio track.");
            throw new InvalidOperationException();
        }
    }
}
