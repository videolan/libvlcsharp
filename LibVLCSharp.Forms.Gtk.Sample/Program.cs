using LibVLCSharp.Forms.Sample;
using LibVLCSharp.Forms.Shared;
using System;
using Xamarin.Forms.Platform.GTK;

namespace LibVLCSharp.Forms.Gtk.Sample
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            global::Gtk.Application.Init();
            LibVLCSharpFormsRenderer.Init();

            // For some reason, Xamarin does not pick the LibVLCSharp.Form.Platforms.Gtk assembly as a renderer assembly. Add it manually.
            global::Xamarin.Forms.Forms.Init(new [] { typeof(LibVLCSharp.Forms.Platforms.Gtk.VideoViewRenderer).Assembly });

            var app = new App();
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("GTK# LibVLCSharp sample");
            window.Show();

            global::Gtk.Application.Run();
        }
    }
}
