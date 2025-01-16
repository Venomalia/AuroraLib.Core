using System;
using System.Diagnostics;

namespace AuroraLib.Core.Format
{
    /// <summary>
    /// Represents a media type consisting of a MIME type and a subtype.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class MediaType : IEquatable<MediaType>
    {
        /// <summary>
        /// Gets the MIME type
        /// </summary>
        public readonly MIMEType Type;

        /// <summary>
        /// Gets the media name or MIME subtype
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaType"/> class with the specified MIME type and subtype.
        /// </summary>
        /// <param name="type">The MIME type (e.g., "application", "image").</param>
        /// <param name="name">The MIME subtype (e.g., "json", "jpeg").</param>
        public MediaType(MIMEType type, string name)
        {
            ThrowIf.NullOrWhiteSpace(name, nameof(name));
            Type = type;
            Name = name;
        }

        /// <inheritdoc/>
        public bool Equals(MediaType? other) => other != null && other.Type == Type && other.Name == Name;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is MediaType media && Equals(media);

        /// <inheritdoc/>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <inheritdoc/>
        public override string ToString()
            => $"{Type.ToString().ToLower()}/{Name}";
    }
}
