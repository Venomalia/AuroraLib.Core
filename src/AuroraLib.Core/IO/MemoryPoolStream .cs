using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Represents a <see cref="Stream"/> that uses a <see cref="ArrayPool{byte}"/> for efficient allocation and management.
    /// </summary>
    public sealed class MemoryPoolStream : Stream
    {
        private const int defaultcapacity = 512;

        /// <inheritdoc/>
        public override bool CanRead => _open;
        /// <inheritdoc/>
        public override bool CanSeek => _open;
        /// <inheritdoc/>
        public override bool CanWrite => _open;
        /// <inheritdoc/>
        public override long Length => _Length;

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
        private bool _open;
        private long _Length;
        private long _Position;

        private readonly ArrayPool<byte> _APool;
        private byte[] _Buffer;

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
        public MemoryPoolStream(ArrayPool<byte> aPool, byte[] buffer, int length = 0)
        {
            _APool = aPool;
            _Buffer = buffer;
            _Length = length;
            _Position = 0;
            _open = true;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolStream"/> class with a copy of the specified <paramref name="stream"/>.
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryPoolStream(ReadOnlySpan<byte> span) : this(span.Length)
        {
            span.CopyTo(_Buffer);
            _Length = span.Length;
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
        public override int ReadByte()
        {
            if (Position > Length)
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
            long newpos = Position + buffer.Length;
            if (newpos > _Length)
            {
                SetLength(newpos);
            }
            buffer.CopyTo(_Buffer.AsSpan((int)_Position, buffer.Length));
            _Position = newpos;
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
                SetBuffer((int)length);
            }
            else if (_Position > length)
            {
                _Position = length;
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

        /// <inheritdoc />
        public override void CopyTo(Stream destination, int bufferSize)
        {
            if (!_open)
            {
                throw new ObjectDisposedException(nameof(MemoryPoolStream));
            }
            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination), "Stream is null.");
            }
            destination.Write(UnsaveAsSpan((int)_Position));
        }

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
                _Buffer = Array.Empty<byte>();
            }
            _Length = _Position = 0;
            _open = false;
        }
        #endregion

        /// <inheritdoc cref="UnsaveAsSpan(int, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsaveAsSpan()
            => _Buffer.AsSpan(0, (int)_Length);

        /// <inheritdoc cref="UnsaveAsSpan(int, int)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsaveAsSpan(int start)
            => _Buffer.AsSpan(start, (int)_Length - start);

        /// <summary>
        /// Returns a <see cref="Span{byte}"/> representing the data of the <see cref="MemoryPoolStream"/>, it should not be written to the stream while the span is in use.
        /// </summary>
        /// <param name="start">The index at which to begin the span.</param>
        /// <param name="length">The span representation of the array.</param>
        /// <returns>The span representation of the <see cref="MemoryPoolStream"/>.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> UnsaveAsSpan(int start, int length)
            => _Buffer.AsSpan(start, length);

        /// <summary>
        /// Returns a <see cref="Memory{T}{byte}"/> representing the data of the <see cref="MemoryPoolStream"/>, it should not be written to the stream while the memory is in use.
        /// </summary>
        /// <returns>The memory representation of the <see cref="MemoryPoolStream"/>.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> UnsaveAsMemory()
            => _Buffer.AsMemory(0, (int)_Length);

        /// <summary>
        /// Gets the byte buffer of the <see cref="MemoryPoolStream"/>, it should not be written to the stream while the byte buffer is in use.
        /// </summary>
        /// <returns>The byte buffer of the <see cref="MemoryPoolStream"/>.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] UnsaveGetBuffer()
            => _Buffer;

        /// <inheritdoc cref="MemoryStream.WriteTo(Stream)"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(Stream stream)
        {
            if (!_open)
            {
                throw new ObjectDisposedException(nameof(MemoryPoolStream));
            }
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream), "Stream is null.");
            }
            stream.Write(_Buffer.AsSpan(0, (int)_Length));
        }

        /// <inheritdoc cref="MemoryStream.TryGetBuffer(out ArraySegment{byte})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetBuffer(out ArraySegment<byte> buffer)
        {
            buffer = new ArraySegment<byte>(_Buffer, 0, (int)_Length);
            return _open;
        }

        /// <inheritdoc cref="MemoryStream.ToArray"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToArray()
        {
            byte[] numArray = new byte[_Length];
            _Buffer.AsSpan(0, (int)_Length).CopyTo(numArray);
            return numArray;
        }

    }
}
