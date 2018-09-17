using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

//using Cauldron.Interception;

namespace LibVLCSharp.Shared
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property, 
        AllowMultiple = true, 
        Inherited = false)]
    public class ApiVersion : Attribute//, IMethodInterceptor, IPropertyGetterInterceptor
    {
        struct Native
        {
            /// <summary>Retrieve libvlc version.</summary>
            /// <returns>a string containing the libvlc version</returns>
            /// <remarks>Example: &quot;1.1.0-git The Luggage&quot;</remarks>
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_version")]
            internal static extern IntPtr LibVLCVersion();
        }

        readonly Version _requiredVersion;
        Version _dllVersion;
        readonly bool _minimum;
        readonly bool _strict;

        public ApiVersion(int major, int minor = 0, bool min = true, bool strict = false)
        {
            _requiredVersion = new Version(major, minor);
            _minimum = min;
            _strict = strict;
        }

        Version DllVersion
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

        bool Check
        {
            get
            {
                if(_minimum)
                {
                    if(_strict)
                        return DllVersion.CompareTo(_requiredVersion) > 0;
                    return DllVersion.CompareTo(_requiredVersion) >= 0;
                }
                if (_strict)
                    return DllVersion.CompareTo(_requiredVersion) < 0;
                return DllVersion.CompareTo(_requiredVersion) <= 0;
            }
        }


        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            PerformCheck();
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        //public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        //{
        //    PerformCheck();
        //}

        void PerformCheck()
        {
            if (!Check)
                throw new VLCException("This API requires " + (_minimum ? "minimum" : "maximum") 
                    + $" version {_requiredVersion.Major}.{_requiredVersion.Minor} of libvlc. " 
                    + $"Currently used dll version is {_dllVersion.Major}.{_dllVersion.Minor}");
        }
    }
}