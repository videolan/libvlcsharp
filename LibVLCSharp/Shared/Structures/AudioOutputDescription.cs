using System;
using System.Runtime.InteropServices;
using System.Security;

namespace LibVLCSharp.Shared.Structures
{
    /// <summary>
    /// <para>Description for audio output. It contains</para>
    /// <para>name, description and pointer to next record.</para>
    /// </summary>
    public unsafe partial class AudioOutputDescription : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public partial struct Internal
        {
            internal IntPtr psz_name;

            internal IntPtr psz_description;

            internal IntPtr p_next;


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "??0libvlc_audio_output_t@@QEAA@AEBU0@@Z")]
            internal static extern global::System.IntPtr cctor(global::System.IntPtr instance, global::System.IntPtr _0);



        }

        public global::System.IntPtr __Instance { get; protected set; }

        protected int __PointerAdjustment;
        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, AudioOutputDescription> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, AudioOutputDescription>();
        protected void*[] __OriginalVTables;

        protected bool __ownsNativeInstance;

        internal static AudioOutputDescription __CreateInstance(global::System.IntPtr native, bool skipVTables = false)
        {
            return new AudioOutputDescription(native.ToPointer(), skipVTables);
        }

        internal static AudioOutputDescription __CreateInstance(AudioOutputDescription.Internal native, bool skipVTables = false)
        {
            return new AudioOutputDescription(native, skipVTables);
        }

        private static void* __CopyValue(AudioOutputDescription.Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(AudioOutputDescription.Internal));
            *(AudioOutputDescription.Internal*)ret = native;
            return ret.ToPointer();
        }

        private AudioOutputDescription(AudioOutputDescription.Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected AudioOutputDescription(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new global::System.IntPtr(native);
        }

        public AudioOutputDescription()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(AudioOutputDescription.Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public AudioOutputDescription(AudioOutputDescription _0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(AudioOutputDescription.Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((AudioOutputDescription.Internal*)__Instance) = *((AudioOutputDescription.Internal*)_0.__Instance);
        }


        public void Dispose()
        {
            Dispose(disposing: true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (__Instance == IntPtr.Zero)
                return;

            AudioOutputDescription __dummy;
            NativeToManagedMap.TryRemove(__Instance, out __dummy);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        //public string Name => Marshal.PtrToStringAnsi(((Native *) NativeReference)->psz_name);
        public string Name => (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(((Internal*)__Instance)->psz_name);

        public string Description => (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(((Internal*)__Instance)->psz_description);

        public AudioOutputDescription Next
        {
            get
            {
                AudioOutputDescription __result0;
                if (((AudioOutputDescription.Internal*)__Instance)->p_next == IntPtr.Zero) __result0 = null;
                else if (AudioOutputDescription.NativeToManagedMap.ContainsKey(((AudioOutputDescription.Internal*)__Instance)->p_next))
                    __result0 = (AudioOutputDescription)AudioOutputDescription.NativeToManagedMap[((AudioOutputDescription.Internal*)__Instance)->p_next];
                else __result0 = AudioOutputDescription.__CreateInstance(((AudioOutputDescription.Internal*)__Instance)->p_next);
                return __result0;
            }

            set
            {
                ((AudioOutputDescription.Internal*)__Instance)->p_next = ReferenceEquals(value, null) ? global::System.IntPtr.Zero : value.__Instance;
            }
        }
    }
}
