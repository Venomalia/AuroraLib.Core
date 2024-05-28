using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AuroraLib.Core
{
    public static class PathX
    {
        public const char ExtensionSeparatorChar = '.';

        /// <summary>
        /// Gets the relative path from the given path to the main path.
        /// </summary>
        /// <param name="path">The path for which to obtain the relative path.</param>
        /// <param name="MainPath">The main path to which the relative path should be determined.</param>
        /// <returns>The relative path from the given path to the main path.</returns>
        public static ReadOnlySpan<char> GetRelativePath(ReadOnlySpan<char> path, ReadOnlySpan<char> MainPath)
        {
            if (path.StartsWith(MainPath))
                return path.Slice(MainPath.Length, path.Length).TrimStart(Path.DirectorySeparatorChar);
            return path;
        }

        /// <summary>
        /// Gets the file name without the file extension from the given path.
        /// </summary>
        /// <param name="path">The path from which to retrieve the file name without extension.</param>
        /// <returns>The file name without the file extension.</returns>
        public static ReadOnlySpan<char> GetFileWithoutExtension(ReadOnlySpan<char> path)
        {
            if (path.LastIndexOf(ExtensionSeparatorChar) > path.LastIndexOf(Path.DirectorySeparatorChar))
                return path.Slice(0, path.LastIndexOf(ExtensionSeparatorChar));
            return path;
        }

        /// <summary>
        /// Checks if the given path contains any invalid characters according to the operating system's rules for path names.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path contains any invalid characters; otherwise, false.</returns>
        public static bool CheckInvalidPathChars(ReadOnlySpan<char> path)
            => path.IndexOfAny(Path.GetInvalidPathChars()) == 0;

        /// <summary>
        /// Converts a potentially invalid file path to a valid one by replacing or removing illegal characters.
        /// </summary>
        /// <param name="path">The path to convert.</param>
        /// <returns>A valid file path with illegal characters replaced or removed.</returns>
        public static string GetValidPath(string path)
        {
            Match match = illegalChars.Match(path);
            while (match.Success)
            {
                int index = match.Index;
                if (match.Groups["X"].Success)
                    index = match.Groups["X"].Index;
                char llegalChar = path[index];
                path = path.Remove(index, 1);
                path = path.Insert(index, $"{(byte)llegalChar:X2}");
                match = illegalChars.Match(path);
            }
            return path.TrimEnd(' ', '\\', '/');
        }
        internal static readonly Regex illegalChars = new Regex(@"^(.*(//|\\))?(?'X'PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*| )((//|\\).*)?$|[\x00-\x1f\x7F?*:""<>|]| ((//|\\).*)?$", RegexOptions.CultureInvariant);

        /// <summary>
        /// Converts a byte size into a human-readable string with size suffixes.
        /// </summary>
        /// <param name="value">The size value to convert.</param>
        /// <param name="decimalPlaces">The number of decimal places to round the adjusted size.</param>
        /// <param name="suffixes">The type of size suffixes to use (decimal or binary).</param>
        /// <returns>A string representation of the size with the appropriate size suffix.</returns>
        public static string AddSizeSuffix(long value, int decimalPlaces = 0, SuffixesType suffixes = SuffixesType.Decimal)
        {
            int ex = (int)Math.Max(0, Math.Log(value, (int)suffixes));
            double adjustedSize = Math.Round(value / Math.Pow((int)suffixes, ex), decimalPlaces);
            if (suffixes == SuffixesType.Decimal)
            {
                return adjustedSize + DecimalSizeSuffixes[ex];
            }
            return adjustedSize + BinarySizeSuffixes[ex];
        }
        private static readonly string[] DecimalSizeSuffixes = { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "RB", "QB" };
        private static readonly string[] BinarySizeSuffixes = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB", "RiB", "QiB" };

        /// <summary>
        /// The type of size suffixes to use.
        /// </summary>
        public enum SuffixesType : int
        {
            Decimal = 1000,
            Binary = 1024,
        }
    }
}
