using System;

using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

using LibVLCSharp.Shared;

using Org.Videolan.Libvlc;

namespace LibVLCSharp.Platforms.Android
{
    public class VideoView : SurfaceView, IVLCVoutCallback, AWindow.ISurfaceCallback
    {
        AWindow _awindow;
        LayoutChangeListener _layoutListener;

        #region ctors

        public VideoView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public VideoView(Context context) : base(context) { }

        public VideoView(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public VideoView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }

        public VideoView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes) { }

        #endregion

        private ISource _source;
        public ISource Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    Detach();
                    _source = value;
                    Attach();
                }
            }
        }

        void Attach()
        {
            if (Source != null)
            {
                _awindow = new AWindow(this);
                _awindow.AddCallback(this);
                _awindow.SetVideoView(this);
                _awindow.AttachViews();

                Source.SetAndroidContext(_awindow.Handle);

                _layoutListener = new LayoutChangeListener(_awindow);
                AddOnLayoutChangeListener(_layoutListener);
            }
        }

        void Detach()
        {
            if (Source != null)
            {
                _awindow.RemoveCallback(this);
                _awindow.DetachViews();

                Source.SetAndroidContext(IntPtr.Zero);

                RemoveOnLayoutChangeListener(_layoutListener);
                _layoutListener.Dispose();
                _layoutListener = null;

                _awindow.Dispose();
                _awindow = null;
            }
        }

        public virtual void OnSurfacesCreated(IVLCVout vout)
        {
        }

        public virtual void OnSurfacesDestroyed(IVLCVout vout)
        {
        }        

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Detach();
        }

        void AWindow.ISurfaceCallback.OnSurfacesCreated(AWindow aWindow)
        {
        }

        void AWindow.ISurfaceCallback.OnSurfacesDestroyed(AWindow aWindow)
        {
        }
    }
}