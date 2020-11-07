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
    /// Avalonia Video for Windows, Linux and Mac.
    /// </summary>
    public class Video : NativeControlHost
    {
        private IPlatformHandle PlatformHandle { get; set; }
        /// <summary>
        /// MediaPlayer Data Bound property
        /// </summary>
        /// <summary>
        /// Defines the <see cref="MediaPlayer"/> property.
        /// </summary>
        public static readonly DirectProperty<Video, MediaPlayer> MediaPlayerProperty =
            AvaloniaProperty.RegisterDirect<Video, MediaPlayer>(
                nameof(MediaPlayer),
                o => o.MediaPlayer,
                (o, v) => o.MediaPlayer = v,
                defaultBindingMode: BindingMode.TwoWay);

        private MediaPlayer _mediaPlayer;
        
        /// <summary>
        /// Gets or sets the MediaPlayer that will be displayed.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get { return _mediaPlayer; }
            set
            {
                if (_mediaPlayer != null)
                {
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

                if (PlatformHandle != null)
                {
                    SetHandler(value, PlatformHandle);
                }
                
                _mediaPlayer = value;
            }
        }

        private static void SetHandler(MediaPlayer player, IPlatformHandle platformHandle)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                player.Hwnd = platformHandle.Handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                player.XWindow = (uint) platformHandle.Handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                player.NsObject = platformHandle.Handle;
            }
        }
       
        /// <inheritdoc />
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            PlatformHandle = base.CreateNativeControlCore(parent);

            if (MediaPlayer == null)
                return PlatformHandle;
            
            SetHandler(MediaPlayer, PlatformHandle);
            
            return PlatformHandle;
        }

        /// <inheritdoc />
        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            base.DestroyNativeControlCore(control);
         
            if (PlatformHandle != null)
            {
                PlatformHandle = null;
            }
            
            if (MediaPlayer == null)
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MediaPlayer.Hwnd = IntPtr.Zero;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                MediaPlayer.XWindow = 0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                MediaPlayer.NsObject = IntPtr.Zero;
            }
        }
    }
}
