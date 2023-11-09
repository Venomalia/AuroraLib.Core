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
|ValueStringBuilder| Stream extensions to read and write various structures.                  |
|StreamEx        | Stream extensions to read and write various structures.                    |
|BitConverterX   | Extension to the BitConverter.                                             |

| Values Types   | Description                                                                |
|----------------|----------------------------------------------------------------------------|
|Int24           | 24-bit unsigned integer.                                                   |
|UInt24          | 24-bit signed integer.                                                     |
|UInt128         | 128-bit unsigned integer.                                                  |
|Identifier32    | 4-byte array, for file signatures similar to dword in c++.                 |
|Identifier64    | 8-byte array, for file signatures.                                         |

| Hash           | Description                                                                |
|----------------|----------------------------------------------------------------------------|
|Adler32         | 32-bit Adler implementation.                                               |
|Adler64         | 64-bit Adler implementation.                                               |
|CityHash32      | [google](https://github.com/google/cityhash) 32-bit CityHash implementation.                                     |
|CityHash64      | [google](https://github.com/google/cityhash) 64-bit CityHash implementation.                                     |
|CityHash128     | [google](https://github.com/google/cityhash) 128-bit CityHash implementation.                                    |
|Crc32           | Fully customizable Crc32 algorithm                                         |
|Fnv1 32         | 32-bit Fnv1 or Fnv1a implementation.                                       |
|Fnv1 64         | 64-bit Fnv1 or Fnv1a implementation.                                       |
|MurmurHash3 32  | 32-bit MurmurHash3 implementation from [HashDepot](https://github.com/ssg/HashDepot).                          |
|MurmurHash3 128 | 128-bit MurmurHash3 implementation from [HashDepot](https://github.com/ssg/HashDepot).                         |
|XXHash32        | 32-bit XXHash implementation from [HashDepot](https://github.com/ssg/HashDepot).                               |
|XXHash64        | 64-bit XXHash implementation from [HashDepot](https://github.com/ssg/HashDepot).                               |

## Benchmarks

[ReadStruct](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/ReadStruct.cs)

|                    Method |        Mean |     Error |    StdDev |      Median |   Gen0 | Allocated |
|---------------------------|------------:|----------:|----------:|------------:|-------:|----------:|
|         BinaryReader_Read | 16,051.4 ns | 212.14 ns | 165.63 ns | 16,022.4 ns | 0.0305 |     160 B |
|     BinaryPrimitives_Read |  8,804.4 ns |  55.60 ns |  46.43 ns |  8,812.8 ns |      - |         - |
|           AuroraCore_Read |  9,359.9 ns |  49.50 ns |  43.88 ns |  9,362.7 ns |      - |         - |
|      AuroraCore_ReadArray |    945.5 ns |  18.72 ns |  48.00 ns |    928.9 ns | 3.8166 |   16024 B |
|  AuroraCore_ReadArrayPool |    404.7 ns |   7.91 ns |   8.47 ns |    400.1 ns |      - |         - |
| AuroraCore_ReadSpanBuffer |    405.3 ns |   8.09 ns |  10.52 ns |    401.1 ns |      - |         - |

[WriteStream](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/WriteStream.cs)

|                 Method |      N |         Mean |      Error |     StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|------------------------|------- |-------------:|-----------:|-----------:|---------:|---------:|---------:|----------:|
|     MemoryStream_Write |    100 |     1.105 us |  0.0216 us |  0.0191 us |   0.4597 |        - |        - |    1928 B |
| MemoryPoolStream_Write |    100 |     1.051 us |  0.0106 us |  0.0088 us |   0.0153 |        - |        - |      64 B |
|     MemoryStream_Write |  10000 |   160.457 us |  3.0166 us |  3.9225 us |  41.5039 |  41.5039 |  41.5039 |  262206 B |
| MemoryPoolStream_Write |  10000 |    99.370 us |  1.9834 us |  2.0368 us |        - |        - |        - |      64 B |
|     MemoryStream_Write | 100000 | 1,774.142 us | 18.0700 us | 15.0893 us | 498.0469 | 498.0469 | 498.0469 | 2097441 B |
| MemoryPoolStream_Write | 100000 |   952.500 us |  6.1853 us |  5.1650 us |        - |        - |        - |      66 B |