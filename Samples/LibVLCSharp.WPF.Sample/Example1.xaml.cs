using System;
using System.Windows;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Example1 : Window
    {
        readonly Controls _controls;

        public Example1()
        {
            InitializeComponent();

            _controls = new Controls(this);
            VideoView.Content = _controls;
        }

        protected override void OnClosed(EventArgs e)
        {
            VideoView.Dispose();
        }
    }
}
