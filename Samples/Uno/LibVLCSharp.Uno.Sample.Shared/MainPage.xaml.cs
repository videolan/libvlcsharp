using Windows.UI.Xaml.Controls;

namespace LibVLCSharp.Uno.Sample
{
    /// <summary>
    /// Main page
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MainPage"/> class
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
        }

        /// <summary>
        /// Gets the main view model
        /// </summary>
        public MainViewModel ViewModel { get; }

        private void VideoView_Initialized(object sender, InitializedEventArgs e)
        {
            ViewModel.InitializedCommand.Execute(e.SwapChainOptions);
        }
    }
}
