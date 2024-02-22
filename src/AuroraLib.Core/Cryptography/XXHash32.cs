using AuroraLib.Core.Interfaces;
using HashDepot;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// 32-bit xxHash implementation.
    /// </summary>
    public sealed class XXHash32 : IHash<uint>
    {
        /// <inheritdoc />
        public uint Value { get; private set; }

        /// <inheritdoc />
        public int ByteSize => 4;

        /// <inheritdoc />
        public void Compute(ReadOnlySpan<byte> input)
            => Value = XXHash.Hash32(input, Value);

        /// <inheritdoc />
        public byte[] GetBytes()
            => BitConverterX.GetBytes(Value);

        /// <inheritdoc />
        public void Write(Span<byte> destination)
        {
            uint vaule = Value;
            MemoryMarshal.Write(destination, ref vaule);
        }

        /// <inheritdoc />
        public void SetSeed(uint seed = 0u)
            => Value = seed;

        /// <inheritdoc />
        public void Reset() => Value = 0;

        /// <summary>
        /// Generate a 32-bit xxHash value
        /// </summary>
        /// <param name="input">The input data to compute the hash for.</param>
        /// <param name="seed">The seed value to set.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Generate(ReadOnlySpan<byte> input, uint seed = 0u)
            => XXHash.Hash32(input, seed);
    }
}
