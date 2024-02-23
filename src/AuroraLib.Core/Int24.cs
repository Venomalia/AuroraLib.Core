using System.Diagnostics;

namespace AuroraLib.Core
{
    /// <summary>
    /// Represents a 3-byte, 24-bit signed integer.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Value}")]
    public readonly struct Int24 : IComparable, IFormattable, IConvertible, IComparable<Int24>, IComparable<Int32>, IEquatable<Int24>, IEquatable<Int32>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly byte b0, b1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly sbyte b2;

        /// <summary>
        /// Represents the largest possible value of <see cref="Int24"/>, This field is constant.
        /// </summary>
        public const int MaxValue = 8388607;

        /// <summary>
        /// Represents the smallest possible value of <see cref="Int24"/>, This field is constant.
        /// </summary>
        public const int MinValue = -8388608;

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
        public override string ToString() => Value.ToString();

        /// <inheritdoc/>
        public string ToString(IFormatProvider? provider) => Value.ToString(provider);

        /// <inheritdoc/>
        public string ToString(string? format) => Value.ToString(format);

        /// <inheritdoc/>
        public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Int24 i24 && i24.Value == Value;

        /// <inheritdoc/>
        public bool Equals(Int24 other) => this == other;

        /// <inheritdoc/>
        public bool Equals(int other) => this.Value == other;

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc/>
        public int CompareTo(object? value)
        {
            if (value == null)
            {
                return 1;
            }
            else if (value is Int24 num24)
                return this.CompareTo(num24);
            if (value is int num32)
                return this.CompareTo(num32);
            throw new ArgumentException("Argument must be an Int32 or an Int24");
        }

        /// <inheritdoc/>
        public int CompareTo(Int24 value)
            => CompareTo(value.Value);

        /// <inheritdoc/>
        public int CompareTo(int value)
        {
            return (Value < value ? -1 : (Value > value ? 1 : 0));
        }

        #region operators
        public static Int24 operator ++(Int24 a) => new(a.Value + 1);

        public static Int24 operator --(Int24 a) => new(a.Value - 1);

        public static implicit operator Int24(byte x) => new(x);

        public static explicit operator byte(Int24 x) => (byte)x.Value;

        public static implicit operator Int24(sbyte x) => new(x);

        public static explicit operator sbyte(Int24 x) => (sbyte)x.Value;

        public static implicit operator Int24(short x) => new(x);

        public static explicit operator short(Int24 x) => (short)x.Value;

        public static implicit operator Int24(ushort x) => new(x);

        public static explicit operator ushort(Int24 x) => (ushort)x.Value;

        public static explicit operator Int24(UInt24 x) => new((int)x.Value);

        public static explicit operator UInt24(Int24 x) => new((uint)x.Value);

        public static implicit operator Int24(int x) => new(x);

        public static explicit operator int(Int24 x) => x.Value;

        public static explicit operator Int24(uint x) => new((int)x);

        public static implicit operator uint(Int24 x) => (uint)x.Value;

        public static explicit operator Int24(long x) => new((int)x);

        public static implicit operator long(Int24 x) => x.Value;

        public static explicit operator Int24(ulong x) => new((int)x);

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
    }
}
