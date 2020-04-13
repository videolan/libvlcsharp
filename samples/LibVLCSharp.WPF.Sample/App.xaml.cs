using System.Windows;
using LibVLCSharp;

namespace LibVLCSharp.WPF.Sample
{
    public partial class App : Application
    {
        public App()
        {
            Core.Initialize();
        }
    }
}