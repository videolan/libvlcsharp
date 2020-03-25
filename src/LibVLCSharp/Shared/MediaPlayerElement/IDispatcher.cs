using System;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Interface for an object that provides services for managing the queue of work items for a thread
    /// </summary>
    internal interface IDispatcher
    {
        /// <summary>
        /// Schedules the provided callback on the UI thread from a worker threa
        /// </summary>
        /// <param name="action">The callback on which the dispatcher returns when the event is dispatched</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        Task InvokeAsync(Action action);
    }
}
