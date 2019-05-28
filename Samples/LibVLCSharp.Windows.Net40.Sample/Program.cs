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
            var media = new Media(libVLC, "https://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4", FromType.FromLocation);
            var mp = new MediaPlayer(media);
            mp.Play();
            Console.ReadKey();
        }
    }
}