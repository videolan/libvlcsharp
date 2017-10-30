using System;
using System.Diagnostics;
using System.Linq;
using libvlcsharp;
using NUnit.Framework;

namespace Bindings.Tests
{
    [TestFixture]
    public class InstanceTests
    {
        [Test]
        public void DisposeInstanceNativeRelease()
        {
            var instance = new Instance(0, null);
            instance.Dispose();
            Assert.AreEqual(IntPtr.Zero, instance.NativeReference);
        }

        [Test]
        public void AddInterface()
        {
            var instance = new Instance(0, null);
            Assert.True(instance.AddInterface(string.Empty));
        }

        [Test]
        public void AudioFilters()
        {
            var instance = new Instance(0, null);
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
            var instance = new Instance(0, null);
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
            var instance = new Instance(0, null);
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
            var instance = new Instance(0, null);
            var outputs = instance.AudioOutputs;
            var name = outputs.Last().Name;
            var audioOutputDevices = instance.AudioOutputDevices(name);

            foreach (var audioOutputDevice in audioOutputDevices)
            {
                //Debug.WriteLine(audioOutputDevice.Description);
                Debug.WriteLine(audioOutputDevice.Device);
            }
        }
    }
}