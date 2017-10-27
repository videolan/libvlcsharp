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
        public void InstanceAudioFilters()
        {
            var instance = new Instance(0, null);
            var audioFilters = instance.AudioFilters;
            foreach (var filter in audioFilters)
            {
                Debug.WriteLine(filter.Name);
                //filter.Dispose(); TODO: Fix me
            }
        }

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
    }
}