using System;
using System.IO;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    public abstract class BaseSetup
    {
        [SetUp]
        public void SetUp()
        {
            Core.Initialize();
        }

        protected string RealStreamMediaPath => "http://streams.videolan.org/streams/mp3/Owner-MPEG2.5.mp3";

        protected string RealMp3Path =>
            Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "sample.mp3");

        protected string RealMp3PathSpecialCharacter =>
            Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "motörhead.mp3");
    }
}