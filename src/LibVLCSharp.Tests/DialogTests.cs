using System;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class DialogTests : BaseSetup
    {
        const string UrlRequireAuth = "http://httpbin.org/basic-auth/user/passwd";
        const string Username = "username";
        const string Password = "password";

        [Test]
        [Retry(3)]
        [Ignore("requires network calls that may fail when run from CI")]
        public async Task PostLogin()
        {
            var tcs = new TaskCompletionSource<bool>();

            _libVLC.SetDialogHandlers((dialog, title, text, username, store, token) =>
            {
                // show UI dialog
                // On "OK" call PostLogin
                dialog.PostLogin(Username, Password, false);
                tcs.TrySetResult(true);
                return Task.CompletedTask;
            },
            (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
            (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
            (dialog, position, text) => Task.CompletedTask);

            var mp = new MediaPlayer(_libVLC)
            {
                Media = new Media(UrlRequireAuth, FromType.FromLocation)
            };

            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        [Retry(3)]
        [Ignore("requires network calls that may fail when run from CI")]
        public async Task ShouldThrowIfPostLoginsTwice()
        {
            var tcs = new TaskCompletionSource<bool>();

            _libVLC.SetDialogHandlers((dialog, title, text, username, store, token) =>
            {
                dialog.PostLogin(Username, Password, false);
                Assert.Throws<VLCException>(() => dialog.PostLogin(Username, Password, false), "Calling method on dismissed Dialog instance");
                tcs.TrySetResult(true);
                return Task.CompletedTask;
            },
            (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
            (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
            (dialog, position, text) => Task.CompletedTask);

            var mp = new MediaPlayer(_libVLC)
            {
                Media = new Media(UrlRequireAuth, FromType.FromLocation)
            };

            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        [Retry(3)]
        [Ignore("requires network calls that may fail when run from CI")]
        public async Task ShouldNotThrowAndReturnFalseIfDimissingTwice()
        {
            var tcs = new TaskCompletionSource<bool>();

            _libVLC.SetDialogHandlers((dialog, title, text, username, store, token) =>
            {
                var result = dialog.Dismiss();
                Assert.IsTrue(result);
                result = dialog.Dismiss();
                Assert.IsFalse(result);
                tcs.TrySetResult(true);
                return Task.CompletedTask;
            },
            (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
            (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
            (dialog, position, text) => Task.CompletedTask);

            var mp = new MediaPlayer(_libVLC)
            {
                Media = new Media(UrlRequireAuth, FromType.FromLocation)
            };

            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        public void ShouldUnsetDialogHandlersWhenInstanceDisposed()
        {
            _libVLC.SetDialogHandlers((dialog, title, text, username, store, token) => Task.CompletedTask,
                (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
                (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
                (dialog, position, text) => Task.CompletedTask);

            Assert.True(_libVLC.DialogHandlersSet);

            _libVLC.Dispose();

            Assert.False(_libVLC.DialogHandlersSet);
        }

        [Test]
        [Ignore("depends on LibVLC error dialog callbacks during media opening and can hang under LibVLC 4")]
        public async Task ShouldRaiseErrorCallback()
        {
            var tcs = new TaskCompletionSource<bool>();

            _libVLC.SetErrorDialogCallback(DisplayError);
            var missingFile = Path.Combine(TestContext.CurrentContext.WorkDirectory, "missing-media-file.mp4");
            using var media = new Media(new Uri(missingFile));
            using var mp = new MediaPlayer(_libVLC, media);
            mp.Play();

            Task DisplayError(string title, string error)
            {
                try
                {
                    Assert.AreEqual("Your input can't be opened", title);
                    StringAssert.Contains("VLC is unable to open the MRL", error);
                    StringAssert.Contains("Check the log for details.", error);
                    tcs.TrySetResult(true);
                }
                catch(Exception ex)
                {
                    tcs.TrySetException(ex);
                }
                return Task.CompletedTask;
            }

            Assert.AreSame(tcs.Task, await Task.WhenAny(tcs.Task, Task.Delay(5000)));
            Assert.True(await tcs.Task);
        }
    }
}
