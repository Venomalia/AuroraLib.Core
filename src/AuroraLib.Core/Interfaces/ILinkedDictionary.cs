using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AuroraLib.Core.Interfaces
{
    /// <summary>
    /// Represents a dictionary where values can be automatically linked to their corresponding keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public interface ILinkedDictionary<TKey, TValue> : ICollection<TValue>, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<TValue>
    {
        /// <summary>
        /// Attempts to add an item to the dictionary if it does not already exist.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns><c>True</c> if the item was added, <c>false</c> if the item already exists in the dictionary.</returns>
        bool TryAdd(TValue item);

        /// <summary>
        /// Removes the item with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns><c>True</c> if the item was removed successfully; otherwise, <c>false</c>.</returns>
        bool Remove(TKey key);

        /// <summary>
        /// Removes the item with the specified key from the dictionary and returns the removed item.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <param name="value">The value of the item that was removed, if it exists.</param>
        /// <returns><c>True</c> if the item was removed successfully; otherwise, <c>false</c>.</returns>
#if NET6_0_OR_GREATER
        bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value);
#else
        bool Remove(TKey key, out TValue value);
#endif
    }
}
