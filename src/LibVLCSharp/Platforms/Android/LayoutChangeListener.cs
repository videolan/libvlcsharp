using Android.Views;
using Org.Videolan.Libvlc;

namespace LibVLCSharp.Platforms.Android
{
    /// <summary>
    /// LayoutChangeListener is a Java type necessary for interop with libvlc android
    /// </summary>
    public class LayoutChangeListener : Java.Lang.Object, View.IOnLayoutChangeListener
    {
        readonly AWindow _aWindow;

        /// <summary>
        /// Standard constructor for LayoutChangeListener
        /// </summary>
        /// <param name="awindow">AWindow is the main public API object to interact with the native libvlc android API</param>
        public LayoutChangeListener(AWindow awindow)
        {
            _aWindow = awindow;
        }

        /// <summary>
        /// Callback for layout change to adjust the window size
        /// </summary>
        /// <param name="v">view</param>
        /// <param name="left">left</param>
        /// <param name="top">top</param>
        /// <param name="right">right</param>
        /// <param name="bottom">bottom</param>
        /// <param name="oldLeft">previous left</param>
        /// <param name="oldTop">previous top</param>
        /// <param name="oldRight">previous right</param>
        /// <param name="oldBottom">previous bottom</param>
        public void OnLayoutChange(View? v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight,
            int oldBottom)
        {
            if (left != oldLeft || top != oldTop || right != oldRight || bottom != oldBottom)
            {
                _aWindow.SetWindowSize(right - left, bottom - top);
            }
        }
    }
}
