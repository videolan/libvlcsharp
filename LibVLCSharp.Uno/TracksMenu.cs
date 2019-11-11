using LibVLCSharp.Shared.MediaPlayerElement;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Tracks menu
    /// </summary>
    internal class TracksMenu
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TracksMenu"/> class
        /// </summary>
        /// <param name="manager">tracks manager</param>
        /// <param name="availableStateName">available state name</param>
        /// <param name="unavailableStateName">unavailable state name</param>
        /// <param name="hasNoneItem">value indicating whether the menu has a 'None' entry</param>
        public TracksMenu(TracksManager manager, string availableStateName, string unavailableStateName, bool hasNoneItem)
        {
            Manager = manager;
            AvailableStateName = availableStateName;
            UnavailableStateName = unavailableStateName;
            HasNoneItem = hasNoneItem;
        }

        /// <summary>
        /// Gets the tracks manager
        /// </summary>
        public TracksManager Manager { get; }

        /// <summary>
        /// Gets the available state name
        /// </summary>
        public string AvailableStateName { get; }

        /// <summary>
        /// Gets the unavailable state name
        /// </summary>
        public string UnavailableStateName { get; }

        /// <summary>
        /// Gets a value indicating whether the menu has a 'None' entry
        /// </summary>
        public bool HasNoneItem { get; }
    }
}
