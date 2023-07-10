using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Extensions
{
    public static class ListEx
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
        public static void Move<T>(this IList<T> list, int OldIndex, int NewIndex)
        {
            T item = list[OldIndex];
            list.RemoveAt(OldIndex);
            list.Insert(NewIndex, item);
        }

        /// <summary>
        /// Determines whether two <see cref="IEnumerable"/> collections are equal by comparing their elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="left">The first enumerable collection to compare.</param>
        /// <param name="right">The second enumerable collection to compare.</param>
        /// <returns>True if the collections are equal; otherwise, false.</returns>
        [DebuggerStepThrough]
        public static bool Equals<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            using (var leftE = left.GetEnumerator())
            using (var rightE = right.GetEnumerator())
            {
                while (leftE.MoveNext() && rightE.MoveNext())
                {
                    if (!leftE.Current.Equals(rightE.Current))
                        return false;
                }

                return !leftE.MoveNext() && !rightE.MoveNext();
            }
        }

        /// <summary>
        /// Computes the hash code for an <see cref="IEnumerable{T}"/> collection of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="list">The enumerable collection of elements to compute the hash code from.</param>
        /// <param name="starting">The starting value for the hash code computation.</param>
        /// <param name="additive">The additive value used in the hash code computation.</param>
        /// <returns>The computed hash code.</returns>
        [DebuggerStepThrough]
        public static int GetHashCode<T>(this IEnumerable<T> list, int starting = 0, int additive = 1)
        {
            int hashcode = starting;

            foreach (var item in list)
            {
                hashcode = hashcode * additive + item.GetHashCode();
            }

            return hashcode;
        }
    }
}
