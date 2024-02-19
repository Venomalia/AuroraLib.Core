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

        #region GetStringFast
        /// <summary>
        /// Converts a span of bytes to a string, stopping at the specified terminator byte.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="terminator">The terminator byte indicating the end of the string.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        public static string GetStringFast(ReadOnlySpan<byte> bytes, byte terminator = 0x0)
        {
            int length = bytes.IndexOf(terminator);
            if (length == -1) length = bytes.Length;
            return InternalGetStringFast(bytes[..length]);
        }

        /// <summary>
        /// Converts a ReadOnlySpan of bytes to Span of characters.
        /// </summary>
        /// <param name="bytes">The bytes to convert to characters.</param>
        /// <param name="chars">The span to write the characters into.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetCharsFast(ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            for (int i = 0; i < chars.Length; i++)
                chars[i] = (char)bytes[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string InternalGetStringFast(ReadOnlySpan<byte> bytes)
        {
            Span<char> chars = stackalloc char[bytes.Length];
            GetCharsFast(bytes, chars);
            return new(chars);
        }


        #endregion

        #region GetString
        /// <summary>
        /// Converts a span of bytes to a string using the default encoding.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(ReadOnlySpan<byte> bytes)
            => DefaultEncoding.GetString(bytes);

        /// <summary>
        /// Converts a span of bytes to a string using the specified character encoding.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(ReadOnlySpan<byte> bytes, Encoding encoding)
            => encoding.GetString(bytes);


        /// <summary>
        /// Converts a span of bytes to a string using the default encoding, stopping at the specified terminator byte.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="terminator">The terminator byte indicating the end of the string.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(ReadOnlySpan<byte> bytes, in byte terminator)
            => GetString(bytes, DefaultEncoding, terminator);

        /// <summary>
        /// Converts a span of bytes to a string using the specified encoding, stopping at the specified terminator byte.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <param name="terminator">The terminator byte indicating the end of the string.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(ReadOnlySpan<byte> bytes, Encoding encoding, in byte terminator)
        {
            int end = bytes.IndexOf(terminator);
            return encoding.GetString(bytes[..(end == -1 ? bytes.Length : end)]);
        }
        #endregion

        #region GetValidString
        /// <summary>
        /// Converts a span of bytes to a string, excluding invalid characters (bytes with values less than 0x20 or equal to 127).
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetValidString(ReadOnlySpan<byte> bytes)
            => GetStringFast(bytes[..GetValidByteCount(bytes)]);

        /// <summary>
        /// Converts a span of bytes to a string, excluding bytes that match the specified invalid byte predicate.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="invalidByte">The predicate used to determine if a byte is invalid.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetValidString(ReadOnlySpan<byte> bytes, Predicate<byte> invalidByte)
            => GetStringFast(bytes[..GetValidByteCount(bytes, invalidByte)]);

        /// <summary>
        /// Converts a span of bytes to a string using the specified encoding, excluding invalid characters (bytes with values less than 0x20 or equal to 127).
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetValidString(ReadOnlySpan<byte> bytes, Encoding encoding)
            => encoding.GetString(bytes[..GetValidByteCount(bytes)]);

        /// <summary>
        /// Converts a span of bytes to a string using the specified encoding, excluding bytes that match the specified invalid byte predicate.
        /// </summary>
        /// <param name="bytes">The span of bytes to convert.</param>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <param name="invalidByte">The predicate used to determine if a byte is invalid.</param>
        /// <returns>The resulting string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetValidString(ReadOnlySpan<byte> bytes, Encoding encoding, Predicate<byte> invalidByte)
            => encoding.GetString(bytes[..GetValidByteCount(bytes, invalidByte)]);
        #endregion

        #region GetValidByteCount
        /// <summary>
        /// Determines the number of valid bytes in the given span of bytes.
        /// </summary>
        /// <param name="bytes">The span of bytes to check.</param>
        /// <returns>The number of valid bytes before encountering an invalid byte.</returns>
        [DebuggerStepThrough]
        public static int GetValidByteCount(ReadOnlySpan<byte> bytes)
        {
            int end = bytes.Length;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] < 0x20 || bytes[i] == 127)
                {
                    end = i;
                    break;
                }
            }
            return end;
        }

        /// <summary>
        /// Determines the number of valid bytes in the given span of bytes based on a custom condition predicate.
        /// </summary>
        /// <param name="bytes">The span of bytes to check.</param>
        /// <param name="invalidByte">The predicate that defines the condition for an invalid byte.</param>
        /// <returns>The number of valid bytes before encountering an invalid byte.</returns>
        [DebuggerStepThrough]
        public static int GetValidByteCount(ReadOnlySpan<byte> bytes, Predicate<byte> invalidByte)
        {
            int end = bytes.Length;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (invalidByte.Invoke(bytes[i]))
                {
                    end = i;
                    break;
                }
            }
            return end;
        }
        #endregion

        /// <summary>
        /// Converts the string to an array of bytes using the default encoding.
        /// </summary>
        /// <param name="String">The string to convert.</param>
        /// <returns>An array of bytes representing the string.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this string String)
            => DefaultEncoding.GetBytes(String);
    }
}
