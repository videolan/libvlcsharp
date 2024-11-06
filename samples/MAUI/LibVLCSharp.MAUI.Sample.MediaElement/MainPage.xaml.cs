using Microsoft.Maui.Controls;

namespace LibVLCSharp.MAUI.Sample.MediaElement
{
    /// <summary>
    /// Represnets the Main Page.
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
