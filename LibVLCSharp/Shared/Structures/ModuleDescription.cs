using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared.Structures
{
    // TODO: cleanup
    public unsafe partial class ModuleDescription : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Internal
        {
            //[FieldOffset(0)]
            //[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))]
            internal IntPtr psz_name;

            //[FieldOffset(8)]
            //[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))]
            internal IntPtr psz_shortname;

            //[FieldOffset(16)]
            //[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))]
            internal IntPtr psz_longname;

            //[FieldOffset(24)]
            //[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))]
            internal IntPtr psz_help;

            //[FieldOffset(32)]
            //[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))]
            internal IntPtr p_next;

            //[SuppressUnmanagedCodeSecurity]
            //[DllImport(Constants.LibraryName, CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
            //    EntryPoint="??0libvlc_module_description_t@@QEAA@AEBU0@@Z")]
            //internal static extern global::System.IntPtr cctor(global::System.IntPtr instance, global::System.IntPtr _0);


        }

        public IntPtr NativeReference { get; protected set; }

        protected int __PointerAdjustment;
        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, ModuleDescription> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, ModuleDescription>();
        protected void*[] __OriginalVTables;

        protected bool __ownsNativeInstance;

        internal static ModuleDescription __CreateInstance(global::System.IntPtr native, bool skipVTables = false)
        {
            return new ModuleDescription(native.ToPointer(), skipVTables);
        }

        internal static ModuleDescription __CreateInstance(ModuleDescription.Internal native, bool skipVTables = false)
        {
            return new ModuleDescription(native, skipVTables);
        }

        private static void* __CopyValue(ModuleDescription.Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(ModuleDescription.Internal));
            *(ModuleDescription.Internal*)ret = native;
            return ret.ToPointer();
        }

        private ModuleDescription(ModuleDescription.Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;
        }

        protected ModuleDescription(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            NativeReference = new global::System.IntPtr(native);
        }

        public ModuleDescription()
        {
            NativeReference = Marshal.AllocHGlobal(sizeof(ModuleDescription.Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;
        }

        public ModuleDescription(ModuleDescription _0)
        {
            NativeReference = Marshal.AllocHGlobal(sizeof(ModuleDescription.Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;
            *((ModuleDescription.Internal*)NativeReference) = *((ModuleDescription.Internal*)_0.NativeReference);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (NativeReference == IntPtr.Zero)
                return;


            NativeToManagedMap.TryRemove(NativeReference, out var dummy);


            NativeReference = IntPtr.Zero;
        }

        public string Name => Marshal.PtrToStringAnsi(((Internal*)NativeReference)->psz_name);

        public string Shortname => Marshal.PtrToStringAnsi(((Internal*)NativeReference)->psz_shortname);

        public string Longname => Marshal.PtrToStringAnsi(((Internal*)NativeReference)->psz_longname);

        public string Help => Marshal.PtrToStringAnsi(((Internal*)NativeReference)->psz_help);

        public ModuleDescription Next
        {
            get
            {
                ModuleDescription __result0;
                if (((ModuleDescription.Internal*)NativeReference)->p_next == IntPtr.Zero) __result0 = null;
                else if (NativeToManagedMap.ContainsKey(((Internal*)NativeReference)->p_next))
                    __result0 = NativeToManagedMap[((Internal*)NativeReference)->p_next];
                else __result0 = __CreateInstance(((Internal*)NativeReference)->p_next);
                return __result0;
            }

            set
            {
                ((ModuleDescription.Internal*)NativeReference)->p_next = ReferenceEquals(value, null) ? global::System.IntPtr.Zero : value.NativeReference;
            }
        }
    }

}
