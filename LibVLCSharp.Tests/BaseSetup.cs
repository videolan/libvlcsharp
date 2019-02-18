using System;
using System.IO;
using System.Reflection;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    public abstract class BaseSetup
    {
        protected LibVLC _libVLC;

        [SetUp]
        public void SetUp()
        {
            Core.Initialize();

            _libVLC = new LibVLC("--no-audio", "--no-video");
        }

        protected string RealStreamMediaPath => "http://streams.videolan.org/streams/mp3/Owner-MPEG2.5.mp3";

        protected string RealMp3Path => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "sample.mp3");

        protected string RealMp3PathSpecialCharacter => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "motörhead.mp3");
    }
}