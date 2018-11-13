using System;

using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

using LibVLCSharp.Shared;

using Org.Videolan.Libvlc;

namespace LibVLCSharp.Platforms.Android
{
    public class VideoView : SurfaceView, IVLCVoutCallback, IVideoView, AWindow.ISurfaceCallback
    {
        MediaPlayer _mediaPlayer;
        AWindow _awindow;
        LayoutChangeListener _layoutListener;

        #region ctors

        public VideoView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public VideoView(Context context) : base(context)
        {
        }

        public VideoView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public VideoView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public VideoView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) 
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion

        /// <summary>
        /// The MediaPlayer object attached to this VideoView. Use this to manage playback and more
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set
            {
                if (_mediaPlayer != value)
                {
                    Detach();
                    _mediaPlayer = value;

                    if (_mediaPlayer != null)
                    {
                        Attach();
                    }
                }
            }
        }

        void Attach()
        {
            if (_mediaPlayer == null)
                throw new NullReferenceException(nameof(_mediaPlayer));

            _awindow = new AWindow(this);
            _awindow.AddCallback(this);
            _awindow.SetVideoView(this);
            _awindow.AttachViews();

            _mediaPlayer.SetAndroidContext(_awindow.Handle);

            _layoutListener = new LayoutChangeListener(_awindow);
            AddOnLayoutChangeListener(_layoutListener);
        }

        void Detach()
        {
            _awindow?.RemoveCallback(this);
            _awindow?.DetachViews();

            _mediaPlayer?.SetAndroidContext(IntPtr.Zero);

            if (_layoutListener != null)
                RemoveOnLayoutChangeListener(_layoutListener);

            _layoutListener?.Dispose();
            _layoutListener = null;

            _awindow?.Dispose();
            _awindow = null;
        }

        /// <summary>
        /// This is to workaround the first layout change not being raised when VideoView is behind a Xamarin.Forms custom renderer on Android.
        /// </summary>
        public void TriggerLayoutChangeListener() => _awindow?.SetWindowSize(Width, Height);
        
        public virtual void OnSurfacesCreated(IVLCVout vout)
        {
        }

        public virtual void OnSurfacesDestroyed(IVLCVout vout)
        {
        }

        /// <summary>
        /// Detach the mediaplayer from the view and dispose the view
        /// </summary>
        /// <param name="disposing"></param>
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