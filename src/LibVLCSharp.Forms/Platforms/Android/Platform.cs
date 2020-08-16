using System;
using Android.App;
using Android.OS;
using static Android.App.Application;

namespace LibVLCSharp.Forms.Platforms.Android
{
    /// <summary>
    /// To get the current activity.
    /// </summary>
    public static class Platform
    {
        private class ActivityLifecycleContextListener : Java.Lang.Object, IActivityLifecycleCallbacks
        {
            private WeakReference<Activity?> CurrentActivity { get; } = new WeakReference<Activity?>(null);

            /// <summary>
            /// Gets or sets the current activity
            /// </summary>
            public Activity? Activity
            {
                get => CurrentActivity.TryGetTarget(out var a) ? a : null;
                set => CurrentActivity.SetTarget(value);
            }

            public void OnActivityCreated(Activity? activity, Bundle? savedInstanceState)
            {
                Activity = activity;
            }

            public void OnActivityDestroyed(Activity? activity)
            {
            }

            public void OnActivityPaused(Activity? activity)
            {
                Activity = activity;
            }

            public void OnActivityResumed(Activity? activity)
            {
                Activity = activity;
            }

            public void OnActivitySaveInstanceState(Activity? activity, Bundle? outState)
            {
            }

            public void OnActivityStarted(Activity? activity)
            {
            }

            public void OnActivityStopped(Activity? activity)
            {
            }
        }

        private static ActivityLifecycleContextListener? LifecycleListener { get; set; }

        /// <summary>
        /// Gets the current activity.
        /// </summary>
        internal static Activity? Activity => LifecycleListener?.Activity;

        /// <summary>
        /// Sets the activity.
        /// </summary>
        /// <param name="activity">Current activity.</param>
        public static void Init(Activity activity)
        {
            var lifecycleListener = LifecycleListener;
            if (lifecycleListener == null)
            {
                lifecycleListener = new ActivityLifecycleContextListener();
                LifecycleListener = lifecycleListener;
                var app = activity.Application;
                if (app is null)
                {
                    throw new InvalidOperationException("The given activity is not linked to an Application instance (activity.Application is null)");
                }

                app.RegisterActivityLifecycleCallbacks(lifecycleListener);
            }
            lifecycleListener.Activity = activity;
        }
    }
}
