﻿using System.Collections.Generic;
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
        /// Enable/disable hardware decoding (crossplatform).
        /// </summary>
        public bool EnableHardwareDecoding
        {
            get => _enableHardwareDecoding;
            set
            {
                _enableHardwareDecoding = value;
                _options[nameof(EnableHardwareDecoding)] = HardwareDecodingOptionString(_enableHardwareDecoding);
            }
        }

        int _fileCaching;
        /// <summary>
        /// Caching value for local files, in milliseconds [0 .. 60000ms]
        /// </summary>
        public int FileCaching
        {
            get => _fileCaching;
            set
            {
                _fileCaching = value;
                _options[nameof(FileCaching)] = _fileCaching.ToString();
            }
        }

        int _networkCaching;
        /// <summary>
        /// Caching value for network resources, in milliseconds [0 .. 60000ms]
        /// </summary>
        public int NetworkCaching
        {
            get => _networkCaching;
            set
            {
                _networkCaching = value;
                _options[nameof(NetworkCaching)] = _networkCaching.ToString();
            }
        }

#if ANDROID
        const string ENABLE_HW_ANDROID = ":codec=mediacodec_ndk";
        const string DISABLE_HW_ANDROID = "";
#endif
        const string ENABLE_HW_APPLE = ":videotoolbox";
        const string ENABLE_HW_WINDOWS = ":avcodec-hw=d3d11va";

        const string DISABLE_HW_APPLE = ":no-videotoolbox";
        const string DISABLE_HW_WINDOWS = ":avcodec-hw=none";

        private string HardwareDecodingOptionString(bool enable)
        {
            if(enable)
            {
#if ANDROID
                return ENABLE_HW_ANDROID;
#elif APPLE
                return ENABLE_HW_APPLE;
#else
                if (PlatformHelper.IsWindows)
                    return ENABLE_HW_WINDOWS;
                if (PlatformHelper.IsMac)
                    return ENABLE_HW_APPLE;
                return string.Empty;
#endif
            }
            else
            {
#if ANDROID
                return DISABLE_HW_ANDROID;
#elif APPLE
                return DISABLE_HW_APPLE;
#else
                if (PlatformHelper.IsWindows)
                    return DISABLE_HW_WINDOWS;
                if (PlatformHelper.IsMac)
                    return DISABLE_HW_APPLE;
                return string.Empty;
#endif
            }

        }

        /// <summary>
        /// Builds the current MediaConfiguration for consumption by libvlc (or storage)
        /// </summary>
        /// <returns>Configured libvlc options as strings</returns>
        public string[] Build() => _options.Values.Where(option => !string.IsNullOrEmpty(option)).ToArray();
    }
}