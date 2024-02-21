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

        private Stream stream;

        [GlobalSetup]
        public void GlobalSetup()
        {
            stream = new FileStream("Test.tmp", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            stream.Write(0, SIZE * n / 4);
            stream.Position = 0;
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            stream.Dispose();
        }

        [Benchmark]
        public void BinaryReader_Read()
        {
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new(stream);
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
            stream.Seek(0, SeekOrigin.Begin);
            Span<byte> bytes = stackalloc byte[SIZE];
            for (var i = 0; i < n; ++i)
            {
                stream.Read(bytes);
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
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = stream.ReadInt64();
                _ = stream.ReadInt32();
                _ = stream.ReadInt16();
                _ = stream.ReadByte();
                _ = stream.ReadByte();
            }
        }

        [Benchmark]
        public void AuroraCore_ReadT()
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = stream.Read<TestStruct>();
            }
        }

        [Benchmark]
        public void AuroraCore_ReadSpanBufferT()
        {
            stream.Seek(0, SeekOrigin.Begin);
            using SpanBuffer<TestStruct> buffer = new(n);
            stream.Read<TestStruct>(buffer);
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
