using System;
using System.Collections.Generic;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Defines the <see cref="EqualizerUtils" />.
    /// </summary>
    public class EqualizerUtils
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
        /// <param name="equalizer">The equalizer<see cref="Equalizer"/>.</param>
        /// <returns>The <see cref="List{Preset}"/>.</returns>
        public static List<Preset> LoadPresets(Equalizer equalizer)
        {
            var presetCount = equalizer.PresetCount;
            var presets = new List<Preset>((int)presetCount);
            for (var index = 0; index < presetCount; index++)
            {
                var preset = new Preset(index, equalizer.PresetName((uint)index))
                {
                    Preamp = equalizer.Preamp,
                    BandCount = (int)equalizer.BandCount
                };

                var bands = new List<Band>(preset.BandCount);
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
        /// Apply the new amplicatification to the Equalizer.
        /// This method is used when the Snap band mode is not enable.
        /// </summary>
        /// <param name="bandId">The bandId<see cref="int"/>.</param>
        /// <param name="amp">The amp<see cref="float"/>.</param>
        /// <param name="equalizer">The equalizer<see cref="Equalizer"/>.</param>
        public static void ApplyNewAmplicatification(int bandId, float amp, Equalizer equalizer)
        {
            equalizer.SetAmp(amp, (uint)bandId);
        }


        /// <summary>
        /// Perform frequency smoothing.
        /// This method is used when the Snap band mode is enable.
        /// </summary>
        /// <param name="bandId">The bandId<see cref="int"/>.</param>
        /// <param name="oldAmp">The previous vlaue of the the band's amplication<see cref="float"/>.</param>
        /// <param name="newAmp">The new vlaue of the the band's amplication<see cref="float"/>.</param>
        /// <param name="bands">The bands<see cref="List{Band}"/>.</param>
        /// <param name="equalizer">The equalizer<see cref="Equalizer"/>.</param>
        public static void SmoothOutFrequencies(int bandId, float oldAmp, float newAmp, List<Band> bands, Equalizer equalizer)
        {
            var delta = newAmp - oldAmp;
            foreach (var band in bands)
            {
                if (bandId == band.BandId)
                {
                    continue;
                }

                band.Amp = oldAmp + delta / (Math.Abs(band.BandId - bandId) * Math.Abs(band.BandId - bandId) * Math.Abs(band.BandId - bandId) + 1);

                var ampToApply = (band.Amp - RANGE) / PRECISION;
                equalizer.SetAmp(ampToApply, (uint)band.BandId);
            }
        }
    }
}
