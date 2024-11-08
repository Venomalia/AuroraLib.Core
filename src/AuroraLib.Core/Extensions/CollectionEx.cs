using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Extensions
{
    /// <summary>
    /// Provides extension methods and utilities for collections.
    /// </summary>
    public static class CollectionEx
    {
        /// <summary>
        /// Moves an item within the list from the specified old index to the new index.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to modify.</param>
        /// <param name="oldIndex">The old index of the item to move.</param>
        /// <param name="newIndex">The new index where the item should be moved.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// Counts the occurrences of a specific <paramref name="value"/> of <typeparamref name="T"/> in a <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span.</typeparam>
        /// <param name="list">The list to count the occurrences in.</param>
        /// <param name="value">The value to count.</param>
        /// <returns>The number of occurrences of the value in the span.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this List<T> list, T value) where T : IEquatable<T>
            => list.UnsafeAsSpan().Count(value);

        /// <summary>
        /// Gets a <see cref="Span{T}"/> view over the data in a list. Items should not be added or removed from the <see cref="List{T}"/> while the <see cref="Span{T}"/> is in use.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List.</typeparam>
        /// <param name="list">The List to convert to Span.</param>
        /// <returns>A Span representing the elements of the List.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> UnsafeAsSpan<T>(this List<T> list)
#if NET5_0_OR_GREATER
            => CollectionsMarshal.AsSpan(list);
#else
        {
            FieldInfo itemsField = typeof(List<T>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            T[] items = (T[])itemsField.GetValue(list);
            return items.AsSpan(0, list.Count);
        }
#endif

        /// <summary>
        /// Computes the hash code for the elements in the specified list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list. It must be an unmanaged type.</typeparam>
        /// <param name="list">The list containing the elements to compute the hash code for.</param>
        /// <returns>The computed hash code.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SequenceGetHashCode<T>(this List<T> list) where T : unmanaged
            => SpanEx.SequenceGetHashCode(list.UnsafeAsSpan());

        /// <summary>
        /// Computes the hash code for the elements in the specified collection./>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="list">The enumerable collection of elements to compute the hash code from.</param>
        /// <returns>The computed hash code.</returns>
        [DebuggerStepThrough]
        public static int SequenceGetHashCode<T>(this IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return 0;

#if NET20_OR_GREATER || NETSTANDARD2_0
            int hashCode = 17;
            foreach (var item in list)
            {
                hashCode = hashCode * 23 + (item == null ? 0 : item.GetHashCode());
            }
            return hashCode;
#else
            HashCode gen = default;
            foreach (T b in list)
            {
                gen.Add(b);
            }
            return gen.ToHashCode();
#endif
        }
    }
}
