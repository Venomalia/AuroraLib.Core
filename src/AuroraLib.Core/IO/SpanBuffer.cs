using AuroraLib.Core.Extensions;
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

        /// <summary>
        /// The number of items in the <see cref="SpanBuffer{T}"/>.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Gets the <see cref="Span{T}"/> representing the buffer's elements.
        /// </summary>
        public Span<T> Span => _buffer.AsSpan(0, Length);

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanBuffer{T}"/> class with the specified length.
        /// </summary>
        /// <param name="length">The length of the buffer.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanBuffer(int length)
        {
            Length = length;
            _buffer = ArrayPool<T>.Shared.Rent(length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanBuffer{T}"/> class with a copy of the specified <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        /// <param name="span">The ReadOnlySpan from which to initialize the buffer's data.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanBuffer(ReadOnlySpan<T> span) : this(span.Length)
            => span.CopyTo(Span);

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanBuffer{T}"/> class with a copy of the specified <see cref="List{T}"/>.
        /// </summary>
        /// <param name="list">The List from which to initialize the buffer's data.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanBuffer(List<T> span) : this(span.UnsaveAsSpan())
        { }

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
