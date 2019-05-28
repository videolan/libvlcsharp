using System;
using LibVLCSharp.Shared;

namespace LibVLCSharp.NetCore.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Initialize();

            using(var libVLC = new LibVLC())
            {
                var media = new Media(libVLC, "https://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4", FromType.FromLocation);
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