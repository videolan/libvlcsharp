using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
            var libVLC = new LibVLC();
            libVLC.Dispose();
            Assert.AreEqual(IntPtr.Zero, libVLC.NativeReference);
        }

        [Test]
        public void AddInterface()
        {
            var libVLC = new LibVLC();
            Assert.True(libVLC.AddInterface(string.Empty));
        }

        [Test]
        public void AudioFilters()
        {
            var libVLC = new LibVLC();
            var audioFilters = libVLC.AudioFilters;
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
            var libVLC = new LibVLC();
            var videoFilters = libVLC.VideoFilters;
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
            var libVLC = new LibVLC();
            var audioOuputs = libVLC.AudioOutputs;
            foreach (var audioOutput in audioOuputs)
            {
                Debug.WriteLine(audioOutput.Name);
                Debug.WriteLine(audioOutput.Description);
            }
        }

        [Test]
        public void AudioOutputDevices()
        {
            var libVLC = new LibVLC();
            var outputs = libVLC.AudioOutputs;
            var name = outputs.First(output => output.Name.Equals("mmdevice")).Name;
            var audioOutputDevices = libVLC.AudioOutputDevices(name);

            foreach (var audioOutputDevice in audioOutputDevices)
            {
                Debug.WriteLine(audioOutputDevice.Description);
                Debug.WriteLine(audioOutputDevice.DeviceIdentifier);
            }
        }

        [Test]
        public void EqualityTests()
        {
            Assert.AreNotSame(new LibVLC(), new LibVLC());
        }

        [Test]
        public void Categories()
        {
            var libVLC = new LibVLC();
            var md1 = libVLC.MediaDiscoverers(MediaDiscovererCategory.Devices);
            var md2 = libVLC.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md3 = libVLC.MediaDiscoverers(MediaDiscovererCategory.Localdirs);
        }

        [Test]
        public void SetExitHandler()
        {
            var libVLC = new LibVLC();
            var called = false;

            var exitCb = new ExitCallback(() =>
            {
                called = true;
            });

            libVLC.SetExitHandler(exitCb, IntPtr.Zero);

            libVLC.Dispose();

            Assert.IsTrue(called);
        }

        [Test]
        public async Task SetLogCallback()
        {
            var libVLC = new LibVLC();
            var logCallbackCalled = false;

            libVLC.SetLogCallback(message =>
            {
                if (message.Message.Contains("VLC media player"))
                {
                    logCallbackCalled = true;
                }
            }, LibVLCLogLevel.Debug);

            await Task.Delay(1000);
            
            libVLC.SetLogCallback(null, LibVLCLogLevel.Error);

            Assert.IsTrue(logCallbackCalled);
        }
        
        [Test]
        public void SetLogFile()
        {
            var libVLC = new LibVLC();
            var path = Path.GetTempFileName();
            libVLC.SetLogFile(path);
            libVLC.UnsetLog();
            var logs = File.ReadAllText(path);
            Assert.True(logs.StartsWith("VLC media player"));
        }

        [Test]
        public void DisposeLibVLC()
        {
            var libvlc = new LibVLC();

            libvlc.SetLogCallback(message => { }, LibVLCLogLevel.Error);
            libvlc.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, defaultUsername, askStore, token) => Task.CompletedTask,
                (dialog, title, text, type, cancelText, firstActionText, secondActonText, token) => Task.CompletedTask,
                (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
                (dialog, position, text) => Task.CompletedTask);

            Assert.IsTrue(libvlc.DialogHandlersSet);

            libvlc.Dispose();

            Assert.AreEqual(IntPtr.Zero, libvlc.NativeReference);
            Assert.IsFalse(libvlc.DialogHandlersSet);
        }
    }
}