using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Sample.Manual;

namespace libvlcsharp
{
    public class Instance
    {
        [StructLayout(LayoutKind.Explicit, Size = 0)]
        internal struct Internal
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_new")]
            internal static extern unsafe IntPtr LibVLCNew(int argc, sbyte** argv);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_release")]
            internal static extern void LibVLCRelease(IntPtr instance);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_add_intf")]
            internal static extern int LibVLCAddInterface(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string name);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_exit_handler")]
            internal static extern void LibVLCSetExitHandler(IntPtr instance, IntPtr cb, IntPtr opaque);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_user_agent")]
            internal static extern void LibVLCSetUserAgent(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string name, 
                [MarshalAs(UnmanagedType.LPStr)] string http);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_app_id")]
            internal static extern void LibVLCSetAppId(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string id, 
                [MarshalAs(UnmanagedType.LPStr)] string version, [MarshalAs(UnmanagedType.LPStr)] string icon);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_log_unset")]
            internal static extern void LibVLCLogUnset(IntPtr instance);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_log_set_file")]
            internal static extern void LibVLCLogSetFile(IntPtr instance, IntPtr stream);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_module_description_list_release")]
            internal static extern void LibVLCModuleDescriptionListRelease(IntPtr moduleDescriptionList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_filter_list_get")]
            internal static extern IntPtr LibVLCAudioFilterListGet(IntPtr instance);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_filter_list_get")]
            internal static extern IntPtr LibVLCVideoFilterListGet(IntPtr instance);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_list_get")]
            internal static extern IntPtr LibVLCAudioOutputListGet(IntPtr instance);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_list_release")]
            internal static extern void LibVLCAudioOutputListRelease(IntPtr list);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_list_get")]
            internal static extern IntPtr LibVLCAudioOutputDeviceListGet(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string aout);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_list_release")]
            internal static extern void LibVLCAudioOutputDeviceListRelease(IntPtr list);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_list_get")]
            internal static extern IntPtr LibVLCMediaDiscovererListGet(IntPtr instance, MediaDiscovererCategory category, ref IntPtr pppServices);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_list_release")]
            internal static extern void LibVLCMediaDiscovererListRelease(IntPtr ppServices, ulong count);
        }

        public IntPtr NativeReference { get; protected set; }

        internal static readonly System.Collections.Concurrent.ConcurrentDictionary<IntPtr, Instance> NativeToManagedMap 
            = new System.Collections.Concurrent.ConcurrentDictionary<IntPtr, Instance>();

        protected bool __ownsNativeInstance;

        /// <summary>
        /// <para>Create and initialize a libvlc instance.</para>
        /// <para>This functions accept a list of &quot;command line&quot; arguments similar to the</para>
        /// <para>main(). These arguments affect the LibVLC instance default configuration.</para>
        /// </summary>
        /// <param name="argc">the number of arguments (should be 0)</param>
        /// <param name="args">list of arguments (should be NULL)</param>
        /// <returns>the libvlc instance or NULL in case of error</returns>
        /// <remarks>
        /// <para>LibVLC may create threads. Therefore, any thread-unsafe process</para>
        /// <para>initialization must be performed before calling libvlc_new(). In particular</para>
        /// <para>and where applicable:</para>
        /// <para>- setlocale() and textdomain(),</para>
        /// <para>- setenv(), unsetenv() and putenv(),</para>
        /// <para>- with the X11 display system, XInitThreads()</para>
        /// <para>(see also libvlc_media_player_set_xwindow()) and</para>
        /// <para>- on Microsoft Windows, SetErrorMode().</para>
        /// <para>- sigprocmask() shall never be invoked; pthread_sigmask() can be used.</para>
        /// <para>On POSIX systems, the SIGCHLD signalmust notbe ignored, i.e. the</para>
        /// <para>signal handler must set to SIG_DFL or a function pointer, not SIG_IGN.</para>
        /// <para>Also while LibVLC is active, the wait() function shall not be called, and</para>
        /// <para>any call to waitpid() shall use a strictly positive value for the first</para>
        /// <para>parameter (i.e. the PID). Failure to follow those rules may lead to a</para>
        /// <para>deadlock or a busy loop.</para>
        /// <para>Also on POSIX systems, it is recommended that the SIGPIPE signal be blocked,</para>
        /// <para>even if it is not, in principles, necessary, e.g.:</para>
        /// <para>On Microsoft Windows Vista/2008, the process error mode</para>
        /// <para>SEM_FAILCRITICALERRORS flagmustbe set before using LibVLC.</para>
        /// <para>On later versions, that is optional and unnecessary.</para>
        /// <para>Also on Microsoft Windows (Vista and any later version), setting the default</para>
        /// <para>DLL directories to SYSTEM32 exclusively is strongly recommended for</para>
        /// <para>security reasons:</para>
        /// <para>Arguments are meant to be passed from the command line to LibVLC, just like</para>
        /// <para>VLC media player does. The list of valid arguments depends on the LibVLC</para>
        /// <para>version, the operating system and platform, and set of available LibVLC</para>
        /// <para>plugins. Invalid or unsupported arguments will cause the function to fail</para>
        /// <para>(i.e. return NULL). Also, some arguments may alter the behaviour or</para>
        /// <para>otherwise interfere with other LibVLC functions.</para>
        /// <para>There is absolutely no warranty or promise of forward, backward and</para>
        /// <para>cross-platform compatibility with regards to libvlc_new() arguments.</para>
        /// <para>We recommend that you do not use them, other than when debugging.</para>
        /// </remarks>
        public Instance(int argc, string[] args)
        {         
            unsafe
            {
                if (args == null || !args.Any())
                    NativeReference = Internal.LibVLCNew(argc, null);
                else
                {

                    fixed (byte* arg0 = Encoding.ASCII.GetBytes(args[0]),
                        arg1 = Encoding.ASCII.GetBytes(args[1]),
                        arg2 = Encoding.ASCII.GetBytes(args[2]))
                    {
                        sbyte*[] arr = { (sbyte*)arg0, (sbyte*)arg1, (sbyte*)arg2 };
                        fixed (sbyte** argv = arr)
                        {
                            NativeReference = Internal.LibVLCNew(argc, argv);
                        }
                    }
                }
            }

            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;
        }

        /// <para>Decrement the reference count of a libvlc instance, and destroy it</para>
        /// <para>if it reaches zero.</para>
        public void Dispose()
        {
            Dispose(disposing: true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (NativeReference == IntPtr.Zero)
                return;

            Internal.LibVLCRelease(NativeReference);
            
            NativeToManagedMap.TryRemove(NativeReference, out var dummy);
            NativeReference = IntPtr.Zero;
        }
        
        public static bool operator ==(Instance obj1, Instance obj2)
        {
            return obj1?.NativeReference == obj2?.NativeReference;
        }

        public static bool operator !=(Instance obj1, Instance obj2)
        {
            return obj1?.NativeReference != obj2?.NativeReference;
        }

        /**
         * Try to start a user interface for the libvlc instance.
         *
         * \param name  interface name, or empty string for default
        */
        public bool AddInterface(string name)
        {
            return Internal.LibVLCAddInterface(NativeReference, name ?? string.Empty) == 0;
        }
        
        /// <summary>
        /// <para>Registers a callback for the LibVLC exit event. This is mostly useful if</para>
        /// <para>the VLC playlist and/or at least one interface are started with</para>
        /// <para>libvlc_playlist_play() or libvlc_add_intf() respectively.</para>
        /// <para>Typically, this function will wake up your application main loop (from</para>
        /// <para>another thread).</para>
        /// </summary>
        /// <param name="cb">
        /// <para>callback to invoke when LibVLC wants to exit,</para>
        /// <para>or NULL to disable the exit handler (as by default)</para>
        /// </param>
        /// <param name="opaque">data pointer for the callback</param>
        /// <remarks>
        /// <para>This function should be called before the playlist or interface are</para>
        /// <para>started. Otherwise, there is a small race condition: the exit event could</para>
        /// <para>be raised before the handler is registered.</para>
        /// <para>This function and libvlc_wait() cannot be used at the same time.</para>
        /// </remarks>
        public void SetExitHandler(Delegates.Action_IntPtr cb, IntPtr opaque)
        {
            var cbFunctionPointer = cb == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(cb);
            Internal.LibVLCSetExitHandler(NativeReference, cbFunctionPointer, opaque);
        }
        
        /// <summary>
        /// <para>Sets the application name. LibVLC passes this as the user agent string</para>
        /// <para>when a protocol requires it.</para>
        /// </summary>
        /// <param name="name">human-readable application name, e.g. &quot;FooBar player 1.2.3&quot;</param>
        /// <param name="http">HTTP User Agent, e.g. &quot;FooBar/1.2.3 Python/2.6.0&quot;</param>
        /// <remarks>LibVLC 1.1.1 or later</remarks>
        public void SetUserAgent(string name, string http)
        {
            Internal.LibVLCSetUserAgent(NativeReference, name, http);
        }

        /// <summary>
        /// <para>Sets some meta-information about the application.</para>
        /// <para>See also libvlc_set_user_agent().</para>
        /// </summary>
        /// <param name="id">Java-style application identifier, e.g. &quot;com.acme.foobar&quot;</param>
        /// <param name="version">application version numbers, e.g. &quot;1.2.3&quot;</param>
        /// <param name="icon">application icon name, e.g. &quot;foobar&quot;</param>
        /// <remarks>LibVLC 2.1.0 or later.</remarks>
        public void SetAppId(string id, string version, string icon)
        {
            Internal.LibVLCSetAppId(NativeReference, id, version, icon);
        }

        /// <summary>Unsets the logging callback.</summary>
        /// <remarks>
        /// <para>This function deregisters the logging callback for a LibVLC instance.</para>
        /// <para>This is rarely needed as the callback is implicitly unset when the instance</para>
        /// <para>is destroyed.</para>
        /// <para>This function will wait for any pending callbacks invocation to</para>
        /// <para>complete (causing a deadlock if called from within the callback).</para>
        /// <para>LibVLC 2.1.0 or later</para>
        /// </remarks>
        public void UnsetLog()
        {
            Internal.LibVLCLogUnset(NativeReference);
        }

        public void SetLog()
        {
            
        }
        //TODO: void logSet(LogCb&& logCb)

        /// <summary>Sets up logging to a file.</summary>
        /// <param name="stream">
        /// <para>FILE pointer opened for writing</para>
        /// <para>(the FILE pointer must remain valid until libvlc_log_unset())</para>
        /// </param>
        /// <remarks>LibVLC 2.1.0 or later</remarks>
        public void SetLogFile(IntPtr stream)
        {
            Internal.LibVLCLogSetFile(NativeReference, stream);
        }

        /// <summary>Returns a list of audio filters that are available.</summary>
        /// <returns>
        /// <para>a list of module descriptions. It should be freed with libvlc_module_description_list_release().</para>
        /// <para>In case of an error, NULL is returned.</para>
        /// </returns>
        /// <remarks>
        /// <para>libvlc_module_description_t</para>
        /// <para>libvlc_module_description_list_release</para>
        /// </remarks>
        public IEnumerable<ModuleDescription> AudioFilters
        {
            get
            {
                return Retrieve(() => Internal.LibVLCAudioFilterListGet(NativeReference),
                    Marshal.PtrToStructure<ModuleDescription.Internal>,
                    intern => ModuleDescription.__CreateInstance(intern),
                    module => module.Next, Internal.LibVLCModuleDescriptionListRelease);
            }
        }

        private IEnumerable<TU> Retrieve<T, TU>(Func<IntPtr> getRef, Func<IntPtr, T> retrieve, 
            Func<T, TU> create, Func<TU, TU> next, Action<IntPtr> releaseRef)
        {
            var nativeRef = getRef();
            if (nativeRef == IntPtr.Zero) return Enumerable.Empty<TU>();

            var structure = retrieve(nativeRef);

            var obj = create(structure);

            var resultList = new List<TU>();
            while (obj != null)
            {
                resultList.Add(obj);
                obj = next(obj);
            }
            releaseRef(nativeRef);
            return resultList;
        }

        /// <summary>Returns a list of video filters that are available.</summary>
        /// <returns>
        /// <para>a list of module descriptions. It should be freed with libvlc_module_description_list_release().</para>
        /// <para>In case of an error, NULL is returned.</para>
        /// </returns>
        /// <remarks>
        /// <para>libvlc_module_description_t</para>
        /// <para>libvlc_module_description_list_release</para>
        /// </remarks>
        public IEnumerable<ModuleDescription> VideoFilters
        {
            get
            {
                return Retrieve(() => Internal.LibVLCVideoFilterListGet(NativeReference),
                    Marshal.PtrToStructure<ModuleDescription.Internal>,
                    intern => ModuleDescription.__CreateInstance(intern),
                    module => module.Next, Internal.LibVLCModuleDescriptionListRelease);
            }
        }

        /// <summary>Gets the list of available audio output modules.</summary>
        /// <returns>list of available audio outputs. It must be freed with</returns>
        /// <remarks>
        /// <para>libvlc_audio_output_list_release</para>
        /// <para>libvlc_audio_output_t .</para>
        /// <para>In case of error, NULL is returned.</para>
        /// </remarks>
        public IEnumerable<AudioOutputDescription> AudioOutputs
        {
            get
            {
                return Retrieve(() => Internal.LibVLCAudioOutputListGet(NativeReference),
                    Marshal.PtrToStructure<AudioOutputDescription.Internal>,
                    intern => AudioOutputDescription.__CreateInstance(intern),
                    module => module.Next, Internal.LibVLCAudioOutputListRelease);
            }
        }
        
        /// <summary>Gets a list of audio output devices for a given audio output module,</summary>
        /// <param name="audioOutputName">
        /// <para>audio output name</para>
        /// <para>(as returned by libvlc_audio_output_list_get())</para>
        /// </param>
        /// <returns>
        /// <para>A NULL-terminated linked list of potential audio output devices.</para>
        /// <para>It must be freed with libvlc_audio_output_device_list_release()</para>
        /// </returns>
        /// <remarks>
        /// <para>libvlc_audio_output_device_set().</para>
        /// <para>Not all audio outputs support this. In particular, an empty (NULL)</para>
        /// <para>list of devices doesnotimply that the specified audio output does</para>
        /// <para>not work.</para>
        /// <para>The list might not be exhaustive.</para>
        /// <para>Some audio output devices in the list might not actually work in</para>
        /// <para>some circumstances. By default, it is recommended to not specify any</para>
        /// <para>explicit audio device.</para>
        /// <para>LibVLC 2.1.0 or later.</para>
        /// </remarks>
        public IEnumerable<AudioOutputDevice> AudioOutputDevices(string audioOutputName)
        {

            return Retrieve(() => Internal.LibVLCAudioOutputDeviceListGet(NativeReference, audioOutputName), 
                Marshal.PtrToStructure<AudioOutputDevice.Internal>, 
                s => AudioOutputDevice.__CreateInstance(s),
                device => device.Next, Internal.LibVLCAudioOutputDeviceListRelease);
        }

        //https://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__media__discoverer.html#ga9a4676e452daa2fd4b5eb371542afc29
        // code inspired from https://code.videolan.org/videolan/libvlcpp/blob/master/vlcpp/Instance.hpp
        // https://stackoverflow.com/questions/13457375/how-do-i-call-an-unmanaged-function-that-has-a-char-as-out-parameter-from-c
        // https://stackoverflow.com/questions/11131676/how-do-i-marshall-a-pointer-to-a-pointer-of-an-array-of-structures
        public IEnumerable<MediaDiscovererDescription> MediaDiscoverers(MediaDiscovererCategory category)
        {
            var arrayResultPtr = IntPtr.Zero;
            var count = Internal.LibVLCMediaDiscovererListGet(NativeReference, category, ref arrayResultPtr).ToInt32();
            //var count = Internal.LibVLCMediaDiscovererListGet(NativeReference, category, out var arrayResultPtr).ToInt32();

            if (count == 0) return Enumerable.Empty<MediaDiscovererDescription>();

            var size = Marshal.SizeOf(typeof(MediaDiscovererDescription.Internal));
            var mediaDiscovererDescription = new List<MediaDiscovererDescription>();
    
            for (var i = 0; i < count; i++)
            {
                var s = (MediaDiscovererDescription.Internal) Marshal.PtrToStructure(
                    new IntPtr(arrayResultPtr.ToInt32() + (size * i)), typeof(MediaDiscovererDescription.Internal));

                var mdd = MediaDiscovererDescription.__CreateInstance(s);
                Debug.WriteLine(mdd.Longname);
                Debug.WriteLine(mdd.Name);
                mediaDiscovererDescription.Add(mdd);
            }

            return mediaDiscovererDescription;
        }

        /// <summary>Get media discoverer services by category</summary>
        /// <param name="p_inst">libvlc instance</param>
        /// <param name="i_cat">category of services to fetch</param>
        /// <param name="ppp_services">
        /// <para>address to store an allocated array of media discoverer</para>
        /// <para>services (must be freed with libvlc_media_discoverer_list_release() by</para>
        /// <para>the caller) [OUT]</para>
        /// </param>
        /// <returns>the number of media discoverer services (0 on error)</returns>
        /// <remarks>LibVLC 3.0.0 and later.</remarks>
        //private ulong LibvlcMediaDiscovererListGet(global::libvlcsharp.Instance p_inst, global::libvlcsharp.MediaDiscovererCategory i_cat, global::libvlcsharp.MediaDiscovererDescription ppp_services)
        //{
        //    var __arg0 = ReferenceEquals(p_inst, null) ? global::System.IntPtr.Zero : p_inst.NativeReference;
        //    var __arg2 = ReferenceEquals(ppp_services, null) ? global::System.IntPtr.Zero : ppp_services.NativeReference;
        //    var __ret = __Internal.LibvlcMediaDiscovererListGet(__arg0, i_cat, __arg2);
        //    return __ret;
        //}

        /// <summary>Release an array of media discoverer services</summary>
        /// <param name="ppServices">array to release</param>
        /// <param name="count">number of elements in the array</param>
        /// <remarks>
        /// <para>LibVLC 3.0.0 and later.</para>
        /// <para>libvlc_media_discoverer_list_get()</para>
        /// </remarks>
        private void ReleaseMediaDiscovererList(MediaDiscovererDescription ppServices, ulong count)
        {
            Internal.LibVLCMediaDiscovererListRelease(ppServices.NativeReference, count);
        }

        public void SetDialogHandlers()
        {
        }

        public void SetExistHandler(Action cb)
        {
            //Internal.LibVLCSetExitHandler();
        }
    }
}