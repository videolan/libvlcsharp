using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaParserTests
    {
        [Test]
        public void ParserTaskGetMediaBindingIsAvailable()
        {
            NativeBindingAssertions.HasDllImport(typeof(MediaParser), "LibVLCParserTaskGetMedia", "libvlc_parser_task_get_media");
        }
    }
}
