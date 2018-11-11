using System;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Windows.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Initialize();

            using(var libVLC = new LibVLC())
            {
                var media = new Media(libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
                using(var mp = new MediaPlayer(media))
                {
                    media.Dispose();
                    mp.Play();
                    Console.ReadKey();
                }
            }
        }
    }
}