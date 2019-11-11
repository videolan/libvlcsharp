using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared.MediaPlayerElement;
using Windows.UI.Core;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Dispatcher adapter
    /// </summary>
    internal class DispatcherAdapter : IDispatcher
    {
        /// <summary>
        /// Initializes a new instance 
        /// </summary>
        /// <param name="dispatcher"></param>
        public DispatcherAdapter(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        private CoreDispatcher Dispatcher { get; }

        /// <summary>
        /// Schedules the provided callback on the UI thread from a worker threa
        /// </summary>
        /// <param name="action">The callback on which the dispatcher returns when the event is dispatched</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task InvokeAsync(Action action)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(action));
        }
    }
}
