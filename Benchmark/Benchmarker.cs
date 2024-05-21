using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    public class Benchmarker
    {
        static void Main(string[] args)
        {
            var Result = BenchmarkRunner.Run<Pool>();
        }
    }
}
