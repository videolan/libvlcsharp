using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using VideoLAN.LibVLC;

namespace Bindings.Tests
{
    [TestFixture]
    public class InstanceTests
    {
        [Test]
        public void DisposeInstanceNativeRelease()
        {
            var instance = new Instance();
            instance.Dispose();
            Assert.AreEqual(IntPtr.Zero, instance.NativeReference);
        }

        [Test]
        public void AddInterface()
        {
            var instance = new Instance();
            Assert.True(instance.AddInterface(string.Empty));
        }

        [Test]
        public void AudioFilters()
        {
            var instance = new Instance();
            var audioFilters = instance.AudioFilters;
            foreach (var filter in audioFilters)
            {
                Debug.WriteLine(filter.Help);
                Debug.WriteLine(filter.Longname);
                Debug.WriteLine(filter.Name);
                Debug.WriteLine(filter.Shortname);
            }
        }

        [Test]
        public void VideoFilters()
        {
            var instance = new Instance();
            var videoFilters = instance.VideoFilters;
            foreach (var filter in videoFilters)
            {
                Debug.WriteLine(filter.Longname);
                Debug.WriteLine(filter.Name);
                Debug.WriteLine(filter.Shortname);
            }
        }

        [Test]
        public void AudioOutputs()
        {
            var instance = new Instance();
            var audioOuputs = instance.AudioOutputs;
            foreach (var audioOutput in audioOuputs)
            {
                Debug.WriteLine(audioOutput.Name);
                Debug.WriteLine(audioOutput.Description);
            }
        }

        [Test]
        public void AudioOutputDevices()
        {
            var instance = new Instance();
            var outputs = instance.AudioOutputs;
            var name = outputs.Last().Name;
            var audioOutputDevices = instance.AudioOutputDevices(name);

            foreach (var audioOutputDevice in audioOutputDevices)
            {
                //Debug.WriteLine(audioOutputDevice.Description);
                Debug.WriteLine(audioOutputDevice.Device);
            }
        }

        [Test]
        public void EqualityTests()
        {
            var instance1 = new Instance();
            var instance2 = new Instance();
            Assert.True(instance1 != instance2);
        }

        [Test]
        public void Categories()
        {
            var instance = new Instance();
            var md1 = instance.MediaDiscoverers(MediaDiscovererCategory.Devices);
            var md2 = instance.MediaDiscoverers(MediaDiscovererCategory.Lan);
            var md3 = instance.MediaDiscoverers(MediaDiscovererCategory.Localdirs);
        }

        [Test]
        public void SetExitHandler()
        {
            var instance = new Instance();
            var called = false;

            var exitCb = new ExitCallback(() =>
            {
                called = true;
            });

            instance.SetExitHandler(exitCb, IntPtr.Zero);

            instance.Dispose();

            Assert.IsTrue(called);
        }

        [Test]
        public async Task SetLogCallback()
        {
            var instance = new Instance();
            var logCallbackCalled = false;

            void LogCallback(object sender, LogEventArgs args) => logCallbackCalled = true;

            instance.Log += LogCallback;

            await Task.Delay(1000);

            instance.Log -= LogCallback;

            Assert.IsTrue(logCallbackCalled);
        }
        
        [Test]
        public void SetLogFile()
        {
            Assert.Inconclusive();
            var instance = new Instance(0, null);
            var path = Path.GetTempFileName();
            instance.SetLogFile(new FileStream(path, FileMode.OpenOrCreate));
        }
    }
}