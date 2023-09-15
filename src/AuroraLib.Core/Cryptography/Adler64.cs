using AuroraLib.Core.Interfaces;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// 64-bit Adler implementation.
    /// </summary>
    public class Adler64 : IHash<ulong>
    {
        private const ulong Prime = 4294967291;

        public ulong Value { get; private set; }

        public Adler64()
            => Reset();

        /// <inheritdoc />
        public void Compute(ReadOnlySpan<byte> input)
        {
            ulong s1 = Value & 0xFFFFFFFF;
            ulong s2 = Value >> 32;
            var count = input.Length;
            var offset = 0;
            while (count > 0)
            {
                int n = Math.Min(65536, count);
                count -= n;

                while (--n >= 0)
                {
                    s1 += input[offset++];
                    s2 += s1;
                }

                s1 %= Prime;
                s2 %= Prime;
            }
            Value = (s2 << 32) | s1;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes()
            => BitConverterX.GetBytes(Value);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
            => Value = 1;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeed(ulong seed)
            => Value = seed;
    }
}
