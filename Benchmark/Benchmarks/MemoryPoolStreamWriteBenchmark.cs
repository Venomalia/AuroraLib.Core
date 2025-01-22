using AuroraLib.Core.IO;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class MemoryPoolStreamWriteBenchmark
    {
        private const int MBtoBytes = 1048576;
        private const int Steps = 10;

        [Params(1, 10)]
        public int MB;

        private readonly byte[] data = new byte[MBtoBytes / Steps];

        [Benchmark]
        public void MemoryStream_WriteByte()
        {
            using MemoryStream ms = new();
            for (int i = 0; i < MB * MBtoBytes; i++)
            {
                ms.WriteByte(1);
            }
        }

        [Benchmark]
        public void MemoryPoolStream_WriteByte()
        {
            using MemoryPoolStream ms = new();
            for (int i = 0; i < MB * MBtoBytes; i++)
            {
                ms.WriteByte(1);
            }
        }

        [Benchmark]
        public void MemoryStream_WriteBuffer()
        {
            using MemoryStream ms = new();
            for (int i = 0; i < MB * Steps; i++)
            {
                ms.Write(data);
            }
        }

        [Benchmark]
        public void MemoryPoolStream_WriteBuffer()
        {
            using MemoryPoolStream ms = new();
            for (int i = 0; i < MB * Steps; i++)
            {
                ms.Write(data);
            }
        }
    }
}
