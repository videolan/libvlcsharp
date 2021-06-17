namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Force Device Orientation.
    /// </summary>
    public interface IOrientationHandler
    {
        /// <summary>
        /// Force Landscape orientation mode.
        /// </summary>
        void ForceLandscape();

        /// <summary>
        /// Force Portrait orientation mode.
        /// </summary>
        void ForcePortrait();

        /// <summary>
        /// Restore Landscape and Portrait orientation mode.
        /// </summary>
        void ResetOrientation();
    }
}
