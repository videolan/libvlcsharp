using System;
using System.Collections.Generic;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class CoreLoggingTests
    {
        [Test]
        [Platform("Win")]
        public void InitializeWithLogger_SendsLoadMessagesToFactory()
        {
            var factory = new RecordingLoggerFactory();

            try
            {
                // A directory with no libvlc files: the loader logs "Cannot find ..." and then fails.
                Core.InitializeWithLogger(factory, @"C:\this-directory-does-not-exist");
            }
            catch (Exception)
            {
                // Expected: no native libvlc is present in the test environment.
            }

            Assert.That(factory.Messages, Has.Some.StartWith("Cannot find"));
        }
    }

    internal sealed class RecordingLoggerFactory : ILoggerFactory
    {
        public List<string> Messages { get; } = new List<string>();

        public ILogger CreateLogger(string categoryName) => new RecordingLogger(Messages);

        public void AddProvider(ILoggerProvider provider) { }

        public void Dispose() { }
    }

    internal sealed class RecordingLogger : ILogger
    {
        private readonly List<string> _messages;

        public RecordingLogger(List<string> messages) => _messages = messages;

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
            => _messages.Add(formatter(state, exception));
    }
}
