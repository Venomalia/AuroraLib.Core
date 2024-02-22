using AuroraLib.Core.Interfaces;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// 32-bit CityHash implementation.
    /// </summary>
    public sealed class CityHash32 : IHash<uint>
    {
        /// <inheritdoc />
        public uint Value { get; private set; }

        /// <inheritdoc />
        public int ByteSize => 4;

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Compute(ReadOnlySpan<byte> input)
            => Value = CityHash.Hash32(input);

        /// <inheritdoc />
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
            => Value = 0;

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeed(uint seed)
            => Value = seed;

        /// <summary>
        /// Generates a 32-bit CityHash hash from the provided input.
        /// </summary>
        /// <param name="input">The input data to hash as a ReadOnlySpan of bytes.</param>
        /// <returns>A 32-bit hash as a uint value.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Generate(ReadOnlySpan<byte> input)
            => CityHash.Hash32(input);

    }
}
