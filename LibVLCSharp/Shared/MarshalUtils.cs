using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVLCSharp.Shared
{
    public static class MarshalUtils
    {
        internal struct Native
        { 
            #region Windows

            [DllImport(Constants.Msvcrt, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int _wfopen_s(out IntPtr pFile, string filename, string mode = Write);

            [DllImport(Constants.Msvcrt, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fclose", SetLastError = true)]
            public static extern int fcloseWindows(IntPtr stream);

            #endregion

            #region Linux

            [DllImport(Constants.Libc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fopen", CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern IntPtr fopenLinux(string filename, string mode = Write);

            [DllImport(Constants.Libc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fclose", CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern int fcloseLinux(IntPtr file);

            #endregion

            #region Mac

            [DllImport(Constants.libSystem, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fopen", SetLastError = true)]
            public static extern IntPtr fopenMac(string path, string mode = Write);

            [DllImport(Constants.libSystem, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fclose", SetLastError = true)]
            public static extern int fcloseMac(IntPtr file);

            #endregion

            
            const string Write = "w";
        }

        public static TU[] Retrieve<T, TU>(Func<IntPtr> getRef, Func<IntPtr, T> retrieve,
            Func<T, TU> create, Func<TU, TU> next, Action<IntPtr> releaseRef)
        {
            var nativeRef = getRef();
            if (nativeRef == IntPtr.Zero)
            {
#if NETSTANDARD1_1 || NET40
                return new TU[0];
#else
                return Array.Empty<TU>();
#endif
            }
            var structure = retrieve(nativeRef);

            var obj = create(structure);

            var resultList = new List<TU>();
            while (obj != null)
            {
                resultList.Add(obj);
                obj = next(obj);
            }
            releaseRef(nativeRef);
            return resultList.ToArray();
        }

        /// <summary>
        /// Turns an array of UTF16 C# strings to an array of pointer to UTF8 strings
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Array of pointer you need to release when you're done with Marshal.FreeHGlobal</returns>
        public static IntPtr[] ToUtf8(string[] args)
        {
            var utf8Args = new IntPtr[args?.Length ?? 0];
            
            for (var i = 0; i < utf8Args.Length; i++)
            {
                var bytes = Encoding.UTF8.GetBytes(args[i]);
                var buffer = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, buffer, bytes.Length);
                Marshal.WriteByte(buffer, bytes.Length, 0);
                utf8Args[i] = buffer;
            }

            return utf8Args;
        }

        public static T PtrToStructure<T>(IntPtr ptr)
        {
#if NETSTANDARD1_1 || NET40
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
#else
            return Marshal.PtrToStructure<T>(ptr);
#endif
        }

        /// <summary>
        /// Crossplatform dlopen
        /// </summary>
        /// <returns>true if successful</returns>
        public static bool Open(string filename, out IntPtr fileHandle)
        {
            fileHandle = IntPtr.Zero;
#if NET40
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    fileHandle = Native.fopenMac(filename);
                    return fileHandle != IntPtr.Zero;
                case PlatformID.Unix:
                    fileHandle = Native.fopenLinux(filename);
                    return fileHandle != IntPtr.Zero;
                default:
                    return Native._wfopen_s(out fileHandle, filename) == 0;
            }
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if(Native._wfopen_s(out fileHandle, filename) != 0) return false;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fileHandle = Native.fopenLinux(filename);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                fileHandle = Native.fopenMac(filename);
            }
            return fileHandle != IntPtr.Zero;
#endif
        }

        /// <summary>
        /// Crossplatform fclose
        /// </summary>
        /// <param name="file handle"></param>
        /// <returns>true if successful</returns>
        public static bool Close(IntPtr fileHandle)
        {
#if NET40
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return Native.fcloseMac(fileHandle) == 0;
                case PlatformID.Unix:
                    return Native.fcloseLinux(fileHandle) == 0;
                default:
                    return Native.fcloseWindows(fileHandle) == 0;
            }
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Native.fcloseMac(fileHandle) == 0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Native.fcloseLinux(fileHandle) == 0;
            }
            else
            {
                return Native.fcloseWindows(fileHandle) == 0;
            }
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute(Type type)
        {
            Type = type;
        }
        public Type Type { get; private set; }
    }
}