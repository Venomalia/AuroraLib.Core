using AuroraLib.Core.Interfaces;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// 64-bit CityHash implementation.
    /// </summary>
    public sealed class CityHash64 : IHash<ulong>
    {
        /// <inheritdoc />
        public ulong Value { get; private set; }

        /// <inheritdoc />
        public int ByteSize => 8;

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Compute(ReadOnlySpan<byte> input)
            => Value = Value == 0 ? CityHash.Hash64(input) : CityHash.Hash64(input, Value);

        /// <inheritdoc />
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
            => Value = 0;

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeed(ulong seed)
            => Value = seed;

        /// <summary>
        /// Generates a 64-bit CityHash hash from the provided input.
        /// </summary>
        /// <param name="input">The input data to hash as a ReadOnlySpan of bytes.</param>
        /// <returns>A 64-bit hash as a ulong value.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Generate(ReadOnlySpan<byte> input)
            => CityHash.Hash64(input);

        /// <summary>
        /// Generates a 64-bit CityHash hash from the provided input and a seed value.
        /// </summary>
        /// <param name="input">The input data to hash as a ReadOnlySpan of bytes.</param>
        /// <param name="seed">A seed value for the hash calculation.</param>
        /// <returns>A 64-bit hash as a ulong value.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Generate(ReadOnlySpan<byte> input, ulong seed)
            => CityHash.Hash64(input, seed);

    }
}
