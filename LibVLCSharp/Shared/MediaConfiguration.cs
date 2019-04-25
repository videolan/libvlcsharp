using System.Collections.Generic;
using System.Linq;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Configuration helper designed to be used for advanced libvlc configuration
    /// <para/> More info at https://wiki.videolan.org/VLC_command-line_help/
    /// </summary>
    public class MediaConfiguration
    {
        readonly Dictionary<string, string> _options = new Dictionary<string, string>
        {
            { nameof(EnableHardwareDecoding), string.Empty },
            { nameof(FileCaching), string.Empty },
            { nameof(NetworkCaching), string.Empty },
        };

        bool _enableHardwareDecoding;
        /// <summary>
        /// Enable/disable hardware decoding in a crossplatform way.
        /// </summary>
        public bool EnableHardwareDecoding
        {
            get => _enableHardwareDecoding;
            set
            {
                _enableHardwareDecoding = value;
                if (_enableHardwareDecoding)
                    _options[nameof(EnableHardwareDecoding)] = HardwareDecodingOptionString(_enableHardwareDecoding);
            }
        }

        /// <summary>
        /// Caching value for local files, in milliseconds [0 .. 60000ms]
        /// </summary>
        public int FileCaching { get; set; } = -1;

        /// <summary>
        /// Caching value for network resources, in milliseconds [0 .. 60000ms]
        /// </summary>
        public int NetworkCaching { get; set; } = -1;

#if ANDROID
        const string ENABLE_HW_ANDROID = ":codec=mediacodec_ndk";
        const string DISABLE_HW_ANDROID = "";
#elif IOS || TVOS
        const string ENABLE_HW_IOS = "codec";
        const string DISABLE_HW_IOS = "";
#endif
        const string ENABLE_HW_WINDOWS = ":avcodec-hw=d3d11va";
        const string ENABLE_HW_MAC = ":videotoolbox";

        const string DISABLE_HW_WINDOWS =":avcodec-hw=none";
        const string DISABLE_HW_MAC = ":no-videotoolbox";

        private string HardwareDecodingOptionString(bool enable)
        {
            if(enable)
            {
#if ANDROID
                return ENABLE_HW_ANDROID;
#elif IOS || TVOS
                return ENABLE_HW_IOS;
#endif
                if (PlatformHelper.IsWindows)
                    return ENABLE_HW_WINDOWS;
                if (PlatformHelper.IsMac)
                    return ENABLE_HW_MAC;
                return string.Empty;
            }
            else
            {
#if ANDROID
                return DISABLE_HW_ANDROID;
#elif IOS || TVOS
                return DISABLE_HW_IOS;
#endif
                if (PlatformHelper.IsWindows)
                    return DISABLE_HW_WINDOWS;
                if (PlatformHelper.IsMac)
                    return DISABLE_HW_MAC;
                return string.Empty;
            }

        }

        public string[] Build() => _options.Values.ToArray();
    }
}