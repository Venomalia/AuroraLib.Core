using AuroraLib.Core.Extensions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        public Span<T> Span
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if NET5_0_OR_GREATER
                ref T tRef = ref Unsafe.As<byte, T>(ref MemoryMarshal.GetArrayDataReference(_buffer));
                return MemoryMarshal.CreateSpan(ref tRef, Length);
#else
                unsafe
                {
                    fixed (byte* bp = _buffer)
                    {
                        return new Span<T>(bp, Length);
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Gets a Span of bytes representing this <see cref="SpanBuffer{T}"/>.
        /// </summary>
        public unsafe Span<byte> Bytes
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if NET5_0_OR_GREATER
                ref byte bRef = ref MemoryMarshal.GetArrayDataReference(_buffer);
                return MemoryMarshal.CreateSpan(ref bRef, Length * sizeof(T));
#else
                unsafe
                {
                    fixed (byte* bp = _buffer)
                    {
                        return new Span<byte>(bp, Length * sizeof(T));
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Retrieves the underlying buffer as a byte array.
        /// </summary>
        /// <returns>The underlying buffer.</returns>
        [DebuggerStepThrough]
        public byte[] GetBuffer() => _buffer;

        /// <summary>
        /// Retrieves the underlying buffer as a byte memory.
        /// </summary>
        /// <returns>The underlying buffer.</returns>
        [DebuggerStepThrough]
        public Memory<byte> AsMemory()
            => _buffer.AsMemory(0, Length);

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanBuffer{T}"/> class with the specified length.
        /// </summary>
        /// <param name="length">The length of the buffer.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe SpanBuffer(int length)
        {
            Length = length;
            _buffer = length == 0 ? Array.Empty<byte>() : ArrayPool<byte>.Shared.Rent(length * sizeof(T));
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
        public SpanBuffer(List<T> list) : this(list.UnsafeAsSpan())
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
            => Span.Slice(start);

        /// <inheritdoc cref="Span{T}.Slice(int,int)"/>
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
    }
}
