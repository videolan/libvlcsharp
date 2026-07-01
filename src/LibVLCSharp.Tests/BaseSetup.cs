using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    public abstract class BaseSetup
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable. It is initialized in the SetUp, so before the tests take place.
        protected LibVLC _libVLC;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        [SetUp]
        public void SetUp()
        {
            _libVLC = new LibVLC(/*"--no-audio", "--no-video", */"--verbose=2");
        }

        [TearDown]
        public void TearDown()
        {
            _libVLC?.Dispose();
        }

        protected string RemoteAudioStream => "http://streams.videolan.org/streams/mp3/Owner-MPEG2.5.mp3";

        protected string RemoteVideoStream => "https://streams.videolan.org/streams/mp4/Jago-Youtube.mp4";

        protected string LocalAudioFile => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "sample.mp3");

        protected string LocalAudioFileSpecialCharacter => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "motörhead.mp3");

        protected string AttachedThumbnailsMedia => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "multiple-images.mp3");

        /// <summary>
        /// Requests an asynchronous stop and waits up to 3 seconds for it to complete.
        /// </summary>
        protected static async Task<bool> StopWithTimeout(MediaPlayer mp)
        {
            var stopTask = mp.StopAsync();
            return await Task.WhenAny(stopTask, Task.Delay(3000)) == stopTask && await stopTask;
        }

        /// <summary>
        /// Creates a <see cref="TaskCompletionSource{T}"/> whose continuations run asynchronously, so awaiters are
        /// not resumed synchronously on a native LibVLC callback thread.
        /// </summary>
        protected static TaskCompletionSource<T> NewCompletionSource<T>()
#if NETFRAMEWORK || NETSTANDARD2_0 || UWP
            => new TaskCompletionSource<T>();
#else
            => new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
#endif
    }
}
