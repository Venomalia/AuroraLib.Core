using AuroraLib.Core.Format.Identifier;
using System;
using System.Collections.Generic;

namespace AuroraLib.Core.Format
{
    /// <summary>
    /// Provides information for a specific file format.
    /// </summary>
    public interface IFormatInfo : IFormatRecognition, IEquatable<IFormatInfo>
    {
        /// <summary>
        /// Gets the full name of the format.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Gets the MIME type of the format.
        /// </summary>
        MediaType MIMEType { get; }

        /// <summary>
        /// Gets the associated file extensions.
        /// </summary>
        IEnumerable<string> FileExtensions { get; }

        /// <summary>
        /// Gets the format identifier (Magic number), if available.
        /// </summary>
        IIdentifier? Identifier { get; }

        /// <summary>
        /// Gets the identifier offset.
        /// </summary>
        int IdentifierOffset { get; }

        /// <summary>
        /// Gets the associated class type.
        /// </summary>
        Type? Class { get; }

        /// <summary>
        /// Creates a new instance of the associated type.
        /// </summary>
        object? CreateInstance();
    }

}
