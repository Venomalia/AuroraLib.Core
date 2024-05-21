using System;
using System.Diagnostics;

namespace AuroraLib.Core
{
    /// <summary>
    /// Represents a 3-byte, 24-bit unsigned integer.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Value}")]
    public readonly struct UInt24 : IComparable, IFormattable, IConvertible, IComparable<UInt24>, IComparable<uint>, IEquatable<UInt24>, IEquatable<uint>
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
        public override string ToString() => Value.ToString();

        /// <inheritdoc/>
        public string ToString(IFormatProvider provider) => Value.ToString(provider);

        /// <inheritdoc/>
        public string ToString(string format) => Value.ToString(format);

        /// <inheritdoc/>
        public string ToString(string format, IFormatProvider provider) => Value.ToString(format, provider);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is UInt24 ui24 && ui24.Value == Value;

        /// <inheritdoc/>
        public bool Equals(UInt24 other) => this == other;

        /// <inheritdoc/>
        public bool Equals(uint other) => Value == other;

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc/>
        public int CompareTo(object value)
        {
            if (value == null)
                return 1;
            else if (value is UInt24 num24)
                return CompareTo(num24);
            if (value is uint num32)
                return CompareTo(num32);
            throw new ArgumentException("Argument must be an UInt32 or an UInt24");
        }

        /// <inheritdoc/>
        public int CompareTo(UInt24 value)
            => CompareTo(value.Value);

        /// <inheritdoc/>
        public int CompareTo(uint value)
            => Value < value ? -1 : Value > value ? 1 : 0;

        #region operators
        public static UInt24 operator ++(UInt24 a) => new UInt24(a.Value + 1);

        public static UInt24 operator --(UInt24 a) => new UInt24(a.Value - 1);

        public static implicit operator UInt24(byte x) => new UInt24((uint)x);

        public static explicit operator byte(UInt24 x) => (byte)x.Value;

        public static explicit operator UInt24(sbyte x) => new UInt24((uint)x);

        public static explicit operator sbyte(UInt24 x) => (sbyte)x.Value;

        public static explicit operator UInt24(short x) => new UInt24   ((uint)x);

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

        bool IConvertible.ToBoolean(IFormatProvider provider)
            => Convert.ToBoolean(Value, provider);

        char IConvertible.ToChar(IFormatProvider provider)
            => Convert.ToChar(Value, provider);

        sbyte IConvertible.ToSByte(IFormatProvider provider)
            => Convert.ToSByte(Value, provider);

        byte IConvertible.ToByte(IFormatProvider provider)
            => Convert.ToByte(Value, provider);

        short IConvertible.ToInt16(IFormatProvider provider)
            => Convert.ToInt16(Value, provider);

        ushort IConvertible.ToUInt16(IFormatProvider provider)
            => Convert.ToUInt16(Value, provider);

        int IConvertible.ToInt32(IFormatProvider provider)
            => Convert.ToInt32(Value, provider);

        uint IConvertible.ToUInt32(IFormatProvider provider)
            => Value;

        long IConvertible.ToInt64(IFormatProvider provider)
            => Convert.ToInt64(Value, provider);

        ulong IConvertible.ToUInt64(IFormatProvider provider)
            => Convert.ToUInt64(Value, provider);

        float IConvertible.ToSingle(IFormatProvider provider)
            => Convert.ToSingle(Value, provider);

        double IConvertible.ToDouble(IFormatProvider provider)
            => Convert.ToDouble(Value, provider);

        decimal IConvertible.ToDecimal(IFormatProvider provider)
            => Convert.ToDecimal(Value, provider);

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
            => Convert.ToDateTime(Value, provider);

        object IConvertible.ToType(Type type, IFormatProvider provider)
            => Convert.ChangeType(Value, type, provider);

        /// <inheritdoc/>
        public TypeCode GetTypeCode()
            => TypeCode.UInt32;
        #endregion IConvertible
    }
}
