using AuroraLib.Core.Buffers;
using AuroraLib.Core.Cryptography;
using AuroraLib.Core.Interfaces;
using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;

namespace Benchmark.Benchmarks
{
    public class Cryptography
    {

        [Params(typeof(Adler32), typeof(Adler64), typeof(CityHash32), typeof(CityHash64), typeof(CityHash128), typeof(Crc32), typeof(Fnv1_32), typeof(Fnv1_64), typeof(MurmurHash3_32), typeof(MurmurHash3_128), typeof(XXHash32), typeof(XXHash64))]
        public Type Algorithm;
        public IHash Instance;

        public SpanBuffer<byte> Data;

        [Params(10, 100)]
        public int MB;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Instance = (IHash)Activator.CreateInstance(Algorithm);
            int bytes = 1024 * 1024;//1mb
            Data = new(bytes);
            Random rng = new();
            Span<int> intSpan = MemoryMarshal.Cast<byte, int>(Data.Span);
            for (int i = 0; i < intSpan.Length; i++)
            {
                intSpan[i] = rng.Next();
            }
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            Data.Dispose();
        }

        [Benchmark]
        public void Compress()
        {
            for (int i = 0; i < MB; i++)
            {
                Instance.Compute(Data.Span);
            }
        }

    }
}
