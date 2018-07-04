using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Wpf
{
    public class VLCHostProvider : IDisposable, INotifyPropertyChanged
    {
        private MediaPlayer mediaPlayer;
        private LibVLC libVLC;
        private MediaPlayer.LibVLCVideoFormatCb VideoFormatCb;
        private MediaPlayer.LibVLCVideoCleanupCb VideoCleanupCb;
        private MediaPlayer.LibVLCVideoDisplayCb VideoDisplayCb;
        private MediaPlayer.LibVLCVideoLockCb VideoLockCb;
        private MediaPlayer.LibVLCVideoUnlockCb VideoUnlockCb;

        public event PropertyChangedEventHandler PropertyChanged;

        public VLCHostProvider()
        {
            Core.Initialize();
            libVLC = new LibVLC();
            mediaPlayer = new MediaPlayer(libVLC);

            VideoFormatCb = new MediaPlayer.LibVLCVideoFormatCb(VideoFormat);
            VideoCleanupCb =new MediaPlayer.LibVLCVideoCleanupCb(VideoCleanUp);
            VideoDisplayCb = new MediaPlayer.LibVLCVideoDisplayCb(VideoDisplay);
            VideoLockCb = new MediaPlayer.LibVLCVideoLockCb(VideoLock);
            VideoUnlockCb = new MediaPlayer.LibVLCVideoUnlockCb(VideoUnlock);

            mediaPlayer.SetVideoFormatCallbacks(VideoFormatCb, VideoCleanupCb);
            mediaPlayer.SetVideoCallbacks(VideoLockCb, VideoUnlockCb, VideoDisplayCb);
        }


        //TODO https://github.com/ZeBobo5/Vlc.DotNet/blob/develop/src/Vlc.DotNet.Wpf/VlcVideoSourceProvider.cs
        private uint VideoFormat(ref IntPtr userData, IntPtr chroma, ref uint width, ref uint height, ref uint pitches, ref uint lines)
        {
            return 0;
        }

        private void VideoCleanUp(ref System.IntPtr opaque)
        {

        }

        private void VideoDisplay(System.IntPtr opaque, System.IntPtr picture)
        {

        }

        private System.IntPtr VideoLock(System.IntPtr opaque, System.IntPtr planes)
        {
            return IntPtr.Zero;
        }

        private void VideoUnlock(System.IntPtr opaque, System.IntPtr picture, System.IntPtr planes)
        {

        }

        public void Dispose()
        {
            mediaPlayer.Dispose();
            libVLC.Dispose();
        }
    }
}
