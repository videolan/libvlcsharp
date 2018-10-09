using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// taken from https://github.com/nikki-nn4/NimDemo
namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Marshal unicode string param to utf-8 string,usage:[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))]
    /// </summary>
#if !NETSTANDARD1_1
    public class Utf8StringMarshaler : ICustomMarshaler
#else
    public class Utf8StringMarshaler
#endif
    {
        private static readonly Utf8StringMarshaler _instance = new Utf8StringMarshaler();

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
                return IntPtr.Zero;
            if (!(ManagedObj is string))
                throw new InvalidOperationException("Utf8StringMarshaler:ManagedObj must be string");

            byte[] utf8bytes = Encoding.UTF8.GetBytes(ManagedObj as string);
            IntPtr ptr = Marshal.AllocCoTaskMem(utf8bytes.Length + 1);
            Marshal.Copy(utf8bytes, 0, ptr, utf8bytes.Length);
            Marshal.WriteByte(ptr, utf8bytes.Length, 0);
            return ptr;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
                return null;
            List<byte> bytes = new List<byte>();
            for (int offset = 0; ; offset++)
            {
                byte b = Marshal.ReadByte(pNativeData, offset);
                if (b == 0)
                    break;
                else bytes.Add(b);
            }

            var str = Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
            return str;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeCoTaskMem(pNativeData);
            pNativeData = IntPtr.Zero;
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public static Utf8StringMarshaler GetInstance()
        {
            return _instance;
        }
    }
}
