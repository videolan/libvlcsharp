using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LibVLCSharp.Shared;

namespace LibVLCSharp.WinForms.Sample
{
    public partial class Form1 : Form
    {
        public RotateFlipType RotateFlipType { get; set; } = RotateFlipType.RotateNoneFlipNone;

        public LibVLC LibVlc;
        public MediaPlayer Mp;

        public long Duration = 0;

        public DateTime DateTimeStarted;

        public Form1()
        {
            if (!DesignMode)
            {
                Core.Initialize();
            }

            InitializeComponent();
            LibVlc = new LibVLC(GetPlayerParameters());
            Mp = new MediaPlayer(LibVlc);
            videoView1.MediaPlayer = Mp;
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //LibVlc.Log += LibVLC_Log;

            //_mp.Media = new Media(_libVLC, "rtsp://admin:admin123@192.168.2.210:554", FromType.FromLocation);
            Mp.Media = new Media(LibVlc, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", FromType.FromLocation);

            var mediaConfiguration = new MediaConfiguration { NetworkCaching = 150 };

            Mp.Media.AddOption(mediaConfiguration);

            //_mp.Media.AddOption(@":sout=#duplicate{dst=display, dst=file{dst=C:\ProgramData\HRID\HDView\Videos\ABC.mp4}");

            DateTimeStarted = DateTime.Now;

            Mp.Play(Mp.Media);
        }

        private void LibVLC_Log(object sender, LogEventArgs e)
        {
            Debugger.Log(0,e.SourceFile, e.Message);
        }

        private string[] GetPlayerParameters()
        {
            switch (RotateFlipType)
            {
                case RotateFlipType.RotateNoneFlipNone:
                    return new[]
                    {
                        "--clock-synchro=0",
                        "--clock-jitter=0"
                    };
                case RotateFlipType.Rotate90FlipNone:
                    return new[]
                    {
                        "--vout-filter=transform",
                        "--transform-type=90",
                        "--video-filter=transform{true}",
                        "--clock-synchro=0",
                        "--clock-jitter=0"
                    };
                case RotateFlipType.Rotate180FlipNone:
                    return new[]
                    {
                        "--vout-filter=transform",
                        "--transform-type=180",
                        "--video-filter=transform{true}",
                        "--clock-synchro=0",
                        "--clock-jitter=0"
                    };
                case RotateFlipType.Rotate270FlipNone:
                    return new[]
                    {
                        "--vout-filter=transform",
                        "--transform-type=270",
                        "--video-filter=transform{true}",
                        "--clock-synchro=0",
                        "--clock-jitter=0"
                    };
            }
            return new[]
            {
                "--clock-synchro=0",
                "--clock-jitter=0"
            };
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var streamWriter = new StreamWriter(@"C:\ProgramData\HRID\HDView\Videos\ABC.vtt");

            var from = TimeSpan.FromMilliseconds(0).ToString(@"hh\:mm\:ss\.fff");
            var to = TimeSpan.FromMilliseconds(Mp.Time).ToString(@"hh\:mm\:ss\.fff");

            var to2 = (DateTime.Now - DateTimeStarted).ToString(@"hh\:mm\:ss\.fff");

            streamWriter.WriteLine(from + " --> " + to + "\n" + to + " " + to2);

            streamWriter.Close();

            Mp.Stop();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Mp.Stop();
            
            Mp.Media.AddOption(@":sout=#duplicate{dst=display, dst=file{dst=C:\ProgramData\HRID\HDView\Videos\ABC.mp4}");

            Mp.Play();

            timer1.Enabled = false;
        }
    }
}
