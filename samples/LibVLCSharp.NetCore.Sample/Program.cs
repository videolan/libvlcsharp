using System;
using LibVLCSharp.Shared;

namespace LibVLCSharp.NetCore.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using var libVLC = new LibVLC(enableDebugLogs: true);
            using var media = new Media(libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4"));
            using var mp = new MediaPlayer(media);
            mp.Play();
            Console.ReadKey();
        }
    }
}
