using AuroraLib.Core.Collections;
using AuroraLib.Core.Exceptions;
using AuroraLib.Core.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuroraLib.Core.IO
{
    public static partial class StreamEx
    {

        #region Read Span<char>
        /// <inheritdoc cref=" Read(Stream,Span{char}, Encoding)"/>
        [DebuggerStepThrough]
        public static int Read(this Stream stream, Span<char> chars)
        {
#if NET20_OR_GREATER
            byte[] bytes = new byte[chars.Length];
            int r = stream.Read(bytes, 0, bytes.Length);
#else
            Span<byte> bytes = stackalloc byte[chars.Length];
            int r = stream.Read(bytes);
#endif
            EncodingX.GetChars(bytes, chars);
            return r;
        }

        /// <summary>
        /// Reads characters from the stream into the provided character span, until it is filled.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="chars">The span to write the characters into.</param>
        /// <param name="encoding">The encoding used to decode the bytes read from the stream.</param>
        [DebuggerStepThrough]
        public static int Read(this Stream stream, Span<char> chars, Encoding encoding)
        {
#if NET20_OR_GREATER
            byte[] bytes = new byte[encoding.GetByteCount(chars)];
            int r = stream.Read(bytes, 0, bytes.Length);
#else
            Span<byte> bytes = stackalloc byte[encoding.GetByteCount(chars)];
            int r = stream.Read(bytes);
#endif
            encoding.GetChars(bytes, chars);
            return r;
        }
        #endregion

        #region Write ReadOnlySpan<char>
        /// <inheritdoc cref="Write(Stream, ReadOnlySpan{char}, Encoding)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(this Stream stream, ReadOnlySpan<char> chars)
            => stream.Write(chars, EncodingX.DefaultEncoding);

        /// <summary>
        /// Writes a sequence of characters to the <paramref name="stream"/>.
        /// won't be null terminated.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="chars">The characters to write.</param>
        /// <param name="encoding">The encoding to use for converting characters to bytes.</param>
        [DebuggerStepThrough]
        public static void Write(this Stream stream, ReadOnlySpan<char> chars, Encoding encoding)
        {
#if NET20_OR_GREATER
            byte[] buffer = new byte[encoding.GetByteCount(chars)];
#else
            Span<byte> buffer = stackalloc byte[encoding.GetByteCount(chars)];
#endif
            encoding.GetBytes(chars, buffer);
#if NET20_OR_GREATER
            stream.Write(buffer, 0, buffer.Length);
#else
            stream.Write(buffer);
#endif
        }

        #endregion

        #region ReadString

        /// <inheritdoc cref="ReadString(Stream, int, Encoding, byte)"/>
        [DebuggerStepThrough]
        public static string ReadString(this Stream stream, int length, byte padding = 0x0)
            => ReadString(stream, length, EncodingX.DefaultEncoding, padding);

        /// <summary>
        /// Reads a string by reading the specified number of bytes from the specified <paramref name="stream"/>.
        /// <paramref name="padding"/> bytes are removed from the resulting string.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="encoding">The encoding to use for converting the read bytes to a string.</param>
        /// <param name="padding">The byte to be removed from the resulting string if found (optional).</param>
        /// <returns>The string read from the stream with the terminator byte removed if present.</returns>
        [DebuggerStepThrough]
        public static string ReadString(this Stream stream, int length, Encoding encoding, byte padding = 0x0)
        {
#if NET20_OR_GREATER
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, bytes.Length);
#else
            Span<byte> bytes = stackalloc byte[length];
            stream.Read(bytes);
#endif
            return EncodingX.GetCString(bytes, encoding, padding);
        }

        #endregion

        #region WriteString

        /// <inheritdoc cref="Write(Stream, ReadOnlySpan{char}, Encoding)"/>
        public static void WriteString(this Stream stream, ReadOnlySpan<char> chars)
            => stream.Write(chars, EncodingX.DefaultEncoding);

        /// <inheritdoc cref="Write(Stream, ReadOnlySpan{char}, Encoding)"/>
        public static void WriteString(this Stream stream, ReadOnlySpan<char> chars, Encoding encoding)
            => stream.Write(chars, encoding);

        /// <summary>
        /// Writes a specified number of characters to the <paramref name="stream"/> and adds a <paramref name="padding"/> <see cref="byte"/> at the end.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="chars">The characters to write.</param>
        /// <param name="encoding">The encoding to use for converting characters to bytes.</param>
        /// <param name="length">The maximum number of bytes to write.</param>
        /// <param name="padding">The padding byte to add at the end (default is 0x0).</param>
        /// <exception cref="ArgumentException">Thrown when the encoded bytes exceed the specified length.</exception>
        [DebuggerStepThrough]
        public static void WriteString(this Stream stream, ReadOnlySpan<char> chars, Encoding encoding, int length, byte padding = 0x0)
        {
            if (encoding.GetByteCount(chars) > length)
            {
                throw new ArgumentException();
            }

            Span<byte> buffer = stackalloc byte[length];
            buffer.Fill(padding);
            encoding.GetBytes(chars, buffer);
            stream.Write(buffer);
        }

        /// <inheritdoc cref="WriteString(Stream, ReadOnlySpan{char}, Encoding,int,byte)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteString(this Stream stream, ReadOnlySpan<char> chars, int length, byte padding = 0x0)
            => stream.WriteString(chars, EncodingX.DefaultEncoding, length, padding);

        #endregion

        #region ReadCString

        /// <summary>
        /// Reads a string from the specified <paramref name="stream"/>.
        /// The string is read until an terminator byte is encountered.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="terminator">The byte that indicates the end of the string.</param>
        /// <returns>The string read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadCString(this Stream stream, byte terminator = 0x0)
            => ReadCString(stream, s => s == terminator);

        /// <summary>
        /// Reads a string from the specified <paramref name="stream"/>.
        /// The string is read until the specified <paramref name="terminator"/> byte is encountered.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding to use when getting the string.</param>
        /// <param name="terminator">The byte that indicates the end of the string.</param>
        /// <returns>The string read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadCString(this Stream stream, Encoding encoding, byte terminator = 0x0)
            => ReadCString(stream, encoding, s => s == terminator);

        ///  <inheritdoc cref="ReadCString(Stream, Encoding, Predicate{byte})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadCString(this Stream stream, Predicate<byte> ifstopByte)
        {
            List<byte> bytes = ReadCStringBytes(stream, ifstopByte);
#if NET5_0_OR_GREATER
            return EncodingX.GetString(bytes.UnsafeAsSpan());
#else
            return EncodingX.GetString(bytes.ToArray());
#endif
        }

        /// <summary>
        /// Reads a string from the <paramref name="stream"/> using the specified encoding until the provided <paramref name="ifstopByte"/> predicate returns true.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding used to decode the string.</param>
        /// <param name="ifstopByte">The predicate that determines when to stop reading.</param>
        /// <returns>The string read from the stream.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadCString(this Stream stream, Encoding encoding, Predicate<byte> ifstopByte)
        {
            List<byte> bytes = ReadCStringBytes(stream, ifstopByte);
#if NET5_0_OR_GREATER
            return encoding.GetString(bytes.UnsafeAsSpan());
#else
            return encoding.GetString(bytes.ToArray());
#endif
        }

        private static List<byte> ReadCStringBytes(this Stream stream, Predicate<byte> ifstopByte)
        {
            List<byte> bytes = new List<byte>();
            int readByte;
            do
            {
                readByte = stream.ReadByte();
                if (readByte == -1)
                    throw new EndOfStreamException();

                if (ifstopByte.Invoke((byte)readByte))
                    break;

                bytes.Add((byte)readByte);

            } while (true);

            return bytes;
        }
        #endregion

        #region WriteCString

        /// <inheritdoc cref="WriteCString(Stream, ReadOnlySpan{char}, Encoding, byte)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCString(this Stream stream, ReadOnlySpan<char> chars, byte terminator = 0x0)
            => stream.WriteCString(chars, EncodingX.DefaultEncoding, terminator);

        /// <summary>
        /// Writes a string to the given stream using the specified encoding, followed by a <paramref name="terminator"/> byte.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="chars">The characters to be written.</param>
        /// <param name="encoding">The encoding to use for the characters.</param>
        /// <param name="terminator">The terminator byte to append after the string. Default is 0x0.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCString(this Stream stream, ReadOnlySpan<char> chars, Encoding encoding, byte terminator = 0x0)
        {
            stream.Write(chars, encoding);
            stream.WriteByte(terminator);
        }

        #endregion

        #region MatchString
        /// <summary>
        /// Matches the specified <paramref name="expected"/> characters with the data in the <paramref name="stream"/>/>.
        /// </summary>
        /// <param name="stream">The stream to match against.</param>
        /// <param name="expected">The characters to match.</param>
        /// <param name="encoding">The encoding used for converting characters to bytes.</param>
        /// <returns>true if the specified characters match the data in the stream; otherwise, false.</returns>
        [DebuggerStepThrough]
        public static bool Match(this Stream stream, ReadOnlySpan<char> expected, Encoding encoding)
        {
            Span<byte> buffer = stackalloc byte[encoding.GetByteCount(expected)];
            encoding.GetBytes(expected, buffer);
            return stream.Match(buffer);
        }

        /// <inheritdoc cref="Match(Stream, ReadOnlySpan{char},Encoding)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Match(this Stream stream, ReadOnlySpan<char> expected)
            => stream.Match(expected, EncodingX.DefaultEncoding);

        /// <summary>
        /// Matches the characters in the <paramref name="stream"/> with the specified <paramref name="expected"/> characters and throws an <see cref="InvalidIdentifierException"/> if the match fails.
        /// </summary>
        /// <param name="stream">The stream to match against.</param>
        /// <param name="expected">The expected bytes to match.</param>
        /// <param name="encoding">The encoding used for converting characters to bytes.</param>
        /// <exception cref="InvalidIdentifierException">Thrown when the match fails.</exception>
        [DebuggerStepThrough]
        public static void MatchThrow(this Stream stream, ReadOnlySpan<char> expected, Encoding encoding)
        {
            Span<byte> buffer = stackalloc byte[encoding.GetByteCount(expected)];
            encoding.GetBytes(expected, buffer);
            stream.MatchThrow(buffer);
        }

        /// <inheritdoc cref="MatchThrow(Stream, ReadOnlySpan{char},Encoding)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MatchThrow(this Stream stream, ReadOnlySpan<char> expected)
            => stream.MatchThrow(expected, EncodingX.DefaultEncoding);

        #endregion

        #region WriteAlignString
        /// <summary>
        /// Writes padding to the <paramref name="stream"/> to align the position to the specified <paramref name="boundary"/>, using the provided characters.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="boundary">The desired alignment boundary.</param>
        /// <param name="chars">The characters to use for padding.</param>
        /// <param name="encoding">The encoding used to convert the characters to bytes.</param>
        [DebuggerStepThrough]
        public static void WriteAlign(this Stream stream, int boundary, ReadOnlySpan<char> chars, Encoding encoding)
        {
            Span<byte> buffer = stackalloc byte[encoding.GetByteCount(chars)];
            encoding.GetBytes(chars, buffer);
            stream.WriteAlign(boundary, buffer);
        }

        /// <inheritdoc cref="WriteAlign(Stream,int, ReadOnlySpan{char},Encoding)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteAlign(this Stream stream, int boundary, ReadOnlySpan<char> chars)
            => stream.WriteAlign(boundary, chars, EncodingX.DefaultEncoding);
        #endregion
    }
}
