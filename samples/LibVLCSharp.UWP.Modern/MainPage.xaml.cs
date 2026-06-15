using Windows.UI.Xaml.Controls;

namespace LibVLCSharp.UWP.Modern
{
    /// <summary>
    /// The main page of the application.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly MainViewModel _viewModel = new MainViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            DataContext = _viewModel;
            VideoView.Initialized += (sender, e) => _viewModel.Initialize(e);
        }
    }
}
