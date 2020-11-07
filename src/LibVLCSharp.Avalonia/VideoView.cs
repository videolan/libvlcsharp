using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Avalonia
{
    /// <summary>
    /// Avalonia VideoView for Windows, Linux and Mac.
    /// </summary>
    public class VideoView : NativeControlHost
    {
        private IPlatformHandle? _platformHandle = null;
        private MediaPlayer? _mediaPlayer = null;

        /// <summary>
        /// MediaPlayer Data Bound property
        /// </summary>
        /// <summary>
        /// Defines the <see cref="MediaPlayer"/> property.
        /// </summary>
        public static readonly DirectProperty<VideoView, MediaPlayer?> MediaPlayerProperty =
            AvaloniaProperty.RegisterDirect<VideoView, MediaPlayer?>(
                nameof(MediaPlayer),
                o => o.MediaPlayer,
                (o, v) => o.MediaPlayer = v,
                defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Gets or sets the MediaPlayer that will be displayed.
        /// </summary>
        public MediaPlayer? MediaPlayer
        {
            get { return _mediaPlayer; }
            set
            {
                if (ReferenceEquals(_mediaPlayer, value))
                {
                    return;
                }

                Detach();
                _mediaPlayer = value;
                Attach();
            }
        }

        private void Attach()
        {
            if(_mediaPlayer == null || _platformHandle == null || !IsInitialized)
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _mediaPlayer.Hwnd = _platformHandle.Handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _mediaPlayer.XWindow = (uint)_platformHandle.Handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _mediaPlayer.NsObject = _platformHandle.Handle;
            }
        }

        private void Detach()
        {
            if (_mediaPlayer == null)
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _mediaPlayer.Hwnd = IntPtr.Zero;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _mediaPlayer.XWindow = 0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _mediaPlayer.NsObject = IntPtr.Zero;
            }
        }

        /// <inheritdoc />
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            _platformHandle = base.CreateNativeControlCore(parent);

            if (_mediaPlayer == null)
                return _platformHandle;

            Attach();

            return _platformHandle;
        }

        /// <inheritdoc />
        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            Detach();

            base.DestroyNativeControlCore(control);

            if (_platformHandle != null)
            {
                _platformHandle = null;
            }
        }
    }
}
