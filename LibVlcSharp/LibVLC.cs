using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Cauldron.Interception;

namespace VideoLAN.LibVLC
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
    public class LibVLC : Attribute, IMethodInterceptor
    {
        struct Native
        {
            /// <summary>Retrieve libvlc version.</summary>
            /// <returns>a string containing the libvlc version</returns>
            /// <remarks>Example: &quot;1.1.0-git The Luggage&quot;</remarks>
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_version")]
            internal static extern IntPtr LibVLCVersion();
        }

        readonly Version _requiredVersion;
        Version _dllVersion;

        public LibVLC(int major, int minor = 0)
        {
            _requiredVersion = new Version(major, minor);
        }

        private Version DllVersion
        {
            get
            {
                if (_dllVersion != null) return _dllVersion;
                var version = Marshal.PtrToStringAnsi(Native.LibVLCVersion());
                if (string.IsNullOrEmpty(version))
                    throw new VLCException("Cannot retrieve native dll version");

                version = version.Split('-', ' ')[0];
                _dllVersion = new Version(version);
                return _dllVersion;
            }
        }

        private bool Check => DllVersion.Major >= _requiredVersion.Major && DllVersion.Minor >= _requiredVersion.Minor;

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            if (!Check)
                throw new VLCException($"This API requires version {_requiredVersion.Major}.{_requiredVersion.Minor} of libvlc. " +
                                       $"Currently used dll version is {_dllVersion.Major}.{_dllVersion.Minor}");
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }
}