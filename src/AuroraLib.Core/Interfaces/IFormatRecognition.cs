namespace AuroraLib.Core.Interfaces
{
    /// <summary>
    /// Defines an interface for format recognition.
    /// </summary>
    public interface IFormatRecognition
    {
        /// <summary>
        /// Determines whether the content of a provided stream matches the format criteria.
        /// </summary>
        /// <param name="stream">The data stream to be examined.</param>
        /// <param name="extension">The optional file extension to consider.</param>
        /// <returns><c>true</c> if the content matches the format criteria; otherwise, <c>false</c>.</returns>
        bool IsMatch(Stream stream, ReadOnlySpan<char> extension = default);
    }
}
