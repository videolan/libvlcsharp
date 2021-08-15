using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibVLCSharp.Shared;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Defines the <see cref="EqualizerUtils" />.
    /// </summary>
    public static class EqualizerUtils
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
        /// Defines the EqualizerEnablePropertyKey.
        /// </summary>
        private const string EqualizerEnablePropertyKey = "VLC__MediaPlayerElement_IsEqualizerEnable";

        /// <summary>
        /// Defines the EqualizerPresetIndexPropertyKey.
        /// </summary>
        private const string EqualizerPresetIndexPropertyKey = "VLC__MediaPlayerElement_EqualizerPresetIndex";

        /// <summary>
        /// Load all Presets.
        /// </summary>
        /// <returns>The <see cref="List{Preset}"/>.</returns>
        public static List<Preset> LoadAllPresets()
        {
            var presetCount = new Equalizer().PresetCount;
            var presets = new List<Preset>();
            for (var index = 0; index < presetCount; index++)
            {
                var equalizer = new Equalizer((uint)index);
                var preset = new Preset(index, equalizer.PresetName((uint)index))
                {
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
        public static void ApplyNewAmplification(int bandId, float amp, Equalizer equalizer)
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
        public static void SmoothOutFrequencies(int bandId, float oldAmp, float newAmp, ObservableCollection<Band> bands, Equalizer equalizer)
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
        public static void SetEqualizerToMediaPlyer(LibVLCSharp.Shared.MediaPlayer mediaPlayer, Equalizer equalizer)
        {
            mediaPlayer.UnsetEqualizer();
            mediaPlayer.SetEqualizer(equalizer);
        }

        /// <summary>
        /// Save the state of the Equalizer (enable or disable) to the Application properties dictionary.
        /// </summary>
        /// <param name="isEqualizerEnable">The state of the equalizer: Enable or Disable</param>
        public static void SaveEqualizerState(bool isEqualizerEnable)
        {
            var applicationProperties = Application.Current.Properties;
            applicationProperties[EqualizerEnablePropertyKey] = isEqualizerEnable;
        }

        /// <summary>
        /// Get the state of the Equalizer from the application properties.
        /// </summary>
        /// <returns></returns>
        public static bool IsEqualizerEnable()
        {
            var applicationProperties = Application.Current.Properties;
            if(applicationProperties.ContainsKey(EqualizerEnablePropertyKey))
            {
                return (bool)applicationProperties[EqualizerEnablePropertyKey];
            }
            return false;
        }

        /// <summary>
        /// Save a preset index to the Application properties dictionary.
        /// </summary>
        /// <param name="presetIndex"></param>
        public static void SavePreset(int presetIndex)
        {
            var applicationProperties = Application.Current.Properties;
            applicationProperties[EqualizerPresetIndexPropertyKey] = presetIndex;
        }

        /// <summary>
        ///Get the saved preset index.
        /// </summary>
        /// <returns></returns>
        public static int GetSavedPresetIndex() => (int)Application.Current.Properties[EqualizerPresetIndexPropertyKey];

        /// <summary>
        /// Shallow copy a list of bands to a new list.
        /// </summary>
        /// <param name="originalBands">The original list</param>
        /// <returns>A new list</returns>
        public static List<Band> CopyBands(List<Band>? originalBands)
        {
            var newBands = new List<Band>();
            if (originalBands != null)
            {
                foreach (var band in originalBands)
                {
                    newBands.Add(new Band
                    {
                        BandId = band.BandId,
                        Amp = band.Amp,
                        BandFrequency = band.BandFrequency,
                        AmpMin = band.AmpMin,
                        AmpMax = band.AmpMax
                    });
                }
            }
            return newBands;
        }

        /// <summary>
        /// Checks if two amplification values are the same.
        /// </summary>
        /// <param name="firstValue">The first value</param>
        /// <param name="secondValue">The second value</param>
        /// <returns></returns>
        public static bool IsTheSameAmplification(double firstValue, double secondValue) => Math.Round(firstValue, 2) == Math.Round(secondValue, 2);
    }
}
