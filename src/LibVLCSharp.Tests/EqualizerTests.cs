﻿using LibVLCSharp.Shared;
using NUnit.Framework;
using System;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class EqualizerTests : BaseSetup
    {
        [Test]
        public void BasicNativeCallTest()
        {
            var equalizer = new Equalizer();
            equalizer.SetAmp(-1, 1);
            Assert.That(-1 == equalizer.Amp(1));
        }

        [Test]
        public void DisposeEqualizer()
        {
            var equalizer = new Equalizer();
            equalizer.SetAmp(-1, 1);
            equalizer.Dispose();
            Assert.That(IntPtr.Zero == equalizer.NativeReference);
        }
    }
}
