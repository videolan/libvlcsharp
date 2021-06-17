namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Force Device Orientation.
    /// </summary>
    public interface IOrientationHandler
    {
        /// <summary>
        /// Lock device's orientation.
        /// </summary>
        void LockOrientation();

        /// <summary>
        /// Unlock device's orientation.
        /// </summary>
        void UnLockOrientation();
    }
}
