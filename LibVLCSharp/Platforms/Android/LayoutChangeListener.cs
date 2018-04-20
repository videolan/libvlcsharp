using Android.Views;
using Org.Videolan.Libvlc;

namespace LibVLCSharp.Platforms.Android
{
    public class LayoutChangeListener : Java.Lang.Object, View.IOnLayoutChangeListener
    {
        readonly AWindow _aWindow;

        public LayoutChangeListener(AWindow awindow)
        {
            _aWindow = awindow;
        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight,
            int oldBottom)
        {
            if (left != oldLeft || top != oldTop || right != oldRight || bottom != oldBottom)
            {
                _aWindow.SetWindowSize(right - left, bottom - top);
            }

        }
    }
}