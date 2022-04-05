using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.OpenGL;
using Avalonia.ReactiveUI;
using System;
using System.Collections.Generic;

namespace AvaVLCWindow
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        private static AngleOptions GetAngleOptions()
        {
            return new AngleOptions()
            {
                AllowedPlatformApis = new List<AngleOptions.PlatformApi>
                {
                    AngleOptions.PlatformApi.DirectX11
                }
            };
        }

        private static Win32PlatformOptions GetWin32PlatformOptions()
        {
            return new Win32PlatformOptions
            {
                EnableMultitouch = true,
                AllowEglInitialization = false,
                UseDeferredRendering = true
            };
        }

        private static X11PlatformOptions GetX11PlatformOptions()
        {
            return new X11PlatformOptions
            {
                EnableMultiTouch = true,
                UseGpu = true,
                UseEGL = false,
                UseDeferredRendering = true
            };
        }

        private static AvaloniaNativePlatformOptions GetAvaloniaNativePlatformOptions()
        {
            return new AvaloniaNativePlatformOptions
            {
                UseGpu = true,                
                UseDeferredRendering = true
            };
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                //.With(GetWin32PlatformOptions())
                //.With(GetAngleOptions())
                //.With(GetX11PlatformOptions())
                //.With(GetAvaloniaNativePlatformOptions())
                
                //.UseSkia()
                .UseReactiveUI();
    }
}
