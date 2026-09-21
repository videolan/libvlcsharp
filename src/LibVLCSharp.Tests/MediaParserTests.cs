using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaParserTests : BaseSetup
    {
        const int RaceIterations = 200;

        [Test]
        public void ParserTaskGetMediaBindingIsAvailable()
        {
            NativeBindingAssertions.HasDllImport(typeof(MediaParser), "LibVLCParserTaskGetMedia", "libvlc_parser_task_get_media");
        }

        [Test]
        [Platform("Win")]
        [SupportedOSPlatform("windows")]
        public void ParseAsyncDoesNotLeakCancellationRegistrationWhenCompletionRacesSubmit()
        {
            var leaks = RunSubmitRace((parser, media, token) => parser.ParseAsync(media, MediaParseOptions.ParseLocal, token));
            Assert.AreEqual(0, leaks);
        }

        [Test]
        [Platform("Win")]
        [SupportedOSPlatform("windows")]
        public void ThumbnailAsyncDoesNotLeakCancellationRegistrationWhenCompletionRacesSubmit()
        {
            // An audio-only media makes the thumbnailer fail fast, which widens the race window.
            var leaks = RunSubmitRace(async (parser, media, token) =>
            {
                using var picture = await parser.ThumbnailAsync(media, 32, 32, cancellationToken: token).ConfigureAwait(false);
            });
            Assert.AreEqual(0, leaks);
        }

        [Test]
        public void ThumbnailAsyncThrowsWhenCancelled()
        {
            using var media = new Media(new Uri(LocalVideoFile));
            using var parser = new MediaParser(_libVLC);
            Assert.ThrowsAsync<TaskCanceledException>(() => parser.ThumbnailAsync(media, 32, 32, cancellationToken: new CancellationToken(canceled: true)));
        }

        /// <summary>
        /// libvlc_parser_submit() may run the completion callback before it returns. Pinning the process to one CPU and
        /// submitting from a low priority thread lets the parser worker complete the request first. A cancellation
        /// registration that outlives the request keeps the disposed parser reachable from the cancellation token source.
        /// </summary>
        [SupportedOSPlatform("windows")]
        int RunSubmitRace(Func<MediaParser, Media, CancellationToken, Task> request)
        {
            var process = Process.GetCurrentProcess();
            var originalAffinity = process.ProcessorAffinity;
            var sources = new List<CancellationTokenSource>();
            var parsers = new List<WeakReference>();
            try
            {
                process.ProcessorAffinity = (IntPtr)1;
                for (var i = 0; i < RaceIterations; i++)
                {
                    var cts = new CancellationTokenSource();
                    sources.Add(cts);
                    var thread = new Thread(() => parsers.Add(SubmitAndWait(_libVLC, LocalAudioFile, request, cts.Token)))
                    {
                        Priority = ThreadPriority.Lowest
                    };
                    thread.Start();
                    thread.Join();
                }
            }
            finally
            {
                process.ProcessorAffinity = originalAffinity;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            GC.KeepAlive(sources);
            return parsers.FindAll(parser => parser.IsAlive).Count;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static WeakReference SubmitAndWait(LibVLC libVLC, string mrl, Func<MediaParser, Media, CancellationToken, Task> request, CancellationToken token)
        {
            using var media = new Media(new Uri(mrl));
            using var parser = new MediaParser(libVLC);
            request(parser, media, token).GetAwaiter().GetResult();
            return new WeakReference(parser);
        }
    }
}
