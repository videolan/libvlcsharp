using LibVLCSharp.Shared;
using System;

namespace LibVLCSharp.Windows.Net40.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using var libVLC = new LibVLC(enableDebugLogs: true);
            using var media = new Media(libVLC, new Uri("https://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.avi"));
            using var mp = new MediaPlayer(media);
            mp.Play();

            Console.ReadKey();
        }
    }
}
