using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    public static partial class StreamEx
    {
        #region Read
        /// <summary>
        /// Returns a 8-bit unsigned integer, read from one byte at the current position.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 8-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadUInt8(this Stream stream)
            => (byte)stream.ReadByteHelper<byte>();

        /// <summary>
        /// Returns a 8-bit signed integer, read from one byte at the current position.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A 8-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadInt8(this Stream stream)
            => (sbyte)stream.ReadByteHelper<sbyte>();

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 16-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(this Stream stream, Endian order = Endian.Little)
            => (ushort)ReadInt16Helper<ushort>(stream, order);

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 16-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(this Stream stream, Endian order = Endian.Little)
            => (short)ReadInt16Helper<short>(stream, order);

        /// <summary>
        /// Returns a 24-bit unsigned integer converted from three bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 24-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt24 ReadUInt24(Stream stream, Endian order = Endian.Little)
            => (UInt24)ReadInt24Helper<UInt24>(stream, order);

        /// <summary>
        /// Returns a 24-bit signed integer converted from three bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 24-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int24 ReadInt24(this Stream stream, Endian order = Endian.Little)
            => (Int24)ReadInt24Helper<Int24>(stream, order);

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 32-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
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
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(this Stream stream, Endian order = Endian.Little)
            => ReadInt32Helper<int>(stream, order);

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 64-bit unsigned integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(this Stream stream, Endian order = Endian.Little)
            => stream.Read<ulong>(order);

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A 64-bit signed integer.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(this Stream stream, Endian order = Endian.Little)
            => stream.Read<long>(order);

#if NET5_0_OR_GREATER
        /// <summary>
        /// Returns a half-precision floating point number converted from two bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A half-precision floating point number.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half ReadHalf(this Stream stream, Endian order = Endian.Little)
            => (Half)ReadInt16Helper<Half>(stream, order);
#endif
        /// <summary>
        /// Returns a single-precision floating point number converted from four bytes at a specified position.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="order">Byte order</param>
        /// <returns>A single-precision floating point number.</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
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
        /// <inheritdoc cref="ThrowHelper{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(this Stream stream, Endian order = Endian.Little)
            => stream.Read<double>(order);

        /// <summary>
        /// Reads the 16-bit order mark (BOM) and returns the endianness.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The endianness represented by the BOM</returns>
        /// <inheritdoc cref="ThrowHelper{T}()"/>
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
                ThrowHelper<T>();
            return value;
        }

#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        private static int ReadInt16Helper<T>(this Stream stream, Endian order)
        {
            int b1 = stream.ReadByte(), b2 = stream.ReadByteHelper<T>();
            if (order == Endian.Little)
                return b1 | b2 << 8;
            else
                return b2 | b1 << 8;
        }

#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        private static int ReadInt24Helper<T>(Stream stream, Endian order)
        {
            int b1 = stream.ReadByte(), b2 = stream.ReadByte(), b3 = stream.ReadByteHelper<T>();
            if (order == Endian.Little)
                return b1 | b2 << 8 | b3 << 16;
            else
                return b3 | b2 << 8 | b1 << 16;
        }

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
