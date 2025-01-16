using System;
using System.Text;

namespace AuroraLib.Core.Format.Identifier
{
    /// <summary>
    /// Represents an identifier and provides access to individual bytes within the identifier.
    /// </summary>
    public interface IIdentifier : IEquatable<string>, IEquatable<IIdentifier>
    {
        /// <summary>
        /// Returns a <see cref="Span{byte}"/> representation of the identifier.
        /// </summary>
        /// <returns>A <see cref="Span{byte}"/> representing the identifier.</returns>
        Span<byte> AsSpan();

        /// <summary>
        /// Returns the string representation of the identifier.
        /// </summary>
        /// <returns>The string representation of the identifier.</returns>
        string GetString();

        /// <summary>
        /// Returns the string representation of the identifier using the specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The string representation of the identifier.</returns>
        string GetString(Encoding encoding);
    }
}
