// Adapted from .NET 8 implementation for compatibility with older .NET versions.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;

namespace AuroraLib.Core.IO
{
    public static partial class StreamEx
    {
#if NET20_OR_GREATER || NETSTANDARD2_0
        /// <inheritdoc cref="Stream.Read(byte[], int, int)"/>
        public static int Read(this Stream stream, Span<byte> buffer)
        {
            if (buffer.IsEmpty) return 0;

            if (stream is PoolStream pool) return pool.Read(buffer);

            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                int numRead = stream.Read(sharedBuffer, 0, buffer.Length);
                sharedBuffer.AsSpan(0, numRead).CopyTo(buffer);
                return numRead;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }


        /// <inheritdoc cref="Stream.Write(byte[], int, int)"/>
        public static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
        {
            if (buffer.IsEmpty)
                return;

            if (stream is PoolStream pool)
            {
                pool.Write(buffer);
                return;
            }

            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                buffer.CopyTo(sharedBuffer);
                stream.Write(sharedBuffer, 0, buffer.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }
#endif


#if !NET8_0_OR_GREATER


        /// <summary>
        /// Reads <paramref name="count"/> number of bytes from the current stream and advances the position within the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte array with the values
        /// between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced
        /// by the bytes read from the current stream.
        /// </param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The number of bytes to be read from the current stream.</param>
        /// <remarks>
        /// When <paramref name="count"/> is 0 (zero), this read operation will be completed without waiting for available data in the stream.
        /// </remarks>
        public static void ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
#if NET5_0_OR_GREATER
            => ReadAtLeastCore(stream, buffer.AsSpan(offset, count), count, throwOnEndOfStream: true);
#else
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = stream.Read(buffer, offset + totalRead, count - totalRead);
                if (read == 0)
                {
                    ThrowHelper.EndOfStreamException<byte>(count);
                }

                totalRead += read;
            }
        }
#endif
        /// <inheritdoc cref="ReadExactly(Stream, byte[], int, int)"/>
        public static void ReadExactly(this Stream stream, Span<byte> buffer)
            => ReadAtLeastCore(stream, buffer, buffer.Length, throwOnEndOfStream: true);

        /// <summary>
        /// Reads at least a minimum number of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer">A region of memory. When this method returns, the contents of this region are replaced by the bytes read from the current stream.</param>
        /// <param name="minimumBytes">The minimum number of bytes to read into the buffer.</param>
        /// <param name="throwOnEndOfStream">
        /// <see langword="true"/> to throw an exception if the end of the stream is reached before reading <paramref name="minimumBytes"/> of bytes;
        /// <see langword="false"/> to return less than <paramref name="minimumBytes"/> when the end of the stream is reached.
        /// The default is <see langword="true"/>.
        /// </param>
        /// <returns>
        /// The total number of bytes read into the buffer. This is guaranteed to be greater than or equal to <paramref name="minimumBytes"/>
        /// when <paramref name="throwOnEndOfStream"/> is <see langword="true"/>. This will be less than <paramref name="minimumBytes"/> when the
        /// end of the stream is reached and <paramref name="throwOnEndOfStream"/> is <see langword="false"/>. This can be less than the number
        /// of bytes allocated in the buffer if that many bytes are not currently available.
        /// </returns>
        public static int ReadAtLeast(this Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
        {
            if (minimumBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(minimumBytes), "Must be a non-negative value.");

            if (buffer.Length < minimumBytes)
                throw new ArgumentOutOfRangeException(nameof(minimumBytes), "Cannot exceed the buffer length.");

            return ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
        }

        // No argument checking is done here. It is up to the caller.
        private static int ReadAtLeastCore(Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
        {
            Debug.Assert(minimumBytes <= buffer.Length);

            int totalRead = 0;
            while (totalRead < minimumBytes)
            {
                int read = stream.Read(buffer.Slice(totalRead));
                if (read == 0)
                {
                    if (throwOnEndOfStream)
                        ThrowHelper.EndOfStreamException<byte>(minimumBytes);

                    return totalRead;
                }

                totalRead += read;
            }

            return totalRead;
        }
#endif
    }
}
