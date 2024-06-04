using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LibVLCSharp.MAUI.Sample.MediaElement
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ConfigureUnhandledExceptionHandling();

            MainPage = new MainPage();
        }

        private void ConfigureUnhandledExceptionHandling()
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

        private void HandleException(Exception ex)
        {
            if (ex != null)
            {
                // Log the exception or show a message to the user.
                Console.WriteLine(ex.Message);
            }
        }
    }
}
