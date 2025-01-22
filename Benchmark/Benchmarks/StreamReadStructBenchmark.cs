using AuroraLib.Core;
using AuroraLib.Core.Buffers;
using AuroraLib.Core.Interfaces;
using AuroraLib.Core.IO;
using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.Buffers.Binary;

namespace Benchmark.Benchmarks
{
    public class StreamReadTestStructBenchmark
    {
        private const int n = 30;
        private const int SIZE = 16;

        private Stream stream = Stream.Null;

        [GlobalSetup]
        public void GlobalSetup()
        {
            stream = new FileStream("Test.tmp", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            stream.Write(0, SIZE * n / 4);
            stream.Position = 0;
        }

        [GlobalCleanup]
        public void GlobalCleanup() => stream.Dispose();

        [Benchmark]
        public void BinaryReader_ReadTestStruct()
        {
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new(stream);
            for (var i = 0; i < n; ++i)
            {
                _ = new TestStruct(
                    reader.ReadInt64(),
                    reader.ReadInt32(),
                    reader.ReadInt16(),
                    reader.ReadByte(),
                    reader.ReadByte()
                    );
            }
        }

        [Benchmark]
        public void BinaryPrimitives_ReadTestStruct()
        {
            stream.Seek(0, SeekOrigin.Begin);
            Span<byte> bytes = stackalloc byte[SIZE];
            for (var i = 0; i < n; ++i)
            {
                stream.Read(bytes);
                _ = new TestStruct(
                    BinaryPrimitives.ReadInt64LittleEndian(bytes.Slice(0, 8)),
                    BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(8, 4)),
                    BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(12, 2)),
                    bytes[14],
                    bytes[15]
                    );
            }
        }

        [Benchmark]
        public void AuroraCore_ReadTestStruct()
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = stream.Read<TestStruct>();
            }
        }

        [Benchmark]
        public void AuroraCore_ReadTestStruct_EndianBig()
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = stream.Read<TestStruct>(Endian.Big);
            }
        }

        [Benchmark]
        public void AuroraCore_ReadSpanTestStruct()
        {
            stream.Seek(0, SeekOrigin.Begin);
            using SpanBuffer<TestStruct> buffer = new(n);
            stream.Read<TestStruct>(buffer);
        }

        [Benchmark]
        public void AuroraCore_ReadSpanTestStruct_EndianBig()
        {
            stream.Seek(0, SeekOrigin.Begin);
            using SpanBuffer<TestStruct> buffer = new(n);
            stream.Read<TestStruct>(buffer,Endian.Big);
        }

        public readonly record struct TestStruct(long Test1, int Test2, short Test3, byte Test4, byte Test5) : IReversibleEndianness<TestStruct>
        {
            public TestStruct ReverseEndianness()
                => new TestStruct(
                    BinaryPrimitives.ReverseEndianness(Test1),
                    BinaryPrimitives.ReverseEndianness(Test2),
                    BinaryPrimitives.ReverseEndianness(Test3),
                    Test4,
                    Test5
                    );
        }

    }
}
