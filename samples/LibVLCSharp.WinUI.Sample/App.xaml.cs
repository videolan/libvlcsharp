using DirectN;
using Microsoft.UI.Xaml;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibVLCSharp.WinUI.Sample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            m_window.Closed += M_window_Closed;
        }

        private void M_window_Closed(object sender, WindowEventArgs args)
        {
            var dxgiDebug = DXGIFunctions.DXGIGetDebugInterface();
            Debug.WriteLine("ReportLiveObjects");
            dxgiDebug.Object.ReportLiveObjects(DXGIConstants.DXGI_DEBUG_ALL,
                DXGI_DEBUG_RLO_FLAGS.DXGI_DEBUG_RLO_ALL);
            dxgiDebug.Dispose();
        }

        private Window m_window;
    }
}
