using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Represents a stream that reads and writes data in reverse order.
    /// </summary>
    public sealed class BackwardsMemoryStream : PoolStream
    {
        /// <inheritdoc/>
        public override long Position
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Buffer.Length - _Position;
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value < 0 || value > Length)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _Position = _Buffer.Length - value;
            }
        }
        private long _Position;

        #region Constructors
        public BackwardsMemoryStream() : this(512)
        { }

        public BackwardsMemoryStream(int capacity, bool setLength = false) : this(ArrayPool<byte>.Shared, capacity, setLength)
        { }

        public BackwardsMemoryStream(ArrayPool<byte> aPool, int capacity, bool setLength = false) : this(aPool, aPool.Rent(capacity), setLength ? capacity : 0)
        { }

        public BackwardsMemoryStream(ArrayPool<byte> aPool, byte[] buffer, int length = 0) : base(aPool, buffer, length)
        { }

        public BackwardsMemoryStream(Stream stream, int length) : this(length, true)
            => stream.Read(_Buffer.AsSpan(_Buffer.Length - length, length));

        public BackwardsMemoryStream(ReadOnlySpan<byte> span) : this(span.Length, true)
        {
            span.CopyTo(_Buffer.AsSpan(_Buffer.Length - span.Length, span.Length));
        }
        #endregion

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer.AsSpan(offset, count));

        /// <inheritdoc/>
        [DebuggerStepThrough]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public override int Read(Span<byte> buffer)
        {
            int num = (int)(Length - Position);
            if (num > buffer.Length)
            {
                num = buffer.Length;
            }
            _Buffer.AsSpan((int)_Position - num, num).CopyTo(buffer);
            _Position -= num;
            return num;
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count)
            => Write(buffer.AsSpan(offset, count));

        /// <inheritdoc/>
        [DebuggerStepThrough]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (_Position - buffer.Length < _Buffer.Length - Length)
            {
                SetLength(buffer.Length + Position);
            }
            _Position -= buffer.Length;
            Span<byte> section = _Buffer.AsSpan((int)_Position, buffer.Length);
            buffer.CopyTo(section);
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
            return _Buffer[_Position--];
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value)
#if NET20_OR_GREATER
            => Write(new byte[1] { value });
#else
            => Write(stackalloc byte[1] { value });
#endif

        /// <inheritdoc/>
        [DebuggerStepThrough]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        protected override void ExpandBuffer(int minimumLength)
        {
#if !NET6_0_OR_GREATER
            minimumLength = Math.Max(minimumLength, _Buffer.Length * 2);
#endif
            byte[] newBuffer = _APool.Rent(minimumLength);
            _Position += newBuffer.Length - _Buffer.Length;
            if (_Buffer.Length != 0)
            {
                Buffer.BlockCopy(_Buffer, 0, newBuffer, newBuffer.Length - _Buffer.Length, _Buffer.Length);
                _APool.Return(_Buffer);
            }
            _Buffer = newBuffer;
        }

        protected override Span<byte> InternalBufferAsSpan(int start, int length)
            => _Buffer.AsSpan(_Buffer.Length - length - start, length);
    }
}
