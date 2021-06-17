using System;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Custom view that allows user to unlock the MediaPlayerElement View.
    /// </summary>
    public class SwipeToUnLockView : AbsoluteLayout
    {
        private const string ThumbPropertyName = "Thumb";
        private const string TrackBarPropertyName = "TrackBar";
        private const string FillBarPropertyName = "FillBar";

        /// <summary>
        /// Defines the ThumbProperty.
        /// </summary>
        public static readonly BindableProperty ThumbProperty =
            BindableProperty.Create(
                ThumbPropertyName, typeof(View), typeof(SwipeToUnLockView),
                defaultValue: default(View));

        /// <summary>
        /// Gets or sets the Thumb.
        /// </summary>
        public View Thumb
        {
            get { return (View)GetValue(ThumbProperty); }
            set { SetValue(ThumbProperty, value); }
        }

        /// <summary>
        /// Defines the TrackBar Property.
        /// </summary>
        public static readonly BindableProperty TrackBarProperty =
            BindableProperty.Create(
                TrackBarPropertyName, typeof(View), typeof(SwipeToUnLockView),
                defaultValue: default(View));

        /// <summary>
        /// Gets or sets the TrackBar.
        /// </summary>
        public View TrackBar
        {
            get { return (View)GetValue(TrackBarProperty); }
            set { SetValue(TrackBarProperty, value); }
        }

        /// <summary>
        /// Defines the FillBar Property.
        /// </summary>
        public static readonly BindableProperty FillBarProperty =
            BindableProperty.Create(
                FillBarPropertyName, typeof(View), typeof(SwipeToUnLockView),
                defaultValue: default(View));

        /// <summary>
        /// Gets or sets the FillBar.
        /// </summary>
        public View FillBar
        {
            get { return (View)GetValue(FillBarProperty); }
            set { SetValue(FillBarProperty, value); }
        }

        /// <summary>
        /// Defines the PanGesture.
        /// </summary>
        private PanGestureRecognizer _panGesture = new PanGestureRecognizer();

        /// <summary>
        /// Defines the Gesture Listener.
        /// </summary>
        private View _gestureListener;

        /// <summary>
        /// Defaut Constructor.
        /// </summary>
        public SwipeToUnLockView()
        {
            _panGesture.PanUpdated += OnPanGestureUpdated;
            SizeChanged += OnSizeChanged;

            _gestureListener = new ContentView { BackgroundColor = Color.White, Opacity = 0.05 };
            _gestureListener.GestureRecognizers.Add(_panGesture);
        }

        /// <summary>
        /// The event wich will trigger when the view is completely swiped.
        /// </summary>
        public event EventHandler? SlideCompleted = null;

        /// <summary>
        /// Defines the FadeEffect default value.
        /// </summary>
        private const double FadeEffect = 0.5;

        /// <summary>
        /// Defines the Animation Length.
        /// </summary>
        private const uint AnimLength = 50;

        internal async void OnPanGestureUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (Thumb == null || TrackBar == null || FillBar == null)
                return;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    await TrackBar.FadeTo(FadeEffect, AnimLength);
                    break;

                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    var x = Math.Max(0, e.TotalX);
                    if (x > (Width - Thumb.Width))
                        x = (Width - Thumb.Width);

                    // Uncomment this if you want only forward dragging.
                    // if (e.TotalX < Thumb.TranslationX)
                    //    return;
                    Thumb.TranslationX = x;
                    SetLayoutBounds(FillBar, new Rectangle(0, 0, x + Thumb.Width / 2, Height));
                    break;

                case GestureStatus.Completed:
                    var posX = Thumb.TranslationX;
                    SetLayoutBounds(FillBar, new Rectangle(0, 0, 0, Height));

                    // Reset translation applied during the pan
                    await Task.WhenAll(new Task[]
                    {
                        TrackBar.FadeTo(1, AnimLength),
                        Thumb.TranslateTo(0, 0, AnimLength * 2, Easing.CubicIn),
                    });

                    if (posX >= (Width - Thumb.Width - 10 /* keep some margin for error*/))
                        SlideCompleted?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        internal void OnSizeChanged(object sender, EventArgs e)
        {
            if (Width == 0 || Height == 0)
                return;
            if (Thumb == null || TrackBar == null || FillBar == null)
                return;

            Children.Clear();

            SetLayoutFlags(TrackBar, AbsoluteLayoutFlags.SizeProportional);
            SetLayoutBounds(TrackBar, new Rectangle(0, 0, 1, 1));
            Children.Add(TrackBar);

            SetLayoutFlags(FillBar, AbsoluteLayoutFlags.None);
            SetLayoutBounds(FillBar, new Rectangle(0, 0, 0, Height));
            Children.Add(FillBar);

            SetLayoutFlags(Thumb, AbsoluteLayoutFlags.None);
            SetLayoutBounds(Thumb, new Rectangle(0, 0, Width / 5, Height));
            Children.Add(Thumb);

            SetLayoutFlags(_gestureListener, AbsoluteLayoutFlags.SizeProportional);
            SetLayoutBounds(_gestureListener, new Rectangle(0, 0, 1, 1));
            Children.Add(_gestureListener);
        }
    }
}
