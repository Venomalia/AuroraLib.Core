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
        /// <param name="values">The enumerable collection of elements to compute the hash code from.</param>
        /// <returns>The computed hash code.</returns>
        [DebuggerStepThrough]
        public static int SequenceGetHashCode<T>(this IEnumerable<T> values)
        {
            ThrowIf.Null(values);
            if (!values.Any())
                return 0;
            
#if NET20_OR_GREATER || NETSTANDARD2_0
            int hashCode = 17;
            foreach (var item in values)
            {
                hashCode = hashCode * 23 + (item == null ? 0 : item.GetHashCode());
            }
            return hashCode;
#else
            HashCode gen = default;
            foreach (T b in values)
            {
                gen.Add(b);
            }
            return gen.ToHashCode();
#endif
        }

#if !NET5_0_OR_GREATER

        /// <summary>
        /// Attempts to add the specified <paramref name="key"/> and <paramref name="value"/> to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to which the key-value pair will be added.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns><c>true</c> if the key-value pair was added successfully; otherwise, <c>false</c> if the key already exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dictionary"/> is <c>null</c>.</exception>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
        {
            ThrowIf.Null(dictionary, nameof(dictionary));

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to remove a value with the specified key from the given dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary from which to remove the value.</param>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value associated with the specified key if found; otherwise, the default value.</param>
        /// <returns><c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dictionary"/> is <c>null</c>.</exception>
        public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value) where TKey : notnull
        {
            ThrowIf.Null(dictionary);

            if (dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }

            value = default!;
            return false;
        }

#endif

        /// <summary>
        /// Adds a range of elements from the specified sequence to the <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection to which elements will be added.</param>
        /// <param name="span">A read-only span containing the elements to add to the collection.</param>
        public static void AddRange<T>(this ICollection<T> collection, ReadOnlySpan<T> span)
        {
            if (collection is List<T> list)
                list.Capacity = list.Count + span.Length;

            foreach (T value in span)
                collection.Add(value);
        }

        /// <inheritdoc cref="AddRange{T}(ICollection{T}, ReadOnlySpan{T})"/>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            if (collection is List<T> inlist)
            {
                inlist.AddRange(collection);
            }
            else if (enumerable is T[] array)
            {
                collection.AddRange(array.AsSpan());
            }
            else if (enumerable is List<T> list)
            {
                collection.AddRange(list.UnsafeAsSpan());
            }
            else
            {
                foreach (T pair in enumerable)
                    collection.Add(pair);
            }
        }

        /// <summary>
        /// Adds a range of key-value pairs to the <paramref name="dictionary"/>, ensuring that only entries with unique keys are added.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary. Must be non-nullable.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to which the key-value pairs are added.</param>
        /// <param name="span">A read-only span of key-value pairs to add to the dictionary.</param>
        public static void AddUniqueRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, ReadOnlySpan<KeyValuePair<TKey, TValue>> span) where TKey : notnull
        {
            foreach (KeyValuePair<TKey, TValue> pair in span)
            {
                if (!dictionary.ContainsKey(pair.Key))
                    dictionary.Add(pair);
            }
        }

        /// <inheritdoc cref="AddUniqueRange{TKey, TValue}(IDictionary{TKey, TValue}, ReadOnlySpan{KeyValuePair{TKey, TValue}})"/>
        public static void AddUniqueRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> enumerable) where TKey : notnull
        {
            if (enumerable is KeyValuePair<TKey, TValue>[] array)
            {
                AddUniqueRange(dictionary, array.AsSpan());
            }
            else if (enumerable is List<KeyValuePair<TKey, TValue>> list)
            {
                AddUniqueRange(dictionary, list.UnsafeAsSpan());
            }
            else
            {
                foreach (KeyValuePair<TKey, TValue> pair in enumerable)
                {
                    if (!dictionary.ContainsKey(pair.Key))
                        dictionary.Add(pair);
                }
            }
        }
    }
}
