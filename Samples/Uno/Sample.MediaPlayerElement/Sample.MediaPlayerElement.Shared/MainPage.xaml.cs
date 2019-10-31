using Windows.UI.Xaml.Controls;

namespace Sample.MediaPlayerElement
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
    }
}
