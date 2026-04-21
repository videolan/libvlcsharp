
namespace LibVLCSharp.Eto.Sample
{
    using global::Eto.Forms;
    using global::Eto.Drawing;
    using LibVLCSharp.Shared;
    using System;

    public class MainForm : Form
    {
        LibVLC _libVLC;
        MediaPlayer _mp;
        VideoView videoView;
        public MainForm()
        {
            Title = $"Eto.Forms - { Platform } - { Environment.OSVersion }";
            MinimumSize = new Size(16, 9) * 10;
            Size = MinimumSize * 6;

            Core.Initialize();

            _libVLC = new LibVLC();
            _mp = new MediaPlayer(_libVLC);
            
            Content = videoView = new VideoView();
        }
        protected override void OnShown(EventArgs e)
        {
            videoView.MediaPlayer = _mp;
            var media = new Media(_libVLC, new Uri("https://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.avi"));
            _mp.Play(media);
            media.Dispose();
            base.OnShown(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            _mp.Stop();
            _mp.Dispose();
            _libVLC.Dispose();
            videoView.Dispose();
            base.OnClosed(e);
        }
    }
}
