# AuroraLib.Core

High performance zero allocation structure reader and other tools for big and little-endian.

[Nuget Package](https://www.nuget.org/packages/AuroraLib.Core)

## Common

| Utilities      | Description                                                                |
|----------------|----------------------------------------------------------------------------|
|SubStream       | Provides a view into a portion of an underlying Stream.                    |
|MemoryPoolStream| like `MemoryStream` but wrapper around `ArrayPool` for efficient allocation.|
|BitReader       | Offers many methods for reading bit streams.                               |
|BitWriter       | Offers many methods to write a bit stream.                                 |
|SpanBuffer      | A buffer that allocated from the `ArrayPool`.                              |
|CircularBuffer  | A CircularBuffer that allocated from the `ArrayPool`.                      |
|BackwardsMemoryStream| Reads and writes from back to front.                                  |
|ValueStringBuilder| A stack-based string-builder.                                            |
|StreamEx        | Stream extensions to read and write structures and Arrays.                 |
|BitConverterX   | Extension to the BitConverter such as swap and generic methods.            |

| Values Types   | Description                                                                |
|----------------|----------------------------------------------------------------------------|
|Int24           | 24-bit unsigned integer.                                                   |
|UInt24          | 24-bit signed integer.                                                     |
|Identifier32    | 4-byte array, for file signatures similar to dword in c++.                 |
|Identifier64    | 8-byte array, for file signatures.                                         |


## Benchmarks

[ReadStruct](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/ReadStruct.cs)
|                     Method |      Mean |     Error |    StdDev | Allocated |
|--------------------------- |----------:|----------:|----------:|----------:|
|          BinaryReader_Read | 52.570 us | 0.6446 us | 0.6030 us |     160 B |
|      BinaryPrimitives_Read | 17.222 us | 0.3287 us | 0.3785 us |         - |
|            AuroraCore_Read | 31.588 us | 0.6117 us | 0.7045 us |         - |
|           AuroraCore_ReadT | 16.855 us | 0.0501 us | 0.0391 us |         - |
| AuroraCore_ReadSpanBufferT |  2.179 us | 0.0426 us | 0.0437 us |         - |

[WriteStream](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/WriteStream.cs)
|                 Method |  MB |         Mean |        Error |     StdDev |      Gen0 |      Gen1 |      Gen2 |   Allocated |
|----------------------- |---- |-------------:|-------------:|-----------:|----------:|----------:|----------:|------------:|
|     MemoryStream_Write |   1 |     84.80 us |     1.399 us |   1.308 us |  208.8623 |  208.4961 |  208.4961 |   1048750 B |
| MemoryPoolStream_Write |   1 |     21.64 us |     0.421 us |   0.394 us |         - |         - |         - |        64 B |
|     MemoryStream_Write |  10 |  9,307.01 us |    62.965 us |  55.817 us | 1328.1250 | 1328.1250 | 1328.1250 |  32506476 B |
| MemoryPoolStream_Write |  10 |  1,780.05 us |    30.305 us |  41.482 us |         - |         - |         - |        65 B |
|     MemoryStream_Write | 100 | 69,133.05 us | 1,005.067 us | 940.140 us | 3875.0000 | 3875.0000 | 3875.0000 | 267388464 B |
| MemoryPoolStream_Write | 100 | 23,530.49 us |   437.096 us | 429.287 us |         - |         - |         - |           - |
