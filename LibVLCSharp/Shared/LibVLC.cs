using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Shared.Structures;

namespace LibVLCSharp.Shared
{
    public class LibVLC : Internal
    {
        protected bool Equals(LibVLC other)
        {
            return NativeReference.Equals(other.NativeReference);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LibVLC) obj);
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
        bool _logAttached;

        IntPtr _logFileHandle;
        IntPtr _dialogCbsPtr;

        public override int GetHashCode()
        {
            return NativeReference.GetHashCode();
        }

        [StructLayout(LayoutKind.Explicit, Size = 0)]
        internal struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_new")]
            internal static extern IntPtr LibVLCNew(int argc, IntPtr[] argv);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_release")]
            internal static extern void LibVLCRelease(IntPtr libVLC);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_add_intf")]
            internal static extern int LibVLCAddInterface(IntPtr libVLC, [MarshalAs(UnmanagedType.LPStr)] string name);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_exit_handler")]
            internal static extern void LibVLCSetExitHandler(IntPtr libVLC, IntPtr cb, IntPtr opaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_user_agent")]
            internal static extern void LibVLCSetUserAgent(IntPtr libVLC, [MarshalAs(UnmanagedType.LPStr)] string name, 
                [MarshalAs(UnmanagedType.LPStr)] string http);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_app_id")]
            internal static extern void LibVLCSetAppId(IntPtr libVLC, [MarshalAs(UnmanagedType.LPStr)] string id, 
                [MarshalAs(UnmanagedType.LPStr)] string version, [MarshalAs(UnmanagedType.LPStr)] string icon);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_log_unset")]
            internal static extern void LibVLCLogUnset(IntPtr libVLC);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_log_set_file")]
            internal static extern void LibVLCLogSetFile(IntPtr libVLC, IntPtr stream);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                CharSet = CharSet.Ansi, EntryPoint = "libvlc_log_get_context")]
            internal static extern void LibVLCLogGetContext(IntPtr ctx, out IntPtr module, out IntPtr file, out UIntPtr line);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_log_set")]
            internal static extern void LibVLCLogSet(IntPtr libVLC, LogCallback cb, IntPtr data);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_module_description_list_release")]
            internal static extern void LibVLCModuleDescriptionListRelease(IntPtr moduleDescriptionList);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_filter_list_get")]
            internal static extern IntPtr LibVLCAudioFilterListGet(IntPtr libVLC);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_video_filter_list_get")]
            internal static extern IntPtr LibVLCVideoFilterListGet(IntPtr libVLC);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_list_get")]
            internal static extern IntPtr LibVLCAudioOutputListGet(IntPtr libVLC);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_list_release")]
            internal static extern void LibVLCAudioOutputListRelease(IntPtr list);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_list_get")]
            internal static extern IntPtr LibVLCAudioOutputDeviceListGet(IntPtr libVLC, [MarshalAs(UnmanagedType.LPStr)] string aout);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_list_release")]
            internal static extern void LibVLCAudioOutputDeviceListRelease(IntPtr list);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_list_get")]
            internal static extern ulong LibVLCMediaDiscovererListGet(IntPtr libVLC, MediaDiscoverer.Category category, ref IntPtr pppServices);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_list_release")]
            internal static extern void LibVLCMediaDiscovererListRelease(IntPtr ppServices, ulong count);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_dialog_set_callbacks")]
            internal static extern void LibVLCDialogSetCallbacks(IntPtr libVLC, IntPtr callbacks, IntPtr data);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_list_get")]
            internal static extern ulong LibVLCRendererDiscovererGetList(IntPtr libVLC, ref IntPtr discovererList);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_list_release")]
            internal static extern void LibVLCRendererDiscovererReleaseList(IntPtr discovererList, ulong count);

#if ANDROID
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_android_context")]
            internal static extern void LibVLCMediaPlayerSetAndroidContext(IntPtr mediaPlayer, IntPtr aWindow);
#endif

            /// <summary>
            /// Compute the size required by vsprintf to print the parameters.
            /// </summary>
            /// <param name="format"></param>
            /// <param name="ptr"></param>
            /// <returns></returns>
            [DllImport(Constants.Msvcrt, CallingConvention = CallingConvention.Cdecl)]
            public static extern int _vscprintf(string format, IntPtr ptr);

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
            [DllImport(Constants.Msvcrt, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern int vsprintf(
                IntPtr buffer,
                string format,
                IntPtr args);
        }
    
        internal static readonly System.Collections.Concurrent.ConcurrentDictionary<IntPtr, LibVLC> NativeToManagedMap 
            = new System.Collections.Concurrent.ConcurrentDictionary<IntPtr, LibVLC>();

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
        public LibVLC(string[] args = null)
            : base(() =>
            {
                var utf8Args = default(IntPtr[]);
                try
                {

                    utf8Args = MarshalUtils.ToUtf8(args);
                    return Native.LibVLCNew(utf8Args.Length, utf8Args);
                }
                finally
                {
                    foreach (var arg in utf8Args)
                    {
                        if (arg != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(arg);
                        }
                    }
                }
            }, Native.LibVLCRelease)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[NativeReference] = this;
        }

        public override void Dispose()
        {
            if(_logCallback != null)
                UnsetLog();
            UnsetDialogHandlers();
            base.Dispose();
        }

        public static bool operator ==(LibVLC obj1, LibVLC obj2)
        {
            return obj1?.NativeReference == obj2?.NativeReference;
        }

        public static bool operator !=(LibVLC obj1, LibVLC obj2)
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

        public void UnsetDialogHandlers()
        {
            if (_dialogCbsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_dialogCbsPtr);
                _dialogCbsPtr = IntPtr.Zero;
            }
            Native.LibVLCDialogSetCallbacks(NativeReference, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Native close log file handle
        /// </summary>
        /// <returns>true if no file to close or close operation successful, false otherwise</returns>
        bool CloseLogFile()
        {
            if (_logFileHandle == IntPtr.Zero) return true;

            return MarshalUtils.Close(_logFileHandle);
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
        /// Potentially throws a VLCException if FILE * cannot be obtained
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
            var result = MarshalUtils.Open(filename, out var filePtr);
            if (!result)
                throw new VLCException("Could not get FILE * for log_set_file");
            return IntPtr.Zero;
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
                return MarshalUtils.Retrieve(() => Native.LibVLCAudioFilterListGet(NativeReference),
                    MarshalUtils.PtrToStructure<ModuleDescription.Internal>,
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
                return MarshalUtils.Retrieve(() => Native.LibVLCVideoFilterListGet(NativeReference),
                    MarshalUtils.PtrToStructure<ModuleDescription.Internal>,
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
                return MarshalUtils.Retrieve(() => Native.LibVLCAudioOutputListGet(NativeReference),
                    MarshalUtils.PtrToStructure<AudioOutputDescription.Internal>,
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

            return MarshalUtils.Retrieve(() => Native.LibVLCAudioOutputDeviceListGet(NativeReference, audioOutputName), 
                MarshalUtils.PtrToStructure<AudioOutputDevice.Internal>, 
                s => AudioOutputDevice.__CreateInstance(s),
                device => device.Next, Native.LibVLCAudioOutputDeviceListRelease);
        }

        /// <summary>Get media discoverer services by category</summary>
        /// <param name="category">category of services to fetch</param>
        /// <returns>the number of media discoverer services (0 on error)</returns>
        /// <remarks>LibVLC 3.0.0 and later.</remarks>
        public MediaDiscoverer.Description[] MediaDiscoverers(MediaDiscoverer.Category category)
        {
            var arrayResultPtr = IntPtr.Zero;
            var count = Native.LibVLCMediaDiscovererListGet(NativeReference, category, ref arrayResultPtr);
#if NETSTANDARD1_1 || NET40
            if (count == 0) return new MediaDiscoverer.Description[0];
#else
            if (count == 0) return Array.Empty<MediaDiscoverer.Description>();
#endif
            var mediaDiscovererDescription = new MediaDiscoverer.Description[(int)count];

            for (var i = 0; i < (int)count; i++)
            {
                var ptr = Marshal.ReadIntPtr(arrayResultPtr, i * IntPtr.Size);
                var managedStruct = (MediaDiscoverer.Description)Marshal.PtrToStructure(ptr, typeof(MediaDiscoverer.Description));
                mediaDiscovererDescription[i] = managedStruct;
            }

            Native.LibVLCMediaDiscovererListRelease(arrayResultPtr, count);

            return mediaDiscovererDescription;
        }

        readonly Dictionary<IntPtr, CancellationTokenSource> _cts = new Dictionary<IntPtr, CancellationTokenSource>();

        public void SetDialogHandlers(DisplayError error, DisplayLogin login, DisplayQuestion question,
            DisplayProgress displayProgress, UpdateProgress updateProgress)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            if (login == null) throw new ArgumentNullException(nameof(login));
            if (question == null) throw new ArgumentNullException(nameof(question));
            if (displayProgress == null) throw new ArgumentNullException(nameof(displayProgress));
            if (updateProgress == null) throw new ArgumentNullException(nameof(updateProgress));

            var dialogCbs = new DialogCallbacks
            {
                DisplayError = (data, title, text) =>
                {
                    // no dialogId ?!
                    error(title, text);
                },
                DisplayLogin = (data, id, title, text, username, store) =>
                {
                    var cts = new CancellationTokenSource();
                    var dlg = new Dialog(new DialogId { NativeReference = id });
                    _cts.Add(id, cts);
                    login(dlg, title, text, username, store, cts.Token);
                },
                DisplayQuestion = (data, id, title, text, type, cancelText, firstActionText, secondActionText) =>
                {
                    var cts = new CancellationTokenSource();
                    var dlg = new Dialog(new DialogId { NativeReference = id });
                    _cts.Add(id, cts);
                    question(dlg, title, text, type, cancelText, firstActionText, secondActionText, cts.Token);

                },
                DisplayProgress = (data, id, title, text, indeterminate, position, cancelText) =>
                {
                    var cts = new CancellationTokenSource();
                    var dlg = new Dialog(new DialogId { NativeReference = id });
                    _cts.Add(id, cts);
                    displayProgress(dlg, title, text, indeterminate, position, cancelText, cts.Token);
                },
                Cancel = (data, id) =>
                {
                    if (_cts.TryGetValue(id, out var token))
                    {
                        token.Cancel();
                        _cts.Remove(id);
                    }
                },
                UpdateProgress = (data, id, position, text) =>
                {
                    var dlg = new Dialog(new DialogId { NativeReference = id });
                    updateProgress(dlg, position, text);
                }
            };

            _dialogCbsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DialogCallbacks)));
            Marshal.StructureToPtr(dialogCbs, _dialogCbsPtr, true);
            Native.LibVLCDialogSetCallbacks(NativeReference, _dialogCbsPtr, IntPtr.Zero);
        }

        public bool DialogHandlersSet => _dialogCbsPtr != IntPtr.Zero;

        public RendererDescription[] RendererList
        {
            get
            {
                // TODO: Move marshalling logic to generic MarshalUtils func
                var discoverList = IntPtr.Zero;
                var count = Native.LibVLCRendererDiscovererGetList(NativeReference, ref discoverList);

                if (count == 0)
#if NETSTANDARD1_1 || NET40
                    return new RendererDescription[0];
#else
                    return Array.Empty<RendererDescription>();
#endif

                var rendererDiscovererDescription = new RendererDescription[(int)count];

                for (var i = 0; i < (int)count; i++)
                {
                    var ptr = Marshal.ReadIntPtr(discoverList, i * IntPtr.Size);
                    var managedStruct = (RendererDescription)Marshal.PtrToStructure(ptr, typeof(RendererDescription));
                    rendererDiscovererDescription[i] = managedStruct;
                }

                Native.LibVLCRendererDiscovererReleaseList(discoverList, count);

                return rendererDiscovererDescription;
            }
        }

        public struct RendererDescription
        {
            public string Name { get; }
            public string LongName { get; }

            public RendererDescription(string name, string longName)
            {
                Name = name;
                LongName = longName;
            }
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
#if NET40
            Task.Factory.StartNew(() => _log?.Invoke(NativeReference, new LogEventArgs(level, formattedDecodedMessage, module, file, line)));
#else
            Task.Run(() => _log?.Invoke(NativeReference, new LogEventArgs(level, formattedDecodedMessage, module, file, line)));
#endif
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

    /// <summary>Logging messages level.</summary>
    /// <remarks>Future LibVLC versions may define new levels.</remarks>
    public enum LogLevel
    {
        /// <summary>Debug message</summary>
        Debug = 0,
        /// <summary>Important informational message</summary>
        Notice = 2,
        /// <summary>Warning (potential error) message</summary>
        Warning = 3,
        /// <summary>Error message</summary>
        Error = 4
    }

#region Callbacks

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ExitCallback();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogCallback(IntPtr data, LogLevel logLevel, IntPtr logContext,
        [MarshalAs(UnmanagedType.LPStr)] string format, IntPtr args);

#endregion
}