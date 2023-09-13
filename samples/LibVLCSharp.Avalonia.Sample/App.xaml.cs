using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Avalonia.Sample.ViewModels;
using LibVLCSharp.Avalonia.Sample.Views;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Avalonia.Sample
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };

                desktop.Exit += OnExit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        void OnExit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var vm = (MainWindowViewModel)desktop.MainWindow?.DataContext;
                if (vm != null)
                    vm.Dispose();
            }
        }
    }
}
