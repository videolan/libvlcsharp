using LibVLCSharp.Shared;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LibVLCSharp.WinForms
{
    /// <summary>
    /// WinForms VideoView control with a LibVLCSharp MediaPlayer
    /// </summary>
    public class VideoView : Control, IVideoView, IDisposable
    {
        /// <summary>
        /// The VideoView constructor.
        /// </summary>
        public VideoView()
        {
            BackColor = System.Drawing.Color.Black;
        }

        /// <inheritdoc />
        protected override void CreateHandle()
        {
            base.CreateHandle();
            if (!IsInDesignMode)
                Attach();
        }

        /// <inheritdoc />
        protected override void DestroyHandle()
        {
            if (!IsInDesignMode)
                Detach();
            base.DestroyHandle();
        }

        MediaPlayer? _mp;

        /// <summary>
        /// The MediaPlayer attached to this view (or null)
        /// </summary>
        public MediaPlayer? MediaPlayer
        {
            get => _mp;
            set
            {
                if (ReferenceEquals(_mp, value))
                {
                    return;
                }

                Detach();
                _mp = value;
                Attach();
            }
        }

        bool IsInDesignMode
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;

                Control ctrl = this;
                while (ctrl != null)
                {
                    if ((ctrl.Site != null) && ctrl.Site.DesignMode)
                        return true;
                    ctrl = ctrl.Parent;
                }
                return false;
            }
        }

        void Detach()
        {
            if (_mp == null || _mp.NativeReference == IntPtr.Zero)
                return;

            _mp.Hwnd = IntPtr.Zero;
        }

        void Attach()
        {
            if (_mp == null || _mp.NativeReference == IntPtr.Zero)
                return;

            _mp.Hwnd = Handle;
        }

        bool disposedValue;

        /// <summary>
        /// Detaches the MediaPlayer from this VideoView
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (MediaPlayer != null && MediaPlayer.NativeReference != IntPtr.Zero)
                    {
                        MediaPlayer.Hwnd = IntPtr.Zero;
                    }
                }
                disposedValue = true;
            }
        }
    }
}
