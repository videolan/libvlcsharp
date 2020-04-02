using System;
using LibVLCSharp.Shared;

namespace LibVLCSharp.NetCore.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Initialize();

            using var libVLC = new LibVLC();
            using var media = new Media(libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4", FromType.FromLocation);
            using var mp = new MediaPlayer(media);
            mp.Play();
            Console.ReadKey();
        }
    }
}
