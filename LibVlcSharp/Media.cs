using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace VideoLAN.LibVLC
{
    public partial class Media : IDisposable
    {
        static readonly ConcurrentDictionary<IntPtr, StreamData> DicStreams = new ConcurrentDictionary<IntPtr, StreamData>();
        static int _streamIndex;

        internal struct Internal
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_location")]
            internal static extern IntPtr LibVLCMediaNewLocation(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string mrl);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_path")]
            internal static extern IntPtr LibVLCMediaNewPath(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string path);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_as_node")]
            internal static extern IntPtr LibVLCMediaNewAsNode(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string name);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_fd")]
            internal static extern IntPtr LibVLCMediaNewFd(IntPtr instance, int fd);

            /// <summary>
            /// <para>Decrement the reference count of a media descriptor object. If the</para>
            /// <para>reference count is 0, then libvlc_media_release() will release the</para>
            /// <para>media descriptor object. It will send out an libvlc_MediaFreed event</para>
            /// <para>to all listeners. If the media descriptor object has been released it</para>
            /// <para>should not be used again.</para>
            /// </summary>
            /// <param name="media">the media descriptor</param>
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_release")]
            internal static extern void LibVLCMediaRelease(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_media")]
            internal static extern IntPtr LibVLCMediaListMedia(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_callbacks")]
            internal static extern IntPtr LibVLCMediaNewCallbacks(IntPtr instance, IntPtr openCb, IntPtr readCb, IntPtr seekCb, IntPtr closeCb, IntPtr opaque);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_add_option")]
            internal static extern void LibVLCMediaAddOption(IntPtr media, [MarshalAs(UnmanagedType.LPStr)] string options);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_add_option_flag")]
            internal static extern void LibVLCMediaAddOptionFlag(IntPtr media, [MarshalAs(UnmanagedType.LPStr)] string options, uint flags);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_mrl")]
            internal static extern IntPtr LibVLCMediaGetMrl(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_duplicate")]
            internal static extern IntPtr LibVLCMediaDuplicate(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_meta")]
            internal static extern IntPtr LibVLCMediaGetMeta(IntPtr media, MetadataType metadataType);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_set_meta")]
            internal static extern void LibVLCMediaSetMeta(IntPtr media, MetadataType metadataType, [MarshalAs(UnmanagedType.LPStr)] string value);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_save_meta")]
            internal static extern int LibVLCMediaSaveMeta(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_state")]
            internal static extern VLCState LibVLCMediaGetState(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_event_manager")]
            internal static extern IntPtr LibVLCMediaEventManager(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_stats")]
            internal static extern int LibVLCMediaGetStats(IntPtr media, out MediaStats statistics);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_duration")]
            internal static extern long LibVLCMediaGetDuration(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_parse")]
            internal static extern void LibVLCMediaParse(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_parse_async")]
            internal static extern void LibVLCMediaParseAsync(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_is_parsed")]
            internal static extern int LibVLCMediaIsParsed(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_parse_with_options")]
            internal static extern int LibVLCMediaParseWithOptions(IntPtr media, MediaParseOptions mediaParseOptions, int timeout);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_parsed_status")]
            internal static extern MediaParsedStatus LibVLCMediaGetParsedStatus(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_parse_stop")]
            internal static extern void LibVLCMediaParseStop(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_set_user_data")]
            internal static extern void LibVLCMediaSetUserData(IntPtr media, IntPtr userData);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_user_data")]
            internal static extern IntPtr LibVLCMediaGetUserData(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_tracks_get")]
            internal static extern uint LibVLCMediaTracksGet(IntPtr media, ref IntPtr tracksPtr);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_tracks_release")]
            internal static extern void LibVLCMediaTracksRelease(IntPtr tracks, uint count);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_subitems")]
            internal static extern IntPtr LibVLCMediaSubitems(IntPtr media);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_type")]
            internal static extern MediaType LibVLCMediaGetType(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_slaves_add")]
            internal static extern int LibVLCMediaAddSlaves(IntPtr media, MediaSlaveType slaveType, uint priority, [MarshalAs(UnmanagedType.LPStr)] string uri);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_slaves_clear")]
            internal static extern void LibVLCMediaClearSlaves(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_slaves_get")]
            internal static extern uint LibVLCMediaGetSlaves(IntPtr media, ref IntPtr slaves);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_slaves_release")]
            internal static extern void LibVLCMediaReleaseSlaves(IntPtr slaves, uint count);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_retain")]
            internal static extern void LibVLCMediaRetain(IntPtr media);
        }

        #region enums
        /// <summary>Meta data types</summary>
        public enum MetadataType
        {
            Title = 0,
            Artist = 1,
            Genre = 2,
            Copyright = 3,
            Album = 4,
            TrackNumber = 5,
            Description = 6,
            Rating = 7,
            Date = 8,
            Setting = 9,
            URL = 10,
            Language = 11,
            NowPlaying = 12,
            Publisher = 13,
            EncodedBy = 14,
            ArtworkURL = 15,
            TrackID = 16,
            TrackTotal = 17,
            Director = 18,
            Season = 19,
            Episode = 20,
            ShowName = 21,
            Actors = 22,
            AlbumArtist = 23,
            DiscNumber = 24,
            DiscTotal = 25
        }

        public enum FromType
        {
            /// <summary>
            /// Create a media for a certain file path.
            /// </summary>
            FromPath,
            /// <summary>
            /// Create a media with a certain given media resource location,
            /// for instance a valid URL.
            /// note To refer to a local file with this function,
            /// the file://... URI syntax <b>must</b> be used (see IETF RFC3986).
            /// We recommend using FromPath instead when dealing with
            ///local files.
            /// </summary>
            FromLocation,
            /// <summary>
            /// Create a media as an empty node with a given name.
            /// </summary
            AsNode
        }

        /// <summary>
        /// <summary>Parse flags used by libvlc_media_parse_with_options()</summary>
        /// <remarks>libvlc_media_parse_with_options</remarks>
        [Flags]
        public enum MediaParseOptions
        {
            /// <summary>Parse media if it's a local file</summary>
            ParseLocal = 0,
            /// <summary>Parse media even if it's a network file</summary>
            ParseNetwork = 1,
            /// <summary>Fetch meta and covert art using local resources</summary>
            FetchLocal = 2,
            /// <summary>Fetch meta and covert art using network resources</summary>
            FetchNetwork = 4,
            /// <summary>
            /// Interact with the user (via libvlc_dialog_cbs) when preparsing this item
            /// (and not its sub items). Set this flag in order to receive a callback
            /// when the input is asking for credentials.
            /// </summary>
            DoInteract = 8
        }

        /// Parse status used sent by libvlc_media_parse_with_options() or returned by
        /// libvlc_media_get_parsed_status()
        /// </summary>
        /// <remarks>
        /// libvlc_media_parse_with_options
        /// libvlc_media_get_parsed_status
        /// </remarks>
        public enum MediaParsedStatus
        {
            Skipped = 1,
            Failed = 2,
            Timeout = 3,
            Done = 4
        }

        /// <summary>Media type</summary>
        /// <remarks>libvlc_media_get_type</remarks>
        public enum MediaType
        {
            Unknown = 0,
            File = 1,
            Directory = 2,
            Disc = 3,
            Stream = 4,
            Playlist = 5
        }

        #endregion

        public IntPtr NativeReference = IntPtr.Zero;

        /// <summary>
        /// Media Constructs a libvlc Media instance
        /// </summary>
        /// <param name="instance">A libvlc instance</param>
        /// <param name="mrl">A path, location, or node name, depending on the 3rd parameter</param>
        /// <param name="type">The type of the 2nd argument. \sa{FromType}</param>
        public Media(Instance instance, string mrl, FromType type)
        {
            if(instance == null) throw new ArgumentNullException(nameof(instance));
            if(string.IsNullOrEmpty(mrl)) throw new ArgumentNullException(nameof(mrl));

            switch (type)
            {
                case FromType.FromLocation:
                    NativeReference = Internal.LibVLCMediaNewLocation(instance.NativeReference, mrl);
                    break;
                case FromType.FromPath:
                    NativeReference = Internal.LibVLCMediaNewPath(instance.NativeReference, mrl);
                    break;
                case FromType.AsNode:
                    NativeReference = Internal.LibVLCMediaNewAsNode(instance.NativeReference, mrl);
                    break;
            }

            if(NativeReference == IntPtr.Zero)
                throw new ArgumentException($"Failed to construct media for {mrl} of type {type}");
        }

        /// <summary>
        /// Create a media for an already open file descriptor.
        /// The file descriptor shall be open for reading(or reading and writing).
        ///
        /// Regular file descriptors, pipe read descriptors and character device
        /// descriptors(including TTYs) are supported on all platforms.
        /// Block device descriptors are supported where available.
        /// Directory descriptors are supported on systems that provide fdopendir().
        /// Sockets are supported on all platforms where they are file descriptors,
        /// i.e.all except Windows.
        /// 
        /// \note This library will <b>not</b> automatically close the file descriptor
        /// under any circumstance.Nevertheless, a file descriptor can usually only be
        /// rendered once in a media player.To render it a second time, the file
        /// descriptor should probably be rewound to the beginning with lseek().
        /// </summary>
        /// <param name="instance">A libvlc instance</param>
        /// <param name="fd">open file descriptor</param>
        public Media(Instance instance, int fd)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            NativeReference = Internal.LibVLCMediaNewFd(instance.NativeReference, fd);

            if (NativeReference == IntPtr.Zero)
                throw new ArgumentException($"Failed to construct media with {fd}");
        }

        public Media(MediaList mediaList)
        {
            if (mediaList == null) throw new ArgumentNullException(nameof(mediaList));

            //NativeReference = Internal.LibVLCMediaListMedia(mediaList.__Instance);

            if(NativeReference == IntPtr.Zero)
                throw new ArgumentException($"Failed to construct media with {mediaList}");
        }

        /// <summary>
        /// requires libvlc 3.0 or higher
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        [LibVLC(3)]
        public Media(Instance instance, Stream stream, params string[] options)
        {
            if(instance == null) throw new ArgumentNullException(nameof(instance));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var opaque = AddStream(stream);

            if (opaque == IntPtr.Zero)
                throw new InvalidOperationException("Cannot create opaque parameter");

            NativeReference = Internal.LibVLCMediaNewCallbacks(instance.NativeReference, 
                Marshal.GetFunctionPointerForDelegate(new OpenMedia(CallbackOpenMedia)), 
                Marshal.GetFunctionPointerForDelegate(new ReadMedia(CallbackReadMedia)), 
                Marshal.GetFunctionPointerForDelegate(new SeekMedia(CallbackSeekMedia)), 
                Marshal.GetFunctionPointerForDelegate(new CloseMedia(CallbackCloseMedia)),
                opaque);
            
            if(options.Any())
                Internal.LibVLCMediaAddOption(NativeReference, options.ToString());

            if (NativeReference == IntPtr.Zero)
                throw new ArgumentException($"Failed to construct media with {instance}, {stream}");
        }

        public Media(IntPtr mediaPtr)
        {
            if(mediaPtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(mediaPtr));

            NativeReference = mediaPtr;
        }

        public Media() { }

        /// <summary>Add an option to the media.</summary>
        /// <param name="options">the options (as a string)</param>
        /// <remarks>
        /// <para>This option will be used to determine how the media_player will</para>
        /// <para>read the media. This allows to use VLC's advanced</para>
        /// <para>reading/streaming options on a per-media basis.</para>
        /// <para>The options are listed in 'vlc --long-help' from the command line,</para>
        /// <para>e.g. &quot;-sout-all&quot;. Keep in mind that available options and their semantics</para>
        /// <para>vary across LibVLC versions and builds.</para>
        /// <para>Not all options affects libvlc_media_t objects:</para>
        /// <para>Specifically, due to architectural issues most audio and video options,</para>
        /// <para>such as text renderer options, have no effects on an individual media.</para>
        /// <para>These options must be set through libvlc_new() instead.</para>
        /// </remarks>
        public void AddOption(string options)
        {
            if(string.IsNullOrEmpty(options)) throw new ArgumentNullException(nameof(options));

            Internal.LibVLCMediaAddOption(NativeReference, options);
        }

        /// <summary>Add an option to the media with configurable flags.</summary>
        /// <param name="options">the options (as a string)</param>
        /// <param name="flags">the flags for this option</param>
        /// <remarks>
        /// <para>This option will be used to determine how the media_player will</para>
        /// <para>read the media. This allows to use VLC's advanced</para>
        /// <para>reading/streaming options on a per-media basis.</para>
        /// <para>The options are detailed in vlc --long-help, for instance</para>
        /// <para>&quot;--sout-all&quot;. Note that all options are not usable on medias:</para>
        /// <para>specifically, due to architectural issues, video-related options</para>
        /// <para>such as text renderer options cannot be set on a single media. They</para>
        /// <para>must be set on the whole libvlc instance instead.</para>
        /// </remarks>
        public void AddOptionFlag(string options, uint flags)
        {
            if (string.IsNullOrEmpty(options)) throw new ArgumentNullException(nameof(options));

            Internal.LibVLCMediaAddOptionFlag(NativeReference, options, flags);
        }

        string _mrl;
        /// <summary>Get the media resource locator (mrl) from a media descriptor object</summary>
        public string Mrl
        {
            get
            {
                if (string.IsNullOrEmpty(_mrl))
                {
                    var mrlPtr = Internal.LibVLCMediaGetMrl(NativeReference);
                    _mrl = Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(mrlPtr) as string;
                }
                return _mrl;
            }
        }

        /// <summary>Duplicate a media descriptor object.</summary>
        public Media Duplicate()
        {
            var duplicatePtr = Internal.LibVLCMediaDuplicate(NativeReference);
            if(duplicatePtr == IntPtr.Zero) throw new Exception("Failure to duplicate");
            return new Media(duplicatePtr);
        }

        /// <summary>Read the meta of the media.</summary>
        /// <param name="metadataType">the meta to read</param>
        /// <returns>the media's meta</returns>
        /// <remarks>
        /// If the media has not yet been parsed this will return NULL.
        /// </remarks>
        public string Meta(MetadataType metadataType)
        {
            var metaPtr = Internal.LibVLCMediaGetMeta(NativeReference, metadataType);
            if (metaPtr == IntPtr.Zero) return string.Empty;
            return Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(metaPtr) as string;
        }

        /// <summary>
        /// <para>Set the meta of the media (this function will not save the meta, call</para>
        /// <para>libvlc_media_save_meta in order to save the meta)</para>
        /// </summary>
        /// <param name="metadataType">the <see cref="MetadataType"/>  to write</param>
        /// <param name="value">the media's meta</param>
        public void SetMeta(MetadataType metadataType, string value)
        {
            if(string.IsNullOrEmpty(value)) throw new ArgumentNullException(value);

            Internal.LibVLCMediaSetMeta(NativeReference, metadataType, value);
        }

        /// <summary>Save the meta previously set</summary>
        /// <returns>true if the write operation was successful</returns>
        public bool SaveMeta()
        {
            var r = Internal.LibVLCMediaSaveMeta(NativeReference);
            return r != 0;
        }

        /// <summary>
        /// Get current <see cref="VLCState"/> of media descriptor object.
        /// </summary>
        public VLCState State => Internal.LibVLCMediaGetState(NativeReference);

        /// <summary>Get the current statistics about the media
        /// structure that contain the statistics about the media
        /// </summary>
        public MediaStats Statistics
        {
            get
            {
                Internal.LibVLCMediaGetStats(NativeReference, out var stats);
                return stats;
            }
        }

        MediaEventManager _eventManager;
        /// <summary>
        /// <para>Get event manager from media descriptor object.</para>
        /// <para>NOTE: this function doesn't increment reference counting.</para>
        /// </summary>
        /// <returns>event manager object</returns>
        public MediaEventManager EventManager
        {
            get
            {
                if (_eventManager != null) return _eventManager;
                var eventManagerPtr = Internal.LibVLCMediaEventManager(NativeReference);
                _eventManager = new MediaEventManager(eventManagerPtr);
                return _eventManager;
            }
        }

        /// <summary>Get duration (in ms) of media descriptor object item.</summary>
        /// <returns>duration of media item or -1 on error</returns>
        public long Duration => Internal.LibVLCMediaGetDuration(NativeReference);

        /// <summary>Parse a media.
        /// This fetches (local) art, meta data and tracks information.
        /// The method is synchronous.
        /// This function could block indefinitely.
        /// Use libvlc_media_parse_with_options() instead
        /// libvlc_media_parse_with_options
        /// libvlc_media_get_meta
        /// libvlc_media_get_tracks_info
        /// </summary>
        public void Parse()
        {
            Internal.LibVLCMediaParse(NativeReference);
        }

        /// <summary>Parse a media.
        /// This fetches (local) art, meta data and tracks information.
        /// The method is the asynchronous of libvlc_media_parse().
        /// To track when this is over you can listen to libvlc_MediaParsedChanged
        /// event. However if the media was already parsed you will not receive this
        /// event.
        /// You can't be sure to receive the libvlc_MediaParsedChanged
        /// event (you can wait indefinitely for this event).
        /// Use libvlc_media_parse_with_options() instead
        /// libvlc_media_parse
        /// libvlc_MediaParsedChanged
        /// libvlc_media_get_meta
        /// libvlc_media_get_tracks_info
        /// </summary>
        /// TODO: return task by checking libvlc_MediaParsedChanged
        public Task ParseAsync()
        {
            Internal.LibVLCMediaParseAsync(NativeReference);
            return Task.CompletedTask;
        }

        /// <summary>Return true is the media descriptor object is parsed</summary>
        /// <returns>true if media object has been parsed otherwise it returns false</returns>
        public bool IsParsed => Internal.LibVLCMediaIsParsed(NativeReference) != 0;

        /// <summary>Parse the media asynchronously with options.</summary>
        /// <param name="parseOptions">parse options</param>
        /// <param name="timeout">
        /// <para>maximum time allowed to preparse the media. If -1, the</para>
        /// <para>default &quot;preparse-timeout&quot; option will be used as a timeout. If 0, it will</para>
        /// <para>wait indefinitely. If &gt; 0, the timeout will be used (in milliseconds).</para>
        /// </param>
        /// <returns>-1 in case of error, 0 otherwise</returns>
        /// <remarks>
        /// <para>This fetches (local or network) art, meta data and/or tracks information.</para>
        /// <para>This method is the extended version of libvlc_media_parse_with_options().</para>
        /// <para>To track when this is over you can listen to libvlc_MediaParsedChanged</para>
        /// <para>event. However if this functions returns an error, you will not receive any</para>
        /// <para>events.</para>
        /// <para>It uses a flag to specify parse options (see libvlc_media_parse_flag_t). All</para>
        /// <para>these flags can be combined. By default, media is parsed if it's a local</para>
        /// <para>file.</para>
        /// <para>Parsing can be aborted with libvlc_media_parse_stop().</para>
        /// <para>libvlc_MediaParsedChanged</para>
        /// <para>libvlc_media_get_meta</para>
        /// <para>libvlc_media_tracks_get</para>
        /// <para>libvlc_media_get_parsed_status</para>
        /// <para>libvlc_media_parse_flag_t</para>
        /// <para>LibVLC 3.0.0 or later</para>
        /// </remarks>
        public bool ParseWithOptions(MediaParseOptions parseOptions, int timeout = -1)
        {
            return Internal.LibVLCMediaParseWithOptions(NativeReference, parseOptions, timeout) != 0;
        }

        /// <summary>Get Parsed status for media descriptor object.</summary>
        /// <returns>a value of the libvlc_media_parsed_status_t enum</returns>
        /// <remarks>
        /// <para>libvlc_MediaParsedChanged</para>
        /// <para>libvlc_media_parsed_status_t</para>
        /// <para>LibVLC 3.0.0 or later</para>
        /// </remarks>
        public MediaParsedStatus ParsedStatus => Internal.LibVLCMediaGetParsedStatus(NativeReference);

        /// <summary>Stop the parsing of the media</summary>
        /// <remarks>
        /// <para>When the media parsing is stopped, the libvlc_MediaParsedChanged event will</para>
        /// <para>be sent with the libvlc_media_parsed_status_timeout status.</para>
        /// <para>libvlc_media_parse_with_options</para>
        /// <para>LibVLC 3.0.0 or later</para>
        /// </remarks>
        public void ParseStop()
        {
            Internal.LibVLCMediaParseStop(NativeReference);
        }

        // TODO: Whats userData?
        //public IntPtr UserData
        //{
        //    get => Native.LibVLCMediaGetUserData(NativeReference);
        //    set => Native.LibVLCMediaSetUserData(NativeReference, IntPtr.Zero);
        //}

        /// <summary>Get media descriptor's elementary streams description</summary>
        /// <para>address to store an allocated array of Elementary Streams</para>
        /// <para>descriptions (must be freed with libvlc_media_tracks_release</para>
        /// <para>by the caller) [OUT]</para>
        /// <returns>the number of Elementary Streams (zero on error)</returns>
        /// <remarks>
        /// <para>Note, you need to call libvlc_media_parse() or play the media at least once</para>
        /// <para>before calling this function.</para>
        /// <para>Not doing this will result in an empty array.</para>
        /// <para>LibVLC 2.1.0 and later.</para>
        /// </remarks>
        public IEnumerable<MediaTrack> Tracks
        {
            get
            {
                var arrayResultPtr = IntPtr.Zero;
                var count = Internal.LibVLCMediaTracksGet(NativeReference, ref arrayResultPtr);
                if (count == 0 || arrayResultPtr == IntPtr.Zero) return Enumerable.Empty<MediaTrack>();

                var tracks = new List<MediaTrack>();
                for (var i = 0; i < count; i++)
                {
                    var ptr = Marshal.ReadIntPtr(arrayResultPtr, i * IntPtr.Size);
                    var managedStruct = Marshal.PtrToStructure<MediaTrack>(ptr);
                    tracks.Add(managedStruct);

                }

                Internal.LibVLCMediaTracksRelease(arrayResultPtr, count);

                return tracks;
            }
        }

        /// <summary>
        /// <para>Get subitems of media descriptor object. This will increment</para>
        /// <para>the reference count of supplied media descriptor object. Use</para>
        /// <para>libvlc_media_list_release() to decrement the reference counting.</para>
        /// </summary>
        /// <returns>list of media descriptor subitems or NULL</returns>
        public MediaList SubItems
        {
            get
            {
                var ptr = Internal.LibVLCMediaSubitems(NativeReference);
                return new MediaList(ptr);
            }
        }

        public MediaType Type => Internal.LibVLCMediaGetType(NativeReference);

        /// <summary>Add a slave to the current media.</summary>
        /// <param name="type">subtitle or audio</param>
        /// <param name="priority">from 0 (low priority) to 4 (high priority)</param>
        /// <param name="uri">Uri of the slave (should contain a valid scheme).</param>
        /// <returns>0 on success, -1 on error.</returns>
        /// <remarks>
        /// <para>A slave is an external input source that may contains an additional subtitle</para>
        /// <para>track (like a .srt) or an additional audio track (like a .ac3).</para>
        /// <para>This function must be called before the media is parsed (via</para>
        /// <para>libvlc_media_parse_with_options()) or before the media is played (via</para>
        /// <para>libvlc_media_player_play())</para>
        /// <para>LibVLC 3.0.0 and later.</para>
        /// </remarks>
        public bool AddSlave(MediaSlaveType type, uint priority, string uri)
        {
            return Internal.LibVLCMediaAddSlaves(NativeReference, type, priority, uri) != 0;
        }

        /// <summary>
        /// <para>Clear all slaves previously added by libvlc_media_slaves_add() or</para>
        /// <para>internally.</para>
        /// </summary>
        /// <remarks>LibVLC 3.0.0 and later.</remarks>
        public void ClearSlaves()
        {
            Internal.LibVLCMediaClearSlaves(NativeReference);    
        }

        /// <summary>Get a media descriptor's slave list</summary>
        /// <para>address to store an allocated array of slaves (must be</para>
        /// <para>freed with libvlc_media_slaves_release()) [OUT]</para>
        /// <returns>the number of slaves (zero on error)</returns>
        /// <remarks>
        /// <para>The list will contain slaves parsed by VLC or previously added by</para>
        /// <para>libvlc_media_slaves_add(). The typical use case of this function is to save</para>
        /// <para>a list of slave in a database for a later use.</para>
        /// <para>LibVLC 3.0.0 and later.</para>
        /// <para>libvlc_media_slaves_add</para>
        /// </remarks>
        public IEnumerable<MediaSlave> Slaves
        {
            get
            {
                var slaveArrayPtr = IntPtr.Zero;
                var count = Internal.LibVLCMediaGetSlaves(NativeReference, ref slaveArrayPtr);
                if (count == 0) return Enumerable.Empty<MediaSlave>();

                var slaves = new List<MediaSlave>();
                for (var i = 0; i < count; i++)
                {
                    var ptr = Marshal.ReadIntPtr(slaveArrayPtr, i * IntPtr.Size);
                    var managedStruct = Marshal.PtrToStructure<MediaSlave>(ptr);
                    slaves.Add(managedStruct);
                }
                Internal.LibVLCMediaReleaseSlaves(slaveArrayPtr, count);
                return slaves;
            }
        }

        public void Dispose()
        {
            if (NativeReference == IntPtr.Zero)
                return;

            Internal.LibVLCMediaRelease(NativeReference);

            NativeReference = IntPtr.Zero;
        }

        public override bool Equals(object obj)
        {
            return obj is Media media &&
                   EqualityComparer<IntPtr>.Default.Equals(NativeReference, media.NativeReference);
        }

        internal class StreamData
        {
            public IntPtr Handle { get; set; }
            public Stream Stream { get; set; }
            public byte[] Buffer { get; set; }
        }

        #region private

        static int CallbackOpenMedia(IntPtr opaque, ref IntPtr data, out ulong size)
        {
            data = opaque;

            try
            {
                var streamData = GetStream(opaque);

                try
                {
                    size = (ulong)streamData.Stream.Length;
                }
                catch (Exception)
                {
                    // byte length of the bitstream or UINT64_MAX if unknown
                    size = ulong.MaxValue;
                }

                if (streamData.Stream.CanSeek)
                {
                    streamData.Stream.Seek(0L, SeekOrigin.Begin);
                }

                return 0;
            }
            catch (Exception)
            {
                size = 0UL;
                return -1;
            }
        }

        static int CallbackReadMedia(IntPtr opaque, IntPtr buf, uint len)
        {
            try
            {
                var streamData = GetStream(opaque);
                int read;

                lock (streamData)
                {
                    var canRead = Math.Min((int)len, streamData.Buffer.Length);
                    read = streamData.Stream.Read(streamData.Buffer, 0, canRead);
                    Marshal.Copy(streamData.Buffer, 0, buf, read);
                }

                return read;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        static int CallbackSeekMedia(IntPtr opaque, UInt64 offset)
        {
            try
            {
                var streamData = GetStream(opaque);
                streamData.Stream.Seek((long)offset, SeekOrigin.Begin);
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        static void CallbackCloseMedia(IntPtr opaque)
        {
            try
            {
                RemoveStream(opaque);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        static IntPtr AddStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            IntPtr handle;

            lock (DicStreams)
            {
                _streamIndex++;
            

                handle = new IntPtr(_streamIndex);
                DicStreams[handle] = new StreamData
                {
                    Buffer = new byte[0x4000],
                    Handle = handle,
                    Stream = stream
                };
            }
            return handle;
        }

        static StreamData GetStream(IntPtr handle)
        {
            return !DicStreams.TryGetValue(handle, out var result) ? null : result;
        }

        static void RemoveStream(IntPtr handle)
        { 
            DicStreams.TryRemove(handle, out var result);
        }

        void Retain()
        {
            if(NativeReference != IntPtr.Zero)
                Internal.LibVLCMediaRetain(NativeReference);
        }

        #endregion
    }

    #region Callbacks

    /// <summary>
    /// <para>It consists of a media location and various optional meta data.</para>
    /// <para>@{</para>
    /// <para></para>
    /// <para>LibVLC media item/descriptor external API</para>
    /// </summary>
    /// <summary>Callback prototype to open a custom bitstream input media.</summary>
    /// <param name="opaque">private pointer as passed to libvlc_media_new_callbacks()</param>
    /// <param name="data">storage space for a private data pointer [OUT]</param>
    /// <param name="size">byte length of the bitstream or UINT64_MAX if unknown [OUT]</param>
    /// <returns>
    /// <para>0 on success, non-zero on error. In case of failure, the other</para>
    /// <para>callbacks will not be invoked and any value stored in *datap and *sizep is</para>
    /// <para>discarded.</para>
    /// </returns>
    /// <remarks>
    /// <para>The same media item can be opened multiple times. Each time, this callback</para>
    /// <para>is invoked. It should allocate and initialize any instance-specific</para>
    /// <para>resources, then store them in *datap. The instance resources can be freed</para>
    /// <para>in the</para>
    /// <para>For convenience, *datap is initially NULL and *sizep is initially 0.</para>
    /// </remarks>
    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int OpenMedia(IntPtr opaque, ref IntPtr data, out ulong size);

    /// <summary>Callback prototype to read data from a custom bitstream input media.</summary>
    /// <param name="opaque">private pointer as set by the</param>
    /// <param name="buf">start address of the buffer to read data into</param>
    /// <param name="len">bytes length of the buffer</param>
    /// <returns>
    /// <para>strictly positive number of bytes read, 0 on end-of-stream,</para>
    /// <para>or -1 on non-recoverable error</para>
    /// </returns>
    /// <remarks>
    /// <para>callback</para>
    /// <para>If no data is immediately available, then the callback should sleep.</para>
    /// <para>The application is responsible for avoiding deadlock situations.</para>
    /// <para>In particular, the callback should return an error if playback is stopped;</para>
    /// <para>if it does not return, then libvlc_media_player_stop() will never return.</para>
    /// </remarks>
    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int ReadMedia(IntPtr opaque, IntPtr buf, uint len);

    /// <summary>Callback prototype to seek a custom bitstream input media.</summary>
    /// <param name="opaque">private pointer as set by the</param>
    /// <param name="offset">absolute byte offset to seek to</param>
    /// <returns>0 on success, -1 on error.</returns>
    /// <remarks>callback</remarks>
    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int SeekMedia(IntPtr opaque, ulong offset);

    /// <summary>Callback prototype to close a custom bitstream input media.</summary>
    /// <param name="opaque">private pointer as set by the</param>
    /// <remarks>callback</remarks>
    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CloseMedia(IntPtr opaque);
    
    #endregion

    #region Structs

    [StructLayout(LayoutKind.Sequential)]
    public struct MediaStats
    {
        /* Input */
        public int ReadBytes;
        public float InputBitrate;

        /* Demux */
        public int DemuxReadBytes;
        public float DemuxBitrate;
        public int DemuxCorrupted;
        public int DemuxDiscontinuity;

        /* Decoders */
        public int DecodedVideo;
        public int DecodedAudio;


        /* Video Output */
        public int DisplayedPictures;
        public int LostPictures;

        /* Audio output */
        public int PlayedAudioBuffers;
        public int LostAudioBuffers;

        /* Stream output */
        public int SentPackets;
        public int SentBytes;
        public float SendBitrate;
    }

    public struct MediaTrack
    {
        public uint Codec;
        public uint OriginalFourcc;
        public int Id;
        public TrackType TrackType;
        public int Profile;
        public int Level;
        public MediaTrackData Data;
        public uint Bitrate;
        public IntPtr Language;
        public IntPtr Description;
    }

    public struct MediaTrackData
    {
        public AudioTrack Audio;
        public VideoTrack Video;
        public SubtitleTrack Subtitle;
    }

    #endregion
}
