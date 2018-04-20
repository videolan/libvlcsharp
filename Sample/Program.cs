using System;
using LibVLCSharp;
using LibVLCSharp.Shared;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Initialize();

            var instance = new Instance();
            var media = new Media(instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
            var mp = new MediaPlayer(media);
            mp.Play();
            Console.ReadKey();
        }
    }
}