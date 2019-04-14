using System;
using System.Runtime.InteropServices;
using LibVLCSharp.Forms.Platforms.WPF;
using LibVLCSharp.Forms.Shared;
using Xamarin.Forms;

[assembly: Dependency(typeof(PowerManager))]
namespace LibVLCSharp.Forms.Platforms.WPF
{
    /// <summary>
    /// Power manager.
    /// </summary>
    public class PowerManager : IPowerManager
    {
        [DllImport("kernel32.dll")]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [Flags]
        private enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        private bool _keepScreenOn;
        /// <summary>
        /// Gets or sets a value indicating whether the screen should be kept on.
        /// </summary>
        public bool KeepScreenOn
        {
            get => _keepScreenOn;
            set
            {
                if (value)
                {
                    SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
                }
                else
                {
                    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                }
                _keepScreenOn = value;
            }
        }
    }
}
