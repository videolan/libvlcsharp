using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared.MediaPlayerElement;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// Object that provides services for managing the queue of work items for a thread
    /// </summary>
    internal class Dispatcher : Shared.MediaPlayerElement.IDispatcher
    {
        /// <summary>
        /// Schedules the provided callback on the UI thread from a worker thread
        /// </summary>
        /// <param name="action">The callback on which the dispatcher returns when the event is dispatched</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task InvokeAsync(Action action)
        {
            Application.Current?.Dispatcher.Dispatch(action);
            return Task.CompletedTask;
        }
    }
}
