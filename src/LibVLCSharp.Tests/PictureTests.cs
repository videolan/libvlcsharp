using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class PictureTests : BaseSetup
    {
        [Test]
        public async Task RetrieveAttachedThumbnails()
        {
            using var media = new Media(new Uri(AttachedThumbnailsMedia));
            using var parser = new MediaParser(_libVLC);
            var thumbnails = new List<Picture>();

            var status = await parser.ParseAsync(media, attachmentsAdded: args =>
            {
                args.Media?.Dispose();
                thumbnails.AddRange(args.Pictures);
            });

            Assert.AreEqual(MediaParsedStatus.Done, status);
            Assert.AreEqual(2, thumbnails.Count);
            foreach(var thumbnail in thumbnails)
            {
                Assert.AreEqual(PictureType.Png, thumbnail.Type);
                thumbnail.Dispose();
            }
        }
    }
}
