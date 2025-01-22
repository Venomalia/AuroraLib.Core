using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    public class Benchmarker
    {
        static void Main(string[] args)
        {
            var resultReadValue = BenchmarkRunner.Run<StreamReadValueBenchmark>();
            var resultReadTestStruct = BenchmarkRunner.Run<StreamReadTestStructBenchmark>();
            var resultPoolStreamWrite = BenchmarkRunner.Run<MemoryPoolStreamWriteBenchmark>();
            var resultPoolBuffer = BenchmarkRunner.Run<PoolBufferBenchmark>();

        }

    }
}
