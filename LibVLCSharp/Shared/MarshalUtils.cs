using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVLCSharp.Shared
{
    public static class MarshalUtils
    {
        public static TU[] Retrieve<T, TU>(Func<IntPtr> getRef, Func<IntPtr, T> retrieve,
            Func<T, TU> create, Func<TU, TU> next, Action<IntPtr> releaseRef)
        {
            var nativeRef = getRef();
            if (nativeRef == IntPtr.Zero) return Array.Empty<TU>();

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
        /// 
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
    }
}