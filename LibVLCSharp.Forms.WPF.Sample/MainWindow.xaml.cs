using LibVLCSharp.Forms.Platforms.WPF;
using LibVLCSharp.Forms.Shared;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms.Platform.WPF;

namespace LibVLCSharp.Forms.WPF.Sample
{
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            InitializeComponent();
            InitDependencies();
            Xamarin.Forms.Forms.Init();
            LoadApplication(new Forms.Sample.App());
        }

        void InitDependencies()
        {
            var init = new List<Assembly>
            {
                typeof(VideoView).Assembly,
                typeof(VideoViewRenderer).Assembly
            };
        }
    }
}