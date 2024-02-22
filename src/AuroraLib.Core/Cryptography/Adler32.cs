using AuroraLib.Core.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// 32-bit Adler implementation.
    /// </summary>
    public sealed class Adler32 : IHash<uint>
    {
        private const uint Prime = 65521;

        /// <inheritdoc />
        public uint Value { get; private set; }

        /// <inheritdoc />
        public int ByteSize => 4;

        public Adler32()
            => Reset();

        /// <inheritdoc />
        public void Compute(ReadOnlySpan<byte> input)
        {
            uint s1 = Value & 0xFFFF;
            uint s2 = Value >> 16;
            var count = input.Length;
            var offset = 0;
            while (count > 0)
            {
                int n = Math.Min(3800, count);
                count -= n;

                while (--n >= 0)
                {
                    s1 += input[offset++];
                    s2 += s1;
                }

                s1 %= Prime;
                s2 %= Prime;
            }
            Value = (s2 << 16) | s1;
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
            => Value = 1;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeed(uint seed = 1)
            => Value = seed;
    }
}
