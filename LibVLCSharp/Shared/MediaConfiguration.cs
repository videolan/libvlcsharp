using System.Collections.Generic;
using System.Linq;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Small configuration helper
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
        public int FileCaching { get; set; } = -1;
        public int NetworkCaching { get; set; } = -1;

#if ANDROID
        const string ENABLE_HW_ANDROID = ":codec=mediacodec_ndk";
#elif IOS || TVOS
        const string ENABLE_HW_IOS = "";
#endif
        const string ENABLE_HW_WINDOWS = ":avcodec-hw=d3d11va";
        const string ENABLE_HW_MAC = "";

        const string DISABLE_HW_WINDOWS = ":avcodec-hw=none";

        private string HardwareDecodingOptionString(bool enable)
        {
            if(enable)
            {
#if ANDROID
                return ENABLE_HW_ANDROID;
#elif IOS || TVOS
                
#endif
                if (PlatformHelper.IsWindows)
                    return ENABLE_HW_WINDOWS;
                if (PlatformHelper.IsMac)
                    return ENABLE_HW_MAC;
                return "";
            }
            else
            {
                if (PlatformHelper.IsWindows)
                    return DISABLE_HW_WINDOWS;
                return "";
            }

        }

        public string[] Build() => _options.Values.ToArray();
    }
}