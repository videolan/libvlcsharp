using System;
using System.IO;
using System.Reflection;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    public abstract class BaseSetup
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable. It is initialized in the SetUp, so before the tests take place.
        protected LibVLC _libVLC;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        [SetUp]
        public void SetUp()
        {
            _libVLC = new LibVLC("--no-audio", "--no-video");
        }

        protected string RealStreamMediaPath => "http://streams.videolan.org/streams/mp3/Owner-MPEG2.5.mp3";

        protected string RealMp3Path => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "sample.mp3");

        protected string RealMp3PathSpecialCharacter => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "motörhead.mp3");
    }
}
