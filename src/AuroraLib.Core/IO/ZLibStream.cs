#if !NET6_0_OR_GREATER
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace AuroraLib.Core.IO
{
    /// <summary>Provides methods and properties used to compress and decompress streams by using the zlib data format specification.</summary>
    public sealed class ZLibStream : DeflateStream
    {
        private readonly Adler32 adler32;
        private readonly bool _isCompress;
        private readonly bool _leaveOpen;

        public ZLibStream(Stream stream, CompressionMode mode) : this(stream, mode, false) { }

        public ZLibStream(Stream stream, CompressionLevel compressionLevel) : this(stream, compressionLevel, false) { }

        public ZLibStream(Stream stream, CompressionMode mode, bool leaveOpen) : base(stream, mode, true)
        {
            adler32 = new Adler32();
            _isCompress = mode == CompressionMode.Compress;
            _leaveOpen = leaveOpen;
            if (_isCompress)
                stream.Write(new Header(CompressionLevel.Optimal));
            else if (!stream.Read<Header>().Validate())
                throw new InvalidDataException();
        }

        public ZLibStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen) : base(stream, compressionLevel, true)
        {
            adler32 = new Adler32();
            _isCompress = true;
            _leaveOpen = leaveOpen;
            stream.Write(new Header(compressionLevel));
        }

        /// <inheritdoc/>
        public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            adler32.Append(array.AsSpan(offset, count));
            return base.BeginWrite(array, offset, count, asyncCallback, asyncState);
        }

        /// <inheritdoc/>
        public override void Write(byte[] array, int offset, int count)
        {
            base.Write(array, offset, count);
            adler32.Append(array.AsSpan(offset, count));
        }

        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            adler32.Append(buffer.AsSpan(offset, count));
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            var baseStream = base.BaseStream;
            base.Dispose(disposing);
            if (disposing && _isCompress)
               baseStream.Write(adler32.GetCurrentHashAsUInt32(), Endian.Big);

            if (!_leaveOpen)
                baseStream.Dispose();
        }

        private readonly struct Header
        {
            private readonly byte cmf;
            private readonly byte flg;

            public Header(CompressionLevel level)
            {
                cmf = 0x78;
                switch (level)
                {
                    case System.IO.Compression.CompressionLevel.NoCompression:
                    case System.IO.Compression.CompressionLevel.Fastest:
                        flg = 0x01;
                        break;
                    case System.IO.Compression.CompressionLevel.Optimal:
                        flg = 0xDA;
                        break;
                    default:
                        flg = 0x9C;
                        break;
                }
            }

            public bool IsDeflate => (cmf & 0x0F) == 8;

            public byte CompressionInfo => (byte)((cmf >> 4) & 0x0F);

            public bool HasDictionary => ((flg >> 5) & 0x01) != 0;

            public byte CompressionLevel => (byte)((flg >> 6) & 0x03);

            public bool Validate()
            {
                if (!IsDeflate || CompressionInfo > 7)
                    return false;

                return ((cmf * 256 + flg) % 31 == 0);
            }
        }
    }
}
#endif
