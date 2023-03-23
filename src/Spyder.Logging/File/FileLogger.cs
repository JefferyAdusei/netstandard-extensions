using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Spyder.Extensions.Task.Locks;
using Spyder.Logging.Models;

namespace Spyder.Logging.File
{
    /// <inheritdoc />
    /// <summary>
    /// A logger that writes logs as normal text to file
    /// </summary>
    public class FileLogger : ILogger
    {
        #region Private Members

        /// <summary>
        /// The path to the directory the log file is in.
        /// </summary>
        private readonly string _directory;

        /// <summary>
        /// The log settings to use.
        /// </summary>
        private readonly Configurator _configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="configuration">The configuration to use</param>
        public FileLogger(Configurator configuration)
        {
            // Set members
            _directory = Path.GetDirectoryName(configuration.FilePath )!;
            _configuration = configuration;
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets a unique key to lock file access
        /// </summary>
        private static string FileLock => nameof(FileLogger) + Guid.NewGuid();

        #endregion        

        #region Implementation of ILogger

        /// <summary>
        /// Logs the message to file
        /// </summary>
        /// <typeparam name="TState">The type for the state</typeparam>
        /// <param name="logLevel">The log level</param>
        /// <param name="eventId">The event Id</param>
        /// <param name="state">The details of the message</param>
        /// <param name="exception">Any exception to add to the log</param>
        /// <param name="formatter">The formatter for converting the state and exception to a message string</param>
        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            // If we should not log...
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Lock the file
            await AsyncLock.LockAsync(FileLock, async () =>
            {
                // Ensure folder exists
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }

                // Open the file
                await using StreamWriter fileStream =
                    new(System.IO.File.Open(_configuration.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                                               FileShare.ReadWrite));
                fileStream.BaseStream.Seek(0, SeekOrigin.End);

                // Write the message to the file
                await fileStream.WriteLineAsync($"{logLevel}: {DateTimeOffset.Now:g} {formatter(state, exception)}");

                // Flush the stream
                await fileStream.FlushAsync();

                return await Task.FromResult(true);
            });
        }

        /// <inheritdoc />
        /// Enabled if the log level is the same or greater than the configuration
        public bool IsEnabled(LogLevel logLevel) => logLevel >= _configuration.LogLevel;

        /// <inheritdoc />
        /// File loggers are not scoped so this is always null
        public IDisposable? BeginScope<TState>(TState state)
        {
            return null;
        }

        #endregion
    }
}
