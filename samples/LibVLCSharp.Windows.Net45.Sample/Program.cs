using LibVLCSharp;
using System;

namespace LibVLCSharp.Windows.Net45.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Initialize();

            using var libVLC = new LibVLC();
            using var media = new Media(libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", FromType.FromLocation);
            using var mp = new MediaPlayer(media);
            mp.Play();

            Console.ReadKey();
        }
    }
}
