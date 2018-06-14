using Gtk;
using LibVLCSharp.Platforms.Gtk;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Gtk.Sample
{
    using System;

    public class Program
    {
        public static void Main()
        {
            Application.Init();
            
            Window myWin = new Window("LibVLCSharp.Gtk.Sample");
            myWin.Resize(800, 450);

            VideoView view = new VideoView();

            //Add the label to the form
            myWin.Add(view);

            //Show Everything
            myWin.ShowAll();

            view.MediaPlayer.Play(new Media(view.LibVLC,
                    "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                    Media.FromType.FromLocation));

            myWin.DeleteEvent += (sender, args) => {
                Application.Quit();
            };

            Application.Run();
        }

    }
}