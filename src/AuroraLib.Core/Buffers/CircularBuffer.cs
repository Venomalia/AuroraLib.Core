using AuroraLib.Core.IO;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Buffers
{
    /// <summary>
    /// A CircularBuffer class that inherits from <see cref="PoolStream"/>.
    /// </summary>
    public class CircularBuffer : PoolStream
    {
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

        private long _Position;

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
        public CircularBuffer(ArrayPool<byte> aPool, int capacity) : base(aPool, aPool.Rent(capacity), capacity)
        { }

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

        protected override void ExpandBuffer(int minimumLength)
        {
            throw new NotSupportedException();
        }

        protected override Span<byte> InternalBufferAsSpan(int start, int length)
            => _Buffer.AsSpan(start, length);
    }
}
