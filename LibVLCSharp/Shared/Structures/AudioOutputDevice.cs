using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared.Structures
{
    /// <summary>Description for audio output device.</summary>
    public unsafe class AudioOutputDevice : IDisposable
    {
        [StructLayout(LayoutKind.Explicit, Size = 24)]
        public partial struct Internal
        {
            [FieldOffset(0)]
            internal IntPtr next;

            [FieldOffset(8)]
            internal IntPtr device;

            [FieldOffset(16)]
            internal IntPtr description;
        }

        public IntPtr NativeReference { get; protected set; }

        protected int __PointerAdjustment;
        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, AudioOutputDevice> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, AudioOutputDevice>();
        protected void*[] __OriginalVTables;

        protected bool __ownsNativeInstance;

        internal static AudioOutputDevice __CreateInstance(global::System.IntPtr native, bool skipVTables = false)
        {
            return new AudioOutputDevice(native.ToPointer(), skipVTables);
        }

        internal static AudioOutputDevice __CreateInstance(AudioOutputDevice.Internal native, bool skipVTables = false)
        {
            return new AudioOutputDevice(native, skipVTables);
        }

        private static void* __CopyValue(AudioOutputDevice.Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(AudioOutputDevice.Internal));
            *(AudioOutputDevice.Internal*)ret = native;
            return ret.ToPointer();
        }

        private AudioOutputDevice(AudioOutputDevice.Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;
        }

        protected AudioOutputDevice(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            NativeReference = new global::System.IntPtr(native);
        }

        public AudioOutputDevice()
        {
            NativeReference = Marshal.AllocHGlobal(sizeof(AudioOutputDevice.Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;
        }

        public AudioOutputDevice(AudioOutputDevice _0)
        {
            NativeReference = Marshal.AllocHGlobal(sizeof(AudioOutputDevice.Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;
            *((AudioOutputDevice.Internal*)NativeReference) = *((AudioOutputDevice.Internal*)_0.NativeReference);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (NativeReference == IntPtr.Zero)
                return;
            AudioOutputDevice __dummy;
            NativeToManagedMap.TryRemove(NativeReference, out __dummy);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(NativeReference);
            NativeReference = IntPtr.Zero;
        }

        public AudioOutputDevice Next
        {
            get
            {
                AudioOutputDevice __result0;
                if (((AudioOutputDevice.Internal*)NativeReference)->next == IntPtr.Zero) __result0 = null;
                else if (AudioOutputDevice.NativeToManagedMap.ContainsKey(((AudioOutputDevice.Internal*)NativeReference)->next))
                    __result0 = (AudioOutputDevice)AudioOutputDevice.NativeToManagedMap[((AudioOutputDevice.Internal*)NativeReference)->next];
                else __result0 = AudioOutputDevice.__CreateInstance(((AudioOutputDevice.Internal*)NativeReference)->next);
                return __result0;
            }

            set
            {
                ((AudioOutputDevice.Internal*)NativeReference)->next = ReferenceEquals(value, null) ? global::System.IntPtr.Zero : value.NativeReference;
            }
        }

        public string Device => (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(((Internal*)NativeReference)->device);

        public string Description => (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(((Internal*)NativeReference)->description);
    }
}
