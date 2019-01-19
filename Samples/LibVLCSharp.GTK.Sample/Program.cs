using Gtk;
using LibVLCSharp.Shared;

namespace LibVLCSharp.GTK.Sample
{
    public class Program
    {
        public static void Main()
        {
            Core.Initialize();

            // Initializes the GTK# app
            Application.Init();

            using (var libvlc = new LibVLC())
            using (var mediaPlayer = new MediaPlayer(libvlc))
            {
                // Create the window in code. This could be done in glade as well, I guess...
                Window myWin = new Window("LibVLCSharp.GTK.Sample");
                myWin.Resize(800, 450);

                // Creates the video view, and adds it to the window
                VideoView view = new VideoView { MediaPlayer = mediaPlayer };
                myWin.Add(view);

                //Show Everything
                myWin.ShowAll();

                //Starts playing
                using (var media = new Media(libvlc,
                    "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                    Media.FromType.FromLocation))
                {
                    mediaPlayer.Play(media);
                }

                myWin.DeleteEvent += (sender, args) => { Application.Quit(); };
                Application.Run();
            }
        }
    }
}