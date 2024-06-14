using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Write bits to a stream.
    /// </summary>
    public sealed class BitWriter : BitStreamProcessor
    {
        private byte _buffer = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitWriter"/>  class with the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="order">The byte order to use when writing bits.</param>
        /// <param name="leaveOpen">true leave the base stream open when disposing.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        [DebuggerStepThrough]
        public BitWriter(Stream stream, Endian order = Endian.Little, bool leaveOpen = true) : base(stream, order, leaveOpen)
        { }

        /// <summary>
        /// Writes a single boolean bit to the underlying stream.
        /// </summary>
        /// <param name="bit">The boolean bit to write (true for 1, false for 0).</param>
        [DebuggerStepThrough]
        public void Write(bool bit)
        {
            _buffer = BitConverterX.SetBit(_buffer, BitPosition, bit);

            if (BitPosition == 7)
                Flush();
            else
                BitPosition++;
        }

        /// <summary>
        /// Flushes any remaining bits in the buffer to the underlying stream.
        /// </summary>
        [DebuggerStepThrough]
        public void Flush() => FlushBuffer();

        /// <inheritdoc/>
        protected override void FlushBuffer()
        {
            if (BitPosition != 0)
            {
                if (Order != Endian.Little)
                    _buffer = BitConverterX.Swap(_buffer);

                BaseStream.WriteByte(_buffer);
                BitPosition = _buffer = 0;
            }
        }

        [DebuggerStepThrough]
#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        private void WriteInternally(ulong value, int bitCount, int intLength)
        {
            if (bitCount <= 0 || bitCount > intLength)
                throw new ArgumentOutOfRangeException(nameof(bitCount), $"Length must be between 1 and {intLength}.");

            int i = 0;

            if (Order == Endian.Little)
            {
                while (i < bitCount)
                {
                    if (BitPosition != 0 || bitCount - i < 8)
                    {
                        Write(BitConverterX.GetBit(value, i++));
                    }
                    else
                    {
                        byte byteToWrite = (byte)(value >> i);
                        BaseStream.WriteByte(byteToWrite);
                        i += 8;
                    }
                }
            }
            else
            {
                while (i < bitCount)
                {
                    if (BitPosition != 0 || bitCount - i < 8)
                    {
                        Write(BitConverterX.GetBit(value, bitCount - i++ - 1));
                    }
                    else
                    {
                        byte byteToWrite = (byte)(value >> (bitCount - i - 8));
                        BaseStream.WriteByte(byteToWrite);
                        i += 8;
                    }
                }
            }
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        protected override void ResetBuffer()
        {
            if (BitPosition != 0 && BaseStream.CanSeek)
                _buffer = BaseStream.Peek<byte>();
            else
                _buffer = 0;
        }

        #region Write
        /// <summary>
        /// Writes a specified number of bits from the given unsigned 8-bit integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The byte value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 8 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 8.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value, int length = 8)
            => WriteInternally(value, length, 8);

        /// <summary>
        /// Writes a specified number of bits from the given signed 8-bit integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The sbyte value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 8 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 8.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte value, int length = 8)
            => WriteInternally((byte)value, length, 8);

        /// <summary>
        /// Writes a specified number of bits from the given unsigned 16-bit integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The unsigned 16-bit integer value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 16 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 16.</exception>
        [DebuggerStepThrough]
        public void Write(ushort value, int length = 16)
            => WriteInternally(value, length, 16);

        /// <summary>
        /// Writes a specified number of bits from the given signed 16-bit integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The signed 16-bit integer value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 16 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 16.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value, int length = 16)
            => Write((ushort)value, length);

        /// <summary>
        /// Writes a specified number of bits from the given 24-bit unsigned integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The 24-bit unsigned integer value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 24 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 24.</exception>
        [DebuggerStepThrough]
        public void Write(UInt24 value, int length = 24)
            => WriteInternally(value, length, 24);
        /// <summary>
        /// Writes a specified number of bits from the given 24-bit signed integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The 24-bit signed integer value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 24 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 24.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Int24 value, int length = 24)
            => Write((UInt24)value, length);

        /// <summary>
        /// Writes a specified number of bits from the given unsigned 32-bit integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The unsigned 32-bit integer value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 32 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 32.</exception>
        [DebuggerStepThrough]
        public void Write(uint value, int length = 32)
            => WriteInternally(value, length, 32);

        /// <summary>
        /// Writes a specified number of bits from the given signed 32-bit integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The signed 32-bit integer value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 32 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 32.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value, int length = 32)
            => Write((uint)value, length);

        /// <summary>
        /// Writes a specified number of bits from the given unsigned 64-bit integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The unsigned 64-bit integer value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 64 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 64.</exception>
        [DebuggerStepThrough]
        public void Write(ulong value, int length = 64)
            => WriteInternally(value, length, 64);

        /// <summary>
        /// Writes a specified number of bits from the given signed 64-bit integer value to the underlying stream.
        /// </summary>
        /// <param name="value">The signed 64-bit integer value to write bits from.</param>
        /// <param name="length">The number of bits to write (default is 64 bits).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if 'length' is less than 1 or greater than 64.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value, int length = 64)
            => Write((ulong)value, length);
        #endregion
    }
}
