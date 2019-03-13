using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Sample
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
    }
}
