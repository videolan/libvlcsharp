using LibVLCSharp.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace LibVLCSharp
{
    internal static class MediaDownloaderCallbacks
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr BufferCallback(IntPtr opaque, IntPtr task, IntPtr buffer, UIntPtr length, ulong position, ulong total);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void StateCallback(IntPtr opaque, IntPtr task, DownloadStatus status);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void SubitemsCallback(IntPtr opaque, IntPtr task, IntPtr subitems);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void SlavesCallback(IntPtr opaque, IntPtr task, IntPtr slaves, UIntPtr count);

        [StructLayout(LayoutKind.Sequential)]
        struct NativeCallbacks
        {
            public uint Version;
            public IntPtr OnBuffer;
            public IntPtr OnStateUpdate;
            public IntPtr OnSubitems;
            public IntPtr OnSlaves;
        }

        static readonly BufferCallback s_buffer = OnBuffer;
        static readonly StateCallback s_state = OnStateUpdate;
        static readonly SubitemsCallback s_subitems = OnSubitems;
        static readonly SlavesCallback s_slaves = OnSlaves;
        internal static readonly IntPtr Pointer = Build();

        static IntPtr Build()
        {
            var callbacks = new NativeCallbacks
            {
                Version = 0,
                OnBuffer = Marshal.GetFunctionPointerForDelegate(s_buffer),
                OnStateUpdate = Marshal.GetFunctionPointerForDelegate(s_state),
                OnSubitems = Marshal.GetFunctionPointerForDelegate(s_subitems),
                OnSlaves = Marshal.GetFunctionPointerForDelegate(s_slaves)
            };
            var pointer = Marshal.AllocHGlobal(MarshalUtils.SizeOf(callbacks));
            Marshal.StructureToPtr(callbacks, pointer, false);
            return pointer;
        }

        [MonoPInvokeCallback(typeof(BufferCallback))]
        static IntPtr OnBuffer(IntPtr opaque, IntPtr task, IntPtr buffer, UIntPtr length, ulong position, ulong total)
        {
            var state = MarshalUtils.GetInstance<DownloadRequest>(opaque);
            if (state == null) return new IntPtr(-1);
            try
            {
                if (state.Failure != null) return new IntPtr(-1);
                var count = checked((int)length.ToUInt64());
                var consumed = state.OnBuffer(buffer, count, position, total);
                if (consumed < -2 || consumed > count)
                    throw new ArgumentOutOfRangeException("returnValue", consumed,
                        $"The download buffer callback must return a byte count between 0 and {count}, " +
                        "-1 to fail the download, or -2 to cancel it");
                return new IntPtr(consumed);
            }
            catch (Exception exception)
            {
                state.Failure = exception;
                return new IntPtr(-1);
            }
        }

        [MonoPInvokeCallback(typeof(StateCallback))]
        static void OnStateUpdate(IntPtr opaque, IntPtr task, DownloadStatus status)
        {
            var state = MarshalUtils.GetInstance<DownloadRequest>(opaque);
            if (state == null) return;
            state.CurrentStatus = status;
            try { state.StateChanged?.Invoke(status); }
            catch (Exception exception) { Core.Log(exception.ToString()); }
            finally
            {
                if (state.IsTerminal && state.TryBeginCompletion())
                {
                    state.Pin.Free();
#if NET45
                    ThreadPool.QueueUserWorkItem(_ => state.Complete(status));
#else
                    state.Complete(status);
#endif
                }
            }
        }

        [MonoPInvokeCallback(typeof(SubitemsCallback))]
        static void OnSubitems(IntPtr opaque, IntPtr task, IntPtr subitems)
        {
            var state = MarshalUtils.GetInstance<DownloadRequest>(opaque);
            if (state == null) return;
            try
            {
                if (state.Subitems == null) return;
                using (var list = new MediaList(subitems))
                {
                    list.Retain();
                    state.Subitems(list);
                }
            }
            catch (Exception exception) { state.Failure = exception; }
        }

        [MonoPInvokeCallback(typeof(SlavesCallback))]
        static void OnSlaves(IntPtr opaque, IntPtr task, IntPtr slaves, UIntPtr count)
        {
            var state = MarshalUtils.GetInstance<DownloadRequest>(opaque);
            if (state == null) return;
            try
            {
                if (state.Slaves == null) return;
                var result = new MediaSlave[checked((int)count.ToUInt64())];
                for (var i = 0; i < result.Length; i++)
                    result[i] = MarshalUtils.PtrToStructure<MediaSlaveStructure>(Marshal.ReadIntPtr(slaves, checked(i * IntPtr.Size))).Build();
                state.Slaves(result);
            }
            catch (Exception exception) { state.Failure = exception; }
        }
    }
}
