using System;
using System.Buffers.Binary;
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
        private byte[] _buffer = new byte[9];

        /// <inheritdoc/>
        public override long Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BitPosition != 0 ? base.Position - 1 : base.Position;
            set => base.Position = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitReader"/>  class with the specified stream.
        /// </summary>
        /// <param name="stream">The input stream to read bits from.</param>
        /// <param name="order">The byte order to use when reading bits.</param>
        /// <param name="leaveOpen">true leave the base stream open when disposing.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        [DebuggerStepThrough]
        public BitReader(Stream stream, Endian order = Endian.Little, bool leaveOpen = true) : base(stream, order, leaveOpen)
        { }

        /// <summary>
        /// Reads an unsigned integer of the specified length from the stream.
        /// </summary>
        /// <param name="bitCount">The number of bits to read.</param>
        /// <returns>The unsigned integer value read from the stream.</returns>
        [DebuggerStepThrough]
#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public ulong ReadUInt(int bitCount)
        {
            if (bitCount <= 0 || bitCount > 64)
                throw new ArgumentOutOfRangeException(nameof(bitCount), $"Length must be between 1 and 64.");

            int currentPosition = BitPosition;
            int bitsInBuffer = currentPosition + bitCount;
            int index;
            ulong value;
            FillBuffer(bitCount);

            if (Order == Endian.Little)
            {
                value = BinaryPrimitives.ReadUInt64LittleEndian(_buffer);

                // Check for overflow
                if (bitsInBuffer > 64)
                {
                    value = (value >> currentPosition) | (ulong)_buffer[8] << (64 - currentPosition);
                    index = 0;
                }
                else
                {
                    index = currentPosition;
                }
            }
            else
            {
                value = BinaryPrimitives.ReadUInt64BigEndian(_buffer);

                // Check for overflow
                if (bitsInBuffer > 64)
                {
                    int overflowBits = BitPosition;
                    value = (value << overflowBits) | (ulong)_buffer[8] >> (8 - overflowBits);
                    index = 0;
                }
                else
                {
                    index = 64 - bitsInBuffer;
                }
            }

            // Align the buffer for the next read
            _buffer[0] = _buffer[(bitsInBuffer - 1) >> 3];

            if (bitCount == 64)
                return value;

            return value >> index & (1uL << bitCount) - 1;
        }

        /// <summary>
        /// Reads a signed integer of the specified length from the stream.
        /// </summary>
        /// <param name="length">The number of bits to read.</param>
        /// <returns>The signed integer value read from the stream.</returns>
        public long ReadInt(int length)
        {
            int shift = 64 - length;
            return (long)ReadUInt(length) << shift >> shift;
        }

        /// <summary>
        /// Reads a single bit from the underlying stream and returns it as a boolean value.
        /// </summary>
        /// <returns>True if the bit read is 1, false if it's 0.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBit()
        {
            int currentPosition = BitPosition;
            FillBuffer(1);
            if (Order == Endian.Big)
                currentPosition = 7 - currentPosition;

            return BitConverterX.GetBit(_buffer[0], currentPosition);
        }

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
            => (byte)ReadUInt(8);


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
            => (ushort)ReadUInt(16);

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
            => (UInt24)ReadUInt(24);

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
            => (uint)ReadUInt(32);

        /// <summary>
        /// Reads an 64-bit unsigned integer from the underlying stream.
        /// </summary>
        /// <returns>The 64-bit unsigned integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
            => ReadUInt(64);

        /// <summary>
        /// Reads an 64-bit signed integer from the underlying stream.
        /// </summary>
        /// <returns>The 64-bit signed integer read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
            => ReadInt(64);

        [DebuggerStepThrough]
        private unsafe void FillBuffer(int bitCount = 1)
        {
            int bitsBuffered = (8 - BitPosition) & 7;
            if (bitCount > bitsBuffered)
            {
                int start = BitPosition != 0 ? 1 : 0;
                int newBytesRequired = (bitCount - bitsBuffered + 7) >> 3;

                int bytesRead = BaseStream.Read(_buffer, start, newBytesRequired);
                if (bytesRead != newBytesRequired)
                    throw new EndOfStreamException();

            }
            BitPosition = (bitCount + BitPosition) % 8;
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        protected override void ResetBuffer()
        {
            if (BitPosition != 0 && BaseStream.CanSeek)
                _buffer[0] = BaseStream.Peek<byte>();
            else
                _buffer[0] = 0;
        }

        /// <inheritdoc/>
        protected override void FlushBuffer()
        { }
    }
}
