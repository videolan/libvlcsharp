using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Command to update the availability of a control
    /// </summary>
    internal class AvailabilityCommand
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AvailabilityCommand"/> class
        /// </summary>
        /// <param name="parentControl">parent control</param>
        /// <param name="isAvailable">function determining if the control is available</param>
        /// <param name="availableState">available state name</param>
        /// <param name="unavailableState">unavailable state name</param>
        /// <param name="control">control to update </param>
        /// <param name="isEnabled">function to get a value indicating whether the control should be enabled</param>
        public AvailabilityCommand(Control parentControl, Func<bool> isAvailable, string availableState, string? unavailableState,
            Control? control, Func<bool>? isEnabled = null)
        {
            ParentControl = parentControl;
            Control = control;
            IsAvailable = isAvailable;
            AvailableState = availableState;
            UnavailableState = unavailableState;
            IsEnabled = isEnabled;
        }

        private Control ParentControl { get; }

        /// <summary>
        /// Gets the control
        /// </summary>
        public Control? Control { get; }

        private Func<bool> IsAvailable { get; }

        private string AvailableState { get; }

        private string? UnavailableState { get; }

        private Func<bool>? IsEnabled { get; }

        /// <summary>
        /// Update the state of the control
        /// </summary>
        public void Update()
        {
            var available = IsAvailable();
            if (available || UnavailableState != null)
            {
                VisualStateManager.GoToState(ParentControl, available ? AvailableState : UnavailableState, true);
            }
            if (IsEnabled != null && Control != null)
            {
                Control.IsEnabled = IsEnabled();
            }
        }
    }
}
