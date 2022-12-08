namespace Spyder.Extensions.Logging
{
    using System;
    using System.IO;

    /// <summary>
    /// Contains extensions that formats the message including
    /// the source information pulled out of the state
    /// </summary>
    public static class LoggerSourceFormatter
    {
        /// <summary>
        /// Formats the message including the source information pulled out of the state
        /// </summary>
        /// <param name="state">The state information about the log</param>
        /// <param name="exception">The exception associated with the log</param>
        /// <returns>The formatted message</returns>
        public static string Format(object[] state, Exception? exception = null)
        {
            // Get the values from the state
            string origin = (string)state[0];
            string filepath = (string)state[1];
            int lineNumber = (int)state[2];
            string message = (string)state[3];

            // Get any exception message
            string? exceptionMessage = exception?.Source;

            if (exception != null)
            {
                exceptionMessage = $"\n source:{exception.Source} >: {exception.Message}";
            }

            return $"[{Path.GetFileName(filepath)} > {origin} () > Line {lineNumber}] \n{message}{exceptionMessage}";
        }
    }
}
