using Gtk;
using LibVLCSharp.Platforms.Gtk;
using LibVLCSharp.Shared;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Gtk.Sample
{
    using System;

    public class Program
    {
        public static void Main()
        {
            Core.Initialize();
            
            // Initializes the GTK# app
            Application.Init();
            
            // Create the window in code. This could be done in glade as well, I guess...
            Window myWin = new Window("LibVLCSharp.Gtk.Sample");
            myWin.Resize(800, 450);

            // Creates the video view, and adds it to the window
            VideoView view = new VideoView();
            myWin.Add(view);

            //Show Everything
            myWin.ShowAll();

            //Starts playing
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