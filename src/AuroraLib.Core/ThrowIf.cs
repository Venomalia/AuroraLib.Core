using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core
{
    /// <summary>
    /// Provides methods to simplifying common argument validation patterns.
    /// </summary>
    public static class ThrowIf
    {
#if NET6_0_OR_GREATER
        /// <inheritdoc cref="ArgumentNullException.ThrowIfNull(object?, string?)"/>
        public static void Null([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            => ArgumentNullException.ThrowIfNull(argument, paramName);
#else
        public static void Null(object? argument, string? paramName = null)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName);
        }
#endif

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if the provided value is not a valid enum value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to check.</typeparam>
        /// <param name="value">The enum value to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
#if NET6_0_OR_GREATER
        public static void InvalidEnum<TEnum>(TEnum value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where TEnum : struct, Enum
#else
        public static void InvalidEnum<TEnum>(TEnum value, string? paramName = null) where TEnum : struct, Enum
#endif
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new ArgumentOutOfRangeException(paramName, value, InvalidEnumMessage(value, paramName));
        }

        internal static string InvalidEnumMessage<TEnum>(TEnum value, string? paramName = null)
            => $"{paramName} contains an invalid value: {value}";

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if the specified value is outside the defined range.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum bound.</param>
        /// <param name="max">The maximum bound.</param>
        /// <param name="paramName">The name of the parameter.</param>
#if NET6_0_OR_GREATER
        public static void OutOfRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
#else
public static void OutOfRange<T>(T value, T min, T max, string? paramName = null) where T : IComparable<T>
#endif
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be between {min} and {max}.");
            }
        }

        /// <summary>
        /// Validates that the provided character span is not empty, or consists only of whitespace characters.
        /// </summary>
        /// <param name="argument">The character span to validate.</param>
        /// <param name="paramName">The name of the parameter to include in the exception message if validation fails.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided character span is empty or contains only whitespace.
        /// </exception>
#if NET6_0_OR_GREATER
        public static void EmptyOrWhiteSpace(ReadOnlySpan<char> argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
#else
        public static void EmptyOrWhiteSpace(ReadOnlySpan<char> argument, string? paramName = null)
#endif
        {
            Zero(argument.Length, paramName);
            if (argument.IsWhiteSpace())
                throw new ArgumentException("Only consists of whitespace character.", paramName);
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the specified parameter is read-only.
        /// </summary>
        /// <param name="isReadOnly">A boolean value indicating whether the parameter is read-only.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <exception cref="InvalidOperationException"> </exception>
        public static void ReadOnly(bool isReadOnly, string paramName)
        {
            if (isReadOnly)
                throw new InvalidOperationException($"{paramName} is read-only and cannot be modified.");
        }

#if NET8_0_OR_GREATER

        /// <inheritdoc cref="ArgumentException.ThrowIfNullOrEmpty(string?, string?)"/>
        public static void NullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            => ArgumentException.ThrowIfNullOrEmpty(argument, paramName);

        /// <inheritdoc cref="ArgumentException.ThrowIfNullOrWhiteSpace(string?, string?)"/>
        public static void NullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            => ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfLessThan{T}(T, T, string?)"/>
        public static void LessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
            => ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfGreaterThan{T}(T, T, string?)"/>
        public static void GreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
            => ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfEqual{T}(T, T, string?)"/>
        public static void Equal<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
            => ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfNotEqual{T}(T, T, string?)"/>
        public static void NotEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
            => ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfLessThanOrEqual{T}(T, T, string?)"/>
        public static void LessThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
            => ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual{T}(T, T, string?)"/>
        public static void GreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
            => ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfZero{T}(T, string?)"/>
        public static void Zero<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : INumberBase<T>
            => ArgumentOutOfRangeException.ThrowIfZero(value, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfNegative{T}(T, string?)"/>
        public static void Negative<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : INumberBase<T>
            => ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);

        /// <inheritdoc cref="ArgumentOutOfRangeException.ThrowIfNegativeOrZero{T}(T, string?)"/>
        public static void NegativeOrZero<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : INumberBase<T>
            => ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);

        /// <inheritdoc cref="ObjectDisposedException.ThrowIf(bool, object)"/>
        public static void Disposed([DoesNotReturnIf(true)] bool condition, object instance)
            => ObjectDisposedException.ThrowIf(condition, instance);
#else

#if NET6_0_OR_GREATER
        public static void NullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
#else
        public static void NullOrEmpty(string? argument, string? paramName = null)
#endif
        {
            if (string.IsNullOrEmpty(argument))
            {
                Null(argument, paramName);
                ThrowHelper.ThrowIsNullOrEmpty(paramName);
                return;
            }
        }

#if NET6_0_OR_GREATER
        public static void NullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
#else
        public static void NullOrWhiteSpace(string? argument, string? paramName = null)
#endif
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                Null(argument, paramName);
                ThrowHelper.ThrowNullOrWhiteSpace(paramName);
            }
        }

#if NET6_0_OR_GREATER
        public static void LessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
#else
        public static void LessThan<T>(T value, T other, string? paramName = null) where T : IComparable<T>
#endif
        {
            if (value.CompareTo(other) < 0)
                ThrowHelper.ThrowLessThan(value, other, paramName);
        }

#if NET6_0_OR_GREATER
        public static void GreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
#else
        public static void GreaterThan<T>(T value, T other, string? paramName = null) where T : IComparable<T>
#endif
        {
            if (value.CompareTo(other) > 0)
                ThrowHelper.ThrowGreaterThan(value, other, paramName);
        }

#if NET6_0_OR_GREATER
        public static void Equal<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
#else
        public static void Equal<T>(T value, T other, string? paramName = null) where T : IEquatable<T>?
#endif
        {
            if (value?.Equals(other) ?? other is null)
                ThrowHelper.ThrowEqual(value, other, paramName);
        }



#if NET6_0_OR_GREATER
        public static void NotEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
#else
        public static void NotEqual<T>(T value, T other, string? paramName = null) where T : IEquatable<T>?
#endif
        {
            if (!(value?.Equals(other) ?? other is null))
                ThrowHelper.ThrowNotEqual(value, other, paramName);
        }

#if NET6_0_OR_GREATER
        public static void LessThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
#else
        public static void LessThanOrEqual<T>(T value, T other, string? paramName = null) where T : IComparable<T>
#endif
        {
            if (value.CompareTo(other) <= 0)
            {
                LessThan(value, other, paramName);
                ThrowHelper.ThrowEqual(value, other, paramName);
            }
        }

#if NET6_0_OR_GREATER
        public static void GreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
#else
        public static void GreaterThanOrEqual<T>(T value, T other, string? paramName = null) where T : IComparable<T>
#endif
        {
            if (value.CompareTo(other) >= 0)
            {
                GreaterThan(value, other, paramName);
                ThrowHelper.ThrowEqual(value, other, paramName);
            }
        }


#if NET6_0_OR_GREATER
        public static void Zero<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : struct, IEquatable<T>?
#else
        public static void Zero<T>(T value, string? paramName = null) where T : struct, IEquatable<T>?
#endif
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
                ThrowHelper.ThrowZero(value, paramName);
        }


#if NET6_0_OR_GREATER
        public static void Negative<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : struct, IComparable<T>
#else
        public static void Negative<T>(T value, string? paramName = null) where T : struct, IComparable<T>
#endif
        {
            if (value.CompareTo(default) < 0)
            {
                ThrowHelper.ThrowNegative(value, paramName);
            }
        }

#if NET6_0_OR_GREATER
        public static void NegativeOrZero<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : struct, IComparable<T>
#else
        public static void NegativeOrZero<T>(T value, string? paramName = null) where T : struct, IComparable<T>
#endif
        {
            if (value.CompareTo(default) <= 0)
            {
                Negative(value, paramName);
                ThrowHelper.ThrowZero(value, paramName);
            }
        }

#if NET6_0_OR_GREATER
        public static void Disposed([DoesNotReturnIf(true)] bool condition, object instance)
#else
        public static void Disposed(bool condition, object instance)
#endif
        {
            if (condition)
                ThrowHelper.ThrowDisposed(instance);
        }

        private static class ThrowHelper
        {

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowIsNullOrEmpty(string? paramName)
                => throw new ArgumentException("String is Empty.", paramName);

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowNullOrWhiteSpace(string? paramName)
                => throw new ArgumentException("String is Empty or consists only of whitespace character.", paramName);

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowLessThan<T>(T value, T other, string? paramName)
                => throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be greater or equal to {other}.");

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowGreaterThan<T>(T value, T other, string? paramName)
                => throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be less than or equal to {other}.");

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowEqual<T>(T value, T other, string? paramName = null)
                => throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} cannot be equal to {other}.");

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowNotEqual<T>(T value, T other, string? paramName)
                => throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be equal to {other}.");

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowZero<T>(T value, string? paramName = null)
                => throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} cannot be zero.");

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowNegative<T>(T value, string? paramName)
                => throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} cannot be negative.");

#if NET5_0_OR_GREATER
            [DoesNotReturn]
#endif
            public static void ThrowDisposed(object instance)
                => throw new ObjectDisposedException(instance?.GetType().FullName ?? "Unknown object");
        }
#endif
    }
}
