using System;
using LibVLCSharp.Shared;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Initialize();

            var libVLC = new LibVLC();
            var media = new Media(libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
            var mp = new MediaPlayer(media);
            mp.Play();
            Console.ReadKey();
        }
    }
}