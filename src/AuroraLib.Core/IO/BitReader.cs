using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Reads bits from a stream.
    /// </summary>
    public sealed class BitReader : BitStreamProcessor
    {
        private long _buffer = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitReader"/> class.
        /// </summary>
        /// <param name="stream">The input stream to read bits from.</param>
        /// <param name="byteOrder">The byte order to use when reading multi-byte values.</param>
        /// <param name="bitOrder">The bit order to use reading bits.</param>
        /// <param name="leaveOpen">true leave the base stream open when disposing.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        [DebuggerStepThrough]
        public BitReader(Stream stream, Endian byteOrder = Endian.Big, Endian bitOrder = Endian.Little, bool leaveOpen = true)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            Protectbase = leaveOpen;
            ByteOrder = byteOrder;
            BitOrder = bitOrder;
        }

        /// <summary>
        /// Reads an integer of the specified bit length from the underlying stream.
        /// </summary>
        /// <param name="length">The number of bits to read.</param>
        /// <returns>The integer value read from the stream.</returns>
        [DebuggerStepThrough]
        public long ReadInt(int length)
        {
            FillBuffer(length);

            if (ByteOrder == Endian.Big)
            {
                int startbit = (length + BitPosition + 7 & -8) - BitPosition;
                return BitConverterX.GetBits(_buffer, startbit, length);
            }
            else
            {
                return BitConverterX.GetBits(BitConverterX.Swap(_buffer), 64 - BitPosition, length);
            }
        }

        /// <summary>
        /// Reads a single bit from the underlying stream and returns it as a boolean value.
        /// </summary>
        /// <returns>True if the bit read is 1, false if it's 0.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBit()
            => ReadInt(1) != 0;

        /// <summary>
        /// Reads an 8-bit signed integer from the underlying stream.
        /// </summary>
        /// <returns>The 8-bit signed integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadInt8()
            => (sbyte)ReadInt(8);

        /// <summary>
        /// Reads an 8-bit unsigned integer from the underlying stream.
        /// </summary>
        /// <returns>The 8-bit unsigned integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadUInt8()
            => (byte)ReadInt(8);


        /// <summary>
        /// Reads an 16-bit signed integer from the underlying stream.
        /// </summary>
        /// <returns>The 16-bit signed integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
            => (short)ReadInt(16);

        /// <summary>
        /// Reads an 16-bit unsigned integer from the underlying stream.
        /// </summary>
        /// <returns>The 16-bit unsigned integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
            => (ushort)ReadInt(16);

        /// <summary>
        /// Reads an 24-bit signed integer from the underlying stream.
        /// </summary>
        /// <returns>The 24-bit signed integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int24 ReadInt24()
            => (Int24)ReadInt(24);

        /// <summary>
        /// Reads an 24-bit unsigned integer from the underlying stream.
        /// </summary>
        /// <returns>The 24-bit unsigned integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt24 ReadUInt24()
            => (UInt24)ReadInt(24);

        /// <summary>
        /// Reads an 32-bit signed integer from the underlying stream.
        /// </summary>
        /// <returns>The 32-bit signed integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
            => (int)ReadInt(32);

        /// <summary>
        /// Reads an 32-bit unsigned integer from the underlying stream.
        /// </summary>
        /// <returns>The 32-bit unsigned integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
            => (uint)ReadInt(32);

        /// <summary>
        /// Reads an 64-bit signed integer from the underlying stream.
        /// </summary>
        /// <returns>The 64-bit signed integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
            => ReadInt(64);

        /// <summary>
        /// Reads an 64-bit unsigned integer from the underlying stream.
        /// </summary>
        /// <returns>The 64-bit unsigned integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
            => (ulong)ReadInt(64);

        [DebuggerStepThrough]
        private void FillBuffer(int value = 1)
        {
            value += BitPosition;
            BitPosition = value % 8;
            if (Buffered)
                value -= 8;

            while (value > 0)
            {
                _buffer <<= 8;

                if (BitOrder == Endian.Big)
                {
                    _buffer += BitConverterX.Swap(BaseStream.ReadUInt8());
                }
                else
                {
                    _buffer += BaseStream.ReadUInt8();
                }
                value -= 8;
            }
            if (value < 0)
                Buffered = true;
            else
                Buffered = false;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void ResetBuffer()
        {
            _buffer = 0;
            Buffered = false;
        }
    }
}
