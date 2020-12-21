using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class LibVLCTests : BaseSetup
    {
        [Test]
        public void DisposeInstanceNativeRelease()
        {
            _libVLC.Dispose();
            Assert.AreEqual(IntPtr.Zero, _libVLC.NativeReference);
        }

        [Test]
        public void AudioFilters()
        {
            var audioFilters = _libVLC.AudioFilters;
            foreach (var filter in audioFilters)
            {
                Debug.WriteLine(filter.Help);
                Debug.WriteLine(filter.LongName);
                Debug.WriteLine(filter.Name);
                Debug.WriteLine(filter.ShortName);
            }
        }

        [Test]
        public void VideoFilters()
        {
            var videoFilters = _libVLC.VideoFilters;
            foreach (var filter in videoFilters)
            {
                Debug.WriteLine(filter.LongName);
                Debug.WriteLine(filter.Name);
                Debug.WriteLine(filter.ShortName);
            }
        }

        [Test]
        public void AudioOutputs()
        {
            var audioOuputs = _libVLC.AudioOutputs;
            foreach (var audioOutput in audioOuputs)
            {
                Debug.WriteLine(audioOutput.Name);
                Debug.WriteLine(audioOutput.Description);
            }
        }

        [Test]
        public void AudioOutputDevices()
        {
            var outputs = _libVLC.AudioOutputs;
            var name = outputs.First(output => output.Name.Equals("mmdevice")).Name;
            var audioOutputDevices = _libVLC.AudioOutputDevices(name);

            foreach (var audioOutputDevice in audioOutputDevices)
            {
                Debug.WriteLine(audioOutputDevice.Description);
                Debug.WriteLine(audioOutputDevice.DeviceIdentifier);
            }
        }

        [Test]
        public void EqualityTests()
        {
            Assert.AreNotSame(new LibVLC("--no-audio"), new LibVLC("--no-audio"));
        }

        [Test]
        public void Categories()
        {
            var md1 = _libVLC.MediaDiscoverers(MediaDiscovererCategory.Devices);
            var md2 = _libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md3 = _libVLC.MediaDiscoverers(MediaDiscovererCategory.Localdirs);
        }

        [Test]
        public void SetExitHandler()
        {
            var exitCb = new ExitCallback(() =>
            {
            });

            _libVLC.SetExitHandler(exitCb);
        }

        [Test]
        public void SetLogFile()
        {
            var path = Path.GetTempFileName();
            _libVLC.SetLogFile(path);
            _libVLC.CloseLogFile();
            var logs = File.ReadAllText(path);
            Assert.True(logs.StartsWith("VLC media player"));
        }

        [Test]
        public void DisposeLibVLC()
        {
            _libVLC.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, defaultUsername, askStore, token) => Task.CompletedTask,
                (dialog, title, text, type, cancelText, firstActionText, secondActonText, token) => Task.CompletedTask,
                (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
                (dialog, position, text) => Task.CompletedTask);

            Assert.IsTrue(_libVLC.DialogHandlersSet);

            _libVLC.Dispose();

            Assert.AreEqual(IntPtr.Zero, _libVLC.NativeReference);
            Assert.IsFalse(_libVLC.DialogHandlersSet);
        }

        [Test]
        public void LibVLCVersion()
        {
            Assert.True(_libVLC.Version.StartsWith("3"));
        }

        [Test]
        public void LibVLCChangeset()
        {
            Assert.IsNotNull(_libVLC.Changeset);
        }
    }
}
