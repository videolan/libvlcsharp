using LibVLCSharp;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LibVLCSharp.Downloader.Cli.Sample
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: downloader <URL-or-file> <output-file>");
                return 1;
            }

            try
            {
                using var libVLC = new LibVLC("--quiet");
                using var downloader = new MediaDownloader(libVLC);
                using var media = Uri.TryCreate(args[0], UriKind.Absolute, out var uri)
                    ? new Media(uri) : new Media(Path.GetFullPath(args[0]));
                using var cancellation = new CancellationTokenSource();
                ConsoleCancelEventHandler cancelHandler = (_, e) => { e.Cancel = true; cancellation.Cancel(); };
                Console.CancelKeyPress += cancelHandler;
                try
                {
                    await downloader.DownloadAsync(media, args[1], new ConsoleProgress(), cancellation.Token);
                    Console.WriteLine($"\nFinished: {Path.GetFullPath(args[1])}");
                    return 0;
                }
                finally { Console.CancelKeyPress -= cancelHandler; }
            }
            catch (OperationCanceledException)
            {
                Console.Error.WriteLine("\nDownload cancelled. The output may be incomplete.");
                return 1;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                return 1;
            }
        }

        // Report inline so queued console updates cannot appear after the completion message.
        sealed class ConsoleProgress : IProgress<DownloadProgress>
        {
            public void Report(DownloadProgress value) =>
                Console.Write($"\rDownloaded {value.BytesWritten:N0} / {value.TotalBytes:N0} bytes");
        }
    }
}
