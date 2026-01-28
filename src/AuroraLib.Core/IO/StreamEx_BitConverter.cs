using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    public static partial class StreamEx
    {
        #region Read

        #region sbyte & byte
        /// <summary>
        /// Returns a 8-bit unsigned integer, read from one byte at the current position.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 8-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadUInt8(this Stream stream)
            => (byte)stream.ReadByteHelper<byte>();

        /// <summary>
        /// Returns a 8-bit signed integer, read from one byte at the current position.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 8-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadInt8(this Stream stream)
            => (sbyte)stream.ReadByteHelper<sbyte>();

        #endregion

        #region short & ushort

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 16-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(this Stream stream, Endian order = Endian.Little)
            => (ushort)ReadInt16Helper(stream, order);

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 16-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(this Stream stream, Endian order = Endian.Little)
            => (short)ReadInt16Helper(stream, order);

        /// <summary>
        /// Reads a 16-bit signed integer (big-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 24-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16BigEndian(this Stream stream)
#if BIGENDIAN
            => (short)(stream.ReadByte() | stream.ReadByteHelper<short>() << 8);
#else
    => (short)(stream.ReadByte() << 8 | stream.ReadByteHelper<short>());
#endif

        /// <summary>
        /// Reads a 16-bit signed integer (little-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 16-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16LittleEndian(this Stream stream)
#if BIGENDIAN
            => (short)(stream.ReadByte() << 8| stream.ReadByteHelper<short>());
#else
            => (short)(stream.ReadByte() | stream.ReadByteHelper<short>() << 8);
#endif


        /// <summary>
        /// Reads a 16-bit unsigned integer (big-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 16-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16BigEndian(this Stream stream) => (ushort)stream.ReadInt16BigEndian();


        /// <summary>
        /// Reads a 16-bit unsigned integer (little-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 16-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16LittleEndian(this Stream stream) => (ushort)stream.ReadInt16LittleEndian();

        #endregion

        #region Int24 & UInt24

        /// <summary>
        /// Returns a 24-bit unsigned integer converted from three bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 24-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt24 ReadUInt24(this Stream stream, Endian order = Endian.Little)
            => (UInt24)ReadInt24Helper(stream, order);

        /// <summary>
        /// Returns a 24-bit signed integer converted from three bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 24-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int24 ReadInt24(this Stream stream, Endian order = Endian.Little)
            => ReadInt24Helper(stream, order);

        /// <summary>
        /// Reads a 24-bit signed integer (big-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 24-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int24 ReadInt24BigEndian(this Stream stream)
        {
            int b1 = stream.ReadByte(), b2 = stream.ReadByte(), b3 = stream.ReadByteHelper<Int24>();
#if BIGENDIAN
            return b1 | b2 << 8 | b3 << 16;
#else
            return b3 | b2 << 8 | b1 << 16;
#endif
        }

        /// <summary>
        /// Reads a 24-bit signed integer (little-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 24-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int24 ReadInt24LittleEndian(this Stream stream)
        {
            int b1 = stream.ReadByte(), b2 = stream.ReadByte(), b3 = stream.ReadByteHelper<Int24>();
#if BIGENDIAN
            return b3 | b2 << 8 | b1 << 16;
#else
            return b1 | b2 << 8 | b3 << 16;
#endif
        }

        /// <summary>
        /// Reads a 24-bit unsigned integer (big-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 24-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt24 ReadUInt24BigEndian(this Stream stream) => (ushort)stream.ReadInt24BigEndian();

        /// <summary>
        /// Reads a 24-bit unsigned integer (little-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 24-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt24 ReadUInt24LittleEndian(this Stream stream) => (UInt24)stream.ReadInt24LittleEndian();
        #endregion

        #region int & uint

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 32-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(this Stream stream, Endian order = Endian.Little)
            => (uint)ReadInt32Helper<uint>(stream, order);

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 32-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(this Stream stream, Endian order = Endian.Little)
            => ReadInt32Helper<int>(stream, order);

        /// <summary>
        /// Reads a 32-bit signed integer (big-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 32-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32BigEndian(this Stream stream)
        {
            int b1 = stream.ReadByte(), b2 = stream.ReadByte(), b3 = stream.ReadByte(), b4 = stream.ReadByteHelper<int>();
#if BIGENDIAN
            return b1 | b2 << 8 | b3 << 16 | b4 << 24;
#else
            return b4 | b3 << 8 | b2 << 16 | b1 << 24;
#endif
        }

        /// <summary>
        /// Reads a 32-bit signed integer (little-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 32-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32LittleEndian(this Stream stream)
        {
            int b1 = stream.ReadByte(), b2 = stream.ReadByte(), b3 = stream.ReadByte(), b4 = stream.ReadByteHelper<int>();
#if BIGENDIAN
            return b4 | b3 << 8 | b2 << 16 | b1 << 24;
#else
            return b1 | b2 << 8 | b3 << 16 | b4 << 24;
#endif
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer (big-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 32-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32BigEndian(this Stream stream) => (uint)stream.ReadInt32BigEndian();

        /// <summary>
        /// Reads a 32-bit unsigned integer (little-endian) from the current position of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 32-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32LittleEndian(this Stream stream) => (uint)stream.ReadInt32LittleEndian();

        #endregion

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 64-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(this Stream stream, Endian order = Endian.Little)
            => stream.ReadGenericHelper<ulong>(order);

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 64-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(this Stream stream, Endian order = Endian.Little)
            => stream.ReadGenericHelper<long>(order);

#if NET5_0_OR_GREATER
        /// <summary>
        /// Returns a half-precision floating point number converted from two bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A half-precision floating point number.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half ReadHalf(this Stream stream, Endian order = Endian.Little)
            => (Half)ReadInt16Helper(stream, order);
#endif
        /// <summary>
        /// Returns a single-precision floating point number converted from four bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A single-precision floating point number.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(this Stream stream, Endian order = Endian.Little)
            => ReadInt32Helper<float>(stream, order);

        /// <summary>
        /// Returns a double-precision floating point number converted from eight bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A double-precision floating point number.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(this Stream stream, Endian order = Endian.Little)
            => stream.Read<double>(order);

        /// <summary>
        /// Reads the 16-bit order mark (BOM) and returns the endianness.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The endianness represented by the BOM</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Endian ReadBOM(this Stream stream)
            => stream.Read<Endian>();
        #endregion

        #region internal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReadByteHelper<T>(this Stream stream)
        {
            int value = stream.ReadByte();
            if (value == -1)
                ThrowHelper.EndOfStreamException<T>();
            return value;
        }

#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        private static int ReadInt16Helper(this Stream stream, Endian order)
            => order == Endian.Little ? stream.ReadInt16LittleEndian() : stream.ReadInt16BigEndian();

#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        private static Int24 ReadInt24Helper(Stream stream, Endian order)
            => order == Endian.Little ? stream.ReadInt24LittleEndian() : stream.ReadInt24BigEndian();

#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        private static int ReadInt32Helper<T>(Stream stream, Endian order)
        {
            int b1 = stream.ReadByte(), b2 = stream.ReadByte(), b3 = stream.ReadByte(), b4 = stream.ReadByteHelper<T>();
            if (order == Endian.Little)
                return b1 | b2 << 8 | b3 << 16 | b4 << 24;
            else
                return b4 | b3 << 8 | b2 << 16 | b1 << 24;
        }
        #endregion
    }
}
