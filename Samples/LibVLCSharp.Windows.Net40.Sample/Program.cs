using LibVLCSharp.Shared;
using System;

namespace LibVLCSharp.Windows.Net40.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Initialize();

            var libVLC = new LibVLC();
            var media = new Media(libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", FromType.FromLocation);
            var mp = new MediaPlayer(media);
            mp.Play();
            Console.ReadKey();
        }
    }
}