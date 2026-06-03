using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;
using NUnit.Framework;

namespace LibVLCSharp.Avalonia.Tests
{
    public class VideoViewTests
    {
        [TestCase(true)]
        [TestCase(false)]
        [AvaloniaTest]
        public void VideoView_Should_Be_Removed_From_VisualTree_When_Parent_Is_Removed(bool withContent)
        {
            // Setup controls:
            var videoView = new VideoView
            {
                Name = "VideoView",
                Content = withContent ? new TextBox() { Name = "VideoViewTextBox", Text = "" } : null
            };
            var secondView = new TextBox() { Name = "Second", Text = "" };
            var window = new Window { Content = videoView };

            // Open window:
            window.Show();
            try
            {                
                // change Content of window:
                window.Content = secondView;
                // wait for the UI to process the change:
                window.Dispatcher.RunJobs();
            }
            catch
            {
                // fail the test if any exception occurs during the process
                Assert.Fail("An exception occurred while changing the window content.");
            }

            var RemovedVideoView = window.FindDescendantOfType<VideoView>(false, t => t.Name == "VideoView");
            var secondTextBox = window.FindDescendantOfType<TextBox>(false, t => t.Name == "Second");
            Assert.That(RemovedVideoView, Is.Null);
            Assert.That(secondTextBox, Is.Not.Null);
        }
    }
}
