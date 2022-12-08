namespace Spyder.Extensions.Logging.Extensions
{
    using Microsoft.Extensions.Logging;
    using Spyder.Extensions.Types.Array;
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Extensions for loggers implementing the <see cref="ILogger"/> interface.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs a critical log message, including the source of the log
        /// </summary>
        /// <param name="logger">The specific ILogger calling the extension</param>
        /// <param name="message">The log message</param>
        /// <param name="eventId">The log event identifier</param>
        /// <param name="exception">The exception that comes along with the log</param>
        /// <param name="origin">The callers member/function name</param>
        /// <param name="filePath">The source code file path</param>
        /// <param name="lineNumber">The line number in the code file of the caller</param>
        /// <param name="args">The additional arguments</param>
        public static void LogCriticalSource(
            this ILogger logger,
            string message,
            EventId eventId = new EventId(),
            Exception? exception = null,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args) =>
            logger?.Log(LogLevel.Critical, eventId, args.Prepend(origin, filePath, lineNumber, message),
                        exception, LoggerSourceFormatter.Format);

        /// <summary>
        /// Logs a verbose trace message, including the source of the log
        /// </summary>
        /// <param name="logger">The specific ILogger calling the extension</param>
        /// <param name="message">The log message</param>
        /// <param name="eventId">The log event identifier</param>
        /// <param name="exception">The exception that comes along with the log</param>
        /// <param name="origin">The callers member/function name</param>
        /// <param name="filePath">The source code file path</param>
        /// <param name="lineNumber">The line number in the code file of the caller</param>
        /// <param name="args">The additional arguments</param>
        public static void LogTraceSource(
            this ILogger logger,
            string message,
            EventId eventId = new EventId(),
            Exception? exception = null,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args) =>
            logger?.Log(LogLevel.Trace, eventId, args.Prepend(origin, filePath, lineNumber, message),
                        exception, LoggerSourceFormatter.Format);

        /// <summary>
        /// Logs a debug message, including the source of the log
        /// </summary>
        /// <param name="logger">The specific ILogger calling the extension</param>
        /// <param name="message">The log message</param>
        /// <param name="eventId">The log event identifier</param>
        /// <param name="exception">The exception that comes along with the log</param>
        /// <param name="origin">The callers member/function name</param>
        /// <param name="filePath">The source code file path</param>
        /// <param name="lineNumber">The line number in the code file of the caller</param>
        /// <param name="args">The additional arguments</param>
        public static void LogDebugSource(
            this ILogger logger,
            string message,
            EventId eventId = new EventId(),
            Exception? exception = null,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args) =>
            logger?.Log(LogLevel.Debug, eventId, args.Prepend(origin, filePath, lineNumber, message),
                        exception, LoggerSourceFormatter.Format);

        /// <summary>
        /// Logs an error message, including the source of the log
        /// </summary>
        /// <param name="logger">The specific ILogger calling the extension</param>
        /// <param name="message">The log message</param>
        /// <param name="eventId">The log event identifier</param>
        /// <param name="exception">The exception that comes along with the log</param>
        /// <param name="origin">The callers member/function name</param>
        /// <param name="filePath">The source code file path</param>
        /// <param name="lineNumber">The line number in the code file of the caller</param>
        /// <param name="args">The additional arguments</param>
        public static void LogErrorSource(
            this ILogger logger,
            string message,
            EventId eventId = new EventId(),
            Exception? exception = null,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args) =>
            logger?.Log(LogLevel.Error, eventId, args.Prepend(origin, filePath, lineNumber, message),
                        exception, LoggerSourceFormatter.Format);

        /// <summary>
        /// Logs an informative message, including the source of the log
        /// </summary>
        /// <param name="logger">The specific ILogger calling the extension</param>
        /// <param name="message">The log message</param>
        /// <param name="eventId">The log event identifier</param>
        /// <param name="exception">The exception that comes along with the log</param>
        /// <param name="origin">The callers member/function name</param>
        /// <param name="filePath">The source code file path</param>
        /// <param name="lineNumber">The line number in the code file of the caller</param>
        /// <param name="args">The additional arguments</param>
        public static void LogInformationSource(
            this ILogger logger,
            string message,
            EventId eventId = new EventId(),
            Exception? exception = null,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args) =>
            logger?.Log(LogLevel.Information, eventId, args.Prepend(origin, filePath, lineNumber, message),
                        exception, LoggerSourceFormatter.Format);


        /// <summary>
        /// Logs a warning message, including the source of the log
        /// </summary>
        /// <param name="logger">The specific ILogger calling the extension</param>
        /// <param name="message">The log message</param>
        /// <param name="eventId">The log event identifier</param>
        /// <param name="exception">The exception that comes along with the log</param>
        /// <param name="origin">The callers member/function name</param>
        /// <param name="filePath">The source code file path</param>
        /// <param name="lineNumber">The line number in the code file of the caller</param>
        /// <param name="args">The additional arguments</param>
        public static void LogWarningSource(
            this ILogger logger,
            string message,
            EventId eventId = new EventId(),
            Exception? exception = null,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args) =>
            logger?.Log(LogLevel.Warning, eventId, args.Prepend(origin, filePath, lineNumber, message),
                        exception, LoggerSourceFormatter.Format);
    }
}
