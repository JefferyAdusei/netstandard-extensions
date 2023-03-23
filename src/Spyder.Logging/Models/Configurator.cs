using Microsoft.Extensions.Logging;
using Spyder.Logging.Extensions;

namespace Spyder.Logging.Models
{
    public class Configurator
    {
        #region public Properties

        /// <summary>
        /// Gets or sets the path to log the file
        /// </summary>
        public string Path { get; set; } = "log";


        /// <summary>
        /// Gets or sets the level of the log that should be processed.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// Gets or sets the time event on which to roll the log to another file
        /// </summary>
        public Roll Roll { get; set; } = Roll.Daily;

        /// <summary>
        /// Gets or sets a value indicating whether the log level should be
        /// part of the log message.
        /// </summary>
        public bool OutputLogLevel { get; set; } = true;

        #endregion

        #region Generated Properties

        /// <summary>
        /// Gets the OS normalized and resolved file path to the path specified
        /// </summary>
        public string FilePath => Path.PathRoll(Roll).NormalizePath().ResolvePath();

        #endregion
    }
}
