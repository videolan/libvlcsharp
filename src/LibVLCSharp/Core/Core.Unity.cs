#if UNITY

using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    public static partial class Core
    {
        partial struct Native
        {
            [DllImport(Constants.UnityPlugin)]
            internal static extern void SetPluginPath(string path);

            [DllImport(Constants.UnityPlugin)]
            internal static extern void Print(string toPrint);
        }

        /// <summary>
        /// Initializes libvlc for libvlcsharp in a Unity context. By using runtime checks, we can have the same libvlcsharp
        /// netstandard dll running on all Unity platforms, which in turn simplifies usage from the Unity Editor when debugging
        /// actively on multiple platforms.
        /// </summary>
        /// <param name="libvlcDirectoryPath">Path to load libvlc from</param>
        /// <exception cref="VLCException"></exception>
        internal static void InitializeUnity(string? libvlcDirectoryPath = null)
        {
            // only VLC for Unity on Windows and MacOS currently requires pre-initialization logic
            if(!PlatformHelper.IsWindows && !PlatformHelper.IsMac)
              return;

            if(string.IsNullOrEmpty(libvlcDirectoryPath))
            {
                throw new VLCException("Please provide UnityEngine.Application.dataPath to Core.Initialize for proper initialization.");
            }

            Native.SetPluginPath(libvlcDirectoryPath!);

            libvlcDirectoryPath = $"{libvlcDirectoryPath}\\Plugins";

            InitializeDesktop(libvlcDirectoryPath);
        }
    }
}
#endif
