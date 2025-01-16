using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Helpers;

namespace LibVLCSharp
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
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LibVLC) obj);
        }

        readonly object _logLock = new object();

        /// <summary>
        /// The real log event handlers.
        /// </summary>
        EventHandler<LogEventArgs>? _log;

#if DESKTOP
        IntPtr _logFileHandle;
#endif

        /// <summary>
        /// The GCHandle to be passed to callbacks as userData
        /// </summary>
        GCHandle _gcHandle;

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
            internal static extern void LibVLCLogSet(IntPtr libVLC, InternalLogCallback cb, IntPtr data);

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
                EntryPoint = "libvlc_media_discoverer_list_get")]
            internal static extern UIntPtr LibVLCMediaDiscovererListGet(IntPtr libVLC, MediaDiscovererCategory category, out IntPtr pppServices);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_list_release")]
            internal static extern void LibVLCMediaDiscovererListRelease(IntPtr ppServices, UIntPtr count);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_dialog_set_callbacks")]
            internal static extern void LibVLCDialogSetCallbacks(IntPtr libVLC, DialogCallbacks callbacks, IntPtr data);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_dialog_set_error_callback")]
            internal static extern void LibVLCDialogSetErrorCallbacks(IntPtr libVLC, IntPtr errorCallback, IntPtr data);

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

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_clock")]
            internal static extern long LibVLCClock();

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_abi_version")]
            internal static extern int LibVLCABIVersion();

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
        /// <para/> This will throw a <see cref="VLCException"/> if the native libvlc libraries cannot be found or loaded.
        /// <para/> It may also throw a <see cref="VLCException"/> if the LibVLC and LibVLCSharp major versions do not match.
        /// See https://code.videolan.org/videolan/LibVLCSharp/-/blob/master/docs/versioning.md for more info about the versioning strategy.
        /// <example>
        /// <code>
        /// // example <br/>
        /// using var libvlc = new LibVLC("--verbose=2");
        /// <br/> // or <br/>
        /// using var libvlc = new LibVLC("--verbose", "2");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="options">list of arguments, in the form "--option=value"</param>
        /// <returns>the libvlc instance or NULL in case of error</returns>
        public LibVLC(params string[] options)
            : base(() => MarshalUtils.CreateWithOptions(PatchOptions(options), Native.LibVLCNew), Native.LibVLCRelease)
        {
            _gcHandle = GCHandle.Alloc(this);
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
        /// <para/> This will throw a <see cref="VLCException"/> if the native libvlc libraries cannot be found or loaded.
        /// <para/> It may also throw a <see cref="VLCException"/> if the LibVLC and LibVLCSharp major versions do not match.
        /// See https://code.videolan.org/videolan/LibVLCSharp/-/blob/master/docs/versioning.md for more info about the versioning strategy.
        /// </summary>
        /// <param name="enableDebugLogs">enable verbose debug logs</param>
        /// <param name="options">list of arguments (should be NULL)</param>
        public LibVLC(bool enableDebugLogs, params string[] options)
            : base(() => MarshalUtils.CreateWithOptions(PatchOptions(options, enableDebugLogs), Native.LibVLCNew), Native.LibVLCRelease)
        {
            _gcHandle = GCHandle.Alloc(this);
        }

        /// <summary>
        /// Make dirty hacks to include necessary defaults on some platforms.
        /// </summary>
        /// <param name="options">The options given by the user</param>
        /// <param name="enableDebugLogs">enable debug logs</param>
        /// <returns>The patched options</returns>
        static string[] PatchOptions(string[] options, bool enableDebugLogs = false)
        {
            if(enableDebugLogs)
            {
                options = options.Concat(new[] { "--verbose=2" }).ToArray();
            }
            return options;
        }

        /// <summary>
        /// Dispose of this libvlc instance
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsetDialogHandlers();
                Native.LibVLCLogUnset(NativeReference);
                _gcHandle.Free();
                _exitCallback = null;
                _log = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Determines whether 2 instances of libvlc are equals
        /// </summary>
        /// <param name="libvlc1">1st instance of libvlc</param>
        /// <param name="libvlc2">2nd instance of libvlc</param>
        /// <returns></returns>
        public static bool operator ==(LibVLC? libvlc1, LibVLC? libvlc2)
        {
            return libvlc1?.NativeReference == libvlc2?.NativeReference;
        }

        /// <summary>
        /// Determines whether 2 instances of libvlc are different
        /// </summary>
        /// <param name="libvlc1">1st instance of libvlc</param>
        /// <param name="libvlc2">2nd instance of libvlc</param>
        /// <returns></returns>
        public static bool operator !=(LibVLC? libvlc1, LibVLC? libvlc2)
        {
            return libvlc1?.NativeReference != libvlc2?.NativeReference;
        }

        internal ExitCallback? _exitCallback;

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
        public void SetAppId(string? id, string? version, string? icon)
        {
            var idUtf8 = id.ToUtf8();
            var versionUtf8 = version.ToUtf8();
            var iconUtf8 = icon.ToUtf8();

            MarshalUtils.PerformInteropAndFree(() => Native.LibVLCSetAppId(NativeReference, idUtf8, versionUtf8, iconUtf8),
                idUtf8, versionUtf8, iconUtf8);
        }

#if DESKTOP
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
                    if (_logSubscriberCount == 0)
                    {
                        // First subscriber, registering log handler
                        Native.LibVLCLogSet(NativeReference, LogCallbackHandle, GCHandle.ToIntPtr(_gcHandle));
                    }

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
                        Native.LibVLCLogUnset(NativeReference);
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
        /// <param name="login">Called when a login dialog needs to be displayed.
        /// You can interact with this dialog by calling Dialog.PostLogin() to post an answer or Dialog.Dismiss() to cancel this dialog.</param>
        /// <param name="question">Called when a question dialog needs to be displayed.
        /// You can interact with this dialog by calling Dialog.PostLogin() to post an answer or Dialog.Dismiss() to cancel this dialog.</param>
        /// <param name="displayProgress">Called when a progress dialog needs to be displayed.</param>
        /// <param name="updateProgress">Called when a progress dialog needs to be updated.</param>
        public void SetDialogHandlers(DisplayLogin login, DisplayQuestion question,
            DisplayProgress displayProgress, UpdateProgress updateProgress)
        {
            _login = login ?? throw new ArgumentNullException(nameof(login));
            _question = question ?? throw new ArgumentNullException(nameof(question));
            _displayProgress = displayProgress ?? throw new ArgumentNullException(nameof(displayProgress));
            _updateProgress = updateProgress ?? throw new ArgumentNullException(nameof(updateProgress));

            Native.LibVLCDialogSetCallbacks(NativeReference, DialogCb, GCHandle.ToIntPtr(_gcHandle));
        }

        /// <summary>
        /// Register callback in order to handle VLC error messages
        /// version 4.0.0 and later
        /// </summary>
        /// <param name="error">the user callback to raise on VLC error, null to unregister the callback</param>
        public void SetErrorDialogCallback(DisplayError error)
        {
            _error = error;
            if(_error == null)
            {
                DisplayErrorHandle = IntPtr.Zero;
                Native.LibVLCDialogSetErrorCallbacks(NativeReference, IntPtr.Zero, IntPtr.Zero);
            }
            else
            { 
                DisplayErrorHandle = Marshal.GetFunctionPointerForDelegate(DisplayErrorCallbackHandle);
                Native.LibVLCDialogSetErrorCallbacks(NativeReference, DisplayErrorHandle, GCHandle.ToIntPtr(_gcHandle));
            }
        }

        /// <summary>
        /// Unset dialog callbacks if previously set
        /// </summary>
        public void UnsetDialogHandlers()
        {
            if (DialogHandlersSet)
            {
                Native.LibVLCDialogSetCallbacks(NativeReference, default, IntPtr.Zero);
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
        public bool DialogHandlersSet => _login != null;
        DisplayError? _error;
        DisplayLogin? _login;
        DisplayQuestion? _question;
        DisplayProgress? _displayProgress;
        UpdateProgress? _updateProgress;
        readonly Dictionary<IntPtr, CancellationTokenSource> _cts = new Dictionary<IntPtr, CancellationTokenSource>();

        void OnDisplayError(string? title, string? text)
        {
            _error?.Invoke(title, text);
        }

        void OnDisplayLogin(IntPtr dialogId, string? title, string? text, string? defaultUsername, bool askStore)
        {
            if (_login == null) return;

            var cts = new CancellationTokenSource();
            var dlg = new Dialog(new DialogId(dialogId));
            _cts[dialogId] = cts;
            _login(dlg, title, text, defaultUsername, askStore, cts.Token);
        }

        void OnDisplayQuestion(IntPtr dialogId, string? title, string? text, DialogQuestionType type,
            string? cancelText, string? firstActionText, string? secondActionText)
        {
            if (_question == null) return;

            var cts = new CancellationTokenSource();
            var dlg = new Dialog(new DialogId(dialogId));
            _cts[dialogId] = cts;
            _question(dlg, title, text, type, cancelText, firstActionText, secondActionText, cts.Token);
        }

        void OnDisplayProgress(IntPtr dialogId, string? title, string? text, bool indeterminate, float position, string? cancelText)
        {
            if (_displayProgress == null) return;

            var cts = new CancellationTokenSource();
            var dlg = new Dialog(new DialogId(dialogId));
            _cts[dialogId] = cts;
            _displayProgress(dlg, title, text, indeterminate, position, cancelText, cts.Token);
        }

        void OnCancel(IntPtr dialogId)
        {
            if (_cts.TryGetValue(dialogId, out var token))
            {
                token.Cancel();
                _cts.Remove(dialogId);
            }
        }

        void OnUpdateProgress(IntPtr dialogId, float position, string? text)
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
        /// <param name="logContext">The log message context (as passed to the <see cref="InternalLogCallback"/>)</param>
        /// <param name="module">The module name storage.</param>
        /// <param name="file">The source code file name storage.</param>
        /// <param name="line">The source code file line number storage.</param>
        static void GetLogContext(IntPtr logContext, out string? module, out string? file, out uint? line)
        {
            Native.LibVLCLogGetContext(logContext, out var modulePtr, out var filePtr, out var linePtr);

            line = linePtr == UIntPtr.Zero ? null : (uint?)linePtr.ToUInt32();
            module = modulePtr.FromUtf8();
            file = filePtr.FromUtf8();
        }

        /// <summary>Increments the native reference counter for this libvlc instance</summary>
        internal void Retain() => Native.LibVLCRetain(NativeReference);

        /// <summary>The version of the LibVLC engine currently used by LibVLCSharp</summary>
        public string Version => Native.LibVLCVersion().FromUtf8()!;

        /// <summary>The changeset of the LibVLC engine currently used by LibVLCSharp</summary>
        public string Changeset => Native.LibVLCChangeset().FromUtf8()!;

        /// <summary>
        /// A human-readable error message for the last LibVLC error in the calling
        /// thread. The resulting string is valid until another error occurs (at least
        /// until the next LibVLC call).
        /// <para/> Null if no error.
        /// </summary>
        public string? LastLibVLCError => Native.LibVLCErrorMessage().FromUtf8();

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
        public string LibVLCCompiler => Native.LibVLCGetCompiler().FromUtf8()!;

        /// <summary>
        /// Return the current time as defined by LibVLC. The unit is the microsecond.
        /// Time increases monotonically (regardless of time zone changes and RTC adjustments).
        /// The origin is arbitrary but consistent across the whole system (e.g. the system uptime, the time since the system was booted).
        /// On systems that support it, the POSIX monotonic clock is used.
        /// </summary>
        public long Clock => Native.LibVLCClock();

        /// <summary>
        /// Get the ABI version of the libvlc library. <br/>
        /// This is different than the VLC version, which is the version of the whole
        /// VLC package. The value is the same as LIBVLC_ABI_VERSION_INT used when
        /// compiling.
        /// </summary>
        /// <returns>a value with the following mask in hexadecimal:
        ///  0xFF000000: major VLC version, similar to VLC major version,
        ///  0x00FF0000: major ABI version, incremented incompatible changes are added,
        ///  0x0000FF00: minor ABI version, incremented when new functions are added
        ///  0x000000FF: micro ABI version, incremented with new release/builds
        ///  <br/>
        ///  This the same value as the.so version but cross platform.
        /// </returns>
        public int ABI => Native.LibVLCABIVersion();

        #region Exit

        static readonly InternalExitCallback ExitCallbackHandle = ExitCallback;

        [MonoPInvokeCallback(typeof(InternalExitCallback))]
        private static void ExitCallback(IntPtr libVLCHandle)
        {
            var libVLC = MarshalUtils.GetInstance<LibVLC>(libVLCHandle);
            libVLC?._exitCallback?.Invoke();
        }
        #endregion

        #region Log
        static readonly InternalLogCallback LogCallbackHandle = LogCallback;

        [MonoPInvokeCallback(typeof(InternalLogCallback))]
        private static void LogCallback(IntPtr libVLCHandle, LogLevel logLevel, IntPtr logContext, IntPtr format, IntPtr args)
        {
            var libVLC = MarshalUtils.GetInstance<LibVLC>(libVLCHandle);

            try
            {
                var message = MarshalUtils.GetLogMessage(format, args);

                GetLogContext(logContext, out var module, out var file, out var line);

                void logAction() => libVLC?._log?.Invoke(null, new LogEventArgs(logLevel, message, module, file, line));

                Task.Run(logAction);
            }
            catch
            {
                // Silently catching OOM exceptions and others as this is not critical if it fails
            }
        }
        #endregion

        #region Dialogs

        private IntPtr DisplayErrorHandle;

        private static readonly InternalDisplayErrorCallback DisplayErrorCallbackHandle = DisplayErrorCallback;
        private static readonly InternalDisplayLoginCallback DisplayLoginCallbackHandle = DisplayLoginCallback;
        private static readonly InternalDisplayQuestionCallback DisplayQuestionCallbackHandle = DisplayQuestionCallback;
        private static readonly InternalDisplayProgressCallback DisplayProgressCallbackHandle = DisplayProgressCallback;
        private static readonly InternalCancelCallback CancelCallbackHandle = CancelCallback;
        private static readonly InternalUpdateProgressCallback UpdateProgressCallbackHandle = UpdateProgressCallback;

        private static readonly DialogCallbacks DialogCb = new DialogCallbacks(
            DisplayLoginCallbackHandle,
            DisplayQuestionCallbackHandle,
            DisplayProgressCallbackHandle,
            CancelCallbackHandle,
            UpdateProgressCallbackHandle);

        [MonoPInvokeCallback(typeof(InternalDisplayErrorCallback))]
        static void DisplayErrorCallback(IntPtr libVLCHandle, IntPtr title, IntPtr text)
        {
            var libVLC = MarshalUtils.GetInstance<LibVLC>(libVLCHandle);
            libVLC?.OnDisplayError(title.FromUtf8(), text.FromUtf8());
        }

        [MonoPInvokeCallback(typeof(InternalDisplayLoginCallback))]
        static void DisplayLoginCallback(IntPtr libVLCHandle, IntPtr dialogId, IntPtr title, IntPtr text, IntPtr defaultUsername, bool askStore)
        {
            var libVLC = MarshalUtils.GetInstance<LibVLC>(libVLCHandle);
            libVLC?.OnDisplayLogin(dialogId, title.FromUtf8(), text.FromUtf8(), defaultUsername.FromUtf8(), askStore);
        }

        [MonoPInvokeCallback(typeof(InternalDisplayQuestionCallback))]
        static void DisplayQuestionCallback(IntPtr libVLCHandle, IntPtr dialogId, IntPtr title, IntPtr text, DialogQuestionType type,
            IntPtr cancelText, IntPtr firstActionText, IntPtr secondActionText)
        {
            var libVLC = MarshalUtils.GetInstance<LibVLC>(libVLCHandle);
            libVLC?.OnDisplayQuestion(dialogId, title.FromUtf8(), text.FromUtf8(), type, cancelText.FromUtf8(), firstActionText.FromUtf8(), secondActionText.FromUtf8());
        }

        [MonoPInvokeCallback(typeof(InternalDisplayProgressCallback))]
        static void DisplayProgressCallback(IntPtr libVLCHandle, IntPtr dialogId, IntPtr title, IntPtr text, bool indeterminate, float position, IntPtr cancelText)
        {
            var libVLC = MarshalUtils.GetInstance<LibVLC>(libVLCHandle);
            libVLC?.OnDisplayProgress(dialogId, title.FromUtf8(), text.FromUtf8(), indeterminate, position, cancelText.FromUtf8());
        }

        [MonoPInvokeCallback(typeof(InternalCancelCallback))]
        static void CancelCallback(IntPtr libVLCHandle, IntPtr dialogId)
        {
            var libVLC = MarshalUtils.GetInstance<LibVLC>(libVLCHandle);
            libVLC?.OnCancel(dialogId);
        }

        [MonoPInvokeCallback(typeof(InternalUpdateProgressCallback))]
        static void UpdateProgressCallback(IntPtr libVLCHandle, IntPtr dialogId, float position, IntPtr text)
        {
            var libVLC = MarshalUtils.GetInstance<LibVLC>(libVLCHandle);
            libVLC?.OnUpdateProgress(dialogId, position, text.FromUtf8());
        }
        #endregion

        #region internal callbacks

        /// <summary>
        /// Registers a callback for the LibVLC exit event.
        /// This is mostly useful if the VLC playlist and/or at least one interface are started with libvlc_playlist_play()
        /// or AddInterface() respectively. Typically, this function will wake up your application main loop (from another thread).
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalExitCallback(IntPtr libVLCHandle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalLogCallback(IntPtr data, LogLevel logLevel, IntPtr logContext, IntPtr format, IntPtr args);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalDisplayErrorCallback(IntPtr data, IntPtr title, IntPtr text);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalDisplayLoginCallback(IntPtr data, IntPtr dialogId, IntPtr title, IntPtr text,
            IntPtr defaultUsername, bool askStore);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalDisplayQuestionCallback(IntPtr data, IntPtr dialogId, IntPtr title, IntPtr text,
            DialogQuestionType type, IntPtr cancelText, IntPtr firstActionText, IntPtr secondActionText);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalDisplayProgressCallback(IntPtr data, IntPtr dialogId, IntPtr title, IntPtr text,
            bool indeterminate, float position, IntPtr cancelText);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalCancelCallback(IntPtr data, IntPtr dialogId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalUpdateProgressCallback(IntPtr data, IntPtr dialogId, float position, IntPtr text);

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct DialogCallbacks
        {
            internal DialogCallbacks(InternalDisplayLoginCallback displayLogin,
                InternalDisplayQuestionCallback displayQuestion,
                InternalDisplayProgressCallback displayProgress,
                InternalCancelCallback cancel,
                InternalUpdateProgressCallback updateProgress)
            {
                DisplayLogin = Marshal.GetFunctionPointerForDelegate(displayLogin);
                DisplayQuestion = Marshal.GetFunctionPointerForDelegate(displayQuestion);
                DisplayProgress = Marshal.GetFunctionPointerForDelegate(displayProgress);
                Cancel = Marshal.GetFunctionPointerForDelegate(cancel);
                UpdateProgress = Marshal.GetFunctionPointerForDelegate(updateProgress);
            }

            internal readonly IntPtr DisplayLogin;

            internal readonly IntPtr DisplayQuestion;

            internal readonly IntPtr DisplayProgress;

            internal readonly IntPtr Cancel;

            internal readonly IntPtr UpdateProgress;
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


    /// <summary>
    /// Registers a callback for the LibVLC exit event.
    /// This is mostly useful if the VLC playlist and/or at least one interface are started with libvlc_playlist_play()
    /// or AddInterface() respectively. Typically, this function will wake up your application main loop (from another thread).
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ExitCallback();
    #endregion
}
