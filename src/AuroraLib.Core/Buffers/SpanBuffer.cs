using AuroraLib.Core.Extensions;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Buffers
{
    /// <summary>
    /// Represents a buffer of <typeparamref name="T"/> that allocated from ArrayPool of bytes and provides save access of its elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer.</typeparam>
    public readonly struct SpanBuffer<T> : IDisposable where T : unmanaged
    {
        private readonly byte[] _buffer;

        /// <summary>
        /// The number of items in the <see cref="SpanBuffer{T}"/>.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Gets the <see cref="Span{T}"/> representing of the <see cref="SpanBuffer{T}"/>.
        /// </summary>
        public unsafe Span<T> Span
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (byte* bp = _buffer)
                {
                    return new(bp, Length);
                }
            }
        }

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanBuffer{T}"/> class with the specified length.
        /// </summary>
        /// <param name="length">The length of the buffer.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanBuffer(int length)
        {
            Length = length;
            _buffer = length == 0 ? Array.Empty<byte>() : ArrayPool<byte>.Shared.Rent(length * Unsafe.SizeOf<T>());
        }

        /// <inheritdoc cref="SpanBuffer{T}.SpanBuffer(int)"/>
        public SpanBuffer(uint length) : this((int)length)
        { }

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
        public SpanBuffer(List<T> list) : this(list.UnsaveAsSpan())
        { }

        #endregion

        #region Span

        /// <summary>
        ///  Gets the element at the specified zero-based index
        /// </summary>
        /// <param name="index"> The zero-based index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        public ref T this[int index]
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Span[index];
        }

        /// <inheritdoc cref="Span{T}.Clear"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => Span.Clear();

        /// <inheritdoc cref="Span{T}.Clear"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<T> destination)
            => Span.CopyTo(destination);

        /// <inheritdoc cref="Span{T}.Fill"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(T value)
            => Span.Fill(value);

        /// <inheritdoc cref="Span{T}.GetEnumerator"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T>.Enumerator GetEnumerator()
            => Span.GetEnumerator();

        /// <inheritdoc cref="Span{T}.Slice(int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start)
            => Span[start..];

        /// <inheritdoc cref="Span{T}.Slice(int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start, int length)
            => Span.Slice(start, length);

        /// <inheritdoc cref="Span{T}.ToArray"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
            => Span.ToArray();
        #endregion

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (Length != 0) ArrayPool<byte>.Shared.Return(_buffer);
        }

        public static implicit operator Span<T>(SpanBuffer<T> x) => x.Span;

        public static implicit operator ReadOnlySpan<T>(SpanBuffer<T> x) => x.Span;

        public static implicit operator Span<byte>(SpanBuffer<T> x) => x.Bytes;

        public static implicit operator ReadOnlySpan<byte>(SpanBuffer<T> x) => x.Bytes;
    }
}
