﻿using System.Windows;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Example1 : Window
    {
        readonly Controls _controls;

        public Example1()
        {
            InitializeComponent();

            _controls = new Controls(this);
            Player.Content = _controls;
        }
    }
}