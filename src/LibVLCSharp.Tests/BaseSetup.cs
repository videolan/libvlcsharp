using System;
using System.IO;
using System.Reflection;
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
            Core.Initialize();

            _libVLC = new LibVLC(/*"--no-audio", "--no-video", */"--verbose=2");
        }

        protected string RemoteAudioStream => "http://streams.videolan.org/streams/mp3/Owner-MPEG2.5.mp3";

        protected string RemoteVideoStream => "https://streams.videolan.org/streams/mp4/Jago-Youtube.mp4";

        protected string LocalAudioFile => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "sample.mp3");

        protected string LocalAudioFileSpecialCharacter => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "motörhead.mp3");

        protected string AttachedThumbnailsMedia => Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "multiple-images.mp3");

    }
}
