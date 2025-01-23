using AuroraLib.Core.Text;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AuroraLib.Core.Format.Identifier
{
    /// <summary>
    /// Represents a 32-bit identifier that is not affected by the endian order.
    /// </summary>
    [Serializable]
    public unsafe struct Identifier32 : IIdentifier
    {
#pragma warning disable IDE0044
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte b0, b1, b2, b3;
#pragma warning restore IDE0044

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier32"/> struct using the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes representing the identifier.</param>
        public Identifier32(ReadOnlySpan<byte> bytes) : this(bytes[0], bytes[1], bytes[2], bytes[3]) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier32"/> struct using the specified bytes.
        /// </summary>
        /// <param name="b0">The first byte of the identifier.</param>
        /// <param name="b1">The second byte of the identifier.</param>
        /// <param name="b2">The third byte of the identifier.</param>
        /// <param name="b3">The fourth byte of the identifier.</param>
        public Identifier32(byte b0, byte b1, byte b2, byte b3)
        {
            this.b0 = b0;
            this.b1 = b1;
            this.b2 = b2;
            this.b3 = b3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier32"/> struct using the specified 32-bit value.
        /// </summary>
        /// <param name="value">The integer value to initialize the identifier.</param>
        public Identifier32(int value) : this((uint)value) { }

        /// <inheritdoc cref="Identifier32(int)"/>
        public Identifier32(uint value)
        {
            b0 = (byte)(value & 0xFF);
            b1 = (byte)(value >> 8 & 0xFF);
            b2 = (byte)(value >> 16 & 0xFF);
            b3 = (byte)(value >> 24 & 0xFF);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier32"/> struct using the specified character span.
        /// </summary>
        /// <param name="span">The character span to initialize the identifier. Only the first 4 characters will be considered.</param>
        public Identifier32(ReadOnlySpan<char> span)
        {
            ReadOnlySpan<char> span32 = span.Slice(0, Math.Min(span.Length, 4));
            Span<byte> bytes = stackalloc byte[4];
            Encoding.GetEncoding(28591).GetBytes(span32, bytes);

            b0 = bytes[0];
            b1 = bytes[1];
            b2 = bytes[2];
            b3 = bytes[3];
        }

        #endregion

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
#if NET20_OR_GREATER || NETSTANDARD2_0
        {
            fixed (byte* bytePtr = &b0)
            {
                return new Span<byte>(bytePtr, 4);
            }
        }
#else
            => MemoryMarshal.CreateSpan(ref b0, 4);
#endif

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

        public static implicit operator Identifier32(uint v) => *(Identifier32*)&v;
        public static implicit operator uint(Identifier32 v) => *(uint*)&v;

        public static explicit operator Identifier32(int v) => *(Identifier32*)&v;
        public static explicit operator int(Identifier32 v) => *(int*)&v;

        public static explicit operator Identifier32(string v) => new Identifier32(v.AsSpan());
        public static explicit operator string(Identifier32 v) => v.GetString();

        /// <inheritdoc />
        public override int GetHashCode() => AsSpan().SequenceGetHashCode();

        /// <inheritdoc />
        public override string ToString() => EncodingX.GetDisplayableString(AsSpan());
    }
}
