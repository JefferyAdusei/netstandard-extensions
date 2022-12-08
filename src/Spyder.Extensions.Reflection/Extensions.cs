namespace Spyder.Extensions.Reflection
{
    using System;
    using System.IO;

    public static class Extensions
    {
        /// <summary>
        /// Gets the physical file location of the assembly where this type is stored
        /// </summary>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        /// <returns>Returns the file location of the assembly in which this type is stored</returns>
        public static string FileLocation(this Type type) => type.Assembly.Location;

        /// <summary>
        /// Gets the physical folder location of the assembly where this type is stored
        /// </summary>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        /// <returns>Returns the folder location of the assembly file in which this type is stored</returns>
        public static string FolderLocation(this Type type) => Path.GetDirectoryName(type.FileLocation())!;
    }
}