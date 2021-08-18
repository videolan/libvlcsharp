﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using LibVLCSharp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LibVLCSharp.Shared.MediaPlayerElement;
using System.Collections.Generic;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Represents an object that uses a <see cref="LibVLCSharp.Shared.MediaPlayer"/> to render audio and video to the display.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPlayerElement : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerElement"/> class.
        /// </summary>
        public MediaPlayerElement()
        {
            InitializeComponent();
            EqualizerService = new EqualizerUtils();
            LoadEqualizerControls();
        }

        private bool Initialized { get; set; }

        /// <summary>
        /// Identifies the <see cref="LibVLC"/> dependency property.
        /// </summary>
        public static readonly BindableProperty LibVLCProperty = BindableProperty.Create(nameof(LibVLC), typeof(LibVLC),
            typeof(MediaPlayerElement), propertyChanged: LibVLCPropertyChanged);
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
        /// </summary>
        public LibVLC LibVLC
        {
            get => (LibVLC)GetValue(LibVLCProperty);
            set => SetValue(LibVLCProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property.
        /// </summary>
        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer),
            typeof(LibVLCSharp.Shared.MediaPlayer), typeof(MediaPlayerElement), propertyChanged: MediaPlayerPropertyChanged);
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get => (LibVLCSharp.Shared.MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        private static readonly BindableProperty PlaybackControlsProperty = BindableProperty.Create(nameof(PlaybackControls),
            typeof(PlaybackControls), typeof(MediaPlayerElement), propertyChanged: PlaybackControlsPropertyChanged);
        /// <summary>
        /// Gets or sets the playback controls for the media.
        /// </summary>
        public PlaybackControls PlaybackControls
        {
            get => (PlaybackControls)GetValue(PlaybackControlsProperty);
            set => SetValue(PlaybackControlsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VideoView"/> dependency property
        /// </summary>
        private static readonly BindableProperty VideoViewProperty = BindableProperty.Create(nameof(VideoView), typeof(VideoView),
         typeof(MediaPlayerElement), propertyChanged: VideoViewPropertyChanged);
        /// <summary>
        /// Gets or sets the video view.
        /// </summary>
        public VideoView? VideoView
        {
            get => (VideoView)GetValue(VideoViewProperty);
            private set => SetValue(VideoViewProperty, value);
        }

        private static readonly BindableProperty EnableRendererDiscoveryProperty = BindableProperty.Create(nameof(EnableRendererDiscovery),
            typeof(bool), typeof(PlaybackControls), true, propertyChanged: EnableRendererDiscoveryPropertyChanged);

        /// <summary>
        /// Enable or disable renderer discovery
        /// </summary>
        public bool EnableRendererDiscovery
        {
            get => (bool)GetValue(EnableRendererDiscoveryProperty);
            set => SetValue(EnableRendererDiscoveryProperty, value);
        }


        private void OnVideoViewChanged(VideoView videoView)
        {
            if (videoView != null)
            {
                videoView.MediaPlayer = MediaPlayer;
                var playbackControls = PlaybackControls;
                if (playbackControls != null)
                {
                    playbackControls.VideoView = videoView;
                }
            }
        }

        private void OnLibVLCChanged(LibVLC libVLC)
        {
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.LibVLC = libVLC;
            }
        }

        private void OnMediaPlayerChanged(LibVLCSharp.Shared.MediaPlayer mediaPlayer)
        {
            var videoView = VideoView;
            if (videoView != null)
            {
                videoView.MediaPlayer = mediaPlayer;
            }
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.MediaPlayer = mediaPlayer;

                // Defines Equalizer button click event
                var equalizerButton = PlaybackControls.GetEqualizerButton();
                if (equalizerButton != null)
                {
                    equalizerButton.Clicked += OpenEqualizerView;
                }

                // Check if the Equalizer must be set to the media
                if(MediaPlayer.Length > 0 && IsEqualizerEnable())
                {
                    using var mediaEqualizer = new Equalizer((uint)GetSavedPresetIndex());
                    EqualizerService.SetEqualizerToMediaPlayer(MediaPlayer, mediaEqualizer);
                }
            }
        }

        private void OnPlayControlsChanged(PlaybackControls playbackControls)
        {
            if (playbackControls != null)
            {
                playbackControls.IsCastButtonVisible = EnableRendererDiscovery;
                playbackControls.LibVLC = LibVLC;
                playbackControls.MediaPlayer = MediaPlayer;
                playbackControls.VideoView = VideoView;
            }
        }

        private void OnEnableRendererDiscoveryChanged(bool enableRendererDiscovery)
        {
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.IsCastButtonVisible = enableRendererDiscovery;
            }
        }

        private static void VideoViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnVideoViewChanged((VideoView)newValue);
        }

        private static void LibVLCPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnLibVLCChanged((LibVLC)newValue);
        }

        private static void MediaPlayerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnMediaPlayerChanged((LibVLCSharp.Shared.MediaPlayer)newValue);
        }

        private static void PlaybackControlsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnPlayControlsChanged((PlaybackControls)newValue);
        }

        private static void EnableRendererDiscoveryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnEnableRendererDiscoveryChanged((bool)newValue);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Element.Parent"/> of an element is set. 
        /// Implement this method in order to add behavior when the element is added to a parent.
        /// </summary>
        /// <remarks>Implementors must call the base method.</remarks>
        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null && !Initialized)
            {
                Initialized = true;

                if (VideoView == null)
                {
                    VideoView = new VideoView();
                }

                if (PlaybackControls == null)
                {
                    PlaybackControls = new PlaybackControls();
                }

                var application = Application.Current;
                application.PageAppearing += PageAppearing;
                application.PageDisappearing += PageDisappearing;
            }
        }

        private void PageAppearing(object sender, Page e)
        {
            if (e == this.FindAncestor<Page?>())
            {
                MessagingCenter.Subscribe<LifecycleMessage>(this, "OnSleep", m =>
                {
                    var applicationProperties = Application.Current.Properties;
                    var mediaPlayer = MediaPlayer;
                    if (mediaPlayer != null)
                    {
                        applicationProperties[$"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_Position"] = mediaPlayer.Position;
                        applicationProperties[$"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_IsPlaying"] = mediaPlayer.State == VLCState.Playing;
                        mediaPlayer.Stop();
                    }
                    VideoView = null;
                });
                MessagingCenter.Subscribe<LifecycleMessage>(this, "OnResume", m =>
                {
                    VideoView = new VideoView();
                    var mediaPlayer = MediaPlayer;
                    if (mediaPlayer != null)
                    {
                        var applicationProperties = Application.Current.Properties;
                        if (applicationProperties.TryGetValue($"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_IsPlaying", out var play) && play is true)
                        {
                            mediaPlayer.Play();
                            mediaPlayer.Position = applicationProperties.TryGetValue($"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_Position", out var position)
                                && position is float p ? p : 0;
                        }
                    }
                });
            }
        }

        private void PageDisappearing(object sender, Page e)
        {
            if (e == this.FindAncestor<Page?>())
            {
                MessagingCenter.Unsubscribe<LifecycleMessage>(this, "OnSleep");
                MessagingCenter.Unsubscribe<LifecycleMessage>(this, "OnResume");
            }
        }

        private void GestureRecognized(object sender, EventArgs e)
        {
            if(EqualizerControls != null && EqualizerControls.IsVisible)
            {
                EqualizerControls.IsVisible = false;
            }
            PlaybackControls.Show();
        }

        private Frame? EqualizerControls { get; set; }
        private Switch? EnableEqualizerSwitch { get; set; }
        private Picker? PresetsDataPicker { get; set; }
        private Slider? PreampSlider { get; set; }
        private BindableStackLayout? FrequenciesLayout { get; set; }
        //private Equalizer? MediaEqualizer;
        private Preset? SelectedPreset { get; set; }
        private Switch? SnapBandsSwitch { get; set; }
        private const string EqualizerEnablePropertyKey = "VLC__MediaPlayerElement_IsEqualizerEnable";
        private const string EqualizerPresetIndexPropertyKey = "VLC__MediaPlayerElement_EqualizerPresetIndex";
        private ObservableCollection<BandViewModel> AmpBands = new ObservableCollection<BandViewModel>();
        private EqualizerUtils EqualizerService { get; }
        // Workaround to prevent Slider issues
        private bool EqualizerViewIsOpening = false;
        private bool IgnoreAmpValueChanged = true;
        private int AmpSliderTriggerCount = 0;

        private void LoadEqualizerControls()
        {
            EqualizerControls = this.FindChild<Frame?>(nameof(EqualizerControls));
            EnableEqualizerSwitch = this.FindChild<Switch?>(nameof(EnableEqualizerSwitch));
            PresetsDataPicker = this.FindChild<Picker?>(nameof(PresetsDataPicker));
            PreampSlider = this.FindChild<Slider?>(nameof(PreampSlider));
            FrequenciesLayout = this.FindChild<BindableStackLayout?>(nameof(FrequenciesLayout));
            SnapBandsSwitch = this.FindChild<Switch?>(nameof(SnapBandsSwitch));
            if (EnableEqualizerSwitch != null && PresetsDataPicker != null && PreampSlider != null)
            {
                EnableEqualizerSwitch.Toggled += EnableEqualizerSwitchToggled;
                PresetsDataPicker.SelectedIndexChanged += SelectedPresetIndexChanged;
                PreampSlider.ValueChanged += PreAmpSliderValueChanged;
            }
        }

        /// <summary>
        /// Occurs when a preset is selected.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void SelectedPresetIndexChanged(object sender, EventArgs e)
        {
            EnableEqualizerIfDisable();
            IgnoreAmpValueChanged = true;
            var picker = (Picker)sender;
            if (!EqualizerViewIsOpening && FrequenciesLayout != null && picker.SelectedIndex >= 0)
            {
                SelectedPreset = (Preset)picker.SelectedItem;
                AmpBands = Convert(SelectedPreset.Bands);
                FrequenciesLayout.ItemsSource = AmpBands;

                using var mediaEqualizer = new Equalizer((uint)SelectedPreset.PresetId);
                EqualizerService.SetEqualizerToMediaPlayer(MediaPlayer, mediaEqualizer);
                SaveEqualizerState(true);
                SavePreset(SelectedPreset.PresetId);
            }
            IgnoreAmpValueChanged = false;
        }

        /// <summary>
        /// Occurs when a new value of the amplification is set.
        /// </summary>
        /// <param name="sender">Arguments</param>
        /// <param name="e">Sender</param>
        private void PreAmpSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            EnableEqualizerIfDisable();
            if(!EqualizerViewIsOpening && SelectedPreset != null)
            {
                using var mediaEqualizer = new Equalizer((uint)SelectedPreset.PresetId);
                mediaEqualizer.SetPreamp((float)e.NewValue);
                EqualizerService.SetEqualizerToMediaPlayer(MediaPlayer, mediaEqualizer);
            }
        }

        private void SetPresetsItemsSource(int presetIndex)
        {
            if(PresetsDataPicker != null && PreampSlider != null)
            {
                var allPresets = EqualizerService.LoadAllPresets();

                PresetsDataPicker.BindingContext = this;
                PresetsDataPicker.ItemsSource = allPresets;
                PresetsDataPicker.ItemDisplayBinding = new Binding("Name");

                SelectedPreset = allPresets.First(p => p.PresetId == presetIndex);
                AmpBands = Convert(SelectedPreset.Bands);
                PresetsDataPicker.SelectedIndex = SelectedPreset.PresetId;

                PreampSlider.BindingContext = SelectedPreset;
                PreampSlider.SetBinding(Slider.ValueProperty, nameof(SelectedPreset.Preamp));

                if (FrequenciesLayout != null)
                {
                    FrequenciesLayout.BindingContext = this;
                    FrequenciesLayout.ItemsSource = AmpBands;
                }
            }
        }

        private void BandAmplificationValueChanged(object sender, ValueChangedEventArgs e)
        {
            EnableEqualizerIfDisable();
            if (!EqualizerViewIsOpening && !IgnoreAmpValueChanged && !IsTheSameAmplification(e.OldValue, e.NewValue))
            {
                var ampSwitch = (Slider)sender;
                // Get the current band from where the Slider value was updated
                var mainView = (View)ampSwitch.Parent.Parent;
                var band = (BandViewModel)mainView.BindingContext;
                
                if(SelectedPreset != null  && SnapBandsSwitch != null)
                {
                    using var mediaEqualizer = new Equalizer((uint)SelectedPreset.PresetId);
                    mediaEqualizer.SetPreamp((float)SelectedPreset.Preamp);

                    // Snap Band is enable => Perform frequency smoothing
                    if (SnapBandsSwitch.IsToggled && FrequenciesLayout != null)
                    {
                        AmpSliderTriggerCount++;
                        AmpSliderTriggerCount = AmpSliderTriggerCount >= 10 ? 0 : AmpSliderTriggerCount;
                        if (AmpSliderTriggerCount == 1)
                        {
                            IgnoreAmpValueChanged = true;
                            var bands = Convert(AmpBands);
                            EqualizerService.SmoothOutFrequencies(band.BandId, (float)e.OldValue, (float)e.NewValue, bands, mediaEqualizer);
                            UpdateUIBandsAmplifications(AmpBands, bands);
                            EqualizerService.SetEqualizerToMediaPlayer(MediaPlayer, mediaEqualizer);
                            IgnoreAmpValueChanged = false;
                        }
                    }
                    else
                    {
                        EqualizerService.ApplyNewAmplification(band.BandId, (float)e.NewValue, mediaEqualizer);
                        EqualizerService.SetEqualizerToMediaPlayer(MediaPlayer, mediaEqualizer);
                    }
                }
            }
        }

        private void EnableEqualizerSwitchToggled(object sender, ToggledEventArgs e)
        {
           if(e.Value)
           {      
                if (SelectedPreset != null && !EqualizerViewIsOpening)
                {
                    SaveEqualizerState(true);
                    SavePreset(SelectedPreset.PresetId);

                    using var mediaEqualizer = new Equalizer((uint)SelectedPreset.PresetId);
                    mediaEqualizer.SetPreamp((float)SelectedPreset.Preamp);
                    EqualizerService.SetEqualizerToMediaPlayer(MediaPlayer, mediaEqualizer);
                }
           }
           else
           {
                MediaPlayer.UnsetEqualizer();
                SaveEqualizerState(false);
           }
        }

        private void OpenEqualizerView(object sender, EventArgs e)
        {
            EqualizerViewIsOpening = IgnoreAmpValueChanged = true;
            if (MediaPlayer.Length > 0 && EqualizerControls != null)
            {
                var enableEqualizer = IsEqualizerEnable();
                var presetIndex = enableEqualizer ? GetSavedPresetIndex() : 0;
                SetPresetsItemsSource(presetIndex);
                EqualizerControls.IsVisible = true;
                if (EnableEqualizerSwitch != null && enableEqualizer)
                    EnableEqualizerSwitch.IsToggled = true;
            }
            EqualizerViewIsOpening = IgnoreAmpValueChanged = false;
        }

        /// <summary>
        /// This method is called when the user try to change the preset or the amplification value 
        /// while the equalizer is disable.
        /// </summary>
        private void EnableEqualizerIfDisable()
        {
            if (EnableEqualizerSwitch != null && SelectedPreset != null && !EnableEqualizerSwitch.IsToggled
                && !EqualizerViewIsOpening)
            {
                EnableEqualizerSwitch.IsToggled = true;
                SaveEqualizerState(true);
                SavePreset(SelectedPreset.PresetId);
            }
        }

        /// <summary>
        /// Save the state of the Equalizer (enable or disable) to the Application properties dictionary.
        /// </summary>
        /// <param name="isEqualizerEnable">The state of the equalizer: Enable or Disable</param>
        private void SaveEqualizerState(bool isEqualizerEnable)
        {
            var applicationProperties = Application.Current.Properties;
            applicationProperties[EqualizerEnablePropertyKey] = isEqualizerEnable;
        }

        /// <summary>
        /// Get the state of the Equalizer from the application properties.
        /// </summary>
        /// <returns></returns>
        private bool IsEqualizerEnable()
        {
            var applicationProperties = Application.Current.Properties;
            if (applicationProperties.ContainsKey(EqualizerEnablePropertyKey))
            {
                return (bool)applicationProperties[EqualizerEnablePropertyKey];
            }
            return false;
        }

        /// <summary>
        /// Save a preset index to the Application properties dictionary.
        /// </summary>
        /// <param name="presetIndex"></param>
        private void SavePreset(int presetIndex)
        {
            var applicationProperties = Application.Current.Properties;
            applicationProperties[EqualizerPresetIndexPropertyKey] = presetIndex;
        }

        /// <summary>
        ///Get the saved preset index.
        /// </summary>
        /// <returns></returns>
        private int GetSavedPresetIndex() => (int)Application.Current.Properties[EqualizerPresetIndexPropertyKey];


        /// <summary>
        /// Checks if two amplification values are the same.
        /// </summary>
        /// <param name="firstValue">The first value</param>
        /// <param name="secondValue">The second value</param>
        /// <returns></returns>
        private bool IsTheSameAmplification(double firstValue, double secondValue) => Math.Round(firstValue, 2) == Math.Round(secondValue, 2);
    
        private void UpdateUIBandsAmplifications(ObservableCollection<BandViewModel> bandViewModels, List<Band> bands)
        {
            for(var index = 0; index < bands.Count; index++)
            {
                bandViewModels[index].Amp = bands[index].Amp;
            }
        }

        private ObservableCollection<BandViewModel> Convert(List<Band> bands)
        {
            var bandViewModels = new ObservableCollection<BandViewModel>();
            foreach(var band in bands)
            {
                var bandViewModel = new BandViewModel
                {
                    BandId = band.BandId,
                    Amp = band.Amp,
                    BandFrequency = band.BandFrequency,
                };
                bandViewModels.Add(bandViewModel);

            }
            return bandViewModels;
        }

        private List<Band> Convert(ObservableCollection<BandViewModel> bandsViewModels)
        {
            var bands= new List<Band>();
            foreach (var bandVM in bandsViewModels)
            {
                var band = new Band
                {
                    BandId = bandVM.BandId,
                    Amp = bandVM.Amp,
                    BandFrequency = bandVM.BandFrequency,
                };
                bands.Add(band);
            }
            return bands;
        }
    }
}
