using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// A base class for stream implementations that use memory pools to provide efficient memory management and data handling.
    /// </summary>
    public abstract class PoolStream : Stream
    {
        /// <inheritdoc/>
        public override bool CanRead => _open;
        /// <inheritdoc/>
        public override bool CanSeek => _open;
        /// <inheritdoc/>
        public override bool CanWrite => _open;
        /// <inheritdoc/>
        public override long Length => _Length;

        private bool _open;
        private long _Length;

        /// <inheritdoc cref="ArrayPool{T}"/>
        protected readonly ArrayPool<byte> _APool;
        protected byte[] _Buffer;

        protected PoolStream(ArrayPool<byte> aPool, byte[] buffer, int length = 0)
        {
            ThrowIf.Null(aPool);
            ThrowIf.Null(buffer);
            ThrowIf.Negative(length);

            _APool = aPool;
            _Buffer = buffer;
            _Length = length;
            _open = true;
            Position = 0;
        }


#if NET20_OR_GREATER || NETSTANDARD2_0
        /// <inheritdoc cref="Stream.Read(byte[], int, int)"/>
        public abstract int Read(Span<byte> buffer);
        /// <inheritdoc cref="Stream.Write(byte[], int, int)"/>
        public abstract void Write(ReadOnlySpan<byte> buffer);
#endif

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin) => origin switch
        {
            SeekOrigin.Begin => Position = offset,
            SeekOrigin.Current => Position += offset,
            SeekOrigin.End => Position = Length + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, ThrowIf.InvalidEnumMessage(origin, nameof(origin)))
        };

        /// <summary>
        /// Dummy
        /// </summary>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Flush()
        { }

#if NET20_OR_GREATER || NETSTANDARD2_0
        /// <inheritdoc cref="Stream.CopyTo(Stream)"/>
        public new void CopyTo(Stream destination) => CopyTo(destination, 81920);

        /// <inheritdoc cref="Stream.CopyTo(Stream, int)"/>
        public new virtual void CopyTo(Stream destination, int bufferSize)
#else
        /// <inheritdoc />
        public override void CopyTo(Stream destination, int bufferSize)
#endif
        {
            ThrowIf.Disposed(!CanRead, this);
            ThrowIf.Null(destination, nameof(destination));

            destination.Write(_Buffer, (int)Position, (int)Length);
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        public override void SetLength(long length)
        {
            ThrowIf.Disposed(!_open, this);
            ThrowIf.Negative(length, nameof(length));
            ThrowIf.GreaterThan(length, int.MaxValue);

            if (_Buffer.Length < length)
            {
                ExpandBuffer((int)length);
            }
            else if (Position > length)
            {
                Position = length;
            }
            _Length = length;
        }

        protected abstract void ExpandBuffer(int minimumLength);

        /// <inheritdoc cref="UnsafeAsSpan(int, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsafeAsSpan()
            => InternalBufferAsSpan(0, (int)Length);

        /// <inheritdoc cref="UnsafeAsSpan(int, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsafeAsSpan(int start)
            => InternalBufferAsSpan(start, (int)Length - start);

        /// <summary>
        /// Returns a Span of byte representing the data of the <see cref="PoolStream"/>, it should not be written to the stream while the span is in use.
        /// </summary>
        /// <param name="start">The index at which to begin the span.</param>
        /// <param name="length">The span representation of the array.</param>
        /// <returns>The span representation of the <see cref="PoolStream"/>.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsafeAsSpan(int start, int length)
            => InternalBufferAsSpan(start, length);

        protected abstract Span<byte> InternalBufferAsSpan(int start, int length);

        /// <summary>
        /// Gets the byte buffer of the <see cref="PoolStream"/>, it should not be written to the stream while the byte buffer is in use.
        /// </summary>
        /// <returns>The byte buffer of the <see cref="PoolStream"/>.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] UnsafeGetBuffer()
            => _Buffer;

        /// <inheritdoc cref="MemoryStream.WriteTo(Stream)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(Stream stream)
        {
            ThrowIf.Disposed(!CanRead, this);
            ThrowIf.Null(stream, nameof(stream));

            stream.Write(_Buffer, 0, (int)Length);
        }

        /// <inheritdoc cref="MemoryStream.ToArray"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToArray()
            => UnsafeAsSpan().ToArray();

        /// <inheritdoc/>
        [DebuggerStepThrough]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_Buffer.Length != 0)
            {
                _APool.Return(_Buffer);
                _Buffer = Array.Empty<byte>();
            }
            _Length = Position = 0;
            _open = false;
        }
    }
}
