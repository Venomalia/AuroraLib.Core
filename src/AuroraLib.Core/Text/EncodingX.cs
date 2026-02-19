using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuroraLib.Core.Text
{
    /// <summary>
    /// Encoding Extra functions
    /// </summary>
    public static class EncodingX
    {
        /// <summary>
        /// The default encoding used for operations.
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.GetEncoding(28591);

        /// <summary>
        /// Converts a span of bytes to a string, stopping at the specified terminator byte.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <param name="terminator">The terminator byte indicating the end of the string.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCString(this Encoding encoding, ReadOnlySpan<byte> bytes, byte terminator = 0x0)
        {
            int length = bytes.IndexOf(terminator);
            if (length == -1) length = bytes.Length;
            return encoding.GetString(bytes.Slice(0, length));
        }

#if NET20_OR_GREATER || NETSTANDARD2_0
        /// <inheritdoc cref="Encoding.GetString(byte[])"/>
        public static unsafe string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            fixed (byte* bytePtr = bytes)
            {
                return encoding.GetString(bytePtr, bytes.Length);
            }
        }

        /// <inheritdoc cref="Encoding.GetBytes(char*, int, byte*, int)"/>
        public static unsafe int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            fixed (char* charPtr = chars)
            fixed (byte* bytePtr = bytes)
            {
               return encoding.GetBytes(charPtr, chars.Length, bytePtr, bytes.Length);
            }
        }

        /// <inheritdoc cref="Encoding.GetChars(byte*, int, char*, int)"/>
        public static unsafe int GetChars(this Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            fixed (char* charPtr = chars)
            fixed (byte* bytePtr = bytes)
            {
                return encoding.GetChars(bytePtr, bytes.Length, charPtr, chars.Length);
            }
        }

        /// <inheritdoc cref="Encoding.GetByteCount(char[])"/>
        public static unsafe int GetByteCount(this Encoding encoding, ReadOnlySpan<char> chars)
        {
            fixed (char* charPtr = chars)
            {
                return encoding.GetByteCount(charPtr, chars.Length);
            }
        }
#endif
    }
}
