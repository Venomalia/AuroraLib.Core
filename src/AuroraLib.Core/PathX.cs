using AuroraLib.Core.Extensions;
using System;
using System.Diagnostics;
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

#if NET5_0_OR_GREATER
        /// <inheritdoc cref="Path.GetExtension(ReadOnlySpan{char})"/>
        public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> path) => Path.GetExtension(path);
#else
        /// <inheritdoc cref="Path.GetExtension(string)"/>
        public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> path)
        {
            int dotIndex = path.LastIndexOf(ExtensionSeparatorChar);
            if (dotIndex == -1 || path.Slice(dotIndex).IndexOfAny(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) != -1)
            {
                return ReadOnlySpan<char>.Empty;
            }
            return path.Slice(dotIndex);
        }
#endif

#if NET5_0_OR_GREATER
        /// <inheritdoc cref="Path.Join(ReadOnlySpan{char}, ReadOnlySpan{char})"/>
        public static string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2) => Path.Join(path1, path2);

        /// <inheritdoc cref="Path.Join(ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char})"/>
        public static string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3) => Path.Join(path1, path2, path3);
#else
        /// <inheritdoc cref="Path.Combine(string, string)"/>
        public static string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2)
        {
            if (path1.IsEmpty)
                return path2.ToString();
            if (path2.IsEmpty)
                return path1.ToString();

            return JoinInternal(path1, path2);
        }

        /// <inheritdoc cref="Path.Combine(string, string, string)"/>
        public static string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3)
        {
            if (path1.IsEmpty)
                return Join(path2, path3);
            if (path2.IsEmpty)
                return Join(path1, path3);
            if (path3.IsEmpty)
                return Join(path1, path2);

            return JoinInternal(path1, path2, path3);
        }

        private static string JoinInternal(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
        {
            bool hasSeparator = IsDirectorySeparator(first[first.Length - 1]) || IsDirectorySeparator(second[0]);

            return hasSeparator ?
                SpanEx.StringConcat(first, second) :
                SpanEx.StringConcat(first, DirectorySeparatorString.AsSpan(), second);
        }

        private static unsafe string JoinInternal(ReadOnlySpan<char> first, ReadOnlySpan<char> second, ReadOnlySpan<char> third)
        {
            Debug.Assert(first.Length > 0 && second.Length > 0 && third.Length > 0, "should have dealt with empty paths");

            bool firstHasSeparator = IsDirectorySeparator(first[first.Length - 1]) || IsDirectorySeparator(second[0]);
            bool secondHasSeparator = IsDirectorySeparator(second[second.Length - 1]) || IsDirectorySeparator(third[0]);

            return (firstHasSeparator, secondHasSeparator) switch
            {
                (false, false) => SpanEx.StringConcat(first, SpanEx.StringConcat(DirectorySeparatorString.AsSpan(), second, DirectorySeparatorString.AsSpan()).AsSpan(), third),
                (false, true) => SpanEx.StringConcat(first, DirectorySeparatorString.AsSpan(), second, third),
                (true, false) => SpanEx.StringConcat(first, second, DirectorySeparatorString.AsSpan(), third),
                (true, true) => SpanEx.StringConcat(first, second, third),
            };
        }

        private static readonly string DirectorySeparatorString = Path.DirectorySeparatorChar.ToString();
#endif

#if NET5_0_OR_GREATER
        /// <inheritdoc cref="Path.GetDirectoryName(ReadOnlySpan{char})"/>
        public static ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path) => Path.GetDirectoryName(path);
#else
        /// <inheritdoc cref="Path.GetDirectoryName(string)"/>

        public static ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path)
        {
            if (path.IsEmpty)
                return ReadOnlySpan<char>.Empty;

            int end = GetDirectoryNameOffset(path);

            return end >= 0 ? path.Slice(0, end) : ReadOnlySpan<char>.Empty;
        }

        internal static int GetDirectoryNameOffset(ReadOnlySpan<char> path)
        {
            int rootLength = GetRootLength(path);
            int end = path.Length;

            if (end <= rootLength)
                return -1;

            int lastSeparator = path.LastIndexOfAny(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return lastSeparator > rootLength ? lastSeparator : -1;
        }

        internal static int GetRootLength(ReadOnlySpan<char> path)
        {
            if (path.Length >= 2 && path[1] == Path.VolumeSeparatorChar)
                return 3;

            return path.Length >= 1 && IsDirectorySeparator(path[0]) ? 1 : 0;
        }

        private static bool IsDirectorySeparator(char ch)
            => ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar;
#endif

#if NET5_0_OR_GREATER
        /// <inheritdoc cref="Path.GetFileNameWithoutExtension(ReadOnlySpan{char})"/>
        public static ReadOnlySpan<char> GetFileNameWithoutExtension(ReadOnlySpan<char> path) => Path.GetFileNameWithoutExtension(path);
#else
        /// <inheritdoc cref="Path.GetFileNameWithoutExtension(string)"/>
        public static ReadOnlySpan<char> GetFileNameWithoutExtension(ReadOnlySpan<char> path)
        {
            ReadOnlySpan<char> fileName = GetFileName(path);
            int lastPeriod = fileName.LastIndexOf('.');
            return lastPeriod < 0 ? fileName : fileName.Slice(0, lastPeriod);
        }
#endif

#if NET5_0_OR_GREATER
        /// <inheritdoc cref="Path.GetFileName(ReadOnlySpan{char})"/>
        public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path) => Path.GetFileName(path);
#else
        /// <inheritdoc cref="Path.GetFileName(string)"/>
        public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path)
        {
            int lastSeparator = path.LastIndexOfAny(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (lastSeparator == -1)
                return path;

            return path.Slice(lastSeparator + 1);
        }
#endif


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
