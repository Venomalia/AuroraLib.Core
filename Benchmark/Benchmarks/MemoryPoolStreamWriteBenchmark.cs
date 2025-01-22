using AuroraLib.Core.IO;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class MemoryPoolStreamWriteBenchmark
    {
        [Params(1, 10, 100)]
        public int MB;

        private readonly byte[] data = new byte[1048576];

        [Benchmark]
        public void MemoryStream_Write()
        {
            using MemoryStream ms = new();
            for (int i = 0; i < MB; i++)
            {
                ms.Write(data);
            }
        }

        [Benchmark]
        public void MemoryPoolStream_Write()
        {
            using MemoryPoolStream ms = new();
            for (int i = 0; i < MB; i++)
            {
                ms.Write(data);
            }
        }
    }
}
