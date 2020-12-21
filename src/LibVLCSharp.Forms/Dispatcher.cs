using System;
using System.Threading.Tasks;
using LibVLCSharp.MediaPlayerElement;
using Xamarin.Forms;

namespace LibVLCSharp.Forms
{
    /// <summary>
    /// Object that provides services for managing the queue of work items for a thread
    /// </summary>
    internal class Dispatcher : LibVLCSharp.MediaPlayerElement.IDispatcher
    {
        /// <summary>
        /// Schedules the provided callback on the UI thread from a worker threa
        /// </summary>
        /// <param name="action">The callback on which the dispatcher returns when the event is dispatched</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task InvokeAsync(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
            return Task.CompletedTask;
        }
    }
}
