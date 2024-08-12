using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared.MediaPlayerElement;
using Microsoft.UI.Dispatching;
//using Windows.UI.Core;

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
        public DispatcherAdapter(DispatcherQueue dispatcher)
        {
            Dispatcher = dispatcher;
        }

        private DispatcherQueue Dispatcher { get; }

        /// <summary>
        /// Schedules the provided callback on the UI thread from a worker threa
        /// </summary>
        /// <param name="action">The callback on which the dispatcher returns when the event is dispatched</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task InvokeAsync(Action action)
        {
            return Task.Run(() => Dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, new DispatcherQueueHandler(action)));
        }
    }
}
