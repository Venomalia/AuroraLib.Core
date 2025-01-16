using AuroraLib.Core.Extensions;
using AuroraLib.Core.Text;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AuroraLib.Core.Format.Identifier
{
    /// <summary>
    /// Represents a 64-bit identifier that is not affected by the endian order.
    /// </summary>
    [Serializable]
    public unsafe struct Identifier64 : IIdentifier
    {
        /// <summary>
        /// The lower 32-bit identifier.
        /// </summary>
        public Identifier32 Lower;

        /// <summary>
        /// The higher 32-bit identifier.
        /// </summary>
        public Identifier32 Higher;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier64"/> struct from a span of bytes.
        /// </summary>
        /// <param name="bytes">The span of bytes representing the identifier.</param>
        public Identifier64(ReadOnlySpan<byte> bytes) : this(new Identifier32(bytes.Slice(0, 4)), new Identifier32(bytes.Slice(4, 4)))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier64"/> struct using the specified bytes.
        /// </summary>
        public Identifier64(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7) : this(new Identifier32(b0, b1, b2, b3), new Identifier32(b4, b5, b6, b7))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier64"/> struct with the specified lower and higher identifiers.
        /// </summary>
        /// <param name="identifierLower">The lower 32-bit identifier.</param>
        /// <param name="identifierHigher">The higher 32-bit identifier.</param>
        public Identifier64(Identifier32 identifierLower, Identifier32 identifierHigher)
        {
            Lower = identifierLower;
            Higher = identifierHigher;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier64"/> struct with the specified 64-bit value.
        /// </summary>
        /// <param name="value">The 64-bit value to initialize the identifier.</param>
        public Identifier64(long value) : this((ulong)value) { }

        /// <inheritdoc cref="Identifier64(long)"/>
        public Identifier64(ulong value)
        {
            Lower = new Identifier32((uint)value);
            Higher = new Identifier32((uint)(value >> 32));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier64"/> struct with the specified character span.
        /// </summary>
        /// <param name="span">The character span representing the identifier.</param>
        public Identifier64(ReadOnlySpan<char> span)
        {
            ReadOnlySpan<char> span64 = span.Slice(0, Math.Min(span.Length, 8));
            Span<byte> bytes = stackalloc byte[8];
            Encoding.GetEncoding(28591).GetBytes(span64, bytes);
            Lower = new Identifier32(bytes.Slice(0, 4));
            Higher = new Identifier32(bytes.Slice(4, 4));
        }

        #endregion

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
        {
#if !(NETSTANDARD || NET20_OR_GREATER)

            ref byte tRef = ref Unsafe.As<Identifier32, byte>(ref Lower);
            return MemoryMarshal.CreateSpan(ref tRef, 8);
#else
            unsafe
            {
                fixed (Identifier32* bytePtr = &Lower)
                {
                    return new Span<byte>((byte*)bytePtr, 8);
                }
            }
#endif
        }

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

        /// <inheritdoc />
        public bool Equals(string? other) => other == GetString();

        /// <inheritdoc />
        public bool Equals(IIdentifier? other) => other != null && other.AsSpan().SequenceEqual(AsSpan());

        public static implicit operator Identifier64(ulong v) => *(Identifier64*)&v;
        public static implicit operator ulong(Identifier64 v) => *(ulong*)&v;

        public static explicit operator Identifier64(long v) => *(Identifier64*)&v;
        public static explicit operator long(Identifier64 v) => *(long*)&v;

        public static explicit operator Identifier64(string v) => new Identifier64(v.AsSpan());
        public static explicit operator string(Identifier64 v) => v.GetString();

        /// <inheritdoc />
        public override int GetHashCode() => AsSpan().SequenceGetHashCode();

        /// <inheritdoc />
        public override string ToString() => EncodingX.GetDisplayableString(AsSpan());
    }
}
