using System;
using System.Collections.Generic;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Defines the <see cref="EqualizerUtils" />.
    /// </summary>
    internal class EqualizerUtils
    {
        /// <summary>
        /// Defines the PRECISION.
        /// </summary>
        private const float PRECISION = 10;

        /// <summary>
        /// Defines the RANGE.
        /// </summary>
        private const float RANGE = 200;

        /// <summary>
        /// Load all Presets.
        /// </summary>
        /// <returns>The <see cref="List{Preset}"/>.</returns>
        internal List<Preset> LoadAllPresets()
        {
            using var defaultEqualizer = new Equalizer();
            var presets = new List<Preset>();
            for (var index = 0; index < defaultEqualizer.PresetCount; index++)
            {
                using var equalizer = new Equalizer((uint)index);
                var preset = new Preset
                {
                    PresetId = index,
                    Name = equalizer.PresetName((uint)index),                  
                    Preamp = equalizer.Preamp,
                    BandCount = (int)equalizer.BandCount
                };

                var bands = new List<Band>();
                for (var bandId = 0; bandId < preset.BandCount; bandId++)
                {
                    var band = new Band
                    {
                        BandId = bandId,
                        BandFrequency = equalizer.BandFrequency((uint)bandId),
                        Amp = equalizer.Amp((uint)bandId)
                    };
                    bands.Add(band);
                }

                preset.Bands = bands;
                presets.Add(preset);
            }
            return presets;
        }

        /// <summary>
        /// Apply a new amplification value to the Equalizer.
        /// This method is used when the Snap band mode is not enable.
        /// </summary>
        /// <param name="bandId">The bandId<see cref="int"/>.</param>
        /// <param name="amp">The amp<see cref="float"/>.</param>
        /// <param name="equalizer">The equalizer<see cref="Equalizer"/>.</param>
        internal void ApplyNewAmplification(int bandId, float amp, Equalizer equalizer)
        {
            equalizer.SetAmp(amp, (uint)bandId);
        }

        /// <summary>
        /// Perform frequency smoothing.
        /// This method is used when the Snap band mode is enable.
        /// </summary>
        /// <param name="bandId">The bandId<see cref="int"/>.</param>
        /// <param name="oldAmp">The previous value of the the band's amplification<see cref="float"/>.</param>
        /// <param name="newAmp">The new value of the the band's amplification<see cref="float"/>.</param>
        /// <param name="bands">The bands<see cref="List{Band}"/>.</param>
        /// <param name="equalizer">The equalizer<see cref="Equalizer"/>.</param>
        internal void SmoothOutFrequencies(int bandId, float oldAmp, float newAmp, List<Band> bands, Equalizer equalizer)
        {
            // @see https://code.videolan.org/videolan/vlc-android/-/blob/master/application/vlc-android/src/org/videolan/vlc/gui/audio/EqualizerFragment.kt#L270
            var delta = newAmp - oldAmp;
            foreach (var band in bands)
            {
                if (bandId == band.BandId)
                {
                    continue;
                }

                band.Amp = bands[band.BandId].Amp + delta / (Math.Abs(band.BandId - bandId) * Math.Abs(band.BandId - bandId) * Math.Abs(band.BandId - bandId) + 1);

                var ampToApply = (band.Amp - RANGE) / PRECISION;
                equalizer.SetAmp(ampToApply, (uint)band.BandId);
            }
        }

        /// <summary>
        /// Set an equalizer to a MediaPlayer.
        /// </summary>
        /// <param name="mediaPlayer">The MediaPlayer</param>
        /// <param name="equalizer">the Equalizer</param>
        internal void SetEqualizerToMediaPlayer(MediaPlayer mediaPlayer, Equalizer equalizer)
        {
            mediaPlayer.UnsetEqualizer();
            mediaPlayer.SetEqualizer(equalizer);
        }
    }
}
