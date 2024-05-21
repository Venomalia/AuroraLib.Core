using System;
using System.Diagnostics;
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
        /// Determines whether the given sequence is contained within the larger sequence of values.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequences.</typeparam>
        /// <param name="values">The sequence of values to search within.</param>
        /// <param name="sequence">The sequence to search for.</param>
        /// <returns>true if the sequence is found within the larger sequence; otherwise, false.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceContains<T>(this ReadOnlySpan<T> values, ReadOnlySpan<T> sequence) where T : IEquatable<T>
            => SequenceSearch(values, sequence) != -1;

        /// <summary>
        /// Searches for a <paramref name="sequence"/> within an <see cref="ReadOnlySpan{T}"/> and returns the index of the first occurrence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequences.</typeparam>
        /// <param name="values">The array to search in.</param>
        /// <param name="sequence">The sequence to search for.</param>
        /// <returns>The index of the first occurrence of the sequence, or -1 if not found.</returns>
        [DebuggerStepThrough]
        public static int SequenceSearch<T>(this ReadOnlySpan<T> values, ReadOnlySpan<T> sequence) where T : IEquatable<T>
        {
            if (!sequence.IsEmpty || values.Length < sequence.Length)
                return -1;

            for (int i = 0; i <= values.Length; i++)
            {
                int p;
                for (p = 0; p < sequence.Length; p++)
                {
                    if (!sequence[p].Equals(values[i + p]))
                        break;
                }

                if (p == sequence.Length)
                    return i - sequence.Length;
            }

            return -1;
        }

        /// <summary>
        /// Determines the maximum matching length between two ReadOnlySpans.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequences.</typeparam>
        /// <param name="sequence1">The first ReadOnlySpan to compare.</param>
        /// <param name="sequence2">The second ReadOnlySpan to compare.</param>
        /// <returns>The length of the maximum matching prefix between the two ReadOnlySpans.</returns>
        public static int MaxMatch<T>(this ReadOnlySpan<T> sequence1, ReadOnlySpan<T> sequence2) where T : IEquatable<T>
        {
            int minLength = Math.Min(sequence1.Length, sequence2.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (!sequence1[i].Equals(sequence2[i]))
                {
                    return i;
                }
            }

            return minLength;
        }

        /// <inheritdoc cref="MaxMatch{T}(ReadOnlySpan{T},ReadOnlySpan{T})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxMatch<T>(this Span<T> span, ReadOnlySpan<T> sequence2) where T : IEquatable<T>
            => MaxMatch((ReadOnlySpan<T>)span, sequence2);

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
#if NET20_OR_GREATER
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

#if NET5_0_OR_GREATER
        /// <summary>
        /// Casts an array of one primitive type <typeparamref name="TFrom"/> to a span of another primitive type <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type of elements in the source array.</typeparam>
        /// <typeparam name="TTo">The type of elements in the resulting span.</typeparam>
        /// <param name="buffer">The source array to convert.</param>
        /// <returns>A span containing the elements converted from the source array.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<TTo> ToSpan<TFrom, TTo>(this TFrom[] buffer) where TFrom : unmanaged where TTo : unmanaged
        {
            ref TTo toRef = ref Unsafe.As<TFrom, TTo>(ref MemoryMarshal.GetArrayDataReference(buffer));
            return MemoryMarshal.CreateSpan(ref toRef, buffer.Length / Unsafe.SizeOf<TFrom>() * Unsafe.SizeOf<TTo>());
        }
#endif
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
    }
}
