using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Extensions
{
    /// <summary>
    /// Provides extension methods and utilities for spans.
    /// </summary>
    public static class SpanEx
    {
        /// <summary>
        /// Determines if the given span contains the specified sequence of values.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span, which must implement <see cref="IEquatable{T}"/>.</typeparam>
        /// <param name="values">The span to search in.</param>
        /// <param name="sequence">The sequence of values to check for in the span.</param>
        /// <returns><c>true</c> if the span contains the specified sequence; otherwise, <c>false</c>.</returns>
        [DebuggerStepThrough]
        public static bool Contains<T>(this ReadOnlySpan<T> values, ReadOnlySpan<T> sequence) where T : IEquatable<T>
            => MemoryExtensions.IndexOf(values, sequence) != -1;

        /// <inheritdoc cref="Contains{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
        [DebuggerStepThrough]
        public static bool Contains<T>(this Span<T> values, ReadOnlySpan<T> sequence) where T : IEquatable<T>
            => MemoryExtensions.IndexOf(values, sequence) != -1;

#if !NET6_0_OR_GREATER
        [DebuggerStepThrough]
        public static bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
            => MemoryExtensions.IndexOf(span, value) != -1;

        [DebuggerStepThrough]
        public static bool Contains<T>(this Span<T> span, T value) where T : IEquatable<T>?
            => MemoryExtensions.IndexOf(span, value) != -1;
#endif

#if !NET8_0_OR_GREATER

        /// <summary>
        /// Determines if the given span contains any of the specified values.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span, which must implement <see cref="IEquatable{T}"/>.</typeparam>
        /// <param name="span">The span to search in.</param>
        /// <param name="value0">The first value to check for in the span.</param>
        /// <param name="value1">The second value to check for in the span.</param>
        /// <param name="value2">The third value to check for in the span.</param>
        /// <returns><c>true</c> if the span contains any of the specified values; otherwise, <c>false</c>.</returns>
        [DebuggerStepThrough]
        public static bool ContainsAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T>
            => MemoryExtensions.IndexOfAny(span, value0, value1, value2) != -1;

        /// <inheritdoc cref="ContainsAny{T}(ReadOnlySpan{T}, T, T, T)"/>
        [DebuggerStepThrough]
        public static bool ContainsAny<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T>
            => MemoryExtensions.IndexOfAny(span, value0, value1, value2) != -1;

        /// <inheritdoc cref="ContainsAny{T}(ReadOnlySpan{T}, T, T, T)"/>
        [DebuggerStepThrough]
        public static bool ContainsAny<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>
            => MemoryExtensions.IndexOfAny(span, value0, value1) != -1;

        /// <inheritdoc cref="ContainsAny{T}(ReadOnlySpan{T}, T, T, T)"/>
        [DebuggerStepThrough]
        public static bool ContainsAny<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T>
            => MemoryExtensions.IndexOfAny(span, value0, value1) != -1;

        /// <summary>
        /// Determines if the given span contains any of the values from the specified sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span, which must implement <see cref="IEquatable{T}"/>.</typeparam>
        /// <param name="span">The span to search in.</param>
        /// <param name="values">The sequence of values to check for in the span.</param>
        /// <returns><c>true</c> if the span contains any of the values from the specified sequence; otherwise, <c>false</c>.</returns>
        [DebuggerStepThrough]
        public static bool ContainsAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>
            => MemoryExtensions.IndexOfAny(span, values) != -1;

        /// <inheritdoc cref="ContainsAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
        [DebuggerStepThrough]
        public static bool ContainsAny<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>
            => MemoryExtensions.IndexOfAny(span, values) != -1;

        /// <summary>
        /// Replaces all occurrences of a specified value in a span with a new value.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span.</typeparam>
        /// <param name="span">The span that contains the elements to modify.</param>
        /// <param name="oldValue">The value to search for in the span.</param>
        /// <param name="newValue">The value to replace the <paramref name="oldValue"/> with.</param>
        [DebuggerStepThrough]
        public static void Replace<T>(this Span<T> span, T oldValue, T newValue) where T : IEquatable<T>
        {
            if (oldValue.Equals(newValue))
                return;

            for (int i = 0; i < span.Length; i++)
            {
                if (span[i].Equals(oldValue))
                {
                    span[i] = newValue;
                }
            }
        }

        /// <summary>
        /// Counts the occurrences of a specific <paramref name="value"/> of <typeparamref name="T"/> in a <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequences.</typeparam>
        /// <param name="span">The read-only span to count the occurrences in.</param>
        /// <param name="value">The value to count.</param>
        /// <returns>The number of occurrences of the value in the span.</returns>
        [DebuggerStepThrough]
        public static int Count<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>
        {
            int count = 0;
            foreach (var item in span)
            {
                if (value.Equals(item))
                {
                    count++;
                }
            }

            return count;
        }

        /// <inheritdoc cref="Count{T}(ReadOnlySpan{T}, T)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this Span<T> span, T value) where T : IEquatable<T>
            => Count((ReadOnlySpan<T>)span, value);
#endif

        /// <summary>
        /// Finds the index of the first element in the specified span that matches the condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span.</typeparam>
        /// <param name="span">The read-only span to search.</param>
        /// <param name="condition">The delegate that defines the condition to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that satisfies the condition, if found; otherwise, -1.</returns>
        [DebuggerStepThrough]
        public static int IndexOf<T>(this ReadOnlySpan<T> span, Predicate<T> condition)
        {
            for (int i = 0; i < span.Length; i++)
            {
                if (condition(span[i]))
                    return i;
            }
            return -1;
        }

        /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, Predicate{T})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this Span<T> span, Predicate<T> condition) where T : unmanaged
            => IndexOf((ReadOnlySpan<T>)span, condition);

        /// <summary>
        /// Finds the index of the last element in the specified span that matches the condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span.</typeparam>
        /// <param name="span">The read-only span to search.</param>
        /// <param name="condition">The delegate that defines the condition to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that satisfies the condition, if found; otherwise, -1.</returns>
        [DebuggerStepThrough]
        public static int LastIndexOf<T>(this ReadOnlySpan<T> span, Predicate<T> condition)
        {
            for (int i = span.Length - 1; i >= 0; i--)
            {
                if (condition(span[i]))
                    return i;
            }
            return -1;
        }

        /// <inheritdoc cref="LastIndexOf{T}(ReadOnlySpan{T}, Predicate{T})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOf<T>(this Span<T> span, Predicate<T> condition) where T : unmanaged
            => LastIndexOf((ReadOnlySpan<T>)span, condition);

        /// <summary>
        /// Computes the hash code for the elements in the specified span.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequences.</typeparam>
        /// <param name="span">The read-only span of elements to compute the hash code from.</param>
        /// <returns>The computed hash code.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SequenceGetHashCode<T>(this ReadOnlySpan<T> span) where T : unmanaged
        {
#if NET20_OR_GREATER || NETSTANDARD2_0
            unchecked
            {
                int hash = 17;
                foreach (T item in span)
                {
                    hash = hash * 31 + item.GetHashCode();
                }
                return hash;
            }
#else

#if !NETSTANDARD
            // If char use string implementation
            if (typeof(T) == typeof(char))
            {
                ReadOnlySpan<char> chars = MemoryMarshal.Cast<T, char>(span);
                return String.GetHashCode(chars);
            }
#endif
            HashCode gen = default;
#if NET6_0_OR_GREATER
            ReadOnlySpan<byte> buffer = MemoryMarshal.Cast<T, byte>(span);
            gen.AddBytes(buffer);
#else
            foreach (T b in span)
            {
                gen.Add(b);
            }
#endif
            return gen.ToHashCode();
#endif
        }

        /// <inheritdoc cref="SequenceGetHashCode{T}(ReadOnlySpan{T})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SequenceGetHashCode<T>(this Span<T> span) where T : unmanaged
            => SequenceGetHashCode((ReadOnlySpan<T>)span);

        /// <inheritdoc cref="SequenceGetHashCode{T}(ReadOnlySpan{T})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SequenceGetHashCode<T>(this T[] array) where T : unmanaged
            => SequenceGetHashCode(array.AsSpan());

        /// <summary>
        /// Casts an array of one primitive type <typeparamref name="TFrom"/> to a span of another primitive type <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type of elements in the source array.</typeparam>
        /// <typeparam name="TTo">The type of elements in the resulting span.</typeparam>
        /// <param name="buffer">The source array to convert.</param>
        /// <returns>A span containing the elements converted from the source array.</returns>
        [DebuggerStepThrough]
        public unsafe static Span<TTo> ToSpan<TFrom, TTo>(this TFrom[] buffer) where TFrom : unmanaged where TTo : unmanaged
        {
            int fromSize = sizeof(TFrom);
            int toSize = sizeof(TTo);

            if (fromSize == 0 || toSize == 0)
                throw new ArgumentException($"Size of {nameof(TFrom)} and {nameof(TTo)} must be non-zero.");

            int toLength = buffer.Length * fromSize / toSize;
            if (toLength * toSize != buffer.Length * fromSize)
                throw new ArgumentException($"Buffer length is not a multiple of the target type size of {nameof(TTo)}.");

#if NET5_0_OR_GREATER
            ref TTo toRef = ref Unsafe.As<TFrom, TTo>(ref MemoryMarshal.GetArrayDataReference(buffer));
            return MemoryMarshal.CreateSpan(ref toRef, toLength);
#else
            fixed (TFrom* ptr = buffer)
            {
                return new Span<TTo>(ptr, toLength);
            }
#endif
        }
        /// <summary>
        /// Converts an instance of an unmanaged type to a span of bytes.
        /// </summary>
        /// <typeparam name="T">The type of the buffer.</typeparam>
        /// <param name="buffer">The buffer to convert to bytes.</param>
        /// <returns>A span of bytes representing the same memory as the original buffer.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if !(NETSTANDARD || NET20_OR_GREATER)
        public static Span<byte> AsBytes<T>(this ref T buffer) where T : unmanaged
        {
            ref byte bRef = ref Unsafe.As<T, byte>(ref buffer);
            return MemoryMarshal.CreateSpan(ref bRef, Unsafe.SizeOf<T>());
        }
#else
        public unsafe static Span<byte> AsBytes<T>(this ref T buffer) where T : unmanaged
        {
            fixed (T* bytePtr = &buffer)
            {
                return new Span<byte>(bytePtr, sizeof(T));
            }
        }
#endif

        /// <summary>
        /// Converts a ReadOnlySpan of bytes to an <see cref="UnmanagedMemoryStream"/>.
        /// </summary>
        /// <param name="data">The ReadOnlySpan of bytes to be converted.</param>
        /// <returns>An <see cref="UnmanagedMemoryStream"/> that allows read-only access to the data.</returns>
        public unsafe static UnmanagedMemoryStream AsReadOnlyStream(this ReadOnlySpan<byte> data)
        {
            fixed (byte* ptr = data)
            {
                return new UnmanagedMemoryStream(ptr, data.Length);
            }
        }

#if NET5_0_OR_GREATER
        /// <inheritdoc cref="string.Concat(ReadOnlySpan{char}, ReadOnlySpan{char})"/>
        public static string StringConcat(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2)
            => string.Concat(path1, path2);

        /// <inheritdoc cref="string.Concat(ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char})"/>
        public static string StringConcat(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3)
            => string.Concat(path1, path2, path3);

        /// <inheritdoc cref="string.Concat(ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char})"/>
        public static string StringConcat(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3, ReadOnlySpan<char> path4)
            => string.Concat(path1, path2, path3, path4);
#else

        public static string StringConcat(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2)
        {
            int capacity = path1.Length + path2.Length;
            string str = CreateStringWithCapacity(capacity, out Span<char> strSpan);

            path1.CopyTo(strSpan);
            strSpan = strSpan.Slice(path1.Length);
            path2.CopyTo(strSpan);
            return str;
        }

        public static string StringConcat(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3)
        {
            int capacity = path1.Length + path2.Length + path3.Length;
            string str = CreateStringWithCapacity(capacity, out Span<char> strSpan);

            path1.CopyTo(strSpan);
            strSpan = strSpan.Slice(path1.Length);
            path2.CopyTo(strSpan);
            strSpan = strSpan.Slice(path2.Length);
            path3.CopyTo(strSpan);
            return str;
        }

        public static string StringConcat(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3, ReadOnlySpan<char> path4)
        {
            int capacity = path1.Length + path2.Length + path3.Length + path4.Length;
            string str = CreateStringWithCapacity(capacity, out Span<char> strSpan);

            path1.CopyTo(strSpan);
            strSpan = strSpan.Slice(path1.Length);
            path2.CopyTo(strSpan);
            strSpan = strSpan.Slice(path2.Length);
            path3.CopyTo(strSpan);
            strSpan = strSpan.Slice(path3.Length);
            path4.CopyTo(strSpan);
            return str;
        }

        private static string CreateStringWithCapacity(int capacity, out Span<char> chars)
        {
            string str = new string('\0', capacity);
            chars = MemoryMarshal.AsMemory(str.AsMemory()).Span;
            return str;
        }
#endif

#if !NET5_0_OR_GREATER
        /// <see cref="string.Contains(string)"/>
        public static bool Contains(this string @this, char value) => @this.IndexOf(value) >= 0;
#endif
    }
}
