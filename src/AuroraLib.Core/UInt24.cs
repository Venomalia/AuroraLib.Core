using AuroraLib.Core.Interfaces;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core
{
    /// <summary>
    /// Represents a 3-byte, 24-bit unsigned integer.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Value}")]
#if NET8_0_OR_GREATER
    public readonly struct UInt24 : IComparable, IConvertible, ISpanFormattable, IComparable<UInt24>, IEquatable<UInt24>, IBinaryInteger<UInt24>, IMinMaxValue<UInt24>, IUnsignedNumber<UInt24>, IReversibleEndianness<UInt24>, IUtf8SpanFormattable
#elif NET6_0_OR_GREATER
    public readonly struct UInt24 : IComparable, IConvertible, ISpanFormattable, IComparable<UInt24>, IEquatable<UInt24>, IReversibleEndianness<UInt24>
#else
    public readonly struct UInt24 : IComparable, IFormattable, IConvertible, IComparable<UInt24>, IEquatable<UInt24>, IReversibleEndianness<UInt24>
#endif
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly byte b0, b1, b2;

        /// <summary>
        /// Represents the largest possible value of <see cref="UInt24"/>, This field is constant.
        /// </summary>
        public const uint MaxValue = 0x00ffffff;

        /// <summary>
        /// Represents the smallest possible value of <see cref="UInt24"/>, This field is constant.
        /// </summary>
        public const uint MinValue = 0x00000000;

        /// <summary>
        /// The value of this Int24 as an Int32
        /// </summary>
        public uint Value => (uint)(b0 | b1 << 8 | b2 << 16);

        public UInt24(uint value)
        {
            b0 = (byte)(value & 0xFF);
            b1 = (byte)(value >> 8 & 0xFF);
            b2 = (byte)(value >> 16 & 0xFF);
        }

        public UInt24(UInt24 value)
        {
            b0 = value.b0;
            b1 = value.b1;
            b2 = value.b2;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();

        /// <inheritdoc/>
        public string ToString(IFormatProvider? provider) => Value.ToString(provider);

        /// <inheritdoc/>
        public string ToString(string? format) => Value.ToString(format);

        /// <inheritdoc/>
        public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

        UInt24 IReversibleEndianness<UInt24>.ReverseEndianness() => BitConverterX.ReverseEndianness(this);

        #region IEquatable
        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is UInt24 ui24 && ui24.Value == Value;

        /// <inheritdoc/>
        public bool Equals(UInt24 other) => this == other;
        #endregion

        #region IComparable
        /// <inheritdoc/>
        public int CompareTo(object? value)
        {
            if (value == null)
                return 1;
            else if (value is UInt24 num24)
                return Value.CompareTo(num24.Value);
            else
                return Value.CompareTo(value);
        }

        /// <inheritdoc/>
        public int CompareTo(UInt24 value) => CompareTo(value.Value);
        #endregion

        #region operators
        public static UInt24 operator ++(UInt24 a) => new UInt24(a.Value + 1);

        public static UInt24 operator --(UInt24 a) => new UInt24(a.Value - 1);

        public static implicit operator UInt24(byte x) => new UInt24((uint)x);

        public static explicit operator byte(UInt24 x) => (byte)x.Value;

        public static explicit operator UInt24(sbyte x) => new UInt24((uint)x);

        public static explicit operator sbyte(UInt24 x) => (sbyte)x.Value;

        public static explicit operator UInt24(short x) => new UInt24((uint)x);

        public static explicit operator short(UInt24 x) => (short)x.Value;

        public static implicit operator UInt24(ushort x) => new UInt24((uint)x);

        public static explicit operator ushort(UInt24 x) => (ushort)x.Value;

        public static explicit operator UInt24(int x) => new UInt24((uint)x);

        public static implicit operator int(UInt24 x) => (int)x.Value;

        public static explicit operator UInt24(uint x) => new UInt24(x);

        public static implicit operator uint(UInt24 x) => x.Value;

        public static explicit operator UInt24(long x) => new UInt24((uint)x);

        public static implicit operator long(UInt24 x) => x.Value;

        public static explicit operator UInt24(ulong x) => new UInt24((uint)x);

        public static implicit operator ulong(UInt24 x) => x.Value;

        #endregion operators

        #region IConvertible

        bool IConvertible.ToBoolean(IFormatProvider? provider)
            => Convert.ToBoolean(Value, provider);

        char IConvertible.ToChar(IFormatProvider? provider)
            => Convert.ToChar(Value, provider);

        sbyte IConvertible.ToSByte(IFormatProvider? provider)
            => Convert.ToSByte(Value, provider);

        byte IConvertible.ToByte(IFormatProvider? provider)
            => Convert.ToByte(Value, provider);

        short IConvertible.ToInt16(IFormatProvider? provider)
            => Convert.ToInt16(Value, provider);

        ushort IConvertible.ToUInt16(IFormatProvider? provider)
            => Convert.ToUInt16(Value, provider);

        int IConvertible.ToInt32(IFormatProvider? provider)
            => Convert.ToInt32(Value, provider);

        uint IConvertible.ToUInt32(IFormatProvider? provider)
            => Value;

        long IConvertible.ToInt64(IFormatProvider? provider)
            => Convert.ToInt64(Value, provider);

        ulong IConvertible.ToUInt64(IFormatProvider? provider)
            => Convert.ToUInt64(Value, provider);

        float IConvertible.ToSingle(IFormatProvider? provider)
            => Convert.ToSingle(Value, provider);

        double IConvertible.ToDouble(IFormatProvider? provider)
            => Convert.ToDouble(Value, provider);

        decimal IConvertible.ToDecimal(IFormatProvider? provider)
            => Convert.ToDecimal(Value, provider);

        DateTime IConvertible.ToDateTime(IFormatProvider? provider)
            => Convert.ToDateTime(Value, provider);

        object IConvertible.ToType(Type type, IFormatProvider? provider)
            => Convert.ChangeType(Value, type, provider);

        /// <inheritdoc/>
        public TypeCode GetTypeCode()
            => TypeCode.UInt32;

        #endregion IConvertible

        #region Parse

        public static UInt24 Parse(string s, NumberStyles style, IFormatProvider? provider) => new UInt24(uint.Parse(s, style, provider));

        public static UInt24 Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

        public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out UInt24 result)
        {
            if (uint.TryParse(s, style, provider, out var value) && value <= MaxValue)
            {
                result = new UInt24(value);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        public static bool TryParse(string? s, IFormatProvider? provider, out UInt24 result) => TryParse(s, NumberStyles.Integer, provider, out result);

        #endregion

#if NET6_0_OR_GREATER

        #region Span Parse
        public static UInt24 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => new UInt24(uint.Parse(s, style, provider));
        public static UInt24 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UInt24 result)
        {
            if (uint.TryParse(s, style, provider, out var value) && value <= MaxValue)
            {
                result = new UInt24(value);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UInt24 result) => TryParse(s, NumberStyles.Integer, provider, out result);
        #endregion

        #region ISpanFormattable
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Value.TryFormat(destination, out charsWritten, format, provider);
        #endregion

#endif

#if NET8_0_OR_GREATER

        #region IMinMaxValue
        static UInt24 IMinMaxValue<UInt24>.MaxValue => (UInt24)MaxValue;
        static UInt24 IMinMaxValue<UInt24>.MinValue => (UInt24)MinValue;
        #endregion

        #region INumberBase
        static UInt24 INumberBase<UInt24>.One => 1;
        static int INumberBase<UInt24>.Radix => 2;
        static UInt24 INumberBase<UInt24>.Zero => 0;
        static UInt24 IAdditiveIdentity<UInt24, UInt24>.AdditiveIdentity => 0;
        static UInt24 IMultiplicativeIdentity<UInt24, UInt24>.MultiplicativeIdentity => 1;

        static UInt24 INumberBase<UInt24>.Abs(UInt24 value) => value;
        static bool INumberBase<UInt24>.IsEvenInteger(UInt24 value) => (value.Value & 1) == 0;
        static bool INumberBase<UInt24>.IsOddInteger(UInt24 value) => (value.Value & 1) != 0;
        static bool INumberBase<UInt24>.IsZero(UInt24 value) => value.Value == 0;
        static bool INumberBase<UInt24>.IsPositive(UInt24 value) => true;
        static bool INumberBase<UInt24>.IsNegative(UInt24 value) => false;
        static bool INumberBase<UInt24>.IsComplexNumber(UInt24 value) => false;
        static bool INumberBase<UInt24>.IsImaginaryNumber(UInt24 value) => false;
        static bool INumberBase<UInt24>.IsRealNumber(UInt24 value) => true;
        static bool INumberBase<UInt24>.IsInfinity(UInt24 value) => false;
        static bool INumberBase<UInt24>.IsNegativeInfinity(UInt24 value) => false;
        static bool INumberBase<UInt24>.IsPositiveInfinity(UInt24 value) => false;
        static bool INumberBase<UInt24>.IsInteger(UInt24 value) => true;
        static bool INumberBase<UInt24>.IsNaN(UInt24 value) => false;
        static bool INumberBase<UInt24>.IsFinite(UInt24 value) => true;
        static bool INumberBase<UInt24>.IsSubnormal(UInt24 value) => false;
        static bool INumberBase<UInt24>.IsNormal(UInt24 value) => true;
        static bool INumberBase<UInt24>.IsCanonical(UInt24 value) => true;

        static UInt24 INumberBase<UInt24>.MaxMagnitude(UInt24 x, UInt24 y) => x > y ? x : y;
        static UInt24 INumberBase<UInt24>.MinMagnitude(UInt24 x, UInt24 y) => x < y ? x : y;
        static UInt24 INumberBase<UInt24>.MaxMagnitudeNumber(UInt24 x, UInt24 y) => x > y ? x : y;
        static UInt24 INumberBase<UInt24>.MinMagnitudeNumber(UInt24 x, UInt24 y) => x < y ? x : y;

        static bool INumberBase<UInt24>.TryConvertFromChecked<TOther>(TOther value, out UInt24 result)
        {
            if (TOther.TryConvertToChecked(value, out uint temp) && temp <= MaxValue)
            {
                result = new UInt24(temp);
                return true;
            }

            result = default;
            return false;
        }
        static bool INumberBase<UInt24>.TryConvertToChecked<TOther>(UInt24 value, [MaybeNullWhen(false)] out TOther result) => TOther.TryConvertFromChecked(value.Value, out result);

        static bool INumberBase<UInt24>.TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out UInt24 result)
        {
            if (TOther.TryConvertToSaturating(value, out uint temp))
            {
                result = new UInt24(temp > MaxValue ? MaxValue : temp);
                return true;
            }

            result = default;
            return false;
        }
        static bool INumberBase<UInt24>.TryConvertToSaturating<TOther>(UInt24 value, [MaybeNullWhen(false)] out TOther result) => TOther.TryConvertFromSaturating(value.Value, out result);
        static bool INumberBase<UInt24>.TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out UInt24 result)
        {
            if (TOther.TryConvertToTruncating(value, out uint temp))
            {
                result = new UInt24(temp & MaxValue);
                return true;
            }

            result = default;
            return false;
        }
        static bool INumberBase<UInt24>.TryConvertToTruncating<TOther>(UInt24 value, [MaybeNullWhen(false)] out TOther result) => TOther.TryConvertFromTruncating(value.Value, out result);
        #endregion

        #region IOperators
        static UInt24 IAdditionOperators<UInt24, UInt24, UInt24>.operator +(UInt24 left, UInt24 right) => (UInt24)(left.Value + right.Value);
        static UInt24 IDivisionOperators<UInt24, UInt24, UInt24>.operator /(UInt24 left, UInt24 right) => (UInt24)(left.Value / right.Value);
        static bool IEqualityOperators<UInt24, UInt24, bool>.operator ==(UInt24 left, UInt24 right) => left.Value == right.Value;
        static bool IEqualityOperators<UInt24, UInt24, bool>.operator !=(UInt24 left, UInt24 right) => left.Value != right.Value;
        static UInt24 IMultiplyOperators<UInt24, UInt24, UInt24>.operator *(UInt24 left, UInt24 right) => (UInt24)(left.Value * right.Value);
        static UInt24 ISubtractionOperators<UInt24, UInt24, UInt24>.operator -(UInt24 left, UInt24 right) => (UInt24)(left.Value - right.Value);
        static UInt24 IUnaryNegationOperators<UInt24, UInt24>.operator -(UInt24 value) => (UInt24)(-value.Value);
        static UInt24 IUnaryPlusOperators<UInt24, UInt24>.operator +(UInt24 value) => (UInt24)(+value.Value);
        #endregion

        #region IBitwiseOperators
        static UInt24 IBitwiseOperators<UInt24, UInt24, UInt24>.operator &(UInt24 left, UInt24 right) => (UInt24)(left & right);
        static UInt24 IBitwiseOperators<UInt24, UInt24, UInt24>.operator |(UInt24 left, UInt24 right) => (UInt24)(left | right);
        static UInt24 IBitwiseOperators<UInt24, UInt24, UInt24>.operator ^(UInt24 left, UInt24 right) => (UInt24)(left ^ right);
        static UInt24 IBitwiseOperators<UInt24, UInt24, UInt24>.operator ~(UInt24 value) => (UInt24)(~value);
        #endregion

        #region IComparisonOperators
        static bool IComparisonOperators<UInt24, UInt24, bool>.operator >(UInt24 left, UInt24 right) => left > right;
        static bool IComparisonOperators<UInt24, UInt24, bool>.operator >=(UInt24 left, UInt24 right) => left >= right;
        static bool IComparisonOperators<UInt24, UInt24, bool>.operator <(UInt24 left, UInt24 right) => left < right;
        static bool IComparisonOperators<UInt24, UInt24, bool>.operator <=(UInt24 left, UInt24 right) => left <= right;
        #endregion

        #region IModulusOperators & IShiftOperators
        static UInt24 IModulusOperators<UInt24, UInt24, UInt24>.operator %(UInt24 left, UInt24 right) => (UInt24)(left % right);
        static UInt24 IShiftOperators<UInt24, int, UInt24>.operator <<(UInt24 value, int shiftAmount) => (UInt24)(value << shiftAmount);
        static UInt24 IShiftOperators<UInt24, int, UInt24>.operator >>(UInt24 value, int shiftAmount) => (UInt24)(value >> shiftAmount);
        static UInt24 IShiftOperators<UInt24, int, UInt24>.operator >>>(UInt24 value, int shiftAmount) => (UInt24)(value >>> shiftAmount);
        #endregion

        #region IBinaryInteger
        unsafe int IBinaryInteger<UInt24>.GetByteCount() => sizeof(UInt24);
        int IBinaryInteger<UInt24>.GetShortestBitLength() => BitOperations.Log2(Value) + 1;
        public static UInt24 PopCount(UInt24 value) => (UInt24)uint.PopCount(value);
        public static UInt24 TrailingZeroCount(UInt24 value) => (UInt24)uint.TrailingZeroCount(value);
        public static bool IsPow2(UInt24 value) => uint.IsPow2(value.Value);
        public static UInt24 Log2(UInt24 value) => (UInt24)uint.Log2(value.Value);

        static unsafe bool IBinaryInteger<UInt24>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UInt24 value)
        {
            UInt24 result = default;
            if (source.Length != 0)
            {

                if (!isUnsigned && sbyte.IsNegative((sbyte)source[0]) || (source.Length > sizeof(UInt24)) && (source[..^sizeof(UInt24)].ContainsAnyExcept((byte)0x00)))
                {
                    value = result;
                    return false;
                }

                ref byte sourceRef = ref MemoryMarshal.GetReference(source);
                if (source.Length >= sizeof(UInt24))
                {
                    sourceRef = ref Unsafe.Add(ref sourceRef, source.Length - sizeof(uint));
                    result = Unsafe.ReadUnaligned<UInt24>(ref sourceRef);

                    if (BitConverter.IsLittleEndian)
                        result = BitConverterX.ReverseEndianness(result);
                }
                else
                {
                    for (int i = 0; i < source.Length; i++)
                    {
                        result <<= 8;
                        result |= Unsafe.Add(ref sourceRef, i);
                    }
                }
            }

            value = result;
            return true;
        }

        static unsafe bool IBinaryInteger<UInt24>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UInt24 value)
        {
            UInt24 result = default;

            if (source.Length != 0)
            {
                if (!isUnsigned && sbyte.IsNegative((sbyte)source[^1]) || (source.Length > sizeof(UInt24)) && (source[sizeof(UInt24)..].ContainsAnyExcept((byte)0x00)))
                {
                    value = result;
                    return false;
                }

                if ((source.Length > sizeof(UInt24)) && (source[sizeof(UInt24)..].ContainsAnyExcept((byte)0x00)))
                {
                    value = result;
                    return false;
                }

                ref byte sourceRef = ref MemoryMarshal.GetReference(source);

                if (source.Length >= sizeof(UInt24))
                {
                    result = Unsafe.ReadUnaligned<UInt24>(ref sourceRef);

                    if (!BitConverter.IsLittleEndian)
                        result = BitConverterX.ReverseEndianness(result);
                }
                else
                {
                    for (int i = 0; i < source.Length; i++)
                    {
                        UInt24 part = Unsafe.Add(ref sourceRef, i);
                        part <<= (i * 8);
                        result |= part;
                    }
                }
            }

            value = result;
            return true;
        }

        bool IBinaryInteger<UInt24>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten) => TryWriteEndian(destination, out bytesWritten, true);
        bool IBinaryInteger<UInt24>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) => TryWriteEndian(destination, out bytesWritten, false);
        private unsafe bool TryWriteEndian(Span<byte> destination, out int bytesWritten, bool isBigEndian)
        {
            if (destination.Length >= sizeof(UInt24))
            {
                UInt24 value = (BitConverter.IsLittleEndian == isBigEndian) ? this : BitConverterX.ReverseEndianness(this);
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
                bytesWritten = sizeof(UInt24);
                return true;
            }
            else
            {
                bytesWritten = 0;
                return false;
            }
        }
        #endregion

#endif
    }
}
