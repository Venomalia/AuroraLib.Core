using AuroraLib.Core.Interfaces;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// Can be used for any <see cref="HashAlgorithm"/> implementation.
    /// </summary>
    public sealed class BaseHash : IHash
    {
        private readonly byte[] bytes;

        private readonly HashAlgorithm hashInstance;

        public BaseHash(HashAlgorithm algorithm)
        {
            hashInstance = algorithm;
            bytes = new byte[hashInstance.HashSize / 8];
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Compute(ReadOnlySpan<byte> input)
            => hashInstance.TryComputeHash(input, bytes, out _);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes()
        {
            byte[] output = new byte[bytes.Length];
            bytes.AsSpan().CopyTo(output);
            return output;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            bytes.AsSpan().Clear();
            hashInstance.Initialize();
        }
    }
}
