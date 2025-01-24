using AuroraLib.Core.Format.Identifier;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AuroraLib.Core.Format
{
    /// <inheritdoc cref="IFormatInfo"/>
    [DebuggerDisplay("{FullName} ({MIMEType})")]
#if NET8_0_OR_GREATER
    public sealed class FormatInfo<T> : IFormatInfo, IStaticFormatRecognition where T : IStaticFormatRecognition, new()
#else
    public sealed class FormatInfo<T> : IFormatInfo where T : IFormatRecognition, new()
#endif
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
        public Type? Class => typeof(T);

        public FormatInfo(string fullName, MediaType mediaType, string fileExtension, IIdentifier? identifier = null, int identifierOffset = 0)
            : this(fullName, mediaType, new string[] { fileExtension }, identifier, identifierOffset)
        { }

        public FormatInfo(string fullName, MediaType mediaType, IEnumerable<string> fileExtensions, IIdentifier? identifier = null, int identifierOffset = 0)
        {
            ThrowIf.Null(fullName);
            ThrowIf.Null(mediaType);
            ThrowIf.Null(fileExtensions);

            FullName = fullName;
            MIMEType = mediaType;
            FileExtensions = fileExtensions;
            Identifier = identifier;
            IdentifierOffset = identifierOffset;

#if !NET8_0_OR_GREATER
            IFormatRecognition instance = (IFormatRecognition)CreateInstance();
            IsMatchAction = instance.IsMatch;
#endif
        }

        /// <inheritdoc/>
        public object CreateInstance() => new T();

        /// <inheritdoc/>
        public bool IsMatch(Stream stream, ReadOnlySpan<char> fileNameAndExtension = default)
#if NET8_0_OR_GREATER
            => T.IsMatchStatic(stream, fileNameAndExtension);
#else
            => IsMatchAction(stream, fileNameAndExtension);

        private readonly FormatInfo.MatchAction IsMatchAction;
#endif

        /// <inheritdoc/>
        public bool Equals(IFormatInfo? other) => !(other is null) && other.MIMEType.Equals(MIMEType);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is IFormatInfo info && Equals(info);

        /// <inheritdoc/>
        public override int GetHashCode() => MIMEType.GetHashCode();

#if NET8_0_OR_GREATER
        /// <inheritdoc/>
        public static bool IsMatchStatic(Stream stream, ReadOnlySpan<char> fileNameAndExtension = default) => T.IsMatchStatic(stream, fileNameAndExtension);
#endif
    }
}
