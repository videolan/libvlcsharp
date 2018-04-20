using System.Threading.Tasks;
using NUnit.Framework;
using VideoLAN.LibVLCSharp;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class DialogTests : BaseSetup
    {
        const string UrlRequireAuth = "http://httpbin.org/basic-auth/user/passwd";
        const string Username = "username";
        const string Password = "password";

        [Test]
        public async Task PostLogin()
        {
            var instance = new Instance();
            var tcs = new TaskCompletionSource<bool>();

            instance.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, username, store, token) =>
                {
                    // show UI dialog
                    // On "OK" call PostLogin
                    dialog.PostLogin(Username, Password, false);
                    tcs.SetResult(true);
                    return Task.CompletedTask;
                },
                (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
                (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
                (dialog, position, text) => Task.CompletedTask);

            var mp = new MediaPlayer(instance)
            {
                Media = new Media(instance, UrlRequireAuth, Media.FromType.FromLocation)
            };

            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        public async Task ShouldThrowIfReusingSameDialogAfterLoginCall()
        {
            var instance = new Instance();
            var tcs = new TaskCompletionSource<bool>();

            instance.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, username, store, token) =>
                {
                    dialog.PostLogin(Username, Password, false);
                    Assert.Throws<VLCException>(() => dialog.PostLogin(Username, Password, false), "Calling method on dismissed Dialog instance");
                    tcs.SetResult(true);
                    return Task.CompletedTask;
                },
                (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
                (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
                (dialog, position, text) => Task.CompletedTask);

            var mp = new MediaPlayer(instance)
            {
                Media = new Media(instance, UrlRequireAuth, Media.FromType.FromLocation)
            };

            mp.Play();

            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        public void ShouldUnsetDialogHandlersWhenInstanceDisposed()
        {
            var instance = new Instance();

            instance.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, username, store, token) => Task.CompletedTask,
                (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
                (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
                (dialog, position, text) => Task.CompletedTask);

            Assert.True(instance.DialogHandlersSet);

            instance.Dispose();

            Assert.False(instance.DialogHandlersSet);
        }
    }
}