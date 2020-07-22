using System.Threading.Tasks;
using LibVLCSharp.Shared;
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
        public async Task PostLogin()
        {
            var tcs = new TaskCompletionSource<bool>();

            _libVLC.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, username, store, token) =>
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
                Media = new Media(_libVLC, UrlRequireAuth, FromType.FromLocation)
            };

            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        [Retry(3)]
        public async Task ShouldThrowIfPostLoginsTwice()
        {
            var tcs = new TaskCompletionSource<bool>();

            _libVLC.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, username, store, token) =>
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
                Media = new Media(_libVLC, UrlRequireAuth, FromType.FromLocation)
            };

            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        [Retry(3)]
        public async Task ShouldNotThrowAndReturnFalseIfDimissingTwice()
        {
            var tcs = new TaskCompletionSource<bool>();

            _libVLC.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, username, store, token) =>
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
                Media = new Media(_libVLC, UrlRequireAuth, FromType.FromLocation)
            };

            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        public void ShouldUnsetDialogHandlersWhenInstanceDisposed()
        {
            _libVLC.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, username, store, token) => Task.CompletedTask,
                (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
                (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
                (dialog, position, text) => Task.CompletedTask);

            Assert.True(_libVLC.DialogHandlersSet);

            _libVLC.Dispose();

            Assert.False(_libVLC.DialogHandlersSet);
        }
    }
}
