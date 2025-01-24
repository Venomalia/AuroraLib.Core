using AuroraLib.Core;
using BenchmarkDotNet.Attributes;
using System.Buffers.Binary;

namespace Benchmark.Benchmarks
{
    public class ReverseEndiannessStructBenchmark
    {
        private const int n = 100;
        private TestStruct[] data;
        private ReversibleTestStruct[] reversibleData;

        [GlobalSetup]
        public void Setup()
        {
            data = new TestStruct[n];
            reversibleData = new ReversibleTestStruct[n];
        }

        [Benchmark]
        public void ReverseEndianness_Auto()
        {
            for (int i = 0; i < n; i++)
            {
                data[i] = BitConverterX.ReverseEndianness(data[i]);
            }
        }

        [Benchmark]
        public void ReverseEndianness_IReversibleEndianness()
        {
            for (int i = 0; i < n; i++)
            {
                reversibleData[i] = BitConverterX.ReverseEndianness(reversibleData[i]);
            }
        }

        [Benchmark]
        public void ReverseEndianness_Auto_All() => BitConverterX.ReverseEndianness(data.AsSpan());

        [Benchmark]
        public void ReverseEndianness_IReversibleEndianness_All() => BitConverterX.ReverseEndianness(reversibleData.AsSpan());

        public readonly record struct TestStruct(long Test1, int Test2, short Test3, byte Test4, byte Test5);

        public readonly record struct ReversibleTestStruct(long Test1, int Test2, short Test3, byte Test4, byte Test5) : IReversibleEndianness<ReversibleTestStruct>
        {
            public ReversibleTestStruct ReverseEndianness()
                => new ReversibleTestStruct(
                    BinaryPrimitives.ReverseEndianness(Test1),
                    BinaryPrimitives.ReverseEndianness(Test2),
                    BinaryPrimitives.ReverseEndianness(Test3),
                    Test4,
                    Test5
                    );
        }
    }
}
