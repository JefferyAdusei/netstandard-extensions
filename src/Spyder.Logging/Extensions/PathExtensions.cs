using System;
using System.IO;
using System.Runtime.InteropServices;
using Spyder.Logging.Models;

namespace Spyder.Logging.Extensions
{
    internal static class PathExtensions
    {
        /// <summary>
        /// Normalizes a path based on the current operating system
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>The normalized path</returns>
        public static string? NormalizePath(this string path) => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? path?.Replace('/', '\\').Trim()
            : path?.Replace('\\', '/').Trim();

        /// <summary>
        /// Resolves any relative elements of the path to absolute
        /// </summary>
        /// <param name="path">The path to resolve</param>
        /// <returns>The resolved path</returns>
        public static string ResolvePath(this string? path) => Path.GetFullPath(path!);

        /// <summary>
        /// Generates a file name for logs depending on the user's roll over choice
        /// </summary>
        /// <param name="filepath">The file name to attach to roll over name</param>
        /// <param name="roll">The roll over choice of the user</param>
        /// <returns>The generated file name based of the roll over choice</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string PathRoll(this string filepath, Roll? roll)
        {
            var today = DateTime.Today;
            var filename = Path.GetFileName(filepath);

            switch (roll)
            {
                case Roll.Daily:
                    return $"[{today.Day}][{today.Month}][{today.Year}]{filename}";
                case Roll.Weekly:
                    return $"[{today.Day}][{today.Month}][{today.Date}]{filename}";
                case Roll.Monthly:
                    return $"[{today.Month}][{today.Year}]{filename}";
                case Roll.Yearly:
                    return $"[{today.Year}]{filename}";
                default:
                    return filepath;
            }
        }
    }
}
