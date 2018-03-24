using System;
using System.IO;
using System.Runtime.InteropServices;

using VideoLAN.LibVLC;
using Media = VideoLAN.LibVLC.Media;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var myPath = new Uri(typeof(Instance).Assembly.CodeBase).LocalPath;
            var appExecutionDirectory = Path.GetDirectoryName(myPath);
            if (appExecutionDirectory == null)
                throw new NullReferenceException(nameof(appExecutionDirectory));

            // TODO: check if running in a Store app
            var arch = Environment.Is64BitProcess ? "win-x64" : "win-x86";
            const string libvlc = "libvlc";
            const string libvlccore = "libvlccore";

            var libvlccorePath = Path.Combine(Path.Combine(appExecutionDirectory, libvlc), Path.Combine(arch, $"{libvlccore}.dll"));
            var libvlcPath = Path.Combine(Path.Combine(appExecutionDirectory, libvlc), Path.Combine(arch, $"{libvlc}.dll"));

            var r1 = LoadLibrary(libvlccorePath);
            var r2 = LoadLibrary(libvlcPath);

            var instance = new Instance();
            var media = new Media(instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
            var mp = new MediaPlayer(media);
            mp.Play();
            Console.ReadKey();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadPackagedLibrary(string dllToLoad);
    }
}
