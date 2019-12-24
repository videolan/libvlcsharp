using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Shared.Helpers;
using LibVLCSharp.Shared.Structures;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Main LibVLC API object representing a libvlc instance in native code. 
    /// Note: You may create multiple mediaplayers from a single LibVLC instance
    /// </summary>
    public class LibVLC : Internal
    {
        /// <summary>
        /// Determines whether two object instances are equal. 
        /// </summary>
        /// <param name="other">other libvlc instance to compare with</param>
        /// <returns>true if same instance, false otherwise</returns>
        protected bool Equals(LibVLC other)
        {
            return NativeReference.Equals(other.NativeReference);
        }

        /// <summary>
        /// Determines whether two object instances are equal. 
        /// </summary>
        /// <param name="obj">other libvlc instance to compare with</param>
        /// <returns>true if same instance, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LibVLC) obj);
        }

        LogCallback _logCallback;
        readonly object _logLock = new object();

        /// <summary>
        /// The real log event handlers.
        /// </summary>
        EventHandler<LogEventArgs> _log;

#if NETFRAMEWORK || NETSTANDARD
        IntPtr _logFileHandle;
#endif
        /// <summary>
        /// Returns the hashcode for this libvlc instance
        /// </summary>
        /// <returns></returns>
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

#if NETFRAMEWORK || NETSTANDARD
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_add_intf")]
            internal static extern int LibVLCAddInterface(IntPtr libVLC, IntPtr name);
#endif
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_exit_handler")]
            internal static extern void LibVLCSetExitHandler(IntPtr libVLC, IntPtr cb, IntPtr opaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_user_agent")]
            internal static extern void LibVLCSetUserAgent(IntPtr libVLC, IntPtr name, IntPtr http);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_set_app_id")]
            internal static extern void LibVLCSetAppId(IntPtr libVLC, IntPtr id, IntPtr version, IntPtr icon);

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
            internal static extern IntPtr LibVLCAudioOutputDeviceListGet(IntPtr libVLC, IntPtr aout);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_audio_output_device_list_release")]
            internal static extern void LibVLCAudioOutputDeviceListRelease(IntPtr list);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_list_get")]
            internal static extern UIntPtr LibVLCMediaDiscovererListGet(IntPtr libVLC, MediaDiscovererCategory category, out IntPtr pppServices);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_list_release")]
            internal static extern void LibVLCMediaDiscovererListRelease(IntPtr ppServices, UIntPtr count);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_dialog_set_callbacks")]
            internal static extern void LibVLCDialogSetCallbacks(IntPtr libVLC, DialogCallbacks callbacks, IntPtr data);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_list_get")]
            internal static extern UIntPtr LibVLCRendererDiscovererGetList(IntPtr libVLC, out IntPtr discovererList);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_list_release")]
            internal static extern void LibVLCRendererDiscovererReleaseList(IntPtr discovererList, UIntPtr count);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_retain")]
            internal static extern void LibVLCRetain(IntPtr libVLC);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_version")]
            internal static extern IntPtr LibVLCVersion();

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_changeset")]
            internal static extern IntPtr LibVLCChangeset();

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_errmsg")]
            internal static extern IntPtr LibVLCErrorMessage();

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_clearerr")]
            internal static extern void LibVLCClearError();

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_compiler")]
            internal static extern IntPtr LibVLCGetCompiler();

#if ANDROID
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_android_context")]
            internal static extern void LibVLCMediaPlayerSetAndroidContext(IntPtr mediaPlayer, IntPtr aWindow);
#endif
        }

        /// <summary>
        /// Create and initialize a libvlc instance.
        /// This functions accept a list of &quot;command line&quot; arguments similar to the
        /// main(). These arguments affect the LibVLC instance default configuration.
        /// LibVLC may create threads. Therefore, any thread-unsafe process
        /// initialization must be performed before calling libvlc_new(). In particular
        /// and where applicable:
        /// <para>- setlocale() and textdomain(),</para>
        /// <para>- setenv(), unsetenv() and putenv(),</para>
        /// <para>- with the X11 display system, XInitThreads()</para>
        /// (see also libvlc_media_player_set_xwindow()) and
        /// <para>- on Microsoft Windows, SetErrorMode().</para>
        /// <para>- sigprocmask() shall never be invoked; pthread_sigmask() can be used.</para>
        /// On POSIX systems, the SIGCHLD signalmust notbe ignored, i.e. the
        /// signal handler must set to SIG_DFL or a function pointer, not SIG_IGN.
        /// Also while LibVLC is active, the wait() function shall not be called, and
        /// any call to waitpid() shall use a strictly positive value for the first
        /// parameter (i.e. the PID). Failure to follow those rules may lead to a
        /// deadlock or a busy loop.
        /// Also on POSIX systems, it is recommended that the SIGPIPE signal be blocked,
        /// even if it is not, in principles, necessary, e.g.:
        /// On Microsoft Windows Vista/2008, the process error mode
        /// SEM_FAILCRITICALERRORS flagmustbe set before using LibVLC.
        /// On later versions, that is optional and unnecessary.
        /// Also on Microsoft Windows (Vista and any later version), setting the default
        /// DLL directories to SYSTEM32 exclusively is strongly recommended for
        /// security reasons:
        /// Arguments are meant to be passed from the command line to LibVLC, just like
        /// VLC media player does. The list of valid arguments depends on the LibVLC
        /// version, the operating system and platform, and set of available LibVLC
        /// plugins. Invalid or unsupported arguments will cause the function to fail
        /// (i.e. return NULL). Also, some arguments may alter the behaviour or
        /// otherwise interfere with other LibVLC functions.
        /// There is absolutely no warranty or promise of forward, backward and
        /// cross-platform compatibility with regards to libvlc_new() arguments.
        /// We recommend that you do not use them, other than when debugging.
        /// </summary>
        /// <param name="options">list of arguments (should be NULL)</param>
        /// <returns>the libvlc instance or NULL in case of error</returns>
        public LibVLC(params string[] options)
            : base(() => MarshalUtils.CreateWithOptions(PatchOptions(options), Native.LibVLCNew), Native.LibVLCRelease)
        {
        }

        /// <summary>
        /// Make dirty hacks to include necessary defaults on some platforms.
        /// </summary>
        /// <param name="options">The options given by the user</param>
        /// <returns>The patched options</returns>
        static string[] PatchOptions(string[] options)
        {
#if UWP
            return options.Concat(new[] {"--aout=winstore"}).ToArray();
#else
            return options;
#endif
        }

        /// <summary>
        /// Dipose of this libvlc instance
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed || NativeReference == IntPtr.Zero)
                return;

            if (disposing)
            {
                UnsetDialogHandlers();
                UnsetLog();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Determines whether 2 instances of libvlc are equals
        /// </summary>
        /// <param name="libvlc1">1st instance of libvlc</param>
        /// <param name="libvlc2">2nd instance of libvlc</param>
        /// <returns></returns>
        public static bool operator ==(LibVLC libvlc1, LibVLC libvlc2)
        {
            return libvlc1?.NativeReference == libvlc2?.NativeReference;
        }

        /// <summary>
        /// Determines whether 2 instances of libvlc are different
        /// </summary>
        /// <param name="libvlc1">1st instance of libvlc</param>
        /// <param name="libvlc2">2nd instance of libvlc</param>
        /// <returns></returns>
        public static bool operator !=(LibVLC libvlc1, LibVLC libvlc2)
        {
            return libvlc1?.NativeReference != libvlc2?.NativeReference;
        }

#if NETFRAMEWORK || NETSTANDARD
        /// <summary>
        /// Try to start a user interface for the libvlc instance.
        /// </summary>
        /// <param name="name">interface name, or empty string for default</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool AddInterface(string name)
        {
            var namePtr = name.ToUtf8();
            return MarshalUtils.PerformInteropAndFree(() => Native.LibVLCAddInterface(NativeReference, namePtr) == 0, namePtr);
        }
#endif
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
            var nameUtf8 = name.ToUtf8();
            var httpUtf8 = http.ToUtf8();

            MarshalUtils.PerformInteropAndFree(() => Native.LibVLCSetUserAgent(NativeReference, nameUtf8, httpUtf8), nameUtf8, httpUtf8);
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
            var idUtf8 = id.ToUtf8();
            var versionUtf8 = version.ToUtf8();
            var iconUtf8 = icon.ToUtf8();

            MarshalUtils.PerformInteropAndFree(() => Native.LibVLCSetAppId(NativeReference, idUtf8, versionUtf8, iconUtf8),
                idUtf8, versionUtf8, iconUtf8);
        }

#if NETFRAMEWORK || NETSTANDARD
        /// <summary>
        /// Close log file handle
        /// </summary>
        /// <returns>true if no file to close or close operation successful, false otherwise</returns>
        public bool CloseLogFile()
        {
            if (_logFileHandle == IntPtr.Zero) return true;

            return MarshalUtils.Close(_logFileHandle);
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
            return filePtr;
        }
#endif
        GCHandle _libvlcGcHandle;
        void SetLog(LogCallback cb)
        {
            _logCallback = cb ?? throw new ArgumentException(nameof(cb));

            _libvlcGcHandle = GCHandle.Alloc(this, GCHandleType.Normal);

            Native.LibVLCLogSet(NativeReference, cb, GCHandle.ToIntPtr(_libvlcGcHandle));
        }

        void UnsetLog()
        {
            if (_logCallback == null) return;

            if (_libvlcGcHandle.IsAllocated)
            {
                _libvlcGcHandle.Free();
            }

            _logCallback = null;
            Native.LibVLCLogUnset(NativeReference);
        }

        int _logSubscriberCount = 0;
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
                    if(_logSubscriberCount == 0)
                        SetLog(OnLogInternal);
                    _logSubscriberCount++;
                }
            }

            remove
            {
                lock (_logLock)
                {
                    _log -= value;
                    if (_logSubscriberCount > 0)
                    {
                        _logSubscriberCount--;
                    }
                    if (_logSubscriberCount == 0)
                    {
                        UnsetLog();
                    }
                }
            }
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
        public ModuleDescription[] AudioFilters => MarshalUtils.Retrieve(() => Native.LibVLCAudioFilterListGet(NativeReference),
            MarshalUtils.PtrToStructure<ModuleDescriptionStructure>,
            s => s.Build(),
            module => module.Next, 
            Native.LibVLCModuleDescriptionListRelease);

        /// <summary>Returns a list of video filters that are available.</summary>
        /// <returns>
        /// <para>a list of module descriptions. It should be freed with libvlc_module_description_list_release().</para>
        /// <para>In case of an error, NULL is returned.</para>
        /// </returns>
        /// <remarks>
        /// <para>libvlc_module_description_t</para>
        /// <para>libvlc_module_description_list_release</para>
        /// </remarks>
        public ModuleDescription[] VideoFilters => MarshalUtils.Retrieve(() => Native.LibVLCVideoFilterListGet(NativeReference),
            MarshalUtils.PtrToStructure<ModuleDescriptionStructure>,
            s => s.Build(),
            module => module.Next, 
            Native.LibVLCModuleDescriptionListRelease);

        /// <summary>Gets the list of available audio output modules.</summary>
        /// <returns>list of available audio outputs. It must be freed with</returns>
        /// <remarks>
        /// <para>libvlc_audio_output_list_release</para>
        /// <para>libvlc_audio_output_t .</para>
        /// <para>In case of error, NULL is returned.</para>
        /// </remarks>
        public AudioOutputDescription[] AudioOutputs => MarshalUtils.Retrieve(() => Native.LibVLCAudioOutputListGet(NativeReference), 
            ptr => MarshalUtils.PtrToStructure<AudioOutputDescriptionStructure>(ptr),
            s => s.Build(), 
            s => s.Next, 
            Native.LibVLCAudioOutputListRelease);

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
        public AudioOutputDevice[] AudioOutputDevices(string audioOutputName) => 
            MarshalUtils.Retrieve(() => 
            {
                var audioOutputNameUtf8 = audioOutputName.ToUtf8();
                return MarshalUtils.PerformInteropAndFree(() => 
                    Native.LibVLCAudioOutputDeviceListGet(NativeReference, audioOutputNameUtf8), audioOutputNameUtf8);
            }, 
            MarshalUtils.PtrToStructure<AudioOutputDeviceStructure>,
            s => s.Build(), 
            device => device.Next, 
            Native.LibVLCAudioOutputDeviceListRelease);
        
        /// <summary>Get media discoverer services by category</summary>
        /// <param name="discovererCategory">category of services to fetch</param>
        /// <returns>the number of media discoverer services (0 on error)</returns>
        /// <remarks>LibVLC 3.0.0 and later.</remarks>
        public MediaDiscovererDescription[] MediaDiscoverers(MediaDiscovererCategory discovererCategory) =>
            MarshalUtils.Retrieve(NativeReference, discovererCategory, 
                (IntPtr nativeRef, MediaDiscovererCategory enumType, out IntPtr array) => Native.LibVLCMediaDiscovererListGet(nativeRef, enumType, out array),
            MarshalUtils.PtrToStructure<MediaDiscovererDescriptionStructure>,
            m => m.Build(),
            Native.LibVLCMediaDiscovererListRelease);

        #region DialogManagement

        /// <summary>
        /// Register callbacks in order to handle VLC dialogs. 
        /// LibVLC 3.0.0 and later.
        /// </summary>
        /// <param name="error">Called when an error message needs to be displayed.</param>
        /// <param name="login">Called when a login dialog needs to be displayed.
        /// You can interact with this dialog by calling Dialog.PostLogin() to post an answer or Dialog.Dismiss() to cancel this dialog.</param>
        /// <param name="question">Called when a question dialog needs to be displayed.
        /// You can interact with this dialog by calling Dialog.PostLogin() to post an answer or Dialog.Dismiss() to cancel this dialog.</param>
        /// <param name="displayProgress">Called when a progress dialog needs to be displayed.</param>
        /// <param name="updateProgress">Called when a progress dialog needs to be updated.</param>
        public void SetDialogHandlers(DisplayError error, DisplayLogin login, DisplayQuestion question,
            DisplayProgress displayProgress, UpdateProgress updateProgress)
        {
            _error = error ?? throw new ArgumentNullException(nameof(error));
            _login = login ?? throw new ArgumentNullException(nameof(login));
            _question = question ?? throw new ArgumentNullException(nameof(question));
            _displayProgress = displayProgress ?? throw new ArgumentNullException(nameof(displayProgress));
            _updateProgress = updateProgress ?? throw new ArgumentNullException(nameof(updateProgress));

            _dialogCbs = new DialogCallbacks(Error, Login, Question, DisplayProgress, Cancel, UpdateProgress);
            Native.LibVLCDialogSetCallbacks(NativeReference, _dialogCbs, IntPtr.Zero);
        }

        /// <summary>
        /// Unset dialog callbacks if previously set
        /// </summary>
        public void UnsetDialogHandlers()
        {
            if (DialogHandlersSet)
            {
                _dialogCbs = default;
                Native.LibVLCDialogSetCallbacks(NativeReference, _dialogCbs, IntPtr.Zero);
                _error = null;
                _login = null;
                _question = null;
                _displayProgress = null;
                _updateProgress = null;
            }
        }

        /// <summary>
        /// True if dialog handlers are set
        /// </summary>
        public bool DialogHandlersSet => _dialogCbs.DisplayLogin != IntPtr.Zero;

        DialogCallbacks _dialogCbs;
        static DisplayError _error;
        static DisplayLogin _login;
        static DisplayQuestion _question;
        static DisplayProgress _displayProgress;
        static UpdateProgress _updateProgress;
        static readonly Dictionary<IntPtr, CancellationTokenSource> _cts = new Dictionary<IntPtr, CancellationTokenSource>();

        [MonoPInvokeCallback(typeof(DisplayErrorCallback))]
        static void Error(IntPtr data, string title, string text)
        {
            _error?.Invoke(title, text);
        }

        [MonoPInvokeCallback(typeof(DisplayLoginCallback))]
        static void Login(IntPtr data, IntPtr dialogId, string title, string text, string defaultUsername, bool askStore)
        {
            if (_login == null) return;

            var cts = new CancellationTokenSource();
            var dlg = new Dialog(new DialogId(dialogId));
            _cts[dialogId] = cts;
            _login(dlg, title, text, defaultUsername, askStore, cts.Token);
        }
        
        [MonoPInvokeCallback(typeof(DisplayQuestionCallback))]
        static void Question(IntPtr data, IntPtr dialogId, string title, string text, DialogQuestionType type, 
            string cancelText, string firstActionText, string secondActionText)
        {
            if (_question == null) return;

            var cts = new CancellationTokenSource();
            var dlg = new Dialog(new DialogId(dialogId));
            _cts[dialogId] = cts;
            _question(dlg, title, text, type, cancelText, firstActionText, secondActionText, cts.Token);
        }

        [MonoPInvokeCallback(typeof(DisplayProgressCallback))]
        static void DisplayProgress(IntPtr data, IntPtr dialogId, string title, string text, bool indeterminate, float position, string cancelText)
        {
            if (_displayProgress == null) return;

            var cts = new CancellationTokenSource();
            var dlg = new Dialog(new DialogId(dialogId));
            _cts[dialogId] = cts;
            _displayProgress(dlg, title, text, indeterminate, position, cancelText, cts.Token);
        }

        [MonoPInvokeCallback(typeof(CancelCallback))]
        static void Cancel(IntPtr data, IntPtr dialogId)
        {
            if (_cts.TryGetValue(dialogId, out var token))
            {
                token.Cancel();
                _cts.Remove(dialogId);
            }
        }

        [MonoPInvokeCallback(typeof(UpdateProgressCallback))]
        static void UpdateProgress(IntPtr data, IntPtr dialogId, float position, string text)
        {
            if (_updateProgress == null) return;

            var dlg = new Dialog(new DialogId(dialogId));
            _updateProgress(dlg, position, text);
        }

        #endregion
        
        /// <summary>
        /// List of available renderers used to create RendererDiscoverer objects
        /// Note: LibVLC 3.0.0 and later
        /// </summary>       
        public RendererDescription[] RendererList => MarshalUtils.Retrieve(NativeReference, 
            (IntPtr nativeRef, out IntPtr array) => Native.LibVLCRendererDiscovererGetList(nativeRef, out array),
            MarshalUtils.PtrToStructure<RendererDescriptionStructure>,
            m => m.Build(),
            Native.LibVLCRendererDiscovererReleaseList);

        [MonoPInvokeCallback(typeof(LogCallback))]
        static void OnLogInternal(IntPtr data, LogLevel level, IntPtr ctx, IntPtr format, IntPtr args)
        {
            if (data == IntPtr.Zero)
                return;

            var gch = GCHandle.FromIntPtr(data);

            if (!gch.IsAllocated || !(gch.Target is LibVLC libvlc) || libvlc.IsDisposed)
                return;

            try
            {
                var message = MarshalUtils.GetLogMessage(format, args);

                GetLogContext(ctx, out var module, out var file, out var line);
#if NET40
                Task.Factory.StartNew(() => libvlc._log?.Invoke(null, new LogEventArgs(level, message, module, file, line)));
#else
                Task.Run(() => libvlc._log?.Invoke(null, new LogEventArgs(level, message, module, file, line)));
#endif
            }
            // Silently catching OOM exceptions and others as this is not critical if it fails
            catch
            {
            }
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
        static void GetLogContext(IntPtr logContext, out string module, out string file, out uint? line)
        {
            Native.LibVLCLogGetContext(logContext, out var modulePtr, out var filePtr, out var linePtr);

            line = linePtr == UIntPtr.Zero ? null : (uint?)linePtr.ToUInt32();
            module = modulePtr.FromUtf8();
            file = filePtr.FromUtf8();
        }

        /// <summary>Increments the native reference counter for this libvlc instance</summary>
        internal void Retain() => Native.LibVLCRetain(NativeReference);

        /// <summary>The version of the LibVLC engine currently used by LibVLCSharp</summary>
        public string Version => Native.LibVLCVersion().FromUtf8();

        /// <summary>The changeset of the LibVLC engine currently used by LibVLCSharp</summary>
        public string Changeset => Native.LibVLCChangeset().FromUtf8();

        /// <summary>
        /// A human-readable error message for the last LibVLC error in the calling
        /// thread. The resulting string is valid until another error occurs (at least
        /// until the next LibVLC call). 
        /// <para/> Null if no error.
        /// </summary>
        public string LastLibVLCError => Native.LibVLCErrorMessage().FromUtf8();

        /// <summary>
        /// Clears the LibVLC error status for the current thread. This is optional.
        /// By default, the error status is automatically overridden when a new error
        /// occurs, and destroyed when the thread exits.
        /// </summary>
        public void ClearLibVLCError() => Native.LibVLCClearError();

        /// <summary>
        /// Retrieve the libvlc compiler version.
        /// Example: "gcc version 4.2.3 (Ubuntu 4.2.3-2ubuntu6)"
        /// </summary>
        public string LibVLCCompiler => Native.LibVLCGetCompiler().FromUtf8();
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

    /// <summary>
    /// Registers a callback for the LibVLC exit event. 
    /// This is mostly useful if the VLC playlist and/or at least one interface are started with libvlc_playlist_play() 
    /// or AddInterface() respectively. Typically, this function will wake up your application main loop (from another thread).
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ExitCallback();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void LogCallback(IntPtr data, LogLevel logLevel, IntPtr logContext, IntPtr format, IntPtr args);

#endregion
}
