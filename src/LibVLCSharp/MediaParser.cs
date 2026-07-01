using LibVLCSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LibVLCSharp
{
    /// <summary>
    /// Terminal outcome of a parse request (libvlc_parser_status_t).
    /// </summary>
    enum ParserStatus
    {
        Failed = 0,
        Timeout = 1,
        Cancelled = 2,
        Done = 3
    }

    /// <summary>
    /// Configuration for a <see cref="MediaParser"/> (libvlc_parser_cfg).
    /// </summary>
    public class MediaParserConfiguration
    {
        /// <summary>
        /// The maximum number of threads used by the parser, 0 for default (1 thread).
        /// </summary>
        public uint MaxParserThreads { get; set; }

        /// <summary>
        /// The maximum number of threads used by the thumbnailer, 0 for default (1 thread).
        /// </summary>
        public uint MaxThumbnailerThreads { get; set; }

        /// <summary>
        /// Timeout of the parser in microseconds, 0 for no limits, or -1 to inherit the value of preparse-timeout.
        /// </summary>
        public long Timeout { get; set; }
    }

    /// <summary>
    /// The media parser, introduced in LibVLC 4, parses media (meta, tracks, cover art) and generates
    /// thumbnails asynchronously. It replaces the per-media parse and thumbnail APIs that existed in LibVLC 3.
    /// A single parser owns its own thread pool and can process many requests concurrently.
    /// </summary>
    public class MediaParser : Internal
    {
        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_parser_new")]
            internal static extern IntPtr LibVLCParserNew(IntPtr inst, IntPtr cfg);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_parser_destroy")]
            internal static extern void LibVLCParserDestroy(IntPtr parser);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_parser_queue")]
            internal static extern IntPtr LibVLCParserQueue(IntPtr parser, IntPtr request, IntPtr cbs, IntPtr cbsOpaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_parser_queue_thumbnailing")]
            internal static extern IntPtr LibVLCParserQueueThumbnailing(IntPtr parser, IntPtr request, IntPtr cbs, IntPtr cbsOpaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_parser_cancel_request")]
            internal static extern UIntPtr LibVLCParserCancelRequest(IntPtr parser, IntPtr task);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_parser_task_get_media")]
            internal static extern IntPtr LibVLCParserTaskGetMedia(IntPtr task);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_parser_task_release")]
            internal static extern void LibVLCParserTaskRelease(IntPtr task);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ParserCfg
        {
            public uint Version;
            public uint MaxParserThreads;
            public uint MaxThumbnailerThreads;
            public long Timeout;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ParserRequest
        {
            public uint Version;
            public IntPtr Media;
            public int ParseFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SeekParams
        {
            public int Type;   // libvlc_thumbnailer_seek_type_t
            public long Value; // union { libvlc_time_t time; double pos; } - 8 bytes
            public int Speed;  // libvlc_thumbnailer_seek_speed_t
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ThumbnailerRequest
        {
            public uint Version;
            public IntPtr Media;
            public uint Width;
            public uint Height;
            [MarshalAs(UnmanagedType.I1)] public bool Crop;
            public int Type; // libvlc_picture_type_t
            public SeekParams Seek;
            [MarshalAs(UnmanagedType.I1)] public bool HwDec;
        }

        /// <summary>
        /// Create a media parser.
        /// </summary>
        /// <param name="libVLC">the LibVLC instance</param>
        /// <param name="configuration">optional parser configuration</param>
        public MediaParser(LibVLC libVLC, MediaParserConfiguration? configuration = null)
            : base(() => Create(libVLC, configuration), Native.LibVLCParserDestroy)
        {
        }

        static IntPtr Create(LibVLC libVLC, MediaParserConfiguration? configuration)
        {
            var cfg = new ParserCfg
            {
                Version = 0,
                MaxParserThreads = configuration?.MaxParserThreads ?? 0,
                MaxThumbnailerThreads = configuration?.MaxThumbnailerThreads ?? 0,
                Timeout = configuration?.Timeout ?? 0
            };

            var cfgPtr = Marshal.AllocHGlobal(MarshalUtils.SizeOf(cfg));
            try
            {
                Marshal.StructureToPtr(cfg, cfgPtr, false);
                return Native.LibVLCParserNew(libVLC.NativeReference, cfgPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(cfgPtr);
            }
        }

        /// <summary>
        /// Parse a media asynchronously: fetches (local or network) art, meta data and/or tracks information.
        /// </summary>
        /// <param name="media">the media to parse</param>
        /// <param name="options">parse flags</param>
        /// <param name="cancellationToken">token used to cancel the request</param>
        /// <param name="attachmentsAdded">callback invoked when attached pictures are found before parsing completes</param>
        /// <returns>the terminal parse status</returns>
        public Task<MediaParsedStatus> ParseAsync(Media media, MediaParseOptions options = MediaParseOptions.ParseLocal,
            CancellationToken cancellationToken = default, Action<MediaParserAttachmentsAddedEventArgs>? attachmentsAdded = null)
        {
            if (media == null)
                throw new ArgumentNullException(nameof(media));

            var state = new ParseState(attachmentsAdded);
            var handle = GCHandle.Alloc(state);

            var request = new ParserRequest
            {
                Version = 0,
                Media = media.NativeReference,
                ParseFlags = (int)ToNativeParseFlags(options)
            };

            var requestPtr = Marshal.AllocHGlobal(MarshalUtils.SizeOf(request));
            try
            {
                Marshal.StructureToPtr(request, requestPtr, false);
                var task = Native.LibVLCParserQueue(NativeReference, requestPtr, ParserCallbacks.Pointer, GCHandle.ToIntPtr(handle));
                if (task == IntPtr.Zero)
                {
                    handle.Free();
                    throw new VLCException("Failed to queue the parse request");
                }

                state.TaskHandle = task;
                if (cancellationToken.CanBeCanceled)
                    state.Registration = cancellationToken.Register(() =>
                    {
                        Native.LibVLCParserCancelRequest(NativeReference, task);
                        TrySetCanceled(state.CompletionSource, cancellationToken);
                    });
            }
            finally
            {
                Marshal.FreeHGlobal(requestPtr);
            }

            return state.CompletionSource.Task;
        }

        /// <summary>
        /// Generate a thumbnail for a media asynchronously.
        /// </summary>
        /// <param name="media">the media source of the thumbnail</param>
        /// <param name="width">thumbnail width (0 to derive from the media)</param>
        /// <param name="height">thumbnail height (0 to derive from the media)</param>
        /// <param name="pictureType">the output picture type</param>
        /// <param name="crop">true to crop instead of stretch (only when both width and height are non-zero)</param>
        /// <param name="seekTime">when not null, seek to this time (in microseconds) before generating</param>
        /// <param name="seekPosition">when not null (and seekTime is null), seek to this position [0;1] before generating</param>
        /// <param name="speed">the seek speed</param>
        /// <param name="hardwareDecoding">true to enable the hardware decoder</param>
        /// <param name="cancellationToken">token used to cancel the request</param>
        /// <returns>the generated picture, or null on error/timeout/cancellation</returns>
        public Task<Picture?> ThumbnailAsync(Media media, uint width, uint height,
            PictureType pictureType = PictureType.Argb, bool crop = false,
            long? seekTime = null, double? seekPosition = null,
            ThumbnailerSeekSpeed speed = ThumbnailerSeekSpeed.Fast,
            bool hardwareDecoding = false, CancellationToken cancellationToken = default)
        {
            if (media == null)
                throw new ArgumentNullException(nameof(media));

            var seek = new SeekParams { Speed = (int)speed };
            if (seekTime.HasValue)
            {
                seek.Type = 1; // libvlc_thumbnailer_seek_time
                seek.Value = seekTime.Value;
            }
            else if (seekPosition.HasValue)
            {
                seek.Type = 2; // libvlc_thumbnailer_seek_pos
                seek.Value = BitConverter.DoubleToInt64Bits(seekPosition.Value);
            }

            var state = new ThumbnailState();
            var handle = GCHandle.Alloc(state);

            var request = new ThumbnailerRequest
            {
                Version = 0,
                Media = media.NativeReference,
                Width = width,
                Height = height,
                Crop = crop,
                Type = (int)pictureType,
                Seek = seek,
                HwDec = hardwareDecoding
            };

            var requestPtr = Marshal.AllocHGlobal(MarshalUtils.SizeOf(request));
            try
            {
                Marshal.StructureToPtr(request, requestPtr, false);
                var task = Native.LibVLCParserQueueThumbnailing(NativeReference, requestPtr, ThumbnailerCallbacks.Pointer, GCHandle.ToIntPtr(handle));
                if (task == IntPtr.Zero)
                {
                    handle.Free();
                    throw new VLCException("Failed to queue the thumbnail request");
                }

                state.TaskHandle = task;
                if (cancellationToken.CanBeCanceled)
                    state.Registration = cancellationToken.Register(() => Native.LibVLCParserCancelRequest(NativeReference, task));
            }
            finally
            {
                Marshal.FreeHGlobal(requestPtr);
            }

            return state.CompletionSource.Task;
        }

        static libvlc_media_parse_flag ToNativeParseFlags(MediaParseOptions options)
        {
            libvlc_media_parse_flag flags = 0;
            if ((options & (MediaParseOptions.ParseLocal | MediaParseOptions.ParseNetwork | MediaParseOptions.ParseForced)) != 0)
                flags |= libvlc_media_parse_flag.Parse;
            if ((options & MediaParseOptions.FetchLocal) != 0)
                flags |= libvlc_media_parse_flag.FetchLocal;
            if ((options & MediaParseOptions.FetchNetwork) != 0)
                flags |= libvlc_media_parse_flag.FetchNetwork;
            if ((options & MediaParseOptions.DoInteract) != 0)
                flags |= libvlc_media_parse_flag.DoInteract;
            if (flags == 0)
                flags = libvlc_media_parse_flag.Parse;
            return flags;
        }

        static MediaParsedStatus ToManagedStatus(ParserStatus status) => status switch
        {
            ParserStatus.Failed => MediaParsedStatus.Failed,
            ParserStatus.Timeout => MediaParsedStatus.Timeout,
            ParserStatus.Cancelled => MediaParsedStatus.Cancelled,
            ParserStatus.Done => MediaParsedStatus.Done,
            _ => MediaParsedStatus.None
        };

        [Flags]
        enum libvlc_media_parse_flag
        {
            Parse = 0x01,
            FetchLocal = 0x02,
            FetchNetwork = 0x04,
            DoInteract = 0x08
        }

        static bool TrySetCanceled<T>(TaskCompletionSource<T> completionSource, CancellationToken cancellationToken) =>
#if NET45
            completionSource.TrySetCanceled();
#else
            completionSource.TrySetCanceled(cancellationToken);
#endif

        class ParseState
        {
            public ParseState(Action<MediaParserAttachmentsAddedEventArgs>? attachmentsAdded)
            {
                AttachmentsAdded = attachmentsAdded;
            }

            public readonly Action<MediaParserAttachmentsAddedEventArgs>? AttachmentsAdded;
            public readonly TaskCompletionSource<MediaParsedStatus> CompletionSource = MarshalUtils.NewCompletionSource<MediaParsedStatus>();
            public IntPtr TaskHandle;
            public CancellationTokenRegistration Registration;
        }

        class ThumbnailState
        {
            public readonly TaskCompletionSource<Picture?> CompletionSource = MarshalUtils.NewCompletionSource<Picture?>();
            public IntPtr TaskHandle;
            public CancellationTokenRegistration Registration;
        }

        /// <summary>
        /// Shared native libvlc_parser_cbs (on_parsed). One copy is shared by all parse requests; the per-request
        /// state is carried through cbs_opaque.
        /// </summary>
        static class ParserCallbacks
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            delegate void OnParsedCb(IntPtr opaque, IntPtr task, ParserStatus status);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            delegate void OnAttachmentsAddedCb(IntPtr opaque, IntPtr task, IntPtr pictureList);

            [StructLayout(LayoutKind.Sequential)]
            struct NativeCbs
            {
                public uint Version;
                public IntPtr OnParsed;
                public IntPtr OnAttachmentsAdded;
            }

            static readonly OnParsedCb s_onParsed = OnParsed;
            static readonly OnAttachmentsAddedCb s_onAttachmentsAdded = OnAttachmentsAdded;
            static readonly IntPtr s_pointer = Build();
            internal static IntPtr Pointer => s_pointer;

            static IntPtr Build()
            {
                var cbs = new NativeCbs
                {
                    Version = 0,
                    OnParsed = Marshal.GetFunctionPointerForDelegate(s_onParsed),
                    OnAttachmentsAdded = Marshal.GetFunctionPointerForDelegate(s_onAttachmentsAdded)
                };
                var ptr = Marshal.AllocHGlobal(MarshalUtils.SizeOf(cbs));
                Marshal.StructureToPtr(cbs, ptr, false);
                return ptr;
            }

            static void OnAttachmentsAdded(IntPtr opaque, IntPtr task, IntPtr pictureList)
            {
                try
                {
                    var handle = GCHandle.FromIntPtr(opaque);
                    var state = (ParseState)handle.Target!;
                    MediaParser.OnAttachmentsAdded(state, task, pictureList);
                }
                catch (Exception ex)
                {
                    Core.Log(ex.ToString());
                }
            }

            static void OnParsed(IntPtr opaque, IntPtr task, ParserStatus status)
            {
                try
                {
                    var handle = GCHandle.FromIntPtr(opaque);
                    var state = (ParseState)handle.Target!;
                    state.Registration.Dispose();
                    Native.LibVLCParserTaskRelease(task);
                    handle.Free();
                    state.CompletionSource.TrySetResult(ToManagedStatus(status));
                }
                catch (Exception ex)
                {
                    Core.Log(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Shared native libvlc_thumbnailer_cbs (on_ended).
        /// </summary>
        static class ThumbnailerCallbacks
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            delegate void OnEndedCb(IntPtr opaque, IntPtr task, IntPtr picture);

            [StructLayout(LayoutKind.Sequential)]
            struct NativeCbs
            {
                public uint Version;
                public IntPtr OnEnded;
            }

            static readonly OnEndedCb s_onEnded = OnEnded;
            static readonly IntPtr s_pointer = Build();
            internal static IntPtr Pointer => s_pointer;

            static IntPtr Build()
            {
                var cbs = new NativeCbs { Version = 0, OnEnded = Marshal.GetFunctionPointerForDelegate(s_onEnded) };
                var ptr = Marshal.AllocHGlobal(MarshalUtils.SizeOf(cbs));
                Marshal.StructureToPtr(cbs, ptr, false);
                return ptr;
            }

            static void OnEnded(IntPtr opaque, IntPtr task, IntPtr picture)
            {
                try
                {
                    var handle = GCHandle.FromIntPtr(opaque);
                    var state = (ThumbnailState)handle.Target!;
                    state.Registration.Dispose();
                    // The picture is only valid during this callback; new Picture(ptr) retains it.
                    var result = picture == IntPtr.Zero ? null : new Picture(picture);
                    Native.LibVLCParserTaskRelease(task);
                    handle.Free();
                    state.CompletionSource.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    Core.Log(ex.ToString());
                }
            }
        }

        static void OnAttachmentsAdded(ParseState state, IntPtr task, IntPtr pictureList)
        {
            var handler = state.AttachmentsAdded;
            if (handler == null)
                return;

            var mediaPtr = Native.LibVLCParserTaskGetMedia(task);
            Media? media = null;
            if (mediaPtr != IntPtr.Zero)
            {
                Media.Native.LibVLCMediaRetain(mediaPtr);
                media = new Media(mediaPtr);
            }

            handler(new MediaParserAttachmentsAddedEventArgs(media, PictureList.RetainPictures(pictureList)));
        }
    }

    /// <summary>
    /// Parser event data for attached pictures discovered during parsing.
    /// </summary>
    public sealed class MediaParserAttachmentsAddedEventArgs : EventArgs
    {
        internal MediaParserAttachmentsAddedEventArgs(Media? media, IReadOnlyList<Picture> pictures)
        {
            Media = media;
            Pictures = pictures;
        }

        /// <summary>
        /// The media associated with the parse task. Dispose it when done.
        /// </summary>
        public Media? Media { get; }

        /// <summary>
        /// Retained attached pictures. Dispose each picture when done.
        /// </summary>
        public IReadOnlyList<Picture> Pictures { get; }
    }
}
