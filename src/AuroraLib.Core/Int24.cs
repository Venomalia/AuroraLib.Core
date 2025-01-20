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
    /// Represents a 3-byte, 24-bit signed integer.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Value}")]
#if NET8_0_OR_GREATER
    public readonly struct Int24 : IComparable, IConvertible, ISpanFormattable, IComparable<Int24>, IEquatable<Int24>, IBinaryInteger<Int24>, IMinMaxValue<Int24>, ISignedNumber<Int24>, IUtf8SpanFormattable
#elif NET6_0_OR_GREATER                  
    public readonly struct Int24 : IComparable, IConvertible, ISpanFormattable, IComparable<Int24>, IEquatable<Int24>
#else
    public readonly struct Int24 : IComparable, IFormattable, IConvertible, IComparable<Int24>, IEquatable<Int24>
#endif
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly byte b0, b1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly sbyte b2;

        /// <summary>
        /// Represents the largest possible value of <see cref="Int24"/>, This field is constant.
        /// </summary>
        public const int MaxValue = 8388607; // Equivalent to 0x7FFFFF in 24-bit.

        /// <summary>
        /// Represents the smallest possible value of <see cref="Int24"/>, This field is constant.
        /// </summary>
        public const int MinValue = -8388608; // Equivalent to 0xFF8000 in 24-bit.

        /// <summary>
        /// The value of this Int24 as an Int32
        /// </summary>
        public int Value => b0 | (b1 << 8) | (b2 << 16);

        /// <summary>
        /// Create a new Int24
        /// </summary>
        /// <param name="value"></param>
        public Int24(int value)
        {
            b0 = (byte)((value) & 0xFF);
            b1 = (byte)((value >> 8) & 0xFF);
            b2 = (sbyte)((value >> 16) & 0xFF);
        }

        public Int24(Int24 value)
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

        #region IEquatable
        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Int24 i24 && i24.Value == Value;

        /// <inheritdoc/>
        public bool Equals(Int24 other) => this == other;
        #endregion

        #region IComparable
        /// <inheritdoc/>
        public int CompareTo(object? value)
        {
            if (value == null)
                return 1;
            else if (value is Int24 num24)
                return Value.CompareTo(num24.Value);
            else
                return Value.CompareTo(value);
        }

        /// <inheritdoc/>
        public int CompareTo(Int24 value)
            => CompareTo(value.Value);
        #endregion

        #region operators
        public static Int24 operator ++(Int24 a) => new Int24(a.Value + 1);

        public static Int24 operator --(Int24 a) => new Int24(a.Value - 1);

        public static implicit operator Int24(byte x) => new Int24(x);

        public static explicit operator byte(Int24 x) => (byte)x.Value;

        public static implicit operator Int24(sbyte x) => new Int24(x);

        public static explicit operator sbyte(Int24 x) => (sbyte)x.Value;

        public static implicit operator Int24(short x) => new Int24(x);

        public static explicit operator short(Int24 x) => (short)x.Value;

        public static implicit operator Int24(ushort x) => new Int24(x);

        public static explicit operator ushort(Int24 x) => (ushort)x.Value;

        public static explicit operator Int24(UInt24 x) => new Int24((int)x.Value);

        public static explicit operator UInt24(Int24 x) => new UInt24((uint)x.Value);

        public static implicit operator Int24(int x) => new Int24(x);

        public static explicit operator int(Int24 x) => x.Value;

        public static explicit operator Int24(uint x) => new Int24((int)x);

        public static implicit operator uint(Int24 x) => (uint)x.Value;

        public static explicit operator Int24(long x) => new Int24((int)x);

        public static implicit operator long(Int24 x) => x.Value;

        public static explicit operator Int24(ulong x) => new Int24((int)x);

        public static implicit operator ulong(Int24 x) => (ulong)x.Value;

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
            => Value;

        uint IConvertible.ToUInt32(IFormatProvider? provider)
            => Convert.ToUInt32(Value, provider);

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

        public static Int24 Parse(string s, NumberStyles style, IFormatProvider? provider) => new Int24(int.Parse(s, style, provider));

        public static Int24 Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

        public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Int24 result)
        {
            if (int.TryParse(s, style, provider, out var value) && value <= MaxValue)
            {
                result = new Int24(value);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        public static bool TryParse(string? s, IFormatProvider? provider, out Int24 result) => TryParse(s, NumberStyles.Integer, provider, out result);

        #endregion

#if NET6_0_OR_GREATER

        #region Span Parse
        public static Int24 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => new Int24(int.Parse(s, style, provider));
        public static Int24 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Int24 result)
        {
            if (int.TryParse(s, style, provider, out var value) && value <= MaxValue)
            {
                result = new Int24(value);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Int24 result) => TryParse(s, NumberStyles.Integer, provider, out result);
        #endregion

        #region ISpanFormattable
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Value.TryFormat(destination, out charsWritten, format, provider);
        #endregion

#endif
#if NET8_0_OR_GREATER

        #region IMinMaxValue
        static Int24 IMinMaxValue<Int24>.MaxValue => (Int24)MaxValue;
        static Int24 IMinMaxValue<Int24>.MinValue => (Int24)MinValue;
        #endregion

        #region INumberBase
        static Int24 INumberBase<Int24>.One => 1;
        static int INumberBase<Int24>.Radix => 2;
        static Int24 INumberBase<Int24>.Zero => 0;
        static Int24 IAdditiveIdentity<Int24, Int24>.AdditiveIdentity => 0;
        static Int24 IMultiplicativeIdentity<Int24, Int24>.MultiplicativeIdentity => 1;
        static Int24 ISignedNumber<Int24>.NegativeOne => -1;

        static Int24 INumberBase<Int24>.Abs(Int24 value) => value;
        static bool INumberBase<Int24>.IsEvenInteger(Int24 value) => (value.Value & 1) == 0;
        static bool INumberBase<Int24>.IsOddInteger(Int24 value) => (value.Value & 1) != 0;
        static bool INumberBase<Int24>.IsZero(Int24 value) => value.Value == 0;
        static bool INumberBase<Int24>.IsPositive(Int24 value) => true;
        static bool INumberBase<Int24>.IsNegative(Int24 value) => false;
        static bool INumberBase<Int24>.IsComplexNumber(Int24 value) => false;
        static bool INumberBase<Int24>.IsImaginaryNumber(Int24 value) => false;
        static bool INumberBase<Int24>.IsRealNumber(Int24 value) => true;
        static bool INumberBase<Int24>.IsInfinity(Int24 value) => false;
        static bool INumberBase<Int24>.IsNegativeInfinity(Int24 value) => false;
        static bool INumberBase<Int24>.IsPositiveInfinity(Int24 value) => false;
        static bool INumberBase<Int24>.IsInteger(Int24 value) => true;
        static bool INumberBase<Int24>.IsNaN(Int24 value) => false;
        static bool INumberBase<Int24>.IsFinite(Int24 value) => true;
        static bool INumberBase<Int24>.IsSubnormal(Int24 value) => false;
        static bool INumberBase<Int24>.IsNormal(Int24 value) => true;
        static bool INumberBase<Int24>.IsCanonical(Int24 value) => true;

        static Int24 INumberBase<Int24>.MaxMagnitude(Int24 x, Int24 y) => x > y ? x : y;
        static Int24 INumberBase<Int24>.MinMagnitude(Int24 x, Int24 y) => x < y ? x : y;
        static Int24 INumberBase<Int24>.MaxMagnitudeNumber(Int24 x, Int24 y) => x > y ? x : y;
        static Int24 INumberBase<Int24>.MinMagnitudeNumber(Int24 x, Int24 y) => x < y ? x : y;

        static bool INumberBase<Int24>.TryConvertFromChecked<TOther>(TOther value, out Int24 result)
        {
            if (TOther.TryConvertToChecked(value, out int temp) && temp <= MaxValue)
            {
                result = new Int24(temp);
                return true;
            }

            result = default;
            return false;
        }
        static bool INumberBase<Int24>.TryConvertToChecked<TOther>(Int24 value, [MaybeNullWhen(false)] out TOther result) => TOther.TryConvertFromChecked(value.Value, out result);

        static bool INumberBase<Int24>.TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out Int24 result)
        {
            if (TOther.TryConvertToSaturating(value, out int temp))
            {
                result = new Int24(temp > MaxValue ? MaxValue : temp);
                return true;
            }

            result = default;
            return false;
        }
        static bool INumberBase<Int24>.TryConvertToSaturating<TOther>(Int24 value, [MaybeNullWhen(false)] out TOther result) => TOther.TryConvertFromSaturating(value.Value, out result);

        static bool INumberBase<Int24>.TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out Int24 result)
        {
            if (TOther.TryConvertToTruncating(value, out int temp))
            {
                result = new Int24(temp & MaxValue);
                return true;
            }

            result = default;
            return false;
        }
        static bool INumberBase<Int24>.TryConvertToTruncating<TOther>(Int24 value, [MaybeNullWhen(false)] out TOther result) => TOther.TryConvertFromTruncating(value.Value, out result);
        #endregion

        #region IOperators
        static Int24 IAdditionOperators<Int24, Int24, Int24>.operator +(Int24 left, Int24 right) => (Int24)(left.Value + right.Value);
        static Int24 IDivisionOperators<Int24, Int24, Int24>.operator /(Int24 left, Int24 right) => (Int24)(left.Value / right.Value);
        static bool IEqualityOperators<Int24, Int24, bool>.operator ==(Int24 left, Int24 right) => left.Value == right.Value;
        static bool IEqualityOperators<Int24, Int24, bool>.operator !=(Int24 left, Int24 right) => left.Value != right.Value;
        static Int24 IMultiplyOperators<Int24, Int24, Int24>.operator *(Int24 left, Int24 right) => (Int24)(left.Value * right.Value);
        static Int24 ISubtractionOperators<Int24, Int24, Int24>.operator -(Int24 left, Int24 right) => (Int24)(left.Value - right.Value);
        static Int24 IUnaryNegationOperators<Int24, Int24>.operator -(Int24 value) => (Int24)(-value.Value);
        static Int24 IUnaryPlusOperators<Int24, Int24>.operator +(Int24 value) => (Int24)(+value.Value);
        #endregion

        #region IBitwiseOperators
        static Int24 IBitwiseOperators<Int24, Int24, Int24>.operator &(Int24 left, Int24 right) => (Int24)(left & right);
        static Int24 IBitwiseOperators<Int24, Int24, Int24>.operator |(Int24 left, Int24 right) => (Int24)(left | right);
        static Int24 IBitwiseOperators<Int24, Int24, Int24>.operator ^(Int24 left, Int24 right) => (Int24)(left ^ right);
        static Int24 IBitwiseOperators<Int24, Int24, Int24>.operator ~(Int24 value) => (Int24)(~value);
        #endregion

        #region IComparisonOperators
        static bool IComparisonOperators<Int24, Int24, bool>.operator >(Int24 left, Int24 right) => left > right;
        static bool IComparisonOperators<Int24, Int24, bool>.operator >=(Int24 left, Int24 right) => left >= right;
        static bool IComparisonOperators<Int24, Int24, bool>.operator <(Int24 left, Int24 right) => left < right;
        static bool IComparisonOperators<Int24, Int24, bool>.operator <=(Int24 left, Int24 right) => left <= right;
        #endregion

        #region IModulusOperators & IShiftOperators
        static Int24 IModulusOperators<Int24, Int24, Int24>.operator %(Int24 left, Int24 right) => (Int24)(left % right);
        static Int24 IShiftOperators<Int24, int, Int24>.operator <<(Int24 value, int shiftAmount) => (Int24)(value << shiftAmount);
        static Int24 IShiftOperators<Int24, int, Int24>.operator >>(Int24 value, int shiftAmount) => (Int24)(value >> shiftAmount);
        static Int24 IShiftOperators<Int24, int, Int24>.operator >>>(Int24 value, int shiftAmount) => (Int24)(value >>> shiftAmount);
        #endregion

        #region IBinaryInteger
        unsafe int IBinaryInteger<Int24>.GetByteCount() => sizeof(Int24);
        int IBinaryInteger<Int24>.GetShortestBitLength() => int.Log2(Value) + 1;
        public static Int24 PopCount(Int24 value) => (Int24)int.PopCount(value.Value);
        public static Int24 TrailingZeroCount(Int24 value) => (Int24)int.TrailingZeroCount(value.Value);
        public static bool IsPow2(Int24 value) => int.IsPow2(value.Value);
        public static Int24 Log2(Int24 value) => (Int24)int.Log2(value.Value);

        static unsafe bool IBinaryInteger<Int24>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Int24 value)
        {
            Int24 result = default;
            if (source.Length != 0)
            {

                if (!isUnsigned && sbyte.IsNegative((sbyte)source[0]) || (source.Length > sizeof(Int24)) && (source[..^sizeof(Int24)].ContainsAnyExcept((byte)0x00)))
                {
                    value = result;
                    return false;
                }

                ref byte sourceRef = ref MemoryMarshal.GetReference(source);
                if (source.Length >= sizeof(Int24))
                {
                    sourceRef = ref Unsafe.Add(ref sourceRef, source.Length - sizeof(uint));
                    result = Unsafe.ReadUnaligned<Int24>(ref sourceRef);

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

        static unsafe bool IBinaryInteger<Int24>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Int24 value)
        {
            Int24 result = default;

            if (source.Length != 0)
            {
                if (!isUnsigned && sbyte.IsNegative((sbyte)source[^1]) || (source.Length > sizeof(Int24)) && (source[sizeof(Int24)..].ContainsAnyExcept((byte)0x00)))
                {
                    value = result;
                    return false;
                }

                if ((source.Length > sizeof(Int24)) && (source[sizeof(Int24)..].ContainsAnyExcept((byte)0x00)))
                {
                    value = result;
                    return false;
                }

                ref byte sourceRef = ref MemoryMarshal.GetReference(source);

                if (source.Length >= sizeof(Int24))
                {
                    result = Unsafe.ReadUnaligned<Int24>(ref sourceRef);

                    if (!BitConverter.IsLittleEndian)
                        result = BitConverterX.ReverseEndianness(result);
                }
                else
                {
                    for (int i = 0; i < source.Length; i++)
                    {
                        Int24 part = Unsafe.Add(ref sourceRef, i);
                        part <<= (i * 8);
                        result |= part;
                    }
                }
            }

            value = result;
            return true;
        }

        bool IBinaryInteger<Int24>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten) => TryWriteEndian(destination, out bytesWritten, true);
        bool IBinaryInteger<Int24>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) => TryWriteEndian(destination, out bytesWritten, false);
        private unsafe bool TryWriteEndian(Span<byte> destination, out int bytesWritten, bool isBigEndian)
        {
            if (destination.Length >= sizeof(Int24))
            {
                Int24 value = (BitConverter.IsLittleEndian == isBigEndian) ? this : BitConverterX.ReverseEndianness(this);
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
                bytesWritten = sizeof(Int24);
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
