using Xamarin.Forms;

namespace LibVLCSharp.Forms.Sample.MediaPlayerElement
{
    /// <summary>
    /// Represents the main page.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        void OnAppearing(object sender, System.EventArgs e)
        {
            base.OnAppearing();
            ((MainViewModel)BindingContext).OnAppearing();
        }

        void OnDisappearing(object sender, System.EventArgs e)
        {
            base.OnDisappearing();
            ((MainViewModel)BindingContext).OnDisappearing();
        }
    }
}
