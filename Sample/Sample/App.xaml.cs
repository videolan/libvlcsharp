using LibVLCSharp.Forms.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LibVLCSharp.Forms.Sample.MediaPlayerElement
{
    /// <summary>
    /// Represents the sample application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            MessagingCenter.Send(new LifecycleMessage(), nameof(OnStart));
        }

        /// <summary>
        /// Called when the application enters the sleeping state.
        /// </summary>
        protected override void OnSleep()
        {
            base.OnSleep();
            MessagingCenter.Send(new LifecycleMessage(), nameof(OnSleep));
        }

        /// <summary>
        /// Called when the application resumes from the sleeping state.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            MessagingCenter.Send(new LifecycleMessage(), nameof(OnResume));
        }
    }
}
