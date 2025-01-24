using AuroraLib.Core.Format.Identifier;
using AuroraLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AuroraLib.Core.Format
{
    /// <inheritdoc cref="IFormatInfo"/>
    [DebuggerDisplay("{FullName} ({MIMEType})")]
    public sealed class FormatInfo : IFormatInfo
    {
        /// <inheritdoc/>
        public string FullName { get; }

        /// <inheritdoc/>
        public MediaType MIMEType { get; }

        /// <inheritdoc/>
        public IEnumerable<string> FileExtensions { get; }

        /// <inheritdoc/>
        public IIdentifier? Identifier { get; }

        /// <inheritdoc/>
        public int IdentifierOffset { get; }

        /// <inheritdoc/>
        public Type? Class { get; }

        private readonly MatchAction? IsMatchAction;

        /// <inheritdoc cref="IFormatRecognition.IsMatch(Stream, ReadOnlySpan{char})"/>
        public delegate bool MatchAction(Stream stream, ReadOnlySpan<char> fileNameAndExtension = default);

        public FormatInfo(string fullName, MediaType mediaType, string fileExtension, IIdentifier? identifier = null, int identifierOffset = 0, Type? @class = null, MatchAction? isMatchAction = null)
            : this(fullName, mediaType, new string[] { fileExtension }, identifier, identifierOffset, @class, isMatchAction)
        { }

        public FormatInfo(string fullName, MediaType mediaType, IEnumerable<string> fileExtensions, IIdentifier? identifier = null, int identifierOffset = 0, Type? @class = null, MatchAction? isMatchAction = null)
        {
            ThrowIf.Null(fullName);
            ThrowIf.Null(mediaType);
            ThrowIf.Null(fileExtensions);

            FullName = fullName;
            MIMEType = mediaType;
            FileExtensions = fileExtensions;
            Identifier = identifier;
            IdentifierOffset = identifierOffset;
            IsMatchAction = isMatchAction;
            Class = @class;

            if (!(Class is null) && IsMatchAction is null)
            {
                object? instance = CreateInstance();
                if (instance is IFormatRecognition recognition)
                    IsMatchAction = recognition.IsMatch;

                if (instance is IHasIdentifier hasIdentifier)
                    identifier = hasIdentifier.Identifier;
            }
        }

        /// <inheritdoc/>
        public object? CreateInstance()
        {
            ThrowIf.Null(Class);
            return Activator.CreateInstance(Class);
        }

        /// <inheritdoc/>
        public bool IsMatch(Stream stream, ReadOnlySpan<char> fileNameAndExtension = default)
        {
            if (!(IsMatchAction is null))
                return IsMatchAction(stream, fileNameAndExtension);

            if (!(Identifier is null))
            {
                ReadOnlySpan<byte> identifier = Identifier.AsSpan();
                if (stream.Length >= IdentifierOffset + identifier.Length)
                {
                    stream.Seek(IdentifierOffset, SeekOrigin.Begin);
                    return stream.Match(identifier);
                }
                return false;
            }
            ReadOnlySpan<char> extension = PathX.GetExtension(fileNameAndExtension);
            foreach (var value in FileExtensions)
            {
                if (extension.Contains(value.AsSpan(), StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public bool Equals(IFormatInfo? other) => !(other is null) && other.MIMEType.Equals(MIMEType);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is IFormatInfo info && Equals(info);

        /// <inheritdoc/>
        public override int GetHashCode() => MIMEType.GetHashCode();
    }
}
