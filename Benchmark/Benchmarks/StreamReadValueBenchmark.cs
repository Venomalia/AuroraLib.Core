using AuroraLib.Core;
using AuroraLib.Core.Buffers;
using AuroraLib.Core.IO;
using BenchmarkDotNet.Attributes;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Benchmark.Benchmarks
{
    public class StreamReadValueBenchmark
    {
        private const int n = 30;

        private Stream stream = Stream.Null;

        [GlobalSetup]
        public void GlobalSetup()
        {
            stream = new FileStream("Test.tmp", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            stream.Write(0, Unsafe.SizeOf<long>() * n);
            stream.Position = 0;
        }

        [GlobalCleanup]
        public void GlobalCleanup() => stream.Dispose();

        [Benchmark]
        public void BinaryReader_ReadInt32()
        {
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new(stream);
            for (var i = 0; i < n; ++i)
            {
                _ = reader.ReadInt32();
            }
        }

        [Benchmark]
        public void BinaryPrimitives_ReadInt32()
        {
            stream.Seek(0, SeekOrigin.Begin);
            Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<int>()];
            for (var i = 0; i < n; ++i)
            {
                stream.Read(bytes);
                _ = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(0, 4));
            }
        }

        [Benchmark]
        public void AuroraCore_ReadInt32()
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = stream.ReadInt32();
            }
        }

        [Benchmark]
        public void BinaryPrimitives_ReadInt64()
        {
            stream.Seek(0, SeekOrigin.Begin);
            Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<long>()];
            for (var i = 0; i < n; ++i)
            {
                stream.Read(bytes);
                _ = BinaryPrimitives.ReadInt64LittleEndian(bytes.Slice(0, 8));
            }
        }

        [Benchmark]
        public void AuroraCore_ReadInt64()
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = stream.ReadInt64();
            }
        }

        [Benchmark]
        public void AuroraCore_ReadInt64_EndianBig()
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = stream.ReadInt64(Endian.Big);
            }
        }

        [Benchmark]
        public void AuroraCore_ReadSpanInt64()
        {
            stream.Seek(0, SeekOrigin.Begin);
            using SpanBuffer<long> buffer = new(n);
            stream.Read<long>(buffer);
        }

        [Benchmark]
        public void AuroraCore_ReadSpanInt64_EndianBig()
        {
            stream.Seek(0, SeekOrigin.Begin);
            using SpanBuffer<long> buffer = new(n);
            stream.Read<long>(buffer, Endian.Big);
        }

        [Benchmark]
        public void AuroraCore_ReadListInt64()
        {
            stream.Seek(0, SeekOrigin.Begin);
            List<long> list = new List<long>();
            stream.ReadCollection(list, n);
        }
    }
}
