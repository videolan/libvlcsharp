using Android.App;
using Android.OS;
using Android.Views;
using LibVLCSharp.Shared;
using Org.Videolan.Libvlc;

namespace LibVLCSharp.Platforms.Android
{
    [Activity(Label = "VideoView")]
    public abstract class VideoView : Activity, IVLCVoutCallback
    {
        MediaPlayer _mediaPlayer;
        Instance _instance;
        AWindow _awindow;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Core.Initialize();

            _instance = new Instance();
            _mediaPlayer = new MediaPlayer(_instance);

            _awindow = new AWindow(new SurfaceCallback(_mediaPlayer));
            _awindow.AddCallback(this);
            _mediaPlayer.SetAndroidContext(_awindow.Handle);
        }

        public void AttachSurfaceView(SurfaceView surfaceView)
        {
            _awindow.SetVideoView(surfaceView);
            _awindow.AttachViews();
            surfaceView.AddOnLayoutChangeListener(new LayoutChangeListener(_awindow));
        }

        public virtual void OnSurfacesCreated(IVLCVout p0)
        {
        }

        public virtual void OnSurfacesDestroyed(IVLCVout p0)
        {
        }

        public MediaPlayer MediaPlayer => _mediaPlayer;
    }
}