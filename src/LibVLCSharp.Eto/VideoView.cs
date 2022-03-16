using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Eto
{
    /// <summary>
    /// Eto.Forms VideoView control with a LibVLCSharp MediaPlayer
    /// </summary>
    public class VideoView : Panel, IVideoView, IDisposable
    {
        /// <summary>
        /// The VideoView constructor.
        /// </summary>
        public VideoView()
        {
            BackgroundColor = Colors.Black;
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

                MpAttach(IntPtr.Zero);
                _mp = value;
                MpAttach(NativeHandle);
            }
        }

        void MpAttach(IntPtr handle)
        {
            if (_mp == null || _mp.NativeReference == IntPtr.Zero)
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _mp.Hwnd = handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _mp.NsObject = handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _mp.XWindow = (uint)handle;
            }
            else
            {
                throw new InvalidOperationException("Unsupported OSPlatform");
            }
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
                    MpAttach(IntPtr.Zero);

                disposedValue = true;
            }
        }
    }
}
