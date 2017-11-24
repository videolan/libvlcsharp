using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace VideoLAN.LibVLC.Manual
{
    public class Instance : Internal
    {
        protected bool Equals(Instance other)
        {
            return NativeReference.Equals(other.NativeReference);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Instance) obj);
        }
        LogCallback _logCallback;
        readonly object _logLock = new object();
        /// <summary>
        /// The real log event handlers.
        /// </summary>
        EventHandler<LogEventArgs> _log;

        /// <summary>
        /// A boolean to make sure that we are calling SetLog only once
        /// </summary>
        bool _logAttached = false;

        IntPtr _logFileHandle;

        public override int GetHashCode()
        {
            return NativeReference.GetHashCode();
        }

        [StructLayout(LayoutKind.Explicit, Size = 0)]
        internal struct Native
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
                CharSet = CharSet.Ansi, EntryPoint = "libvlc_log_get_context")]
            internal static extern void LibVLCLogGetContext(IntPtr ctx, out IntPtr module, out IntPtr file, out UIntPtr line);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_log_set")]
            internal static extern void LibVLCLogSet(IntPtr instance, LogCallback cb, IntPtr data);

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
            internal static extern ulong LibVLCMediaDiscovererListGet(IntPtr instance, MediaDiscovererCategory category, ref IntPtr pppServices);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_list_release")]
            internal static extern void LibVLCMediaDiscovererListRelease(IntPtr ppServices, ulong count);

            /// <summary>Retrieve libvlc version.</summary>
            /// <returns>a string containing the libvlc version</returns>
            /// <remarks>Example: &quot;1.1.0-git The Luggage&quot;</remarks>
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_version")]
            internal static extern IntPtr LibVLCVersion();
            
            #region Windows
            
            /// <summary>
            /// Compute the size required by vsprintf to print the parameters.
            /// </summary>
            /// <param name="format"></param>
            /// <param name="ptr"></param>
            /// <returns></returns>
            [DllImport(Windows, CallingConvention = CallingConvention.Cdecl)]
            public static extern int _vscprintf(string format,IntPtr ptr);

            /// <summary>
            /// Format a string using printf style markers
            /// </summary>
            /// <remarks>
            /// See https://stackoverflow.com/a/37629480/2663813
            /// </remarks>
            /// <param name="buffer">The output buffer (should be large enough, use _vscprintf)</param>
            /// <param name="format">The message format</param>
            /// <param name="args">The variable arguments list pointer. We do not know what it is, but the pointer must be given as-is from C back to sprintf.</param>
            /// <returns>A negative value on failure, the number of characters written otherwise.</returns>
            [DllImport(Windows, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern int vsprintf(
                IntPtr buffer,
                string format,
                IntPtr args);

            [DllImport(Windows, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int _wfopen_s(out IntPtr pFile, string filename, string mode = Write);

            [DllImport(Windows, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fclose", SetLastError = true)]
            public static extern int fcloseWindows(IntPtr stream);

            #endregion

            #region Linux

            [DllImport(Linux, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fopen", CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern IntPtr fopenLinux(string filename, string mode = Write);

            [DllImport(Linux, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fclose", CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern int fcloseLinux(IntPtr file);

            #endregion

            #region Mac
            
            [DllImport(Mac, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fopen", SetLastError = true)] 
            public static extern IntPtr fopenMac(string path, string mode = Write);

            [DllImport(Mac, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fclose", SetLastError = true)]
            public static extern int fcloseMac(IntPtr file);

            #endregion

            const string Windows = "msvcrt";
            const string Linux = "libc";
            const string Mac = "libSystem";
            const string Write = "w";
        }
    
        public Version VLCVersion { get; private set; }

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
        public Instance(int argc = 0, string[] args = null)
            : base(() =>
            {
                unsafe
                {
                    if (args == null || !args.Any())
                        return Native.LibVLCNew(argc, null);
                    fixed (byte* arg0 = Encoding.ASCII.GetBytes(args[0]),
                        arg1 = Encoding.ASCII.GetBytes(args[1]),
                        arg2 = Encoding.ASCII.GetBytes(args[2]))
                    {
                        sbyte*[] arr = { (sbyte*)arg0, (sbyte*)arg1, (sbyte*)arg2 };
                        fixed (sbyte** argv = arr)
                        {
                            return Native.LibVLCNew(argc, argv);
                        }
                    }
                }
            }, Native.LibVLCRelease)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;

            var version = Marshal.PtrToStringAnsi(Native.LibVLCVersion());
            if (string.IsNullOrEmpty(version)) return;

            version = version.Split('-', ' ')[0];
            VLCVersion = new Version(version);
        }

        public override void Dispose()
        {
            UnsetLog();
            base.Dispose();
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
            return Native.LibVLCAddInterface(NativeReference, name ?? string.Empty) == 0;
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
        public void SetExitHandler(ExitCallback cb, IntPtr opaque)
        {
            var cbFunctionPointer = cb == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(cb);
            Native.LibVLCSetExitHandler(NativeReference, cbFunctionPointer, opaque);
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
            Native.LibVLCSetUserAgent(NativeReference, name, http);
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
            Native.LibVLCSetAppId(NativeReference, id, version, icon);
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
            Native.LibVLCLogUnset(NativeReference);
            if(!CloseLogFile())
                throw new VLCException("Could not close log file");
        }

        /// <summary>
        /// Native close log file handle
        /// </summary>
        /// <returns>true if no file to close or closed successful, false otherwise</returns>
        bool CloseLogFile()
        {
            if (_logFileHandle == IntPtr.Zero) return true;

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Native.fcloseWindows(_logFileHandle) == 0;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return Native.fcloseLinux(_logFileHandle) == 0;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return Native.fcloseMac(_logFileHandle) == 0;

            return false;
        }

        public void SetLog(LogCallback cb)
        {
            if (cb == null) throw new ArgumentException(nameof(cb));

            _logCallback = cb;

            Native.LibVLCLogSet(NativeReference, cb, IntPtr.Zero);
        }

        /// <summary>
        /// The event that is triggered when a log is emitted from libVLC.
        /// Listening to this event will discard the default logger in libvlc.
        /// </summary>
        public event EventHandler<LogEventArgs> Log
        {
            add
            {
                lock (_logLock)
                {
                    _log += value;
                    if (!_logAttached)
                    {
                        SetLog(OnLogInternal);
                        _logAttached = true;
                    }
                }
            }

            remove
            {
                lock (_logLock)
                {
                    _log -= value;
                }
            }
        }

        /// <summary>Sets up logging to a file.
        /// Watch out: Overwrite contents if file exists!
        /// </summary>
        /// <para>FILE pointer opened for writing</para>
        /// <para>(the FILE pointer must remain valid until libvlc_log_unset())</para>
        /// <param name="filename">open/create file with Write access. If existing, resets content.</param>
        /// <remarks>LibVLC 2.1.0 or later</remarks>
        public void SetLogFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) throw new NullReferenceException(nameof(filename));

            _logFileHandle = NativeFilePtr(filename);

            Native.LibVLCLogSetFile(NativeReference, _logFileHandle);
        }

        IntPtr NativeFilePtr(string filename)
        {
            var filePtr = IntPtr.Zero;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Native._wfopen_s(out filePtr, filename);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                filePtr = Native.fopenLinux(filename);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                filePtr = Native.fopenMac(filename);
            }

            if (filePtr == IntPtr.Zero)
                throw new VLCException("Could not get FILE * for log_set_file");

            return filePtr;
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
        public ModuleDescription[] AudioFilters
        {
            get
            {
                return Retrieve(() => Native.LibVLCAudioFilterListGet(NativeReference),
                    Marshal.PtrToStructure<ModuleDescription.Internal>,
                    intern => ModuleDescription.__CreateInstance(intern),
                    module => module.Next, Native.LibVLCModuleDescriptionListRelease);
            }
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
        public ModuleDescription[] VideoFilters
        {
            get
            {
                return Retrieve(() => Native.LibVLCVideoFilterListGet(NativeReference),
                    Marshal.PtrToStructure<ModuleDescription.Internal>,
                    intern => ModuleDescription.__CreateInstance(intern),
                    module => module.Next, Native.LibVLCModuleDescriptionListRelease);
            }
        }

        /// <summary>Gets the list of available audio output modules.</summary>
        /// <returns>list of available audio outputs. It must be freed with</returns>
        /// <remarks>
        /// <para>libvlc_audio_output_list_release</para>
        /// <para>libvlc_audio_output_t .</para>
        /// <para>In case of error, NULL is returned.</para>
        /// </remarks>
        public AudioOutputDescription[] AudioOutputs
        {
            get
            {
                return Retrieve(() => Native.LibVLCAudioOutputListGet(NativeReference),
                    Marshal.PtrToStructure<AudioOutputDescription.Internal>,
                    intern => AudioOutputDescription.__CreateInstance(intern),
                    module => module.Next, Native.LibVLCAudioOutputListRelease);
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
        public AudioOutputDevice[] AudioOutputDevices(string audioOutputName)
        {

            return Retrieve(() => Native.LibVLCAudioOutputDeviceListGet(NativeReference, audioOutputName), 
                Marshal.PtrToStructure<AudioOutputDevice.Internal>, 
                s => AudioOutputDevice.__CreateInstance(s),
                device => device.Next, Native.LibVLCAudioOutputDeviceListRelease);
        }

        /// <summary>Get media discoverer services by category</summary>
        /// <param name="category">category of services to fetch</param>
        /// <returns>the number of media discoverer services (0 on error)</returns>
        /// <remarks>LibVLC 3.0.0 and later.</remarks>
        public MediaDiscovererDescription[] MediaDiscoverers(MediaDiscovererCategory category)
        {
            var arrayResultPtr = IntPtr.Zero;
            var count = Native.LibVLCMediaDiscovererListGet(NativeReference, category, ref arrayResultPtr);

            if (count == 0) return Array.Empty<MediaDiscovererDescription>();
            
            var mediaDiscovererDescription = new MediaDiscovererDescription[(int)count];
    
            for (var i = 0; i < (int)count; i++)
            {
                var ptr = Marshal.ReadIntPtr(arrayResultPtr, i * IntPtr.Size);
                var managedStruct = (MediaDiscovererDescription.__Internal) Marshal.PtrToStructure(ptr, typeof(MediaDiscovererDescription.__Internal));
                var mdd = MediaDiscovererDescription.__CreateInstance(managedStruct);
                mediaDiscovererDescription[i] = mdd;
            }

            Native.LibVLCMediaDiscovererListRelease(arrayResultPtr, count);

            return mediaDiscovererDescription;
        }
        
        public void SetDialogHandlers()
        {
        }

        TU[] Retrieve<T, TU>(Func<IntPtr> getRef, Func<IntPtr, T> retrieve,
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
        /// Code taken from Vlc.DotNet
        /// </summary>
        /// <param name="data"></param>
        /// <param name="level"></param>
        /// <param name="ctx"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void OnLogInternal(IntPtr data, LogLevel level, IntPtr ctx, string format, IntPtr args)
        {
            if (_log == null) return;

            // Original source for va_list handling: https://stackoverflow.com/a/37629480/2663813
            var byteLength = Native._vscprintf(format, args) + 1;
            var utf8Buffer = Marshal.AllocHGlobal(byteLength);

            string formattedDecodedMessage;
            try
            {
                Native.vsprintf(utf8Buffer, format, args);

                formattedDecodedMessage = (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(utf8Buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(utf8Buffer);
            }

            GetLogContext(ctx, out var module, out var file, out var line);

            // Do the notification on another thread, so that VLC is not interrupted by the logging
            Task.Run(() => _log?.Invoke(NativeReference, new LogEventArgs(level, formattedDecodedMessage, module, file, line)));
        }

        /// <summary>
        /// Gets log message debug infos.
        ///
        /// This function retrieves self-debug information about a log message:
        /// - the name of the VLC module emitting the message,
        /// - the name of the source code module (i.e.file) and
        /// - the line number within the source code module.
        ///
        /// The returned module name and file name will be NULL if unknown.
        /// The returned line number will similarly be zero if unknown.
        /// </summary>
        /// <param name="logContext">The log message context (as passed to the <see cref="LogCallback"/>)</param>
        /// <param name="module">The module name storage.</param>
        /// <param name="file">The source code file name storage.</param>
        /// <param name="line">The source code file line number storage.</param>
        void GetLogContext(IntPtr logContext, out string module, out string file, out uint? line)
        {
            Native.LibVLCLogGetContext(logContext, out var modulePtr, out var filePtr, out var linePtr);

            line = linePtr == UIntPtr.Zero ? null : (uint?)linePtr.ToUInt32();
            module = Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(modulePtr) as string;
            file = Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(filePtr) as string;
        }
    }

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ExitCallback();
    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogCallback(IntPtr data, LogLevel logLevel, IntPtr logContext, [MarshalAs(UnmanagedType.LPStr)] string format, IntPtr args);

    public sealed class LogEventArgs : EventArgs
    {
        public LogEventArgs(LogLevel level, string message, string module, string sourceFile, uint? sourceLine)
        {
            Level = level;
            Message = message;
            Module = module;
            SourceFile = sourceFile;
            SourceLine = sourceLine;
        }

        /// <summary>
        /// The severity of the log message.
        /// By default, you will only get error messages, but you can get all messages by specifying "-vv" in the options.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// The log message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The name of the module that emitted the message
        /// </summary>
        public string Module { get; }

        /// <summary>
        /// The source file that emitted the message.
        /// This may be <see langword="null"/> if that info is not available, i.e. always if you are using a release version of VLC.
        /// </summary>
        public string SourceFile { get; }

        /// <summary>
        /// The line in the <see cref="SourceFile"/> at which the message was emitted.
        /// This may be <see langword="null"/> if that info is not available, i.e. always if you are using a release version of VLC.
        /// </summary>
        public uint? SourceLine { get; }
    }
}