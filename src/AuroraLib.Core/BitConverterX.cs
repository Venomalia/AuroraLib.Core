using AuroraLib.Core.Buffers;
using AuroraLib.Core.Extensions;
using AuroraLib.Core.Interfaces;
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
        #region ReverseEndianness

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(ushort)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReverseEndianness(ushort value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(short)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReverseEndianness(short value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Reverses the byte order (endianness) of the specified <see cref="UInt24"/> value.
        /// </summary>
        /// <param name="value">The 24-bit unsigned integer to reverse.</param>
        /// <returns>A new <see cref="UInt24"/> with reversed byte order.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt24 ReverseEndianness(UInt24 value)
            => new UInt24((value.Value >> 16) | ((value.Value & 0xFF00) << 8) | (value.Value << 16));

        /// <summary>
        /// Reverses the byte order (endianness) of the specified <see cref="Int24"/> value.
        /// </summary>
        /// <param name="value">The 24-bit signed integer to reverse.</param>
        /// <returns>A new <see cref="Int24"/> with reversed byte order.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int24 ReverseEndianness(Int24 value)
            => new Int24((value.Value >> 16) | ((value.Value & 0xFF00) << 8) | (value.Value << 16));

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(uint)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReverseEndianness(uint value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReverseEndianness(int value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(ulong)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReverseEndianness(ulong value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <inheritdoc cref="BinaryPrimitives.ReverseEndianness(long)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReverseEndianness(long value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Reverses the byte order (endianness) of a value of the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="vaule">The value to reverse the endianness of.</param>
        /// <returns>A new value of type <typeparamref name="T"/> with reversed byte order.</returns>
        [DebuggerStepThrough]
        public static T ReverseEndianness<T>(T vaule) where T : unmanaged
        {
            Type typeT = typeof(T);
            switch (Unsafe.SizeOf<T>())
            {
                case 0:
                case 1:
                    return vaule;
                case 2:
#if NET5_0_OR_GREATER
                    if (typeT == typeof(short) || typeT == typeof(ushort) || typeT == typeof(Half) || typeT.IsEnum)
#else
                    if (typeT == typeof(short) || typeT == typeof(ushort) || typeT.IsEnum)
#endif
                    {
                        short shortVaule = BinaryPrimitives.ReverseEndianness(Unsafe.As<T, short>(ref vaule));
                        return Unsafe.As<short, T>(ref shortVaule);
                    }
                    break;
                case 3:
                    if (typeT == typeof(Int24) || typeT == typeof(UInt24))
                    {
                        Int24 Int24Vaule = ReverseEndianness(Unsafe.As<T, Int24>(ref vaule));
                        return Unsafe.As<Int24, T>(ref Int24Vaule);
                    }
                    break;
                case 4:
                    if (typeT == typeof(int) || typeT == typeof(uint) || typeT == typeof(float) || typeT.IsEnum)
                    {
                        int intVaule = BinaryPrimitives.ReverseEndianness(Unsafe.As<T, int>(ref vaule));
                        return Unsafe.As<int, T>(ref intVaule);
                    }
                    break;
                case 8:
                    if (typeT == typeof(long) || typeT == typeof(ulong) || typeT == typeof(double) || typeT.IsEnum)
                    {
                        long longValue = BinaryPrimitives.ReverseEndianness(Unsafe.As<T, long>(ref vaule));
                        return Unsafe.As<long, T>(ref longValue);
                    }
                    break;
                default:
                    break;
            }

            BufferReverseEndiannessDeep(vaule.AsBytes(), typeT);
            return vaule;
        }

        /// <summary>
        /// Reverses the byte order (endianness) of each element in the specified span of unmanaged values.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the span.</typeparam>
        /// <param name="values">The span of values to reverse the byte order of.</param>
        [DebuggerStepThrough]
        public static void ReverseEndianness<T>(Span<T> values) where T : unmanaged
        {
            Type typeT = typeof(T);
            switch (Unsafe.SizeOf<T>())
            {
                case 0:
                case 1:
                    return;
                case 2:
#if NET5_0_OR_GREATER
                    if (typeT == typeof(short) || typeT == typeof(ushort) || typeT == typeof(Half) || typeT.IsEnum)
#else
                    if (typeT == typeof(short) || typeT == typeof(ushort) || typeT.IsEnum)
#endif
                    {
                        Span<short> shortVaules = MemoryMarshal.Cast<T, short>(values);
#if NET8_0_OR_GREATER
                        BinaryPrimitives.ReverseEndianness(shortVaules, shortVaules);
#else
                        for (int i = 0; i < shortVaules.Length; i++)
                            shortVaules[i] = BinaryPrimitives.ReverseEndianness(shortVaules[i]);
#endif
                        return;
                    }
                    break;
                case 3:
                    if (typeT == typeof(Int24) || typeT == typeof(UInt24))
                    {
                        Span<Int24> int24Vaules = MemoryMarshal.Cast<T, Int24>(vaules);
                        for (int i = 0; i < int24Vaules.Length; i++)
                            int24Vaules[i] = ReverseEndianness(int24Vaules[i]);
                        return;
                    }
                    break;
                case 4:
                    if (typeT == typeof(int) || typeT == typeof(uint) || typeT == typeof(float) || typeT.IsEnum)
                    {

                        Span<int> intVaules = MemoryMarshal.Cast<T, int>(values);
#if NET8_0_OR_GREATER
                        BinaryPrimitives.ReverseEndianness(intVaules, intVaules);
#else
                        for (int i = 0; i < intVaules.Length; i++)
                            intVaules[i] = BinaryPrimitives.ReverseEndianness(intVaules[i]);
#endif
                        return;
                    }
                    break;
                case 8:
                    if (typeT == typeof(long) || typeT == typeof(ulong) || typeT == typeof(double) || typeT.IsEnum)
                    {
                        Span<long> longVaules = MemoryMarshal.Cast<T, long>(values);
#if NET8_0_OR_GREATER
                        BinaryPrimitives.ReverseEndianness(longVaules, longVaules);
#else
                        for (int i = 0; i < longVaules.Length; i++)
                            longVaules[i] = BinaryPrimitives.ReverseEndianness(longVaules[i]);
#endif
                        return;
                    }
                    break;
                default:
                    break;
            }

            Span<byte> bytes = MemoryMarshal.Cast<T, byte>(vaules);
            BufferReverseEndiannessDeep(bytes, typeT, vaules.Length);
            return;
        }

            {
            }
            else
            {
            }
        }

        #region BufferReverseEndiannessDeep

        [DebuggerStepThrough]
        private static void BufferReverseEndiannessDeep(Span<byte> buffer, Type type)
        {
            ReadOnlySpan<int> fieldSizes = GetTypeFieldSizes(type);
            int offset = 0;

            foreach (int fieldSize in fieldSizes)
            {
                if (fieldSize > 1)
                    ReverseBuffer(buffer.Slice(offset, fieldSize));
                offset += fieldSize;
            }
        }

        [DebuggerStepThrough]
        private static void BufferReverseEndiannessDeep(Span<byte> buffer, Type type, int count)
        {
            int offset = 0;
            ReadOnlySpan<int> fieldSizes = GetTypeFieldSizes(type);
            for (int i = 0; i < count; i++)
            {
                foreach (int fieldSize in fieldSizes)
                {
                    if (fieldSize > 1)
                        ReverseBuffer(buffer.Slice(offset, fieldSize));
                    offset += fieldSize;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReverseBuffer(Span<byte> values)
        {
            switch (values.Length)
            {
                case 2:
                    Span<short> shortVaule = MemoryMarshal.Cast<byte, short>(values);
                    shortVaule[0] = BinaryPrimitives.ReverseEndianness(shortVaule[0]);
                    return;
                case 4:
                    Span<int> intVaule = MemoryMarshal.Cast<byte, int>(values);
                    intVaule[0] = BinaryPrimitives.ReverseEndianness(intVaule[0]);
                    return;
                case 8:
                    Span<long> longVaule = MemoryMarshal.Cast<byte, long>(values);
                    longVaule[0] = BinaryPrimitives.ReverseEndianness(longVaule[0]);
                    return;
                default:
                    values.Reverse();
                    break;
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<int> GetTypeFieldSizes(Type type)
        {
            lock (_FieldSizes)
            {
                if (_FieldSizes.TryGetValue(type.GetHashCode(), out int[]? primitives))
                    return primitives;

                return GetNewTypeFieldSizes(type);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ReadOnlySpan<int> GetNewTypeFieldSizes(Type type)
        {
            using PoolList<int> primList = new PoolList<int>();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (FieldInfo field in fields)
            {
                if (field.IsStatic)
                    continue;

                Type fieldtype = field.FieldType;

                if (fieldtype.IsEnum)
                    fieldtype = Enum.GetUnderlyingType(fieldtype);

                int TypeSize = GetStandardTypeSize(fieldtype);
                if (TypeSize != -1)
                    primList.Add(TypeSize);
                else
                    primList.AddRange(GetTypeFieldSizes(fieldtype));
            }
            int[] primitives = primList.ToArray();
            _FieldSizes.TryAdd(type.GetHashCode(), primitives);
            return primitives;
        }

        private static int GetStandardTypeSize(Type type) => type switch
        {
            _ when type == typeof(bool) || type == typeof(bool) => sizeof(bool),
            _ when type == typeof(byte) || type == typeof(sbyte) => sizeof(byte),
#if NET5_0_OR_GREATER
            _ when type == typeof(short) || type == typeof(ushort) || type == typeof(Half) => sizeof(short),
#else
            _ when type == typeof(short) || type == typeof(ushort) => sizeof(short),
#endif
            _ when type == typeof(UInt24) || type == typeof(Int24) => 3,
            _ when type == typeof(int) || type == typeof(uint) || type == typeof(float) => sizeof(int),
            _ when type == typeof(long) || type == typeof(ulong) || type == typeof(double) => sizeof(long),
            _ when type == typeof(IntPtr) || type == typeof(UIntPtr) => Unsafe.SizeOf<IntPtr>(),
            _ => -1
        };

        private static readonly Dictionary<int, int[]> _FieldSizes = new Dictionary<int, int[]>();
        #endregion

        #endregion

        #region ReverseBits & SwapAlternateBits
        /// <summary>
        /// Swaps the bits of the specified instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="vaule">The instance to swap the bits for.</param>
        /// <returns>An instance of <typeparamref name="T"/> with swapped bits.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReverseBits<T>(T vaule) where T : unmanaged
        {
            Span<byte> src = vaule.AsBytes();
            src.Reverse();
            for (int i = 0; i < src.Length; i++)
            {
                src[i] = ReverseBits(src[i]);
            }
            return vaule;
        }
        /// <summary>
        /// Swaps the bits in the given byte value.
        /// </summary>
        /// <param name="value">The byte value to swap the bits for.</param>
        /// <returns>The byte value with swapped bits.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReverseBits(byte value)
            => (byte)((value * 0x0202020202ul & 0x010884422010ul) % 1023);

        /// <summary>
        /// Swaps the bits in the given sbyte value.
        /// </summary>
        /// <param name="value">The sbyte value to swap the bits for.</param>
        /// <returns>The byte value with swapped bits.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReverseBits(sbyte value)
            => (sbyte)ReverseBits((byte)value);

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
