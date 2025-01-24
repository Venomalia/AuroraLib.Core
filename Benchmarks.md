# Benchmarks
BenchmarkDotNet=v0.13.5, OS=Windows 10, AMD Ryzen 7 3800X, 1 CPU, 16 logical and 8 physical cores, .NET SDK=8.0.400

## StreamReadValueBenchmark
[Benchmark](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/StreamReadValueBenchmark.cs)
|                             Method |      Mean |    Error |   StdDev |
|----------------------------------- |----------:|---------:|---------:|
|             BinaryReader_ReadInt32 | 363.79 ns | 4.246 ns | 3.972 ns |
|         BinaryPrimitives_ReadInt32 | 294.76 ns | 2.857 ns | 2.673 ns |
|               AuroraCore_ReadInt32 | 230.71 ns | 0.945 ns | 0.789 ns |
|         BinaryPrimitives_ReadInt64 | 297.29 ns | 3.327 ns | 3.112 ns |
|               AuroraCore_ReadInt64 | 259.43 ns | 2.898 ns | 2.711 ns |
|     AuroraCore_ReadInt64_EndianBig | 273.26 ns | 2.411 ns | 2.137 ns |
|           AuroraCore_ReadSpanInt64 |  39.70 ns | 0.374 ns | 0.313 ns |
| AuroraCore_ReadSpanInt64_EndianBig |  51.52 ns | 0.530 ns | 0.496 ns |

## StreamReadTestStructBenchmark
[Benchmark](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/StreamReadTestStructBenchmark.cs)
|                                  Method |        Mean |     Error |   StdDev |
|---------------------------------------- |------------:|----------:|---------:|
|             BinaryReader_ReadTestStruct | 1,358.82 ns | 10.388 ns | 8.675 ns |
|         BinaryPrimitives_ReadTestStruct |   299.86 ns |  2.874 ns | 2.689 ns |
|               AuroraCore_ReadTestStruct |   269.12 ns |  2.924 ns | 2.735 ns |
|     AuroraCore_ReadTestStruct_EndianBig |   347.69 ns |  1.134 ns | 1.061 ns |
|           AuroraCore_ReadSpanTestStruct |    43.20 ns |  0.150 ns | 0.140 ns |
| AuroraCore_ReadSpanTestStruct_EndianBig |    84.12 ns |  1.396 ns | 1.306 ns |

## MemoryPoolStreamWriteBenchmark
[Benchmark](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/MemoryPoolStreamWriteBenchmark.cs)
|                       Method | MB |         Mean |        Error |       StdDev |      Gen0 |      Gen1 |      Gen2 |  Allocated |
|----------------------------- |--- |-------------:|-------------:|-------------:|----------:|----------:|----------:|-----------:|
|       MemoryStream_WriteByte |  1 |  3,159.85 us |    58.917 us |    57.864 us |  496.0938 |  496.0938 |  496.0938 |  2097439 B |
|   MemoryPoolStream_WriteByte |  1 |  1,849.36 us |   101.021 us |   297.862 us |         - |         - |         - |       67 B |
|       MemoryStream_WriteByte | 10 | 33,649.06 us |   630.892 us |   590.136 us | 1437.5000 | 1437.5000 | 1437.5000 | 33555126 B |
|   MemoryPoolStream_WriteByte | 10 | 20,403.18 us | 1,092.003 us | 3,219.796 us |         - |         - |         - |       67 B |
|     MemoryStream_WriteBuffer |  1 |    871.74 us |     5.811 us |     4.853 us |  999.0234 |  999.0234 |  999.0234 |  3251104 B |
| MemoryPoolStream_WriteBuffer |  1 |     38.59 us |     0.577 us |     0.540 us |         - |         - |         - |       64 B |
|     MemoryStream_WriteBuffer | 10 |  6,957.03 us |   106.136 us |   113.565 us | 1492.1875 | 1492.1875 | 1492.1875 | 26739300 B |
| MemoryPoolStream_WriteBuffer | 10 |  1,412.33 us |    27.551 us |    32.797 us |         - |         - |         - |       65 B |

## PoolBufferBenchmark
[Benchmark](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/PoolBufferBenchmark.cs)
|           Method |    N |      Mean |    Error |   StdDev |   Gen0 | Allocated |
|----------------- |----- |----------:|---------:|---------:|-------:|----------:|
|            Array |   10 |  11.56 ns | 0.254 ns | 0.237 ns | 0.0249 |     208 B |
|   ArrayPool_Rent |   10 |  56.60 ns | 0.888 ns | 0.831 ns |      - |         - |
| Using_SpanBuffer |   10 |  15.29 ns | 0.314 ns | 0.322 ns | 0.0249 |     208 B |
|            Array |  100 |  35.41 ns | 0.715 ns | 0.765 ns | 0.1645 |    1376 B |
|   ArrayPool_Rent |  100 |  56.93 ns | 0.123 ns | 0.109 ns |      - |         - |
| Using_SpanBuffer |  100 |  41.40 ns | 0.744 ns | 0.621 ns | 0.0660 |     552 B |
|            Array | 1000 | 283.65 ns | 2.875 ns | 6.605 ns | 1.5635 |   13072 B |
|   ArrayPool_Rent | 1000 |  56.60 ns | 0.563 ns | 0.499 ns |      - |         - |
| Using_SpanBuffer | 1000 |  60.47 ns | 0.881 ns | 0.824 ns |      - |         - |
