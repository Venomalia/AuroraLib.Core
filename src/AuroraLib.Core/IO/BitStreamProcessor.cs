using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    /// <summary>
    /// Basic class for <see cref="BitReader"/> and <see cref="BitWriter"/>
    /// </summary>
    public abstract class BitStreamProcessor : IDisposable
    {
        /// <summary>
        /// Returns the underlying stream.
        /// </summary>
        public Stream BaseStream
        {
            get
            {
                if (disposedValue)
                    throw new ObjectDisposedException(GetType().Name);

                return basestream;
            }
            protected set => basestream = value;
        }
        private Stream basestream;

        /// <inheritdoc cref="Stream.Length"/>
        public long Length => basestream.Length;


        /// <inheritdoc cref="Stream.Position"/>
        public virtual long Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => basestream.Position;
            set
            {
                FlushBuffer();
                basestream.Seek(value, SeekOrigin.Begin);
                ResetBuffer();
            }
        }

        /// <summary>
        /// The current position within the byte.
        /// </summary>
        public int BitPosition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _bit;
            set
            {
                if (value >= 8 || value < 0)
                {
                    int shift = value / 8;

                    if (BitPosition != 0)
                        shift--;

                    value %= 8;
                    basestream.Seek(shift, SeekOrigin.Current);
                    ResetBuffer();
                }
                _bit = value;
            }
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _bit;

        /// <summary>
        /// Byte Order, in which blocks of Bits are read and write.
        /// Endian.Little   [0][1][2][3] [0,1,2,3,4,5,6,7]
        /// Endian.Big      [3][2][1][0] [7,6,5,4,3,2,1,0]
        /// </summary>
        public Endian Order = Endian.Little;

        private readonly bool Protectbase;

        protected BitStreamProcessor(Stream stream, Endian order = Endian.Little, bool leaveOpen = true)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            Protectbase = leaveOpen;
            Order = order;
            _bit = 0;
        }

        /// <summary>
        /// Resets the buffer.
        /// </summary>
        protected abstract void ResetBuffer();

        /// <summary>
        /// Flush the buffer.
        /// </summary>
        protected abstract void FlushBuffer();

        /// <inheritdoc cref="Stream.Seek(long, SeekOrigin)"/>
        public virtual void Seek(long offset, SeekOrigin origin, int bitposition = 0)
        {
            FlushBuffer();
            BaseStream.Seek(offset, origin);
            ResetBuffer();
            BitPosition = bitposition;
        }

        #region Dispose

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (basestream != null && !Protectbase)
                {
                    try { basestream.Dispose(); }
                    catch { }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Dispose
    }
}
