using LibVLCSharp;
using System;

namespace LibVLCSharp.Windows.Net45.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using var libVLC = new LibVLC(enableDebugLogs: true);
            using var media = new Media(new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));
            using var mp = new MediaPlayer(libVLC, media);
            mp.Play();

            Console.ReadKey();
        }
    }
}
