#if IOS
namespace LibVLCSharp.Forms
{
    /// <summary>
    /// Empty shell used to load the custom renderer assembly.
    /// </summary>
    internal static class Platform
    {
        /// <summary>
        /// Call this to load the custom renderer assembly.
        /// </summary>
        internal static void Init()
        {
        }
    }
}
#endif
