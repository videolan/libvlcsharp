using LibVLCSharp.Uno;

namespace LibVLCSharp.Uno.WinUI.Sample;

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

    private void MediaPlayerElement_Initialized(object sender, InitializedEventArgs e)
    {
        ViewModel.InitializedCommand.Execute(e.SwapChainOptions);
    }
}
