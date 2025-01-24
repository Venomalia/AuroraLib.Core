using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Buffers
{
    /// <summary>
    /// Represents a buffer of <typeparamref name="T"/> that allocated from ArrayPool of bytes and provides save access of its elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer.</typeparam>
    public readonly struct SpanBuffer<T> : IMemoryOwner<T> where T : unmanaged
    {
        private const int SmallArrayBoundary = 512;

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

        /// <inheritdoc/>
        public Memory<T> Memory => new MemoryCastManager<byte, T>(_buffer.AsMemory(0, Length * Unsafe.SizeOf<T>())).Memory;

        /// <summary>
        /// Retrieves the underlying buffer as a byte array.
        /// </summary>
        /// <returns>The underlying buffer.</returns>
        [DebuggerStepThrough]
        public byte[] GetBuffer() => _buffer;

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanBuffer{T}"/> class with the specified length.
        /// </summary>
        /// <param name="length">The length of the buffer.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe SpanBuffer(int length)
        {
            ThrowIf.Negative(length, nameof(length));

            Length = length;
            if (length == 0)
            {
                _buffer = Array.Empty<byte>();
            }
            else
            {
                int bytes = length * sizeof(T);
                _buffer = bytes > SmallArrayBoundary ? ArrayPool<byte>.Shared.Rent(bytes) : new byte[bytes];
            }
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
        #endregion

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (_buffer.Length > SmallArrayBoundary)
                ArrayPool<byte>.Shared.Return(_buffer);
        }

        public static implicit operator Span<T>(SpanBuffer<T> x) => x.Span;

        public static implicit operator ReadOnlySpan<T>(SpanBuffer<T> x) => x.Span;
    }
}
