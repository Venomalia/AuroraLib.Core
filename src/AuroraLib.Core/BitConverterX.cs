using AuroraLib.Core.Buffers;
using AuroraLib.Core.Extensions;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core
{
    /// <summary>
    /// BitConverter Extra functions
    /// </summary>
    public static class BitConverterX
    {
        #region ConvertGeneric
        /// <summary>
        /// Converts a instance of <typeparamref name="T"/> to a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>A byte array representing the value.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET8_0_OR_GREATER
        public static byte[] GetBytes<T>(in T value) where T : unmanaged
        {
            byte[] result = new byte[Unsafe.SizeOf<T>()];
            MemoryMarshal.Write(result, in value);
            return result;
        }
#else
        public static byte[] GetBytes<T>(ref T value) where T : unmanaged
        {
            byte[] result = new byte[Unsafe.SizeOf<T>()];
            MemoryMarshal.Write(result, ref value);
            return result;
        }

        /// <inheritdoc cref="GetBytes{T}(ref T)"/>
        [DebuggerStepThrough]
        public static byte[] GetBytes<T>(T value) where T : unmanaged
            => GetBytes(ref value);
#endif
        #endregion

        #region SwapGeneric
        /// <summary>
        /// Swaps the byte order of the specified instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="vaule"></param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Swap<T>(T vaule) where T : unmanaged
        {
            Span<byte> src = vaule.AsBytes();
            Swap<T>(src);
            return vaule;
        }

        /// <summary>
        /// Swaps the bits of the specified instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="vaule">The instance to swap the bits for.</param>
        /// <returns>An instance of <typeparamref name="T"/> with swapped bits.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SwapBits<T>(T vaule) where T : unmanaged
        {
            Span<byte> src = vaule.AsBytes();
            src.Reverse();
            for (int i = 0; i < src.Length; i++)
            {
                src[i] = Swap(src[i]);
            }
            return vaule;
        }

        /// <summary>
        /// Swaps the byte order of the specified buffer based on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to reverse.</param>
        /// <param name="type">The type of the data in the buffer.</param>
        [DebuggerStepThrough]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public static void Swap(Span<byte> buffer, Type type)
        {
            int offset = 0;
            ReadOnlySpan<int> fields = GetPrimitiveTypeSizes(type);
            foreach (int fieldSize in fields)
            {
                buffer.Slice(offset, fieldSize).Reverse();
                offset += fieldSize;
            }
        }

        /// <summary>
        /// Swaps the byte order of the specified buffer based on the given array of <paramref name="type"/>.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to reverse.</param>
        /// <param name="type">The type of the data in the buffer.</param>
        /// <param name="count">The number of elements.</param>
        [DebuggerStepThrough]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public static void Swap(Span<byte> buffer, Type type, int count)
        {
            int offset = 0;
            ReadOnlySpan<int> fields = GetPrimitiveTypeSizes(type);
            for (int i = 0; i < count; i++)
            {
                foreach (int fieldSize in fields)
                {
                    buffer.Slice(offset, fieldSize).Reverse();
                    offset += fieldSize;
                }
            }
        }

        /// <inheritdoc cref="Swap(Span{byte}, Type)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap<T>(Span<byte> buffer)
            => Swap(buffer, typeof(T));

        /// <summary>
        /// Retrieves a list the sizes of primitive types and nested primitive types within the specified type.
        /// </summary>
        /// <param name="type">The type to analyze.</param>
        /// <returns>An array containing the sizes of primitive types within the specified type.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<int> GetPrimitiveTypeSizes(Type type)
        {
            lock (TypePrimitives)
            {
                if (TypePrimitives.TryGetValue(type.GetHashCode(), out int[]? primitives))
                    return primitives;

                return NewPrimitiveTypeSizes(type);
            }
        }
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        private static ReadOnlySpan<int> NewPrimitiveTypeSizes(Type type)
        {
            List<int> primList = new List<int>();
            if (type == typeof(Int24) || type == typeof(UInt24))
            {
                primList.Add(3);
            }
            else
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (FieldInfo field in fields)
                {
                    if (field.IsStatic) continue;

                    Type fieldtype = field.FieldType;

                    if (fieldtype.IsEnum)
                        fieldtype = Enum.GetUnderlyingType(fieldtype);

                    if (fieldtype.IsPrimitive || fieldtype == typeof(UInt24) || fieldtype == typeof(Int24))
                        primList.Add(Marshal.SizeOf(fieldtype));
                    else
                        primList.AddRange(GetPrimitiveTypeSizes(fieldtype).ToArray());
                }

            }
            int[] primitives = primList.ToArray();
            TypePrimitives.Add(type.GetHashCode(), primitives);
            return primitives;
        }

        private static readonly Dictionary<int, int[]> TypePrimitives = new Dictionary<int, int[]>();
        #endregion

        #region Swap
        /// <summary>
        /// Swaps the bits in the given byte value.
        /// </summary>
        /// <param name="value">The byte value to swap the bits for.</param>
        /// <returns>The byte value with swapped bits.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Swap(byte value)
            => (byte)((value * 0x0202020202ul & 0x010884422010ul) % 1023);

        /// <summary>
        /// Swaps the bits in the given sbyte value.
        /// </summary>
        /// <param name="value">The sbyte value to swap the bits for.</param>
        /// <returns>The byte value with swapped bits.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Swap(sbyte value)
            => (sbyte)Swap((byte)value);

        /// <summary>
        /// Swaps the alternate bits of a byte value.
        /// </summary>
        /// <param name="value">The byte value to swap the bits for.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SwapAlternateBits(byte value)
            => (byte)((value & 0xAA) >> 1 | (value & 0x55) << 1);

        /// <summary>
        /// Swaps the alternate bits of a sbyte value.
        /// </summary>
        /// <param name="value">The sbyte value to swap the bits for.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SwapAlternateBits(sbyte value)
            => (sbyte)SwapAlternateBits((byte)value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(ushort)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Swap(ushort value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(short)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Swap(short value)
            => BinaryPrimitives.ReverseEndianness(value);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt24 Swap(UInt24 value)
            => new UInt24((value.Value >> 16) | ((value.Value & 0xFF00) << 8) | (value.Value << 16));

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int24 Swap(Int24 value)
            => new Int24((value.Value >> 16) | ((value.Value & 0xFF00) << 8) | (value.Value << 16));

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(uint)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Swap(uint value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Swap(int value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(ulong)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Swap(ulong value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(long)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Swap(long value)
            => BinaryPrimitives.ReverseEndianness(value);
        #endregion

        #region GetBit
        /// <summary>
        /// Get a bit from a 8-bit unsigned integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be read</param>
        /// <returns>bit as bool</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(byte b, int index = 0)
            => (b >> index & 1) != 0;

        /// <summary>
        /// Get a bit from a 16-bit signed integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be read</param>
        /// <returns>bit as bool</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(ushort b, int index = 0)
            => (b >> index & 1) != 0;

        /// <summary>
        /// Get a bit from a 32-bit signed integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be read</param>
        /// <returns>bit as bool</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(uint b, int index = 0)
            => (b >> index & 1) != 0;

        /// <summary>
        /// Get a bit from a 64-bit signed integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be read</param>
        /// <returns>bit as bool</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(ulong b, int index = 0)
            => (b >> index & 1) != 0;
        #endregion

        #region GetBits
        /// <summary>
        /// Get a <paramref name="length"/>-bit signed integer from a 8-bit unsigned integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be read</param>
        /// <param name="length">length of the bits to be read</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetBits(byte b, int index, int length)
        {
            if (index + length > 8)
            {
                GetBitsThrowHelper(8);
            }
            return (byte)(b >> index & (1 << length) - 1);
        }

        /// <summary>
        /// Get a <paramref name="length"/>-bit signed integer from a 16-bit signed integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be read</param>
        /// <param name="length">length of the bits to be read</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetBits(ushort b, short index, short length)
        {
            if (index + length > 16)
            {
                GetBitsThrowHelper(16);
            }
            return (ushort)(b >> index & (1 << length) - 1);
        }

        /// <summary>
        /// Get a <paramref name="length"/>-bit signed integer from a 32-bit signed integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be read</param>
        /// <param name="length">length of the bits to be read</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetBits(uint b, int index, int length)
        {
            if (index + length > 32)
            {
                GetBitsThrowHelper(32);
            }
            return b >> index & (uint)(1 << length) - 1;
        }

        /// <summary>
        /// Get a <paramref name="length"/>-bit signed integer from a 64-bit signed integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be read</param>
        /// <param name="length">length of the bits to be read</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetBits(ulong b, int index, int length)
        {
            if (length > 64 || index > 64)
            {
                GetBitsThrowHelper(64);
            }
            return b >> index & (1ul << length) - 1;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void GetBitsThrowHelper(int length)
            => throw new ArgumentOutOfRangeException("GetBits", $"Length + index must be between 1 and {length}.");
        #endregion

        #region SetBit
        /// <summary>
        /// Set a bit in a 8-bit unsigned integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be set</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SetBit(byte b, int index, bool value)
        {
            int bitmask = 1 << index;
            return value ? (byte)(b | bitmask) : (byte)(b & ~bitmask);
        }

        /// <summary>
        /// Set a bit in a 32-bit signed integer.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index">bit position that should be set</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetBit(int b, int index, bool value)
        {
            int bitmask = 1 << index;
            return value ? (b | bitmask) : (b & ~bitmask);
        }
        #endregion

        #region DataXor
        /// <summary>
        /// Performs an XOR operation between the elements of the <paramref name="data"/> span and the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="data">The span of data to perform XOR on.</param>
        /// <param name="key">The key value to use for XOR operation.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DataXor(this Span<byte> data, byte key)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ key);
            }
        }

        /// <summary>
        /// Performs an XOR operation between the elements of the <paramref name="data"/> span and the <paramref name="key"/> span.
        /// </summary>
        /// <param name="data">The span of data to perform XOR on.</param>
        /// <param name="key">The span of the key to use for XOR operation.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DataXor(this Span<byte> data, ReadOnlySpan<byte> key)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ key[i % key.Length]);
            }
        }
        #endregion

        #region ToString
        /// <summary>
        /// Converts the numeric value of each element of a specified ReadOnlySpan of bytes to its equivalent hexadecimal string representation.
        /// </summary>
        /// <param name="value">An ReadOnlySpan of bytes.</param>
        /// <returns>A string of hexadecimal pairs separated by hyphens, where each pair represents the corresponding element in <paramref name="value"/>; for example, "7F-2C-4A-00".</returns>
        [DebuggerStepThrough]
        public static string ToString(ReadOnlySpan<byte> value)
        {
            using (SpanBuffer<byte> buffer = new SpanBuffer<byte>(value))
            {
                return BitConverter.ToString(buffer.GetBuffer(), 0, buffer.Length);
            }
        }
        #endregion

    }
}
