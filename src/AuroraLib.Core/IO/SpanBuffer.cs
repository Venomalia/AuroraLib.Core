using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Represents a buffer of <typeparamref name="T"/> that allocated from an <see cref="ArrayPool{T}"/> and provides save access of its elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer.</typeparam>
    public readonly struct SpanBuffer<T> : IDisposable
    {
        private readonly T[] _buffer;
        private readonly int _length;

        /// <summary>
        /// Gets the <see cref="Span{T}"/> representing the buffer's elements.
        /// </summary>
        public Span<T> Span => _buffer.AsSpan(0, _length);

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanBuffer{T}"/> class with the specified length.
        /// </summary>
        /// <param name="length">The length of the buffer.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanBuffer(int length)
        {
            _length = length;
            _buffer = ArrayPool<T>.Shared.Rent(length);
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
            => ArrayPool<T>.Shared.Return(_buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Span<T>(SpanBuffer<T> x) => x.Span;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlySpan<T>(SpanBuffer<T> x) => x.Span;
    }
}
