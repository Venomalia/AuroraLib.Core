using AuroraLib.Core.Cryptography;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Extensions
{
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
        /// <param name="start">The starting index for the search. Default is 0.</param>
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

        /// <summary>
        /// Computes the hash code for a <see cref="ReadOnlySpan{T}"/> of elements using the <see cref="XXHash32"/> algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequences.</typeparam>
        /// <param name="span">The read-only span of elements to compute the hash code from.</param>
        /// <returns>The computed hash code.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCodeFast<T>(this ReadOnlySpan<T> span) where T : unmanaged
        {
            ReadOnlySpan<byte> buffer = MemoryMarshal.Cast<T, byte>(span);
            return (int)XXHash32.Generate(buffer);
        }

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

    }
}
