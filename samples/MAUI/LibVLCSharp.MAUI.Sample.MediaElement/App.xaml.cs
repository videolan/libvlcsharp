using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LibVLCSharp.MAUI.Sample.MediaElement
{
    /// <summary>
    /// Represents the main App.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();
            ConfigureUnhandledExceptionHandling();

            MainPage = new MainPage();
        }

        private static void ConfigureUnhandledExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    HandleException(ex);
                }
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                if (e.Exception is Exception ex)
                {
                    HandleException(ex);
                }
                e.SetObserved();  // Prevents the process from terminating.
            };
        }

        private static void HandleException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
