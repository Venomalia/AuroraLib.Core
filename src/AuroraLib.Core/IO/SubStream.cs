using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Represents a <see cref="SubStream"/> that provides a view into a portion of an underlying <see cref="Stream"/>.
    /// </summary>
    public class SubStream : Stream
    {
        /// <summary>
        /// Returns the underlying stream.
        /// </summary>
        public Stream BaseStream => _basestream;
        private readonly Stream _basestream;

        /// <summary>
        /// Offset to the underlying stream.
        /// </summary>
        public readonly long Offset;

        private readonly bool ProtectBaseStream;

        /// <inheritdoc/>
        public override bool CanRead => BaseStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => BaseStream.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length => _length;
        private readonly long _length;

        /// <inheritdoc/>
        public override long Position
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
            [DebuggerStepThrough]
            set
            {
                if (value < 0 || value > Length)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _position = value;
            }
        }
        protected long _position = 0;

        /// <summary>
        /// Creates a new <see cref="SubStream"/> instance of the specified <paramref name="stream"/> at the specified <paramref name="offset"/> with the specified <paramref name="length"/>.
        /// </summary>
        /// <param name="stream">The underlying stream.</param>
        /// <param name="length">The length of the substream.</param>
        /// <param name="offset">The offset within the underlying stream where the substream starts.</param>
        /// <param name="protectBaseStream">Specifies whether the base stream should be protected from being closed when the substream is closed.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public SubStream(Stream stream, long length, long offset, bool protectBaseStream = true)
        {
            _basestream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || length > BaseStream.Length - offset) throw new ArgumentOutOfRangeException(nameof(length));

            _length = length;
            Offset = offset;
            ProtectBaseStream = protectBaseStream;
        }

        /// <summary>
        /// Creates a new <see cref="SubStream"/> instance of the specified <paramref name="stream"/> from the current Position with the specified <paramref name="length"/>.
        /// </summary>
        /// <param name="stream">The underlying stream.</param>
        /// <param name="length">The length of the substream.</param>
        /// <param name="protectBaseStream">Specifies whether the base stream should be protected from being closed when the substream is closed.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SubStream(Stream stream, long length, bool protectBaseStream = true) : this(stream, length, stream.Position, protectBaseStream)
        { }

        /// <summary>
        /// Creates a new <see cref="SubStream"/> instance of the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The underlying stream.</param>
        /// <param name="protectBaseStream">Specifies whether the base stream should be protected from being closed when the substream is closed.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SubStream(Stream stream, bool protectBaseStream = true) : this(stream, stream.Length, stream.Position, protectBaseStream)
        { }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Flush()
            => BaseStream.Flush();

        /// <inheritdoc/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer.AsSpan(offset, count));

        /// <inheritdoc/>
        [DebuggerStepThrough]
        public override int Read(Span<byte> buffer)
        {
            int num = (int)(_length - _position);
            if (num > buffer.Length)
            {
                num = buffer.Length;
            }
            lock (_basestream)
            {
                BaseStream.Seek(_position + Offset, SeekOrigin.Begin);
                BaseStream.Read(buffer[..num]);
            }
            _position += num;
            return num;
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        public override long Seek(long offset, SeekOrigin origin) => origin switch
        {
            SeekOrigin.Begin => Position = offset,
            SeekOrigin.Current => Position += offset,
            SeekOrigin.End => Position = Length + offset,
            _ => throw new ArgumentException($"Origin {origin} is invalid."),
        };

        /// <inheritdoc/>
        public override void SetLength(long value)
            => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        #region Dispose
        /// <inheritdoc/>
        [DebuggerStepThrough]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_basestream.CanRead && !ProtectBaseStream)
                {
                    try { _basestream.Dispose(); }
                    catch { }
                }
            }
        }
        #endregion Dispose
    }
}
