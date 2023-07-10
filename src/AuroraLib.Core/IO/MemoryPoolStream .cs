using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Represents a <see cref="Stream"/> that uses a <see cref="ArrayPool{byte}"/> for efficient allocation and management.
    /// </summary>
    public class MemoryPoolStream : Stream
    {
        /// <inheritdoc/>
        public override bool CanRead => _open;
        /// <inheritdoc/>
        public override bool CanSeek => _open;
        /// <inheritdoc/>
        public override bool CanWrite => _open;
        private bool _open;

        /// <inheritdoc/>
        public override long Length => _Length;
        private long _Length;

        /// <inheritdoc/>
        public override long Position
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Position;
            [DebuggerStepThrough]
            set
            {
                if (value < 0 || value > Length)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _Position = value;
            }
        }
        private long _Position;

        private readonly ArrayPool<byte> _APool;
        private byte[] _Buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class.
        /// </summary>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream() : this(ArrayPool<byte>.Shared)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the default ArrayPool and the specified capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the stream.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream(int capacity) : this(ArrayPool<byte>.Shared, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the specified <see cref="ArrayPool{T}"/>.
        /// </summary>
        /// <param name="aPool">The ArrayPool used to rent and return byte arrays.</param>
        [DebuggerStepThrough]
        public MemoryPoolStream(ArrayPool<byte> aPool)
        {
            _APool = aPool;
            _Buffer = Array.Empty<byte>();
            _Position = _Length = 0;
            _open = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The Stream from which to read the initial data.</param>
        [DebuggerStepThrough]
        public MemoryPoolStream(Stream stream) : this((int)stream.Length)
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(_Buffer);
            _Length = stream.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the data from the specified ReadOnlySpan.
        /// </summary>
        /// <param name="span">The ReadOnlySpan from which to initialize the stream's data.</param>
        [DebuggerStepThrough]
        public MemoryPoolStream(ReadOnlySpan<byte> span) : this(span.Length)
        {
            span.CopyTo(_Buffer);
            _Length = span.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the specified <see cref="ArrayPool{T}"/> and capacity.
        /// </summary>
        /// <param name="aPool">The ArrayPool used to rent and return byte arrays.</param>
        /// <param name="capacity">The initial capacity of the stream.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream(ArrayPool<byte> aPool, int capacity) : this(aPool)
        {
            _Buffer = aPool.Rent(capacity);
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer.AsSpan(offset, count));

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override int Read(Span<byte> buffer)
        {
            int num = (int)(_Length - _Position);
            if (num > buffer.Length)
            {
                num = buffer.Length;
            }
            _Buffer.AsSpan((int)_Position, num).CopyTo(buffer);
            _Position += num;
            return num;
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin) => origin switch
        {
            SeekOrigin.Begin => Position = offset,
            SeekOrigin.Current => Position += offset,
            SeekOrigin.End => Position = Length + offset,
            _ => throw new ArgumentException($"Origin {origin} is invalid."),
        };

        /// <inheritdoc/>
        [DebuggerStepThrough]
        public override void SetLength(long length)
        {
            if (length < 0 || length > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Maximum supported size {int.MaxValue}.");
            }

            if (_Buffer.Length < length)
            {
                SetBuffer((int)length);
            }
            else if (_Position > _Length)
            {
                _Position = _Length;
            }
            _Length = length;
        }

        private void SetBuffer(int minimumLength)
        {
            byte[] newBuffer = _APool.Rent(minimumLength);
            if (_Buffer.Length != 0)
            {
                _Buffer.AsSpan().CopyTo(newBuffer);
                _APool.Return(_Buffer);
            }
            _Buffer = newBuffer;
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Write(byte[] buffer, int offset, int count)
        {
            long newpos = Position + count;
            if (newpos > _Length)
            {
                SetLength(newpos);
            }
            buffer.AsSpan(offset, count).CopyTo(_Buffer.AsSpan((int)_Position, count));
            _Position = newpos;
        }

        /// <inheritdoc cref="MemoryExtensions.AsSpan{byte}(byte[]?)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
            => _Buffer.AsSpan(0, (int)_Length);

        /// <inheritdoc cref="MemoryExtensions.AsSpan{byte}(byte[]?, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start)
            => _Buffer.AsSpan(start, (int)_Length - start);

        /// <inheritdoc cref="MemoryExtensions.AsSpan{byte}(byte[]?, int, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start, int length)
            => _Buffer.AsSpan(start, length);

        /// <summary>
        /// Dummy
        /// </summary>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Flush()
        { }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_Buffer.Length != 0)
            {
                _APool.Return(_Buffer);
                _Buffer = null;
            }
            _Length = _Position = 0;
            _open = false;
        }
    }
}
