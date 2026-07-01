using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LibVLCSharp;

namespace LibVLCSharp.Thumbnailer.Cli.Sample
{
    static class Program
    {
        const uint ThumbnailWidth = 2048;
        const uint ThumbnailHeight = 2048;
        const long ThumbnailTimeout = 10000;

        static readonly long[] DefaultTimestampsMs = { 0, 2000, 5000, 8000 };

        static void Main(string[] args)
        {
            Environment.Exit(RunAsync(args).GetAwaiter().GetResult());
        }

        static async Task<int> RunAsync(string[] args)
        {
            if (args.Length == 0 || IsHelp(args[0]))
            {
                PrintUsage();
                return args.Length == 0 ? 1 : 0;
            }

            var videoPath = Path.GetFullPath(args[0]);
            if (!File.Exists(videoPath))
            {
                Console.Error.WriteLine($"Video file not found: {videoPath}");
                return 1;
            }

            if (!TryParseTimestamps(args, out var timestampsMs))
            {
                PrintUsage();
                return 1;
            }

            try
            {
                Core.Initialize();

                var libVLC = new LibVLC();
                var videoUri = new Uri(videoPath);

                for (var i = 0; i < timestampsMs.Length; i++)
                {
                    var timestampMs = timestampsMs[i];
                    Console.WriteLine($"Grabbing frame at {timestampMs}ms...");

                    using var media = new Media(videoUri, ":no-audio");
                    var parsed = await media.ParseAsync(libVLC, MediaParseOptions.ParseLocal);
                    if (parsed != MediaParsedStatus.Done)
                    {
                        Console.Error.WriteLine($"Failed to parse media. Parse status: {parsed}");
                        return 1;
                    }

                    using var picture = await media.GenerateThumbnailAsync(
                        libVLC,
                        timestampMs,
                        ThumbnailerSeekSpeed.Precise,
                        ThumbnailWidth,
                        ThumbnailHeight,
                        false,
                        PictureType.Argb,
                        ThumbnailTimeout);

                    if (picture == null)
                    {
                        Console.Error.WriteLine($"No thumbnail was generated at {timestampMs}ms.");
                        return 1;
                    }

                    var centerPixelColor = ReadCenterPixelColor(picture);
                    var frameHash = ComputeFnv1A64(picture);
                    Console.WriteLine(
                        $"Frame at {timestampMs}ms grabbed. " +
                        $"Generated at {picture.Time}ms, size {picture.Width}x{picture.Height}, " +
                        $"center pixel color: {centerPixelColor}, frame hash: 0x{frameHash:x16}");

                    if (i == timestampsMs.Length - 1)
                        Environment.Exit(0);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }
        }

        static bool TryParseTimestamps(string[] args, out long[] timestampsMs)
        {
            if (args.Length == 1)
            {
                timestampsMs = DefaultTimestampsMs;
                return true;
            }

            timestampsMs = new long[args.Length - 1];
            for (var i = 1; i < args.Length; i++)
            {
                if (!long.TryParse(args[i], NumberStyles.None, CultureInfo.InvariantCulture, out var timestampMs) ||
                    timestampMs < 0)
                {
                    Console.Error.WriteLine($"Invalid timestamp: {args[i]}");
                    return false;
                }

                timestampsMs[i - 1] = timestampMs;
            }

            return true;
        }

        static int ReadCenterPixelColor(Picture picture)
        {
            if (picture.Type != PictureType.Argb)
                throw new InvalidOperationException($"Expected an ARGB picture, got {picture.Type}.");

            var buffer = picture.Buffer;
            var offset = checked((int)(((picture.Height / 2) * picture.Stride) + ((picture.Width / 2) * 4)));
            if (offset + 4 > (long)buffer.size)
                throw new InvalidOperationException("The generated picture buffer is smaller than expected.");

            return Marshal.ReadInt32(buffer.buffer, offset);
        }

        static ulong ComputeFnv1A64(Picture picture)
        {
            if (picture.Type != PictureType.Argb)
                throw new InvalidOperationException($"Expected an ARGB picture, got {picture.Type}.");

            const ulong fnvOffsetBasis = 14695981039346656037;
            const ulong fnvPrime = 1099511628211;

            var buffer = picture.Buffer;
            var rowBytes = checked((long)picture.Width * 4);
            var stride = picture.Stride;
            var requiredBytes = checked(((long)picture.Height - 1) * stride + rowBytes);
            if (rowBytes > stride || requiredBytes > (long)buffer.size)
                throw new InvalidOperationException("The generated picture buffer is smaller than expected.");

            var hash = fnvOffsetBasis;

            for (var y = 0L; y < picture.Height; y++)
            {
                var rowOffset = checked((int)(y * stride));
                for (var x = 0; x < rowBytes; x++)
                {
                    hash ^= Marshal.ReadByte(buffer.buffer, rowOffset + (int)x);
                    hash *= fnvPrime;
                }
            }

            return hash;
        }

        static bool IsHelp(string value)
        {
            return value == "-h" || value == "--help" || value == "/?";
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project samples/LibVLCSharp.Thumbnailer.Cli.Sample -- <video-path> [timestamp-ms ...]");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project samples/LibVLCSharp.Thumbnailer.Cli.Sample -- C:\\Videos\\sample.mp4");
            Console.WriteLine("  dotnet run --project samples/LibVLCSharp.Thumbnailer.Cli.Sample -- C:\\Videos\\sample.mp4 0 2000 5000 8000");
        }
    }
}
