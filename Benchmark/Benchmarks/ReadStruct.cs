using AuroraLib.Core;
using AuroraLib.Core.Buffers;
using AuroraLib.Core.IO;
using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.Buffers.Binary;

namespace Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class ReadStruct
    {
        private const int n = 1000;
        private const int SIZE = 16;
        private static readonly byte[] src = new byte[SIZE * n];

        private static readonly MemoryStream ms = new(src);

        [Benchmark]
        public void BinaryReader_Read()
        {
            ms.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new(ms);
            for (var i = 0; i < n; ++i)
            {
                _ = reader.ReadInt64();
                _ = reader.ReadInt32();
                _ = reader.ReadInt16();
                _ = reader.ReadByte();
                _ = reader.ReadByte();
            }
        }

        [Benchmark]
        public void BinaryPrimitives_Read()
        {
            ms.Seek(0, SeekOrigin.Begin);
            Span<byte> bytes = stackalloc byte[SIZE];
            for (var i = 0; i < n; ++i)
            {
                ms.Read(bytes);
                _ = BinaryPrimitives.ReadInt64LittleEndian(bytes.Slice(0, 8));
                _ = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(8, 4));
                _ = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(12, 2));
                _ = bytes[14];
                _ = bytes[15];
            }
        }

        [Benchmark]
        public void AuroraCore_Read()
        {
            ms.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = ms.Read<TestStruct>();
            }
        }

        [Benchmark]
        public void AuroraCore_ReadArray()
        {
            ms.Seek(0, SeekOrigin.Begin);
            _ = ms.Read<TestStruct>(n);
        }

        [Benchmark]
        public void AuroraCore_ReadArrayPool()
        {
            ms.Seek(0, SeekOrigin.Begin);
            TestStruct[] testStructs = ArrayPool<TestStruct>.Shared.Rent(n);
            ms.Read(testStructs.AsSpan()[..n]);
            ArrayPool<TestStruct>.Shared.Return(testStructs);
        }

        [Benchmark]
        public void AuroraCore_ReadSpanBuffer()
        {
            ms.Seek(0, SeekOrigin.Begin);
            using (SpanBuffer<TestStruct> buffer = new(n))
            {
                ms.Read<TestStruct>(buffer);
            }
        }

        private struct TestStruct
        {
            public long Test1 { get; set; }
            public int Test2 { get; set; }
            public short Test3 { get; set; }
            public byte Test4 { get; set; }
            public byte Test5 { get; set; }
        }

    }
}
