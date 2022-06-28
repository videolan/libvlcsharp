using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class PictureTests : BaseSetup
    {
        [Test]
        public async Task RetrieveAttachedThumbnails()
        {
            using var media = new Media(_libVLC, new Uri(AttachedThumbnailsMedia));
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
