using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Themes = Avalonia.Themes;
using LibVLCSharp.Avalonia.Sample.ViewModels;
using LibVLCSharp.Avalonia.Sample.Views;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Avalonia.Sample
{
    public class App : Application
    {
        public override void Initialize()
        {
            Core.Initialize();
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
            }

            var theme = new Themes.Default.DefaultTheme();
            theme.TryGetResource("Button", out _);

            var theme1 = new Themes.Fluent.FluentTheme();
            theme1.TryGetResource("Button", out _);

            base.OnFrameworkInitializationCompleted();
        }
    }
}
