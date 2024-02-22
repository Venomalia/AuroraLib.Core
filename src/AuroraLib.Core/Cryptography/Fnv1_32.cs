using AuroraLib.Core.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// 32-bit FNV-1 algorithms.
    /// </summary>
    public sealed class Fnv1_32 : IHash<uint>
    {
        private const uint Prime = 0x01000193;
        private const uint Initial = 0x811c9dc5;
        private readonly bool A;

        /// <inheritdoc />
        public uint Value { get; private set; }

        /// <inheritdoc />
        public int ByteSize => 4;

        public Fnv1_32()
            => Value = Initial;

        public Fnv1_32(bool a) : this()
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
            uint vaule = Value;
            MemoryMarshal.Write(destination, ref vaule);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
            => Value = Initial;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeed(uint seed = 1)
            => Value = seed;
    }
}
