using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraLib.Core.Extensions
{
    public static class GenericEx
    {
        /// <summary>
        /// Returns a value of <typeparamref name="T"/> clamped between the specified minimum and maximum values.
        /// </summary>
        /// <typeparam name="T">The type of value to clamp.</typeparam>
        /// <param name="val">The value to clamp.</param>
        /// <param name="min">The minimum value to clamp to.</param>
        /// <param name="max">The maximum value to clamp to.</param>
        /// <returns>The clamped value.</returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// Returns the maximum of two values of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of values to compare.</typeparam>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>The maximum value.</returns>
        public static T Max<T>(this T val1, T val2) where T : IComparable<T>
            => val1.CompareTo(val2) >= 0 ? val1 : val2;

        /// <summary>
        /// Returns the minimum of two values of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of values to compare.</typeparam>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>The minimum value.</returns>
        public static T Min<T>(this T val1, T val2) where T : IComparable<T>
            => val1.CompareTo(val2) <= 0 ? val1 : val2;
    }
}
