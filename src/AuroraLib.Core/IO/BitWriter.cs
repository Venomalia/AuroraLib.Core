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
        /// Initializes a new instance of the <see cref="BitWriter"/> class.
        /// </summary>
        /// <param name="stream">The output stream to write bits to.</param>
        /// <param name="byteOrder">The byte order to use when writing multi-byte values.</param>
        /// <param name="bitOrder">The bit order to use when writing individual bits within bytes.</param>
        /// <param name="leaveOpen">true leave the base stream open when disposing.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        [DebuggerStepThrough]
        public BitWriter(Stream stream, Endian byteOrder = Endian.Big, Endian bitOrder = Endian.Little, bool leaveOpen = true)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            Protectbase = leaveOpen;
            ByteOrder = byteOrder;
            BitOrder = bitOrder;
        }

        /// <summary>
        /// Writes a single boolean bit to the underlying stream.
        /// </summary>
        /// <param name="bit">The boolean bit to write (true for 1, false for 0).</param>
        [DebuggerStepThrough]
        public void Write(bool bit)
        {
            if (bit)
            {
                _buffer |= (byte)(1 << BitPosition);
            }

            if (BitPosition == 7)
            {
                Flush();
            }
            else
            {
                BitPosition++;
            }
        }

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
        {
            if (ByteOrder == Endian.Big)
            {
                value = BitConverterX.Swap(value);
            }
            WriteInternally(value, length, 16);
        }

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
        {
            if (ByteOrder == Endian.Big)
            {
                value = BitConverterX.Swap(value);
            }
            WriteInternally(value, length, 24);
        }
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
        {
            if (ByteOrder == Endian.Big)
            {
                value = BitConverterX.Swap(value);
            }
            WriteInternally(value, length, 32);
        }

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
        {
            if (ByteOrder == Endian.Big)
            {
                value = BitConverterX.Swap(value);
            }
            WriteInternally(value, length, 64);
        }

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

        /// <summary>
        /// Flushes any remaining bits in the buffer to the underlying stream.
        /// </summary>
        [DebuggerStepThrough]
        public void Flush()
        {
            if (BitPosition != 0)
            {
                if (BitOrder == Endian.Little)
                    BaseStream.Write(_buffer);
                else
                    BaseStream.Write(BitConverterX.Swap(_buffer));
                BitPosition = _buffer = 0;
            }
        }

        [DebuggerStepThrough]
        private void WriteInternally(ulong value, int length, int intlength)
        {
            if (length > intlength)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Length must be between 1 and {intlength}.");
            }

            int i = 0;
            while (i < length)
            {
                if (BitPosition == 0 && length - i >= 8)
                {
                    byte int8 = (byte)(value >> i & 0xFF);
                    if (BitOrder == Endian.Little)
                        BaseStream.Write(int8);
                    else
                        BaseStream.Write(BitConverterX.Swap(int8));
                    i += 8;
                }
                else
                {
                    Write(((value >> i) & 1) == 1);
                    i++;
                }

            }
        }

        [DebuggerStepThrough]
        protected override void ResetBuffer()
        {
            _buffer = 0;

            if (BitPosition != 0)
            {
                _buffer = BitConverterX.GetBits(BaseStream.Peek<byte>(), 0, BitPosition);
            }
        }
    }
}
