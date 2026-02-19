using AuroraLib.Core.IO;
using BenchmarkDotNet.Attributes;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Benchmark.Benchmarks
{
    public class StreamReadValueBenchmark
    {
        private const int n = 50;

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
                _ = reader.ReadUInt32();
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
                _ = stream.ReadInt32(); //fastes
            }
        }

        [Benchmark]
        public void AuroraCore_ReadInt32LittleEndian()
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = stream.ReadInt32LittleEndian();
            }
        }

        [Benchmark]
        public void AuroraCore_ReadGenericInt32A() // fastet net 10
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = ReadGenericHelper_A<int>(stream);
            }
        }
        [Benchmark]
        public void AuroraCore_ReadGenericInt32B() // fastet net 10
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (var i = 0; i < n; ++i)
            {
                _ = ReadGenericHelper_A<int>(stream);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static T ReadGenericHelper_A<T>(Stream stream) where T : unmanaged
        {
            T value;
            Span<byte> buffer = new Span<byte>(&value, sizeof(T));

            if (stream.Read(buffer) != buffer.Length)
                EndOfStreamException<T>();

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static T ReadGenericHelper_B<T>(Stream stream) where T : unmanaged
        {
            T value;
            Span<byte> buffer = new Span<byte>(&value, sizeof(T));

            if (stream.Read(buffer) != buffer.Length)
                throw GetEndOfStreamException<T>();

            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void EndOfStreamException<T>()
    => throw new EndOfStreamException($"Cannot read a {typeof(T)}, is beyond the end of the stream.");


        [MethodImpl(MethodImplOptions.NoInlining)]
        public static EndOfStreamException GetEndOfStreamException<T>()
    => new EndOfStreamException($"Cannot read a {typeof(T)}, is beyond the end of the stream.");

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
            Span<long> buffer = stackalloc long[n];
            stream.Read<long>(buffer);
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
