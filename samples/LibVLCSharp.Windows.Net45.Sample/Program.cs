using LibVLCSharp;
using System;

namespace LibVLCSharp.Windows.Net45.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using var libVLC = new LibVLC(enableDebugLogs: true);
            using var media = new Media(new Uri("https://streams.videolan.org/misc/unity-samples/BigBuckBunny.avi"));
            using var mp = new MediaPlayer(libVLC, media);
            mp.Play();

            Console.ReadKey();
        }
    }
}
