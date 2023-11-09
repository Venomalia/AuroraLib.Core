using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Represents a <see cref="Stream"/> that uses a <see cref="ArrayPool{byte}"/> for efficient allocation and management.
    /// </summary>
    public sealed class MemoryPoolStream : PoolStream
    {
        private const int defaultcapacity = 512;

        /// <inheritdoc/>
        public override long Position
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Position;
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value < 0 || value > Length)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _Position = value;
            }
        }
        private long _Position;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class.
        /// </summary>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream() : this(ArrayPool<byte>.Shared)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the stream.</param>
        /// <param name="setLength">Set the capacity as the stream length.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream(int capacity, bool setLength = false) : this(ArrayPool<byte>.Shared, capacity, setLength)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the specified <see cref="ArrayPool{byte}"/>.
        /// </summary>
        /// <param name="aPool">The ArrayPool used to rent and return byte arrays.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream(ArrayPool<byte> aPool) : this(aPool, defaultcapacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the specified <see cref="ArrayPool{T}"/> and capacity.
        /// </summary>
        /// <param name="aPool">The ArrayPool used to rent and return byte arrays.</param>
        /// <param name="capacity">The initial capacity of the stream.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream(ArrayPool<byte> aPool, int capacity, bool setLength = false) : this(aPool, aPool.Rent(capacity), setLength ? capacity : 0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with a buffer from the specified ArrayPool.
        /// </summary>
        /// <param name="aPool">The ArrayPool<byte> to use for buffer management.</param>
        /// <param name="buffer">The byte[] buffer to use for stream data storage.</param>
        /// <param name="length">The initial capacity of the stream.</param>
        [DebuggerStepThrough]
        public MemoryPoolStream(ArrayPool<byte> aPool, byte[] buffer, int length = 0) : base(aPool, buffer, length)
        { }


        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with a copy of the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The Stream from which to read the initial data.</param>
        [DebuggerStepThrough]
        public MemoryPoolStream(Stream stream) : this((int)stream.Length, true)
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(_Buffer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with the data from the specified ReadOnlySpan.
        /// </summary>
        /// <param name="span">The ReadOnlySpan from which to initialize the stream's data.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream(ReadOnlySpan<byte> span) : this(span.Length, true)
        {
            span.CopyTo(_Buffer);
        }

        #endregion

        #region Stream Overrides

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
            int num = (int)(Length - _Position);
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
        public override int ReadByte()
        {
            if (Position >= Length)
            {
                return -1;
            }
            return _Buffer[Position++];
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value) => Write(stackalloc byte[1] { value });

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count)
            => Write(buffer.AsSpan(offset, count));

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            long newpos = _Position + buffer.Length;
            if (newpos > Length)
            {
                SetLength(newpos);
            }
            buffer.CopyTo(_Buffer.AsSpan((int)_Position, buffer.Length));
            _Position = newpos;
        }
        #endregion

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        protected override void ExpandBuffer(int minimumLength)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Span<byte> InternalBufferAsSpan(int start, int length)
            => _Buffer.AsSpan(start, length);
    }
}
