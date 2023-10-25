using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Buffers
{
    /// <summary>
    /// A CircularBuffer class that inherits from Stream, uses a <see cref="ArrayPool{byte}"/> for efficient allocation and management.
    /// </summary>
    public class CircularBuffer : Stream
    {
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
                if (value < 0)
                    _Position = Length + value;
                else if (value >= Length)
                    _Position = value % Length;
                else
                    _Position = value;
            }
        }

        private bool _open;
        private long _Length;
        private long _Position;

        private readonly ArrayPool<byte> _APool;
        protected byte[] _Buffer;

        /// <summary>
        /// Initializes a new instance of the CircularBuffer class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the circular buffer.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CircularBuffer(int capacity) : this(ArrayPool<byte>.Shared, capacity)
        { }


        /// <summary>
        /// Initializes a new instance of the CircularBuffer class with the specified ArrayPool and capacity.
        /// </summary>
        /// <param name="aPool">The ArrayPool<byte> to use for buffer management.</param>
        /// <param name="capacity">The capacity of the circular buffer.</param>
        [DebuggerStepThrough]
        public CircularBuffer(ArrayPool<byte> aPool, int capacity)
        {
            _APool = aPool;
            _Buffer = aPool.Rent(capacity);
            _Length = capacity;
            _Position = 0;
            _open = true;
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
            int total = 0;
            while (buffer.Length > total)
            {
                int num = (int)Math.Min(Length - Position, buffer.Length);
                _Buffer.AsSpan((int)Position, num).CopyTo(buffer.Slice(total, num));

                Position += num;
                total += num;
            }
            //always fill the entire buffer.
            return buffer.Length;
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int ReadByte()
            => _Buffer[Position++];

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
            if (Length >= Position + buffer.Length)
            {
                // The entire buffer fits without wrapping around.
                buffer.CopyTo(_Buffer.AsSpan((int)Position));
                Position += buffer.Length;
            }
            else
            {
                // Partially write and wrap around.
                int bytesNeeded = (int)Math.Min(Length, buffer.Length);
                int left = (int)(Length - (Position));
                // If the buffer.length is greater than this CircularBuffer we don't need to write the bytes.
                buffer.Slice(buffer.Length - bytesNeeded, left).CopyTo(_Buffer.AsSpan((int)Position, left));

                if (bytesNeeded > left)
                {
                    left = bytesNeeded - left;
                    buffer.Slice(buffer.Length - left, left).CopyTo(_Buffer.AsSpan(0, left));
                }
                Position += bytesNeeded;
            }
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value)
            => _Buffer[Position++] = value;

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


        /// <summary>
        /// Not supported!
        /// </summary>
        [DebuggerStepThrough]
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
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
    }
}
