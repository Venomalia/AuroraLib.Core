# AuroraLib.Core

High performance zero allocation structure reader and other tools for big and little-endian.


## Common Utilities

|                | Description                                                                |
|----------------|----------------------------------------------------------------------------|
|Int24           | 24-bit unsigned integer.                                                   |
|UInt24          | 24-bit signed integer.                                                     |
|UInt128         | 128-bit unsigned integer.                                                  |
|Identifier32    | 4-byte array, for file signatures                                          |
|Identifier64    | 8-byte array, for file signatures                                          |
|SubStream       | Provides a view into a portion of an underlying Stream                     |
|MemoryPoolStream| like `MemoryStream` but wrapper around `ArrayPool` for efficient allocation|
|SpanBuffer      | A Safe buffer that allocated from the `ArrayPool`                          |
|StreamEx        | Stream extensions                                                          |
|Crc32           | Fully customizable Crc32 algorithm                                         |

## Benchmark
[ReadStruct](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/ReadStruct.cs)

|                        Method |        Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------------------:|------------:|----------:|----------:|-------:|----------:|
|              BinaryReaderRead | 15,679.6 ns | 124.60 ns | 104.05 ns | 0.0305 |     160 B |
|          BinaryPrimitivesRead |  8,705.4 ns |  46.75 ns |  43.73 ns |      - |         - |
|          AuroraCoreReadStruct |  9,262.2 ns |  50.73 ns |  44.97 ns |      - |         - |
|      AuroraCoreReadArrayStruct|    984.6 ns |  19.13 ns |  42.78 ns | 3.8166 |   16024 B |
|  AuroraCoreReadArrayPoolStruct|    419.3 ns |   8.36 ns |   8.59 ns |      - |         - |
| AuroraCoreReadSpanBufferStruct|    429.1 ns |   8.41 ns |   9.35 ns |      - |         - |