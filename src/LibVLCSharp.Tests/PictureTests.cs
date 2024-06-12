using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class PictureTests : BaseSetup
    {
        [Test]
        [Ignore("parsing hanging in unit test")]
        public async Task RetrieveAttachedThumbnails()
        {
            using var media = new Media(new Uri(AttachedThumbnailsMedia));
            uint thumbnailsFound = 0;
            media.AttachedThumbnailsFound += (sender, args) =>
            {
                thumbnailsFound = args.AttachedThumbnails.Count;
                foreach(var thumbnail in args.AttachedThumbnails)
                {
                    Assert.AreEqual(PictureType.Png, thumbnail.Type);
                }
            };
            await media.ParseAsync(_libVLC);
            Assert.AreEqual(2, thumbnailsFound);
        }
    }
}
