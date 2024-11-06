namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// Interface for power management.
    /// </summary>
    public interface IPowerManager
    {
        /// <summary>
        /// Gets or sets a value indicating whether the screen should be kept on.
        /// </summary>
        bool KeepScreenOn { get; set; }
    }
}
