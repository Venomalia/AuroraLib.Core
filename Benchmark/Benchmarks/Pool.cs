﻿using AuroraLib.Core.Buffers;
using BenchmarkDotNet.Attributes;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class Pool
    {
        [Params(10, 100, 1000)]
        public int N;


        [Benchmark]
        public void Array()
        {
            byte[] bytedata = new byte[N];
            int[] intdata = new int[N];
            long[] longdata = new long[N];
        }

        [Benchmark]
        public void ArrayPool_Rent()
        {
            byte[] bytedata = ArrayPool<byte>.Shared.Rent(N);
            int[] intdata = ArrayPool<int>.Shared.Rent(N);
            long[] longdata = ArrayPool<long>.Shared.Rent(N);
            ArrayPool<byte>.Shared.Return(bytedata);
            ArrayPool<int>.Shared.Return(intdata);
            ArrayPool<long>.Shared.Return(longdata);
        }

        [Benchmark]
        public void Using_SpanBuffer()
        {
            using SpanBuffer<byte> bytedata = new(N);
            using SpanBuffer<int> intdata = new(N);
            using SpanBuffer<long> longdata = new(N);
        }
    }
}
