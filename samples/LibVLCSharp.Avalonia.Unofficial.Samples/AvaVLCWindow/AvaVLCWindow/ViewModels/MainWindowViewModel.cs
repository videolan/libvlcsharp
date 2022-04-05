using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using LibVLCSharp.Shared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AvaVLCWindow.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        LibVLC? _libVLC;
        public MediaPlayer? MediaPlayer;
        
        public MainWindowViewModel()
        {
            if (!Avalonia.Controls.Design.IsDesignMode)
            {
                //var os = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem;
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
                //string[] Media_AdditionalOptions = {
                //    $":avcodec-hw=any"
                //};
                string[] Media_AdditionalOptions = { };

                using var media = new Media(
                    _libVLC, 
                    new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"),
                    Media_AdditionalOptions
                    );
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

        public void Dispose()
        {
            MediaPlayer?.Dispose();
            _libVLC?.Dispose();
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
