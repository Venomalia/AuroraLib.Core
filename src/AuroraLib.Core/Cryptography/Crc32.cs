using AuroraLib.Core.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// Represents a CRC-32 hash algorithm.
    /// </summary>
    public sealed class Crc32 : IHash<uint>
    {
        /// <inheritdoc />
        public uint Value => _value ^= _xorOut;
        private uint _value;

        /// <inheritdoc />
        public int ByteSize => 4;

        private readonly uint _init;
        private readonly uint _xorOut;
        private readonly bool _reverse;
        private readonly uint[] _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc32"/> class.
        /// </summary>
        /// <param name="polynomial">The polynomial used for CRC calculation.</param>
        /// <param name="reverse">Use reverse calculation.</param>
        /// <param name="initial">The initial value for the CRC calculation.</param>
        /// <param name="xorOut">The XOR output value for the CRC calculation.</param>
        public Crc32(uint polynomial, bool reverse = false, uint initial = uint.MaxValue, uint xorOut = uint.MaxValue)
        {
            _table = InitializeTable(polynomial, reverse);
            _xorOut = xorOut;
            _init = initial;
            _value = initial;
            _reverse = reverse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc32"/> class with the specified <see cref="Crc32Algorithm"/>.
        /// </summary>
        /// <param name="alg">The CRC-32 algorithm to use.</param>
        /// <remarks>
        /// This constructor simplifies the creation of a <see cref="Crc32"/> instance by providing a high-level interface
        /// that abstracts the specific CRC-32 algorithm details. It internally retrieves the polynomial, reverse, initial,
        /// and xorOut values associated with the specified algorithm and initializes the <see cref="Crc32"/> instance using
        /// those values.
        /// </remarks>
        public Crc32(Crc32Algorithm alg) : this(alg.Polynomial(), alg.Reverse(), alg.Initial(), alg.XorOut())
        { }

        ///  <inheritdoc cref="Crc32(uint, bool, uint, uint)"/>
        public Crc32() : this(Crc32Algorithm.Default)
        { }

        /// <inheritdoc />
        public void Compute(ReadOnlySpan<byte> input)
        {
            if (_reverse)
            {
                foreach (byte b in input)
                {
                    _value = _table[(byte)(((_value >> 24) & 0xFF) ^ b)] ^ (_value << 8);
                }
            }
            else
            {
                foreach (byte b in input)
                {
                    _value = (_value >> 8) ^ _table[(_value & 0xff) ^ b];
                }
            }
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes()
            => BitConverterX.GetBytes(Value);

        /// <inheritdoc />
        public void Write(Span<byte> destination)
        {
            uint vaule = Value;
            MemoryMarshal.Write(destination, ref vaule);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
            => _value = _init;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeed(uint seed)
            => _value = seed;

        private static uint[] InitializeTable(uint polynomial, bool reverse)
        {
            //use ArrayPool?
            uint[] polynomialTable = new uint[256];
            if (reverse)
            {
                for (uint i = 0; i < 256; i++)
                {
                    uint entry;
                    entry = i << 24;
                    for (int j = 8 - 1; j >= 0; j--)
                    {
                        if ((entry & 0x80000000) != 0)
                            entry = (entry << 1) ^ polynomial;
                        else
                            entry <<= 1;
                    }
                    polynomialTable[i] = entry;
                }
                return polynomialTable;
            }
            else
            {
                polynomial = BitConverterX.SwapBits(polynomial);
                for (uint i = 0; i < 256; i++)
                {
                    uint entry;
                    entry = i;
                    for (int j = 0; j < 8; j++)
                    {
                        if ((entry & 1) == 1)
                            entry = (entry >> 1) ^ polynomial;
                        else
                            entry >>= 1;
                    }
                    polynomialTable[i] = entry;
                }
                return polynomialTable;
            }
        }
    }
}
