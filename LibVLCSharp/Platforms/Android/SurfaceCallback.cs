using System;
using LibVLCSharp.Shared;
using Org.Videolan.Libvlc;

namespace LibVLCSharp.Platforms.Android
{
    public class SurfaceCallback : Java.Lang.Object, AWindow.ISurfaceCallback
    {
        static readonly object _locker = new object();
        readonly MediaPlayer _mp;

        public SurfaceCallback() { }

        public SurfaceCallback(MediaPlayer mp)
        {
            _mp = mp;
        }

        public void OnSurfacesCreated(AWindow vout)
        {
            var play = false;
            var enableVideo = false;

            lock (_locker)
            {
                if (!_mp.IsPlaying /* && mp.PlayRequested*/)
                {
                    play = true;
                }
                else if (_mp.VoutCount == 0)
                    enableVideo = true;
            }
            if (play)
                _mp.Play();
            else if (enableVideo)
            {
                lock (_locker)
                {
                    _mp.SetVideoTrack(0);
                }
            }
        }

        public void OnSurfacesDestroyed(AWindow vout)
        {
            var disableVideo = false;
            lock (_locker)
            {
                if (_mp.VoutCount > 0)
                    disableVideo = true;
            }
            if (disableVideo)
            {
                //_mp.VideoTrackEnabled = false;
            }
        }
    }
}