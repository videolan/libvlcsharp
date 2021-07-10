using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Discovers the cast renderers
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.LibVLC"/> and <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> properties
    /// need to be set in order to work</remarks>
    internal class CastRenderersDiscoverer : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Occurs when <see cref="CastAvailable"/> property value changes
        /// </summary>
        public event EventHandler? CastAvailableChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="CastRenderersDiscoverer"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public CastRenderersDiscoverer(IDispatcher? dispatcher) : base(dispatcher)
        {
            MediaPlayerChanged += async (sender, e) => await UpdateHasRenderersPropertyValueAsync();
            LibVLCChanged += async (sender, e) => await OnLibVLCChangedAsync();
        }

        private RendererDiscoverer? RendererDiscoverer { get; set; }

        private bool _castAvailable;
        /// <summary>
        /// Gets or sets a value indicating whether there are renderers discovered and <see cref="MediaPlayer"/> is not null
        /// </summary>
        public bool CastAvailable
        {
            get => _castAvailable;
            private set
            {
                if (_castAvailable != value)
                {
                    _castAvailable = value;
                    CastAvailableChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private bool _enabled;
        /// <summary>
        /// Gets or sets a value indicating whether the discover should be enabled or not
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    var rendererDiscoverer = RendererDiscoverer;
                    if (rendererDiscoverer != null)
                    {
                        if (value)
                        {
                            rendererDiscoverer.Start();
                        }
                        else
                        {
                            Stop(rendererDiscoverer);
                            UpdateHasRenderersPropertyValue();
                        }
                    }
                }
            }
        }

        private IList<RendererItem> RenderersList { get; } = new List<RendererItem>();

        private IEnumerable<RendererItem>? _renderers;
        /// <summary>
        /// Gets the renderers list
        /// </summary>
        public IEnumerable<RendererItem> Renderers => _renderers ?? (_renderers = new ObservableCollection<RendererItem>(RenderersList));

        private void UpdateHasRenderersPropertyValue()
        {
            CastAvailable = RenderersList.Any() && MediaPlayer != null;
        }

        private Task UpdateHasRenderersPropertyValueAsync()
        {
            return DispatcherInvokeAsync(UpdateHasRenderersPropertyValue);
        }

        private async Task OnLibVLCChangedAsync()
        {
            var rendererDiscoverer = RendererDiscoverer;
            if (rendererDiscoverer != null)
            {
                Stop(rendererDiscoverer);
                await UpdateHasRenderersPropertyValueAsync();
                rendererDiscoverer.Dispose();
                RendererDiscoverer = null;
            }

            var libVLC = LibVLC;
            if (libVLC != null)
            {
                rendererDiscoverer = new RendererDiscoverer(libVLC);
                rendererDiscoverer.ItemAdded += RendererDiscoverer_ItemAddedAsync;
                rendererDiscoverer.ItemDeleted += RendererDiscoverer_ItemDeletedAsync;
                RendererDiscoverer = rendererDiscoverer;
                if (Enabled)
                {
                    rendererDiscoverer.Start();
                }
            }
        }

        private async void RendererDiscoverer_ItemAddedAsync(object? sender, RendererDiscovererItemAddedEventArgs e)
        {
            RenderersList.Add(e.RendererItem);
            await UpdateHasRenderersPropertyValueAsync();
        }

        private async void RendererDiscoverer_ItemDeletedAsync(object? sender, RendererDiscovererItemDeletedEventArgs e)
        {
            RenderersList.Remove(e.RendererItem);
            await UpdateHasRenderersPropertyValueAsync();
        }

        private void Stop(RendererDiscoverer rendererDiscoverer)
        {
            rendererDiscoverer.Stop();
            RenderersList.Clear();
        }
    }
}
