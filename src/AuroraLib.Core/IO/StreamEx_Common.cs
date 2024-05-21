using AuroraLib.Core.Buffers;
using AuroraLib.Core.Exceptions;
using AuroraLib.Core.Interfaces;
using AuroraLib.Core.Text;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Extension of the <see cref="Stream"/>.
    /// </summary>
    public static partial class StreamEx
    {
#if NET20_OR_GREATER
        /// <inheritdoc cref="Stream.Read(byte[], int, int)"/>
        public static int Read(this Stream stream, Span<byte> buffer)
        {
            if (stream is PoolStream pool)
                return pool.Read(buffer);

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

        #region PeekByte
        /// <summary>
        /// Peek the next byte
        /// </summary>
        /// <param name="stream">this</param>
        /// <returns>The next byte to be read</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte PeekByte(this Stream stream)
        {
            byte val = stream.ReadUInt8();
            stream.Position--;
            return val;
        }
        #endregion

        #region To
        /// <summary>
        /// Writes the stream contents to a byte array, regardless of the <see cref="Stream.Position"/>.
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <returns>A array containing the content of the stream.</returns>
        [DebuggerStepThrough]
        public static byte[] ToArray(this Stream stream)
        {
            byte[] copy = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(copy, 0, copy.Length);
            return copy;
        }

        /// <summary>
        /// Writes the stream contents to a SpanBuffer, regardless of the <see cref="Stream.Position"/>.
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <returns>A SpanBuffer containing the content of the stream.</returns>
        /// <exception cref="NotSupportedException"></exception>
        [DebuggerStepThrough]
        public static SpanBuffer<byte> ToSpanBuffer(this Stream stream)
        {
            if (stream.Length > int.MaxValue)
                throw new NotSupportedException($"Stream length exceeds the maximum supported length for {nameof(SpanBuffer<byte>)}.");

            SpanBuffer<byte> copy = new SpanBuffer<byte>((int)stream.Length);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(copy.Span);
            return copy;
        }
        #endregion

        #region Match

        /// <summary>
        /// Matches the specified <paramref name="expected"/> identifier with the data in the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to match against.</param>
        /// <param name="expected">The expected bytes to match.</param>
        /// <returns>true if the specified bytes match the data in the stream; otherwise, false.</returns>
        [DebuggerStepThrough]
        public static bool Match(this Stream stream, ReadOnlySpan<byte> expected)
        {
            Span<byte> buffer = stackalloc byte[expected.Length];
            int i = stream.Read(buffer);
            return i == expected.Length && buffer.SequenceEqual(expected);
        }

        /// <inheritdoc cref="Match(Stream, ReadOnlySpan{byte})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Match(this Stream stream, IIdentifier expected)
            => stream.Match(expected.AsSpan());

        /// <summary>
        /// Matches the identifier in the <paramref name="stream"/> with the specified <paramref name="expected"/> identifier and throws an <see cref="InvalidIdentifierException"/> if the match fails.
        /// </summary>
        /// <param name="stream">The stream to match against.</param>
        /// <param name="expected">The expected bytes to match.</param>
        /// <exception cref="InvalidIdentifierException">Thrown when the match fails.</exception>
        [DebuggerStepThrough]
        public static void MatchThrow(this Stream stream, ReadOnlySpan<byte> expected)
        {
            Span<byte> buffer = stackalloc byte[expected.Length];
            int i = stream.Read(buffer);
            if (i != expected.Length || !buffer.SequenceEqual(expected))
            {
                throw new InvalidIdentifierException(buffer, expected);
            }
        }

        /// <inheritdoc cref="MatchThrow(Stream, ReadOnlySpan{byte})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MatchThrow(this Stream stream, IIdentifier expected)
            => stream.Match(expected.AsSpan());

        #endregion

        #region Search

        /// <summary>
        /// Searches for a specific pattern in a stream and moves its position until the pattern is found.
        /// Moves the current position until a pattern is found or the end is reached.
        /// </summary>
        /// <param name="stream">The stream to search.</param>
        /// <param name="pattern">The pattern to search for.</param>
        /// <returns>True if the pattern is found in the stream, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool Search(this Stream stream, ReadOnlySpan<byte> pattern)
        {
            int i = 0;
            int readbyte;
            while ((readbyte = stream.ReadByte()) > -1)
            {
                if (readbyte == pattern[i])
                {
                    i++;
                    if (i != pattern.Length)
                        continue;

                    stream.Seek(-pattern.Length, SeekOrigin.Current);
                    return true;
                }
                else
                {
                    i = 0;
                }
            }
            return false;
        }

        /// <inheritdoc cref="Search(Stream, ReadOnlySpan{byte})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Search(this Stream stream, string pattern) => stream.Search(EncodingX.DefaultEncoding.GetBytes(pattern));

        /// <inheritdoc cref="Search(Stream, ReadOnlySpan{byte})"/>
        /// <param name="stream">The stream to search.</param>
        /// <param name="patterns">The patterns of bytes to search for.</param>
        /// <param name="match">When this method returns true, contains the matched pattern; otherwise, null.</param>
        public static bool Search(this Stream stream, IEnumerable<byte[]> patterns, out byte[] match)
        {
            int[] i = new int[patterns.Count()];
            for (int p = 0; p < patterns.Count(); p++)
                i[p] = 0;

            int readbyte;
            while ((readbyte = stream.ReadByte()) > -1)
            {
                for (int p = 0; p < patterns.Count(); p++)
                {
                    if (readbyte == patterns.ElementAt(p)[i[p]])
                    {
                        i[p]++;
                        if (i[p] != patterns.ElementAt(p).Length)
                            continue;

                        stream.Seek(-patterns.ElementAt(p).Length, SeekOrigin.Current);
                        match = patterns.ElementAt(p);
                        return true;
                    }
                    else
                        i[p] = 0;
                }
            }
            match = Array.Empty<byte>();
            return false;
        }
        #endregion

        #region DetectByteOrder
        /// <summary>
        /// Detects the byte order (Endianess) of a data stream based on the smallest possible value.
        /// </summary>
        /// <typeparam name="T">The type of the values in the stream..</typeparam>
        /// <param name="stream">The stream containing the data.</param>
        /// <param name="count">The number of items.</param>
        /// <returns>The detected byte order.</returns>
        [DebuggerStepThrough]
        public static unsafe Endian DetectByteOrder<T>(this Stream stream, int count = 1) where T : unmanaged, IComparable<T>
        {
            long orpos = stream.Position;
            int trend = 0;
            for (int i = 0; i < count; i++)
            {
                T valueLE = stream.Read<T>();
                T valueBE = BitConverterX.Swap(valueLE);
                trend += valueLE.CompareTo(valueBE);
            }
            stream.Seek(orpos, SeekOrigin.Begin);
            return trend < 0 ? Endian.Little : Endian.Big;
        }

        /// <summary>
        /// Detects the byte order (Endianess) of a data stream based on the proximity to a reference value.
        /// </summary>
        /// <typeparam name="T">The type of the values in the stream..</typeparam>
        /// <param name="stream">The stream containing the data.</param>
        /// <param name="reference">The reference value used to determine proximity.</param>
        /// <returns>The detected byte order.</returns>
        [DebuggerStepThrough]
        public static unsafe Endian DetectByteOrderByDistance<T>(this Stream stream, T reference) where T : unmanaged, IConvertible
        {
            T valueLE = stream.Peek<T>();
            T valueBE = BitConverterX.Swap(valueLE);

            double dRef = reference.ToDouble(null);
            double diff1 = Math.Abs(valueLE.ToDouble(null) - dRef);
            double diff2 = Math.Abs(valueBE.ToDouble(null) - dRef);

            return diff1 < diff2 ? Endian.Little : Endian.Big;
        }
        #endregion

        #region Skip
        /// <summary>
        /// Skips a specified number of bytes in the stream.
        /// </summary>
        /// <param name="stream">The stream to skip bytes from.</param>
        /// <param name="count">The number of bytes to skip.</param>
        [DebuggerStepThrough]
        public static void Skip(this Stream stream, int count)
        {
            if (count < 1)
                return;

            if (stream.CanSeek)
            {
                stream.Seek(count, SeekOrigin.Current);
            }
            else
            {
                using (SpanBuffer<byte> bytes = new SpanBuffer<byte>(count))
                    stream.Read(bytes.Span);
            }
        }
        #endregion

        #region Align

        /// <summary>
        /// sets the position within the current stream to the nearest possible boundary.
        /// </summary>
        /// <param name="stream">the current stream</param>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <param name="boundary">The byte boundary to Seek to.</param>
        /// <returns>The new position within the current stream.</returns>
        [DebuggerStepThrough]
        public static long Align(this Stream stream, long offset, SeekOrigin origin, int boundary = 32)
        {
            if (boundary <= 1)
                throw new ArgumentException($"{nameof(boundary)}: Must be 2 or more");

#if NET20_OR_GREATER
            switch (origin)
            {
                case SeekOrigin.Begin:
                    offset += stream.Position;
                    break;
                case SeekOrigin.Current:
                    break;
                case SeekOrigin.End:
                    offset = stream.Length - offset;
                    break;
                default:
                    throw new AggregateException();
            }
#else
            offset = origin switch
            {
                SeekOrigin.Begin => offset + stream.Position,
                SeekOrigin.Current => offset,
                SeekOrigin.End => stream.Length - offset,
                _ => throw new AggregateException(),
            };
#endif
            return stream.Seek(AlignPosition(offset, boundary), SeekOrigin.Begin);
        }

        /// <inheritdoc cref="Align(Stream, long, SeekOrigin, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Align(this Stream stream, int boundary = 32)
            => stream.Seek(AlignPosition(stream.Position, boundary), SeekOrigin.Begin);

        #endregion

        #region WriteAlign

        /// <summary>
        /// Writes padding bytes to the stream until its position aligns with the specified boundary.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="boundary">The boundary to align the position of the stream with.</param>
        /// <param name="Padding">The byte value to use for padding (default is 0x00).</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteAlign(this Stream stream, int boundary = 32, byte Padding = 0x00)
        {
            Span<byte> bytes = stackalloc byte[(int)(boundary - (stream.Position % boundary))];
            bytes.Fill(Padding);
            stream.Write(bytes);
        }

        /// <summary>
        /// Writes <paramref name="Padding"/> to the <paramref name="stream"/> to align the position to the specified <paramref name="boundary"/>.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="boundary">The desired alignment boundary.</param>
        /// <param name="padding">The padding characters to write.</param>
        [DebuggerStepThrough]
        public static void WriteAlign(this Stream stream, int boundary, ReadOnlySpan<byte> padding)
        {
            int PadCount = (int)(boundary - (stream.Position % boundary));
            while (PadCount > 0)
            {
                int i = Math.Min(PadCount, padding.Length);
                stream.Write(padding.Slice(0, i));
                PadCount -= i;
            }
        }

        #endregion

        /// <summary>
        /// Calculates the aligned position based on the specified <paramref name="position"/> and <paramref name="boundary"/>.
        /// </summary>
        /// <param name="position">The position to align.</param>
        /// <param name="boundary">The alignment boundary (default is 32).</param>
        /// <returns>The aligned position.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AlignPosition(long position, int boundary = 32)
        {
            long remainder = position % boundary;
            if (remainder != 0)
                return position + boundary - remainder;
            return position;
        }
    }
}
