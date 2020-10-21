using LibVLCSharp;
using System;

namespace LibVLCSharp.Windows.Net45.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Initialize();

            using var libVLC = new LibVLC(enableDebugLogs: true);
            using var media = new Media(libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));
            using var mp = new MediaPlayer(media);
            mp.Play();

            Console.ReadKey();
        }
    }
}
