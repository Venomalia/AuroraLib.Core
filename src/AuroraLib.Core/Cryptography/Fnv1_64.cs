using AuroraLib.Core.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// 64-bit FNV-1 algorithms.
    /// </summary>
    public sealed class Fnv1_64 : IHash<ulong>
    {
        private const ulong Prime = 0x00000100000001B3;
        private const ulong Initial = 0xcbf29ce484222325;
        private readonly bool A;

        /// <inheritdoc />
        public ulong Value { get; private set; }

        /// <inheritdoc />
        public int ByteSize => 8;

        public Fnv1_64()
            => Value = Initial;

        public Fnv1_64(bool a) : this()
            => A = a;

        /// <inheritdoc />
        public void Compute(ReadOnlySpan<byte> input)
        {
            if (A)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    Value = (Value ^ input[i]) * Prime;
                }
            }
            else
            {
                for (int i = 0; i < input.Length; i++)
                {
                    Value = (Value * Prime) ^ input[i];
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
            ulong vaule = Value;
            MemoryMarshal.Write(destination, ref vaule);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
            => Value = Initial;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeed(ulong seed = 1)
            => Value = seed;
    }
}
