using AuroraLib.Core.IO;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class WriteStream
    {
        [Params(100, 10000, 100000)]
        public int N;

        private readonly byte[] data = new byte[10];

        [Benchmark]
        public void MemoryStream_Write()
        {
            using MemoryStream ms = new();
            for (int i = 0; i < N; i++)
            {
                ms.Write(data);
            }
        }

        [Benchmark]
        public void MemoryPoolStream_Write()
        {
            using MemoryPoolStream ms = new();
            for (int i = 0; i < N; i++)
            {
                ms.Write(data);
            }
        }
    }
}
