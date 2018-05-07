using System;

using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

using LibVLCSharp.Shared;

using Org.Videolan.Libvlc;

namespace LibVLCSharp.Platforms.Android
{
    public class VideoView : SurfaceView, IVLCVoutCallback, IVideoView
    {
        MediaPlayer _mediaPlayer;
        Instance _instance;
        AWindow _awindow;
        LayoutChangeListener _layoutListener;

        #region ctors

        public VideoView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => Init();

        public VideoView(Context context) : base(context) => Init();

        public VideoView(Context context, IAttributeSet attrs) : base(context, attrs) => Init();

        public VideoView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) => Init();

        public VideoView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) => Init();

        #endregion

        public MediaPlayer MediaPlayer => _mediaPlayer;
        public Instance Instance => _instance;

        public void Attach()
        {
            _awindow = new AWindow(new SurfaceCallback(_mediaPlayer));
            _awindow.AddCallback(this);
            _awindow.SetVideoView(this);
            _awindow.AttachViews();

            _mediaPlayer.SetAndroidContext(_awindow.Handle);

            _layoutListener = new LayoutChangeListener(_awindow);
            AddOnLayoutChangeListener(_layoutListener);
        }

        public void Detach()
        {
            _awindow.RemoveCallback(this);
            _awindow.DetachViews();

            _mediaPlayer.SetAndroidContext(IntPtr.Zero);

            RemoveOnLayoutChangeListener(_layoutListener);
            _layoutListener.Dispose();
            _layoutListener = null;

            _awindow.Dispose();
            _awindow = null;

        }

        public virtual void OnSurfacesCreated(IVLCVout vout)
        {
        }

        public virtual void OnSurfacesDestroyed(IVLCVout vout)
        {
        }

        void Init()
        {
            Core.Initialize();

            _instance = new Instance();
            _mediaPlayer = new MediaPlayer(_instance);
        }
    }
}