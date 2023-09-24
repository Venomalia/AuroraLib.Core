using AuroraLib.Core.Interfaces;
using AuroraLib.Core.Text;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AuroraLib.Core.Cryptography
{
    public static class HashEX
    {
        /// <summary>
        /// Computes the hash of the specified input using the given <see cref="Encoding"/> and updates the hash value.
        /// </summary>
        /// <param name="hash">The hash algorithm instance.</param>
        /// <param name="input">The input data to compute the hash for.</param>
        /// <param name="encoding">The encoding used to convert the input data to bytes.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Compute(this IHash hash, ReadOnlySpan<char> input, Encoding encoding)
        {
            Span<byte> buffer = stackalloc byte[encoding.GetByteCount(input)];
            encoding.GetBytes(input, buffer);
            hash.Compute(buffer);
        }

        /// <summary>
        /// Computes the hash of the specified input using the default encoding and updates the hash value.
        /// </summary>
        /// <param name="hash">The hash algorithm instance.</param>
        /// <param name="input">The input data as a <see cref="ReadOnlySpan{T}"/> of <see cref="char"/>.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Compute(this IHash hash, ReadOnlySpan<char> input)
            => Compute(hash, input, EncodingX.DefaultEncoding);

        /// <summary>
        /// Computes the hash of the specified input <typeparamref name="T"/> value and updates the hash value.
        /// </summary>
        /// <typeparam name="T">The type of the input value. It must be an unmanaged type.</typeparam>
        /// <param name="hash">The hash algorithm instance.</param>
        /// <param name="input">The input value.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Compute<T>(this IHash hash, T input) where T : unmanaged
            => hash.Compute(new ReadOnlySpan<byte>(&input, sizeof(T)));

        /// <summary>
        /// Computes the hash for a <see cref="ReadOnlySpan{T}"/> of <typeparamref name="T"/> and updates the hash object.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input span.</typeparam>
        /// <param name="hash">The hash object to update.</param>
        /// <param name="input">The <see cref="ReadOnlySpan{T}"/> to compute the hash from.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Compute<T>(this IHash hash, ReadOnlySpan<T> input) where T : unmanaged
        {
            ReadOnlySpan<byte> buffer = MemoryMarshal.Cast<T,byte>(input);
            hash.Compute(buffer);
        }

        /// <summary>
        /// Asynchronously computes a hash value for the specified stream using the given hash algorithm.
        /// </summary>
        /// <typeparam name="T">The type of hash value to compute.</typeparam>
        /// <param name="hash">The hash algorithm to use.</param>
        /// <param name="stream">The stream from which to read data.</param>
        /// <param name="bufferSize">The size of the buffer used for reading data from the stream.</param>
        /// <returns>A task representing the asynchronous operation that computes the hash value.</returns>
        [DebuggerStepThrough]
        public static async Task<T> ComputeAsync<T>(this IHash<T> hash, Stream stream, int bufferSize = 4096) where T : unmanaged
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                int bytes;
                while ((bytes = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
                {
                    hash.Compute(buffer.AsSpan(0, bytes));
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            return hash.Value;
        }

    }
}
