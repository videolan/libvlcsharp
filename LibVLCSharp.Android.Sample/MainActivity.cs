using System;
using System.Runtime.InteropServices;
using System.Security;

using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Com.Example.Orgvideolanlibvlc;
using Java.Interop;

using VideoLAN.LibVLC;

namespace LibVLCSharp.Android.Sample
{
    [Activity(Label = "LibVLCSharp.Android.Sample", MainLauncher = true)]
    public class MainActivity : Activity, IVLCVoutCallback
    {
        Instance _instance;
        MediaPlayer _mp;
        AWindow _awindow;
        SurfaceView _surfaceView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
           
        }
        
        protected override void OnResume()
        {
            base.OnResume();


            var r = JniOnLoad(JniRuntime.CurrentRuntime.InvocationPointer);

            _instance = new Instance();
            _mp = new MediaPlayer(_instance);


            _awindow = new AWindow(new SurfaceCallback(_mp));
            _awindow.AddCallback(this);
            _surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);


            _awindow.SetVideoView(_surfaceView);
            _awindow.AttachViews();
            _surfaceView.AddOnLayoutChangeListener(new LayoutChangeListener(_awindow));

            _mp.SetAndroidContext(_awindow.Handle);

            _mp.Media = new Media(_instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
            _mp.Play();
        }

        [DllImport("libvlc", EntryPoint = "JNI_OnLoad")]
        protected static extern int JniOnLoad(IntPtr javaVm, IntPtr reserved = default(IntPtr));

        [SuppressUnmanagedCodeSecurity]
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "libvlc_get_version")]
        internal static extern IntPtr LibVLCVersion();

        Version _dllVersion;
        Version DllVersion
        {
            get
            {
                if (_dllVersion != null) return _dllVersion;
                var version = Marshal.PtrToStringAnsi(LibVLCVersion());
                if (string.IsNullOrEmpty(version))
                    throw new VLCException("Cannot retrieve native dll version");

                version = version.Split('-', ' ')[0];
                _dllVersion = new Version(version);
                return _dllVersion;
            }
        }
        
        //public void OnSurfaceCreated(IVLCVout vlcVout)
        //{
        //    System.Diagnostics.Debug.WriteLine("OnSurfaceCreated");
        //}

        //public void OnSurfacesDestroyed(IVLCVout vlcVout)
        //{
        //    System.Diagnostics.Debug.WriteLine("OnSurfacesDestroyed");
        //}

        //public void OnNewVideoLayout(IVLCVout vlcVout, int width, int height, int visibleWidth, int visibleHeight, int sarNum,
        //    int sarDen)
        //{
        //    _awindow.SetWindowSize(width, height);
        //}

        //public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        //{

        //}

        //public void SurfaceCreated(ISurfaceHolder holder)
        //{

        //}

        //public void SurfaceDestroyed(ISurfaceHolder holder)
        //{

        //}
        public void OnSurfacesCreated(IVLCVout p0)
        {
        }

        public void OnSurfacesDestroyed(IVLCVout p0)
        {
        }
    }

    public class SurfaceCallback : Java.Lang.Object, AWindow.ISurfaceCallback
    {
        static readonly object _locker = new object();
        readonly MediaPlayer _mp;

        public SurfaceCallback() {}

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

        public void Dispose()
        {
            
        }

        public IntPtr Handle { get; }
    }

    public class LayoutChangeListener : Java.Lang.Object, View.IOnLayoutChangeListener
    {
        readonly AWindow _aWindow;

        public LayoutChangeListener(AWindow awindow)
        {
            _aWindow = awindow;
        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight,
            int oldBottom)
        {
            if (left != oldLeft || top != oldTop || right != oldRight || bottom != oldBottom)
            {
                _aWindow.SetWindowSize(right - left, bottom - top);
            }

        }
    }
}