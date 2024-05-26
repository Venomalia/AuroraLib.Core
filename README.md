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
|          BinaryReader_Read | 76.708 us | 1.4607 us | 3.4715 us |     160 B |
|            AuroraCore_Read | 48.228 us | 0.3241 us | 0.2530 us |         - |
|      BinaryPrimitives_Read | 31.686 us | 0.6183 us | 1.6611 us |         - |
|           AuroraCore_ReadT | 30.527 us | 0.5519 us | 0.5421 us |         - |
| AuroraCore_ReadSpanBufferT |  5.885 us | 0.1163 us | 0.1088 us |         - |

[WriteStream](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/WriteStream.cs)
|                 Method |  MB |          Mean |        Error |       StdDev |      Gen0 |      Gen1 |      Gen2 |   Allocated |
|----------------------- |---- |--------------:|-------------:|-------------:|----------:|----------:|----------:|------------:|
|     MemoryStream_Write |   1 |     138.40 us |     2.734 us |     5.944 us |  215.5762 |  214.8438 |  214.8438 |   1048830 B |
| MemoryPoolStream_Write |   1 |      30.80 us |     0.139 us |     0.116 us |         - |         - |         - |        64 B |
|     MemoryStream_Write |  10 |  12,295.67 us |   163.334 us |   152.783 us | 1328.1250 | 1328.1250 | 1328.1250 |  32506479 B |
| MemoryPoolStream_Write |  10 |   2,370.85 us |    45.901 us |    49.114 us |         - |         - |         - |        67 B |
|     MemoryStream_Write | 100 | 106,403.11 us | 2,321.716 us | 6,845.633 us | 1400.0000 | 1400.0000 | 1400.0000 | 267387696 B |
| MemoryPoolStream_Write | 100 |  22,808.09 us |   449.554 us |   799.082 us |         - |         - |         - |        92 B |