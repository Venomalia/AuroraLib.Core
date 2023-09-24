using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
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

        /// <summary>
        /// A long value representing the length of the stream in bytes.
        /// </summary>
        public long Length => basestream.Length;

        /// <summary>
        /// The current position within the stream.
        /// </summary>
        public long Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Buffered ? basestream.Position - 1 : basestream.Position;
            set
            {
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

                    if (Buffered)
                        shift--;

                    value %= 8;
                    basestream.Seek(shift, SeekOrigin.Current);
                    ResetBuffer();
                }
                _bit = value;
            }
        }
        private int _bit = 0;

        /// <summary>
        /// Byte Order, in which blocks of Bits are read & write.
        /// Endian.Little [0][1][2][3]...->
        /// Endian.Big    <-...[3][2][1][0]
        /// </summary>
        public Endian ByteOrder = Endian.Little;

        /// <summary>
        /// Bit Order, in which Bit are read & write.
        /// Endian.Little [0,1,2,3,4,5,6,7]...->
        /// Endian.Big    <-...[7,6,5,4,3,2,1,0]
        /// </summary>
        public Endian BitOrder
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _bitOrder;
            set
            {
                if (_bitOrder != value)
                {
                    _bitOrder = value;
                    if (Buffered)
                        ResetBuffer();
                }
            }
        }

        private Endian _bitOrder = Endian.Little;

        /// <summary>
        /// Indicates that the buffer has read one byte ahead and this byte is not fully processed yet.
        /// </summary>
        protected bool Buffered = false;

        /// <summary>
        /// when true leave the base stream open when disposing.
        /// </summary>
        protected bool Protectbase = true;

        /// <summary>
        /// Resets the buffer.
        /// </summary>
        protected abstract void ResetBuffer();

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <param name="bitposition"></param>
        public virtual void Seek(long offset, SeekOrigin origin, int bitposition = 0)
        {
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
