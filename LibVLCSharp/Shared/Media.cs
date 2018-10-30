using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace LibVLCSharp.Shared
{
    public class Media : Internal
    {
        static readonly ConcurrentDictionary<IntPtr, StreamData> DicStreams = new ConcurrentDictionary<IntPtr, StreamData>();
        static int _streamIndex;

        internal struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_location")]
            internal static extern IntPtr LibVLCMediaNewLocation(IntPtr libVLC, IntPtr mrl);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_path")]
            internal static extern IntPtr LibVLCMediaNewPath(IntPtr libVLC, IntPtr path);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_as_node")]
            internal static extern IntPtr LibVLCMediaNewAsNode(IntPtr libVLC, IntPtr name);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_fd")]
            internal static extern IntPtr LibVLCMediaNewFd(IntPtr libVLC, int fd);

            /// <summary>
            /// <para>Decrement the reference count of a media descriptor object. If the</para>
            /// <para>reference count is 0, then libvlc_media_release() will release the</para>
            /// <para>media descriptor object. It will send out an libvlc_MediaFreed event</para>
            /// <para>to all listeners. If the media descriptor object has been released it</para>
            /// <para>should not be used again.</para>
            /// </summary>
            /// <param name="media">the media descriptor</param>
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_release")]
            internal static extern void LibVLCMediaRelease(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_media")]
            internal static extern IntPtr LibVLCMediaListMedia(IntPtr mediaList);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_new_callbacks")]
            internal static extern IntPtr LibVLCMediaNewCallbacks(IntPtr libVLC, IntPtr openCb, IntPtr readCb, IntPtr seekCb, IntPtr closeCb, IntPtr opaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_add_option")]
            internal static extern void LibVLCMediaAddOption(IntPtr media, [MarshalAs(UnmanagedType.LPStr)] string options);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_add_option_flag")]
            internal static extern void LibVLCMediaAddOptionFlag(IntPtr media, [MarshalAs(UnmanagedType.LPStr)] string options, uint flags);
            
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_mrl")]
            internal static extern IntPtr LibVLCMediaGetMrl(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_duplicate")]
            internal static extern IntPtr LibVLCMediaDuplicate(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_meta")]
            internal static extern IntPtr LibVLCMediaGetMeta(IntPtr media, MetadataType metadataType);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_set_meta")]
            internal static extern void LibVLCMediaSetMeta(IntPtr media, MetadataType metadataType, [MarshalAs(UnmanagedType.LPStr)] string value);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_save_meta")]
            internal static extern int LibVLCMediaSaveMeta(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_state")]
            internal static extern VLCState LibVLCMediaGetState(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_event_manager")]
            internal static extern IntPtr LibVLCMediaEventManager(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_stats")]
            internal static extern int LibVLCMediaGetStats(IntPtr media, out MediaStats statistics);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_duration")]
            internal static extern long LibVLCMediaGetDuration(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_parse")]
            internal static extern void LibVLCMediaParse(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_parse_async")]
            internal static extern void LibVLCMediaParseAsync(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_is_parsed")]
            internal static extern int LibVLCMediaIsParsed(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_parse_with_options")]
            internal static extern int LibVLCMediaParseWithOptions(IntPtr media, MediaParseOptions mediaParseOptions, int timeout);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_parsed_status")]
            internal static extern MediaParsedStatus LibVLCMediaGetParsedStatus(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_parse_stop")]
            internal static extern void LibVLCMediaParseStop(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_set_user_data")]
            internal static extern void LibVLCMediaSetUserData(IntPtr media, IntPtr userData);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_user_data")]
            internal static extern IntPtr LibVLCMediaGetUserData(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_tracks_get")]
            internal static extern uint LibVLCMediaTracksGet(IntPtr media, ref IntPtr tracksPtr);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_tracks_release")]
            internal static extern void LibVLCMediaTracksRelease(IntPtr tracks, uint count);
            
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_subitems")]
            internal static extern IntPtr LibVLCMediaSubitems(IntPtr media);
            
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_get_type")]
            internal static extern MediaType LibVLCMediaGetType(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_slaves_add")]
            internal static extern int LibVLCMediaAddSlaves(IntPtr media, MediaSlaveType slaveType, uint priority, [MarshalAs(UnmanagedType.LPStr)] string uri);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_slaves_clear")]
            internal static extern void LibVLCMediaClearSlaves(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_slaves_get")]
            internal static extern uint LibVLCMediaGetSlaves(IntPtr media, ref IntPtr slaves);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_slaves_release")]
            internal static extern void LibVLCMediaReleaseSlaves(IntPtr slaves, uint count);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_retain")]
            internal static extern void LibVLCMediaRetain(IntPtr media);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                            EntryPoint = "libvlc_media_get_codec_description")]
            internal static extern string LibvlcMediaGetCodecDescription(TrackType type, uint codec);
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
            /// </summary>
            AsNode
        }

        /// <summary>
        /// Parse flags used by libvlc_media_parse_with_options()
        /// </summary>
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

        /// <summary>
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

        /// <summary>
        /// Media Constructs a libvlc Media instance
        /// </summary>
        /// <param name="libVLC">A libvlc instance</param>
        /// <param name="mrl">A path, location, or node name, depending on the 3rd parameter</param>
        /// <param name="type">The type of the 2nd argument. \sa{FromType}</param>
        public Media(LibVLC libVLC, string mrl, FromType type = FromType.FromPath)
            : base(() => SelectNativeCtor(libVLC, mrl, type), Native.LibVLCMediaRelease)
        {
        }

        static IntPtr SelectNativeCtor(LibVLC libVLC, string mrl, FromType type)
        {
            if (libVLC == null) throw new ArgumentNullException(nameof(libVLC));
            if (string.IsNullOrEmpty(mrl)) throw new ArgumentNullException(nameof(mrl));

            var mrlPtr = Utf8StringMarshaler.GetInstance().MarshalManagedToNative(mrl);
            if (mrlPtr == IntPtr.Zero)
                throw new ArgumentException($"error marshalling {mrl} to UTF-8 for native interop");

            switch (type)
            {
                case FromType.FromLocation:
                    return Native.LibVLCMediaNewLocation(libVLC.NativeReference, mrlPtr);
                case FromType.FromPath:
                    return Native.LibVLCMediaNewPath(libVLC.NativeReference, mrlPtr);
                case FromType.AsNode:
                    return Native.LibVLCMediaNewAsNode(libVLC.NativeReference, mrlPtr);
                default:
                    return IntPtr.Zero;
            }
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
        /// <param name="libVLC">A libvlc instance</param>
        /// <param name="fd">open file descriptor</param>
        public Media(LibVLC libVLC, int fd)
            : base(() => Native.LibVLCMediaNewFd(libVLC.NativeReference, fd), Native.LibVLCMediaRelease)
        {
        }

        public Media(MediaList mediaList)
            : base(() => Native.LibVLCMediaListMedia(mediaList.NativeReference), Native.LibVLCMediaRelease)
        {
        }

        /// <summary>
        /// requires libvlc 3.0 or higher
        /// </summary>
        /// <param name="libVLC"></param>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        public Media(LibVLC libVLC, Stream stream, params string[] options)
            : base(() => CtorFromCallbacks(libVLC, stream), Native.LibVLCMediaRelease)
        {          
            if(options.Any())
                Native.LibVLCMediaAddOption(NativeReference, options.ToString());
        }

        static IntPtr CtorFromCallbacks(LibVLC libVLC, Stream stream)
        {
            if (libVLC == null) throw new ArgumentNullException(nameof(libVLC));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var opaque = AddStream(stream);

            if (opaque == IntPtr.Zero)
                throw new InvalidOperationException("Cannot create opaque parameter");

            return Native.LibVLCMediaNewCallbacks(libVLC.NativeReference,
                Marshal.GetFunctionPointerForDelegate(new OpenMedia(CallbackOpenMedia)),
                Marshal.GetFunctionPointerForDelegate(new ReadMedia(CallbackReadMedia)),
                Marshal.GetFunctionPointerForDelegate(new SeekMedia(CallbackSeekMedia)),
                Marshal.GetFunctionPointerForDelegate(new CloseMedia(CallbackCloseMedia)),
                opaque);
        }

        public Media(IntPtr mediaPtr)
            : base(() => mediaPtr, Native.LibVLCMediaRelease)
        {
        }

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

            Native.LibVLCMediaAddOption(NativeReference, options);
        }

        /// <summary>
        /// Convenience method for crossplatform media configuration
        /// </summary>
        /// <param name="mediaConfiguration">mediaConfiguration translate to strings parsed by the vlc engine, some are platform specific</param>
        public void AddOption(MediaConfiguration mediaConfiguration)
        {
            if (mediaConfiguration == null) throw new ArgumentNullException(nameof(mediaConfiguration));

            AddOption(mediaConfiguration.Build());
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

            Native.LibVLCMediaAddOptionFlag(NativeReference, options, flags);
        }

        string _mrl;
        /// <summary>Get the media resource locator (mrl) from a media descriptor object</summary>
        public string Mrl
        {
            get
            {
                if (string.IsNullOrEmpty(_mrl))
                {
                    var mrlPtr = Native.LibVLCMediaGetMrl(NativeReference);
                    _mrl = Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(mrlPtr) as string;
                }
                return _mrl;
            }
        }

        /// <summary>Duplicate a media descriptor object.</summary>
        public Media Duplicate()
        {
            var duplicatePtr = Native.LibVLCMediaDuplicate(NativeReference);
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
            var metaPtr = Native.LibVLCMediaGetMeta(NativeReference, metadataType);
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

            Native.LibVLCMediaSetMeta(NativeReference, metadataType, value);
        }

        /// <summary>Save the meta previously set</summary>
        /// <returns>true if the write operation was successful</returns>
        public bool SaveMeta() => Native.LibVLCMediaSaveMeta(NativeReference) != 0;

        /// <summary>
        /// Get current <see cref="VLCState"/> of media descriptor object.
        /// </summary>
        public VLCState State => Native.LibVLCMediaGetState(NativeReference);

        /// <summary>Get the current statistics about the media
        /// structure that contain the statistics about the media
        /// </summary>
        public MediaStats Statistics => Native.LibVLCMediaGetStats(NativeReference, out var mediaStats) == 0 
            ? default(MediaStats) : mediaStats;

        MediaEventManager _eventManager;
        /// <summary>
        /// <para>Get event manager from media descriptor object.</para>
        /// <para>NOTE: this function doesn't increment reference counting.</para>
        /// </summary>
        /// <returns>event manager object</returns>
        MediaEventManager EventManager
        {
            get
            {
                if (_eventManager != null) return _eventManager;
                var eventManagerPtr = Native.LibVLCMediaEventManager(NativeReference);
                _eventManager = new MediaEventManager(eventManagerPtr);
                return _eventManager;
            }
        }

        /// <summary>Get duration (in ms) of media descriptor object item.</summary>
        /// <returns>duration of media item or -1 on error</returns>
        public long Duration => Native.LibVLCMediaGetDuration(NativeReference);

        /// <summary>Parse a media.
        /// This fetches (local) art, meta data and tracks information.
        /// The method is synchronous.
        /// This function could block indefinitely.
        /// Use libvlc_media_parse_with_options() instead
        /// libvlc_media_parse_with_options
        /// libvlc_media_get_meta
        /// libvlc_media_get_tracks_info
        /// </summary>
        public void Parse() => Native.LibVLCMediaParse(NativeReference);

        /// <summary>Return true is the media descriptor object is parsed</summary>
        /// <returns>true if media object has been parsed otherwise it returns false</returns>
        public bool IsParsed => Native.LibVLCMediaIsParsed(NativeReference) != 0;

        /// <summary>Get Parsed status for media descriptor object.</summary>
        /// <returns>a value of the libvlc_media_parsed_status_t enum</returns>
        /// <remarks>
        /// <para>libvlc_MediaParsedChanged</para>
        /// <para>libvlc_media_parsed_status_t</para>
        /// <para>LibVLC 3.0.0 or later</para>
        /// </remarks>
        public MediaParsedStatus ParsedStatus => Native.LibVLCMediaGetParsedStatus(NativeReference);

        /// <summary>Stop the parsing of the media</summary>
        /// <remarks>
        /// <para>When the media parsing is stopped, the libvlc_MediaParsedChanged event will</para>
        /// <para>be sent with the libvlc_media_parsed_status_timeout status.</para>
        /// <para>libvlc_media_parse_with_options</para>
        /// <para>LibVLC 3.0.0 or later</para>
        /// </remarks>
        public void ParseStop() => Native.LibVLCMediaParseStop(NativeReference);

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
                var count = Native.LibVLCMediaTracksGet(NativeReference, ref arrayResultPtr);
                if (count == 0 || arrayResultPtr == IntPtr.Zero) return Enumerable.Empty<MediaTrack>();

                var tracks = new List<MediaTrack>();
                for (var i = 0; i < count; i++)
                {
                    var ptr = Marshal.ReadIntPtr(arrayResultPtr, i * IntPtr.Size);
                    var managedStruct = MarshalUtils.PtrToStructure<MediaTrack>(ptr);
                    tracks.Add(managedStruct);

                }

                Native.LibVLCMediaTracksRelease(arrayResultPtr, count);

                return tracks;
            }
        }

        /// <summary>
        /// <para>Get subitems of media descriptor object. This will increment</para>
        /// <para>the reference count of supplied media descriptor object. Use</para>
        /// <para>libvlc_media_list_release() to decrement the reference counting.</para>
        /// </summary>
        /// <returns>list of media descriptor subitems or NULL</returns>
        public MediaList SubItems => new MediaList(Native.LibVLCMediaSubitems(NativeReference));
       
        public MediaType Type => Native.LibVLCMediaGetType(NativeReference);

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
        public bool AddSlave(MediaSlaveType type, uint priority, string uri) => 
            Native.LibVLCMediaAddSlaves(NativeReference, type, priority, uri) != 0;


        /// <summary>
        /// <para>Clear all slaves previously added by libvlc_media_slaves_add() or</para>
        /// <para>internally.</para>
        /// </summary>
        /// <remarks>LibVLC 3.0.0 and later.</remarks>
        public void ClearSlaves() => Native.LibVLCMediaClearSlaves(NativeReference);

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
                var count = Native.LibVLCMediaGetSlaves(NativeReference, ref slaveArrayPtr);
                if (count == 0) return Enumerable.Empty<MediaSlave>();

                var slaves = new List<MediaSlave>();
                for (var i = 0; i < count; i++)
                {
                    var ptr = Marshal.ReadIntPtr(slaveArrayPtr, i * IntPtr.Size);
                    var managedStruct = MarshalUtils.PtrToStructure<MediaSlave>(ptr);
                    slaves.Add(managedStruct);
                }
                Native.LibVLCMediaReleaseSlaves(slaveArrayPtr, count);
                return slaves;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Media media &&
                   EqualityComparer<IntPtr>.Default.Equals(NativeReference, media.NativeReference);
        }

        public override int GetHashCode()
        {
            return this.NativeReference.GetHashCode();
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
                Native.LibVLCMediaRetain(NativeReference);
        }

        #endregion

        #region Events

        public event EventHandler<MediaMetaChangedEventArgs> MetaChanged
        {
            add => EventManager.AttachEvent(EventType.MediaMetaChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaMetaChanged, value);
        }

        public event EventHandler<MediaParsedChangedEventArgs> ParsedChanged
        {
            add => EventManager.AttachEvent(EventType.MediaParsedChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaParsedChanged, value);
        }

        public event EventHandler<MediaParsedChangedEventArgs> SubItemAdded
        {
            add => EventManager.AttachEvent(EventType.MediaSubItemAdded, value);
            remove => EventManager.DetachEvent(EventType.MediaSubItemAdded, value);
        }

        public event EventHandler<MediaDurationChangedEventArgs> DurationChanged
        {
            add => EventManager.AttachEvent(EventType.MediaDurationChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaDurationChanged, value);
        }

        public event EventHandler<MediaFreedEventArgs> MediaFreed
        {
            add => EventManager.AttachEvent(EventType.MediaFreed, value);
            remove => EventManager.DetachEvent(EventType.MediaFreed, value);
        }

        public event EventHandler<MediaStateChangedEventArgs> StateChanged
        {
            add => EventManager.AttachEvent(EventType.MediaStateChanged, value);
            remove => EventManager.DetachEvent(EventType.MediaStateChanged, value);
        }

        public event EventHandler<MediaSubItemAddedEventArgs> SubItemTreeAdded
        {
            add => EventManager.AttachEvent(EventType.MediaSubItemTreeAdded, value);
            remove => EventManager.DetachEvent(EventType.MediaSubItemTreeAdded, value);
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
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
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
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int ReadMedia(IntPtr opaque, IntPtr buf, uint len);

    /// <summary>Callback prototype to seek a custom bitstream input media.</summary>
    /// <param name="opaque">private pointer as set by the</param>
    /// <param name="offset">absolute byte offset to seek to</param>
    /// <returns>0 on success, -1 on error.</returns>
    /// <remarks>callback</remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int SeekMedia(IntPtr opaque, ulong offset);

    /// <summary>Callback prototype to close a custom bitstream input media.</summary>
    /// <param name="opaque">private pointer as set by the</param>
    /// <remarks>callback</remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
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

    /// <summary>Note the order of libvlc_state_t enum must match exactly the order of</summary>
    /// <remarks>
    /// <para>mediacontrol_PlayerStatus,</para>
    /// <para>input_state_e enums,</para>
    /// <para>and VideoLAN.LibVLCSharp.State (at bindings/cil/src/media.cs).</para>
    /// <para>Expected states by web plugins are:</para>
    /// <para>IDLE/CLOSE=0, OPENING=1, PLAYING=3, PAUSED=4,</para>
    /// <para>STOPPING=5, ENDED=6, ERROR=7</para>
    /// </remarks>
    public enum VLCState
    {
        NothingSpecial = 0,
        Opening = 1,
        Buffering = 2,
        Playing = 3,
        Paused = 4,
        Stopped = 5,
        Ended = 6,
        Error = 7
    }

    public enum TrackType
    {
        Unknown = -1,
        Audio = 0,
        Video = 1,
        Text = 2
    }

    public struct AudioTrack
    {
        public uint Channels;
        public uint Rate;
    }

    public struct VideoTrack
    {
        public uint Height;

        public uint Width;

        public uint SarNum;

        public uint SarDen;

        public uint FrameRateNum;

        public uint FrameRateDen;

        public VideoOrientation Orientation;

        public VideoProjection Projection;

        public VideoViewpoint Pose;
    }

    public enum VideoOrientation
    {
        /// <summary>Normal. Top line represents top, left column left.</summary>
        TopLeft = 0,
        /// <summary>Flipped horizontally</summary>
        TopRight = 1,
        /// <summary>Flipped vertically</summary>
        BottomLeft = 2,
        /// <summary>Rotated 180 degrees</summary>
        BottomRight = 3,
        /// <summary>Transposed</summary>
        LeftTop = 4,
        /// <summary>Rotated 90 degrees clockwise (or 270 anti-clockwise)</summary>
        LeftBottom = 5,
        /// <summary>Rotated 90 degrees anti-clockwise</summary>
        RightTop = 6,
        /// <summary>Anti-transposed</summary>
        RightBottom = 7
    }

    [Flags]
    public enum VideoProjection
    {
        Rectangular = 0,
        /// <summary>360 spherical</summary>
        Equirectangular = 1,
        CubemapLayoutStandard = 256
    }

    /// <summary>Viewpoint for video outputs</summary>
    /// <remarks>allocate using libvlc_video_new_viewpoint()</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct VideoViewpoint
    {
        public float Yaw;
        public float Pitch;
        public float Roll;
        public float Fov;
    }

    public struct SubtitleTrack
    {
        public IntPtr Encoding;
    }

    /// <summary>Type of a media slave: subtitle or audio.</summary>
    public enum MediaSlaveType
    {
        Subtitle = 0,
        Audio = 1
    }

    /// <summary>A slave of a libvlc_media_t</summary>
    /// <remarks>libvlc_media_slaves_get</remarks>
    public struct MediaSlave
    {
        public IntPtr Uri;
        public MediaSlaveType Type;
        public uint Priority;
    }
    #endregion

    public class MediaConfiguration
    {
        HashSet<string> _options = new HashSet<string>();

        public MediaConfiguration EnableHardwareDecoding()
        {
#if ANDROID
            _options.Add(":codec=mediacodec_ndk");
#endif
            return this;
        }

        public string Build() => string.Join(",", _options);
    }
}
