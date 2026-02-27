using System;
using System.Timers;

namespace LibVLCSharp.Shared.Helpers
{
    /// <summary>
    /// Provides a more precise playback timer for a <see cref="MediaPlayer"/> by
    /// interpolating the current playback time using LibVLC's monotonic clock.
    /// </summary>
    /// <remarks>
    /// This helper is useful for UI scenarios such as smooth progress bars,
    /// where <see cref="MediaPlayer.Time"/> updates are not frequent enough
    /// and can cause visible jumps.
    ///
    /// The timer listens to <see cref="MediaPlayer.TimeChanged"/> events to store
    /// the last known playback timestamp, and then uses <see cref="LibVLC.Clock"/>
    /// to interpolate the current playback time between updates.
    ///
    /// This class is cross-platform and does not depend on any specific UI framework.
    /// UI updates should be marshalled to the UI thread by the consumer.
    /// </remarks>
    public sealed class PreciseMediaTimer : IDisposable
    {
        private readonly MediaPlayer _mp;
        private readonly LibVLC _libVLC;
        private readonly Timer _timer;

        private long _lastTs;
        private long _lastClock;
        private float _lastRate;
        private bool _hasFirstUpdate;

        private long _lastInterpolatedTime;

        /// <summary>
        /// Occurs when a new interpolated playback position is available.
        /// The value is normalized between 0.0 and 1.0.
        /// </summary>
        public event Action<double>? PrecisePositionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreciseMediaTimer"/> class.
        /// </summary>
        /// <param name="mp">The associated <see cref="MediaPlayer"/> instance.</param>
        /// <param name="libVLC">The associated <see cref="LibVLC"/> instance.</param>
        /// <param name="intervalMs">
        /// The interpolation update interval in milliseconds. Default is 50 ms.
        /// </param>
        public PreciseMediaTimer(MediaPlayer mp, LibVLC libVLC, int intervalMs = 16)
        {
            _mp = mp ?? throw new ArgumentNullException(nameof(mp));
            _libVLC = libVLC ?? throw new ArgumentNullException(nameof(libVLC));

            _timer = new Timer(intervalMs)
            {
                AutoReset = true
            };

            _timer.Elapsed += OnTick;
            _mp.TimeChanged += OnTimeChanged;
        }

        /// <summary>
        /// Handles native TimeChanged events and stores the last known playback state.
        /// </summary>
        private void OnTimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
        {
            // Compare against the current interpolated playback time instead of the last raw VLC timestamp.
            // LibVLC timestamps can lag behind the interpolated value, which represents the actual
            // playback head position used by the UI.
            if (_hasFirstUpdate && e.Time < _lastInterpolatedTime && _mp.Length > 0)
            {
                var progress = (double)_lastInterpolatedTime / _mp.Length;

                // If we are near the end of the media, this backward jump is most likely
                // a known LibVLC artefact (late TimeChanged event). Ignore it to avoid
                // progress bar snapping backwards (e.g., the "stuck at ~92%" issue).
                if (progress > 0.9)
                    return;
            }

            // Accept the new timestamp and update the interpolation reference point.
            // This also correctly handles real user-initiated seeks.
            _lastTs = e.Time;
            _lastClock = _libVLC.Clock;
            _lastRate = _mp.Rate;

            // Reset the monotonic floor to the new VLC time so interpolation resumes from here.
            _lastInterpolatedTime = e.Time;
            _hasFirstUpdate = true;
        }

        /// <summary>
        /// Performs time interpolation using the LibVLC monotonic clock.
        /// </summary>
        private void OnTick(object? sender, ElapsedEventArgs e)
        {
            if (!_hasFirstUpdate || _mp.Length <= 0)
                return;

            if (_mp.State == VLCState.Ended)
            {
                PrecisePositionChanged?.Invoke(1.0);
                return;
            }

            if (_mp.State != VLCState.Playing)
                return;

            var nowClock = _libVLC.Clock;
            var deltaMs = (nowClock - _lastClock) / 1000;
            var interpolatedTime = _lastTs + (long)(deltaMs * _lastRate);

            if (interpolatedTime < _lastInterpolatedTime - 1000)
                _lastInterpolatedTime = interpolatedTime;
            else if (interpolatedTime < _lastInterpolatedTime)
                interpolatedTime = _lastInterpolatedTime;

            _lastInterpolatedTime = interpolatedTime;

            if (interpolatedTime >= _mp.Length)
                interpolatedTime = _mp.Length;

            var position = (double)interpolatedTime / _mp.Length;

            if (position < 0.0)
                position = 0.0;
            if (position > 1.0)
                position = 1.0;

            PrecisePositionChanged?.Invoke(position);
        }

        /// <summary>
        /// Starts the interpolation timer.
        /// </summary>
        public void Start()
        {
            _hasFirstUpdate = false;
            _lastInterpolatedTime = 0;
            _timer.Start();
        }

        /// <summary>
        /// Stops the interpolation timer.
        /// </summary>
        public void Stop() => _timer.Stop();

        /// <summary>
        /// Releases all resources used by the <see cref="PreciseMediaTimer"/>.
        /// </summary>
        public void Dispose()
        {
            _timer.Stop();
            _timer.Elapsed -= OnTick;
            _mp.TimeChanged -= OnTimeChanged;
            _timer.Dispose();
        }
    }
}
