using Avalonia;
using Avalonia.Platform;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaVLCControl.ViewModels
{
    public class PlayerControlModel: ViewModelBase
    {
        LibVLC? _libVLC;
        public MediaPlayer? MediaPlayer;

        public PlayerControlModel()
        {
            if (!Avalonia.Controls.Design.IsDesignMode)
            {
                var os = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem;
                //if (os == OperatingSystemType.WinNT)
                //{
                //    var libVlcDirectoryPath = Path.Combine(Environment.CurrentDirectory, "libvlc", IsWin64() ? "win-x64" : "win-x86");
                //    Core.Initialize(libVlcDirectoryPath);
                //}
                //else
                {
                    Core.Initialize();
                }
                

                _libVLC = new LibVLC(
                    enableDebugLogs: true                    
                    );
                _libVLC.Log += VlcLogger_Event;

                MediaPlayer = new MediaPlayer(_libVLC) {};                

            }
        }

        private void VlcLogger_Event(object? sender, LogEventArgs l)
        {
            Debug.WriteLine(l.Message);
        }

        public void Play()
        {
            if (_libVLC != null && MediaPlayer != null)
            {
                
                using var media = new Media(_libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));                
                MediaPlayer.Play(media);
                media.Dispose();
            }
        }

        public void Stop()
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.Stop();
            }
        }
        
        public static bool IsWin64()
        {
            if (IntPtr.Size == 4)
            {
                return false;
            }
            return true;
        }
    }
}
