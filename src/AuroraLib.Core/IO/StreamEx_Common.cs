using AuroraLib.Core.Exceptions;
using AuroraLib.Core.Interfaces;
using AuroraLib.Core.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Extension of the <see cref="Stream"/>.
    /// </summary>
    public static partial class StreamEx
    {

        #region PeekByte
        /// <summary>
        /// Peek the next byte
        /// </summary>
        /// <param name="FS">this</param>
        /// <returns>The next byte to be read</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte PeekByte(this Stream FS)
        {
            byte val = (byte)FS.ReadByte();
            FS.Position--;
            return val;
        }
        #endregion

        #region ToArray
        /// <summary>
        /// Writes the stream contents to a byte array, regardless of the <see cref="Stream.Position"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static byte[] ToArray(this Stream stream)
        {
            byte[] copy = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(copy);
            return copy;
        }

        /// <summary>
        /// Writes the stream contents to a byte array, from the current <see cref="Stream.Position"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToArrayHere(this Stream stream)
        {
            byte[] copy = new byte[stream.Length - stream.Position];
            stream.Read(copy);
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
        /// searches for a specific pattern in a stream and moves its position until the pattern is found.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="pattern">the string to search for</param>
        /// <returns>"true" when the pattern is found</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Search(this Stream stream, string pattern) => stream.Search(pattern.GetBytes());

        /// <summary>
        /// Searches for the specified <paramref name="pattern"/> of <see cref="byte"/> in the <paramref name="stream"/>.
        /// Moves the current position until a pattern is found or the end is reached.
        /// </summary>
        /// <param name="stream">The stream to search.</param>
        /// <param name="pattern">The pattern of bytes to search for.</param>
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

        /// <summary>
        /// Searches for the specified <paramref name="patterns"/> of <see cref="Byte"/> in the <paramref name="stream"/>.
        /// Moves the current position until a pattern is found or the end is reached.
        /// </summary>
        /// <param name="stream">The stream to search.</param>
        /// <param name="patterns">The patterns of bytes to search for.</param>
        /// <param name="match">When this method returns true, contains the matched pattern; otherwise, null.</param>
        /// <returns>True if any of the patterns are found in the stream, otherwise false.</returns>
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

        #region Align

        /// <summary>
        /// sets the position within the current stream to the nearest possible boundary.
        /// </summary>
        /// <param name="stream">the current stream</param>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <param name="boundary">The byte boundary to Seek to</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static long Align(this Stream stream, long offset, SeekOrigin origin, int boundary = 32)
        {
            if (boundary <= 1)
                throw new ArgumentException($"{nameof(boundary)}: Must be 2 or more");

            switch (origin)
            {
                case SeekOrigin.Current:
                    offset += stream.Position;
                    break;

                case SeekOrigin.End:
                    offset = stream.Length - offset;
                    break;
            }
            return stream.Seek(AlignPosition(offset, boundary), SeekOrigin.Begin);
        }

        /// <summary>
        /// sets the position within the current stream to the nearest possible boundary.
        /// </summary>
        /// <param name="stream">the current stream</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns></returns>
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
                stream.Write(padding[..i]);
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
