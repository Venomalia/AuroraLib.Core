using System;
using System.Collections.Generic;
using System.IO;

namespace AuroraLib.Core.Format
{
    /// <summary>
    /// Defines an interface for format recognition.
    /// </summary>
    public interface IFormatRecognition
    {
        /// <summary>
        /// Determines whether the content of a provided stream matches the format criteria.
        /// </summary>
        /// <param name="stream">The data stream to be analyzed for format detection.</param>
        /// <param name="fileNameAndExtension">The file name and extension (optional).</param>
        /// <returns><c>true</c> if the content matches the format criteria; otherwise, <c>false</c>.</returns>
        bool IsMatch(Stream stream, ReadOnlySpan<char> fileNameAndExtension = default);
    }
#if NET8_0_OR_GREATER
    public interface IStaticFormatRecognition : IFormatRecognition
    {
        /// <inheritdoc cref="IFormatRecognition.IsMatch(Stream, ReadOnlySpan{char})"/>
        static abstract bool IsMatchStatic(Stream stream, ReadOnlySpan<char> fileNameAndExtension = default);
    }
#endif

}
