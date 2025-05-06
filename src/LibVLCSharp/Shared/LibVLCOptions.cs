using System;
using System.Collections.Generic;
using System.Linq;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Fluent builder for constructing LibVLC options.
    /// Mirrors many common libvlc command line parameters.
    /// </summary>
    public class LibVLCOptionsBuilder
    {
        private readonly List<string> _options = new();
        private bool _includeDebugLogs = false;

#if UWP || ANDROID
        private bool _audioResamplerOverridden = false;
#endif

        /// <summary>
        /// Enables verbose debug logging by adding "--verbose=2".
        /// </summary>
        public LibVLCOptionsBuilder EnableDebugLogs()
        {
            _includeDebugLogs = true;
            return this;
        }

        /// <summary>
        /// Sets the verbosity level of the log output.
        /// </summary>
        /// <param name="level">Verbosity level (e.g. 0=errors only, 2=debug).</param>
        public LibVLCOptionsBuilder SetVerbosity(int level)
        {
            _options.Add($"--verbose={level}");
            return this;
        }

        /// <summary>
        /// Sets the audio resampler module.
        /// </summary>
        /// <param name="resampler">The name of the resampler (e.g. speex_resampler, soxr).</param>
        public LibVLCOptionsBuilder SetAudioResampler(string resampler)
        {
            _options.Add($"--audio-resampler={resampler}");
#if UWP || ANDROID
            _audioResamplerOverridden = true;
#endif
            return this;
        }

#if UWP || ANDROID
        /// <summary>
        /// Disables the default resampler for the current platform.
        /// </summary>
        public LibVLCOptionsBuilder DisableDefaultResampler()
        {
            _audioResamplerOverridden = true;
            return this;
        }
#endif

        /// <summary>
        /// Sets the audio output module (e.g. "--aout=directsound").
        /// </summary>
        /// <param name="output">The audio output module name.</param>
        public LibVLCOptionsBuilder SetAudioOutput(string output)
        {
            _options.Add($"--aout={output}");
            return this;
        }

        /// <summary>
        /// Disables video output.
        /// </summary>
        public LibVLCOptionsBuilder DisableVideo()
        {
            _options.Add("--no-video");
            return this;
        }

        /// <summary>
        /// Disables audio output.
        /// </summary>
        public LibVLCOptionsBuilder DisableAudio()
        {
            _options.Add("--no-audio");
            return this;
        }

        /// <summary>
        /// Sets the network caching value in milliseconds.
        /// </summary>
        /// <param name="milliseconds">Caching delay in ms (e.g. 300).</param>
        public LibVLCOptionsBuilder SetNetworkCaching(int milliseconds)
        {
            _options.Add($"--network-caching={milliseconds}");
            return this;
        }

        /// <summary>
        /// Sets the file caching value in milliseconds.
        /// </summary>
        /// <param name="milliseconds">Caching delay in ms (e.g. 300).</param>
        public LibVLCOptionsBuilder SetFileCaching(int milliseconds)
        {
            _options.Add($"--file-caching={milliseconds}");
            return this;
        }

        /// <summary>
        /// Enables hardware-accelerated decoding using the specified method.
        /// </summary>
        /// <param name="method">Decoding method (e.g. dxva2, vaapi, d3d11va).</param>
        public LibVLCOptionsBuilder EnableHardwareDecoding(string method)
        {
            _options.Add($"--avcodec-hw={method}");
            return this;
        }

        /// <summary>
        /// Enables dropping of late video frames.
        /// </summary>
        public LibVLCOptionsBuilder DropLateFrames()
        {
            _options.Add("--drop-late-frames");
            return this;
        }

        /// <summary>
        /// Enables frame skipping.
        /// </summary>
        public LibVLCOptionsBuilder SkipFrames()
        {
            _options.Add("--skip-frames");
            return this;
        }

        /// <summary>
        /// Enables or disables use of Xlib.
        /// </summary>
        /// <param name="enabled">True to enable Xlib, false to disable.</param>
        public LibVLCOptionsBuilder UseXlib(bool enabled)
        {
            _options.Add(enabled ? "--xlib" : "--no-xlib");
            return this;
        }

        /// <summary>
        /// Adds a custom libvlc option.
        /// </summary>
        /// <param name="option">The command-line option to add.</param>
        public LibVLCOptionsBuilder AddOption(string option)
        {
            _options.Add(option);
            return this;
        }

        /// <summary>
        /// Adds multiple custom libvlc options.
        /// </summary>
        /// <param name="options">The options to add.</param>
        public LibVLCOptionsBuilder AddOptions(IEnumerable<string> options)
        {
            _options.AddRange(options);
            return this;
        }


        /// <summary>
        /// Builds the final LibVLCOptions object including any platform-specific defaults.
        /// </summary>
        public LibVLCOptions Build()
        {
            var finalOptions = _options.ToList();

            if (_includeDebugLogs)
                finalOptions.Add("--verbose=2");

#if UWP
            finalOptions.Add("--aout=winstore");
            if (!_audioResamplerOverridden)
                finalOptions.Add("--audio-resampler=speex_resampler");
#elif ANDROID
            if (!_audioResamplerOverridden)
                finalOptions.Add("--audio-resampler=soxr");
#endif

            return new LibVLCOptions(finalOptions);
        }
    }

    /// <summary>
    /// Represents the finalized LibVLC options as an array of command-line arguments.
    /// </summary>
    public class LibVLCOptions
    {
        /// <summary>
        /// Gets the collection of command-line arguments.
        /// </summary>
        public string[] Options { get; }

        /// <summary>
        /// Initializes a new instance of LibVLCOptions with the specified options.
        /// </summary>
        /// <param name="options">The options to include.</param>
        public LibVLCOptions(IEnumerable<string>? options)
        {
            Options = options?.ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        /// Gets an empty LibVLCOptions instance.
        /// </summary>
        public static LibVLCOptions Empty => new LibVLCOptions(Array.Empty<string>());
    }
}
