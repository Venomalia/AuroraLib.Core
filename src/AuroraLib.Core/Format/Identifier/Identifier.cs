using AuroraLib.Core.Text;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuroraLib.Core.Format.Identifier
{
    /// <summary>
    /// Represents a identifier.
    /// </summary>
    public sealed class Identifier : IIdentifier
    {
        /// <summary>
        /// Returns an empty <see cref="Identifier"/>
        /// </summary>
        public static readonly Identifier Empty = new Identifier(Array.Empty<byte>());

        private readonly byte[] Bytes;

        public Identifier(byte[] bytes)
            => Bytes = bytes;

        public Identifier(string span) : this(EncodingX.DefaultEncoding.GetBytes(span))
        { }

        public Identifier(IIdentifier identifier) : this(identifier.AsSpan().ToArray())
        { }

        /// <inheritdoc />
        public Span<byte> AsSpan() => Bytes.AsSpan();

        /// <inheritdoc />
        public bool Equals(string? other) => other == GetString();

        /// <inheritdoc />
        public bool Equals(IIdentifier? other) => !(other is null) && other.AsSpan().SequenceEqual(AsSpan());

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString()
            => EncodingX.GetCString(AsSpan());

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(Encoding encoding)
            => EncodingX.GetCString(AsSpan(), encoding, 0x0);

        public static explicit operator Identifier(byte[] v) => new Identifier(v);
        public static explicit operator byte[](Identifier v) => v.Bytes;

        /// <inheritdoc />
        public override int GetHashCode() => AsSpan().SequenceGetHashCode();

        /// <inheritdoc />
        public int CompareTo(IIdentifier? other) => other == null ? 1 : AsSpan().SequenceCompareTo(other.AsSpan());

        /// <inheritdoc />
        public override string ToString() => EncodingX.GetDisplayableString(AsSpan());
    }
}
