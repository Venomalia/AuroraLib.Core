using AuroraLib.Core.Interfaces;
using HashDepot;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// 128-bit MurmurHash3 algorithms.
    /// </summary>
    public sealed class MurmurHash3_128 : IHash<UInt128>
    {
        /// <inheritdoc />
        public UInt128 Value => new(bytes);

        private byte[] bytes = new byte[12];

        /// <inheritdoc />
        public void Compute(ReadOnlySpan<byte> input)
            => bytes = MurmurHash3.Hash128(input, BitConverter.ToUInt32(bytes));

        /// <inheritdoc />
        public byte[] GetBytes() => bytes;

        /// <inheritdoc />
        public void Reset()
            => bytes.AsSpan().Fill(0);

        /// <inheritdoc />
        public void SetSeed(UInt128 seed)
            => bytes = BitConverterX.GetBytes(seed);

        /// <summary>
        /// Generate a 128-bit MurmurHash3 value
        /// </summary>
        /// <param name="input">The input data to compute the hash for.</param>
        /// <param name="seed">The seed value to set.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Generate(ReadOnlySpan<byte> input, uint seed = 0u)
            => MurmurHash3.Hash128(input, seed);
    }
}
