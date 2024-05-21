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
            _APool = aPool;
            _Buffer = buffer;
            _Length = length;
            _open = true;
            Position = 0;
        }


#if NET20_OR_GREATER
        /// <inheritdoc cref="Stream.Read(byte[], int, int)"/>
        public abstract int Read(Span<byte> buffer);
        /// <inheritdoc cref="Stream.Write(byte[], int, int)"/>
        public abstract void Write(ReadOnlySpan<byte> buffer);
#endif

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET20_OR_GREATER
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    throw new ArgumentException($"Origin {origin} is invalid.");
            }
            return Position;
        }
#else
        public override long Seek(long offset, SeekOrigin origin) => origin switch
        {
            SeekOrigin.Begin => Position = offset,
            SeekOrigin.Current => Position += offset,
            SeekOrigin.End => Position = Length + offset,
            _ => throw new ArgumentException($"Origin {origin} is invalid."),
        };
#endif

        /// <summary>
        /// Dummy
        /// </summary>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Flush()
        { }

#if !NET20_OR_GREATER
        /// <inheritdoc />
        public override void CopyTo(Stream destination, int bufferSize)
        {
            if (!CanRead)
            {
                throw new ObjectDisposedException(nameof(MemoryPoolStream));
            }
            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination), "Stream is null.");
            }

            destination.Write(UnsaveAsSpan((int)Position));
        }
#endif

        /// <inheritdoc/>
        [DebuggerStepThrough]
        public override void SetLength(long length)
        {
            if (!_open)
            {
                throw new ObjectDisposedException(nameof(MemoryPoolStream));
            }
            if (length < 0 || length > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Maximum supported size {int.MaxValue}.");
            }

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

        /// <inheritdoc cref="UnsaveAsSpan(int, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsaveAsSpan()
            => InternalBufferAsSpan(0, (int)Length);

        /// <inheritdoc cref="UnsaveAsSpan(int, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsaveAsSpan(int start)
            => InternalBufferAsSpan(start, (int)Length - start);

        /// <summary>
        /// Returns a Span of byte representing the data of the <see cref="PoolStream"/>, it should not be written to the stream while the span is in use.
        /// </summary>
        /// <param name="start">The index at which to begin the span.</param>
        /// <param name="length">The span representation of the array.</param>
        /// <returns>The span representation of the <see cref="PoolStream"/>.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsaveAsSpan(int start, int length)
            => InternalBufferAsSpan(start, length);

        protected abstract Span<byte> InternalBufferAsSpan(int start, int length);

        /// <summary>
        /// Gets the byte buffer of the <see cref="PoolStream"/>, it should not be written to the stream while the byte buffer is in use.
        /// </summary>
        /// <returns>The byte buffer of the <see cref="PoolStream"/>.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] UnsaveGetBuffer()
            => _Buffer;

        /// <inheritdoc cref="MemoryStream.WriteTo(Stream)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(Stream stream)
        {
            if (!CanRead)
            {
                throw new ObjectDisposedException(nameof(MemoryPoolStream));
            }
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream), "Stream is null.");
            }
#if NET20_OR_GREATER
            stream.Write(_Buffer, 0, (int)Length);
#else
            stream.Write(UnsaveAsSpan());
#endif
        }

        /// <inheritdoc cref="MemoryStream.ToArray"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToArray()
            => UnsaveAsSpan().ToArray();

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
