using AuroraLib.Core.Extensions;
using System;
using System.Diagnostics;
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

        internal static readonly Predicate<byte> InvalidByte = b => b < 32 || b == 127;

        #region GetChars

        /// <summary>
        /// Converts a ReadOnlySpan of bytes to Span of characters.
        /// </summary>
        /// <param name="bytes">The bytes to convert to characters.</param>
        /// <param name="chars">The span to write the characters into.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetChars(ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            for (int i = 0; i < bytes.Length; i++)
                chars[i] = (char)bytes[i];
        }
        #endregion

        #region GetString
        /// <summary>
        /// Converts the specified byte span to a string using the default encoding.
        /// </summary>
        /// <param name="bytes">The byte span to convert to a string.</param>
        /// <returns>A string that contains the characters representing the bytes in the input span.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetString(ReadOnlySpan<byte> bytes)
        {
            Span<char> chars = stackalloc char[bytes.Length];
            GetChars(bytes, chars);
#if NET20_OR_GREATER || NETSTANDARD2_0
            return chars.ToString();
#else
            return new string(chars);
#endif
        }

        /// <summary>
        /// Converts a span of bytes to a string, stopping at the specified terminator byte.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <param name="terminator">The terminator byte indicating the end of the string.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCString(ReadOnlySpan<byte> bytes, Encoding encoding, byte terminator = 0x0)
        {
            int length = bytes.IndexOf(terminator);
            if (length == -1) length = bytes.Length;
            return encoding.GetString(bytes.Slice(0, length));
        }

        /// <inheritdoc cref="GetCString(ReadOnlySpan{byte}, Encoding, byte)"/>
        [DebuggerStepThrough]
        public static string GetCString(ReadOnlySpan<byte> bytes, byte terminator = 0x0)
        {
            int length = bytes.IndexOf(terminator);
            if (length == -1) length = bytes.Length;
            return GetString(bytes.Slice(0, length));
        }
        #endregion

        #region GetDisplayableString
        /// <summary>
        /// Converts the specified byte span to a displayable string, if bytes are found that cannot be displayed, a hexadecimal string is output.
        /// </summary>
        /// <param name="bytes">The byte span to convert to a displayable string.</param>
        /// <returns>A displayable string representing the input bytest.</returns>
        [DebuggerStepThrough]
        public static string GetDisplayableString(ReadOnlySpan<byte> bytes)
        {
            int length = SpanEx.IndexOf(bytes, InvalidByte);
            if (length == -1)
                return GetString(bytes);

            for (int i = length; i < bytes.Length; i++)
            {
                if (bytes[i] != 0)
                    return BitConverter.ToString(bytes.ToArray());
            }
            return GetString(bytes.Slice(0, length));
        }
        #endregion

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
