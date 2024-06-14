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
|          BinaryReader_Read | 55.620 us | 0.2378 us | 0.2225 us |     160 B |
|      BinaryPrimitives_Read | 17.876 us | 0.2764 us | 0.2586 us |         - |
|            AuroraCore_Read | 33.744 us | 0.4052 us | 0.3790 us |         - |
|           AuroraCore_ReadT | 18.449 us | 0.0933 us | 0.0872 us |         - |
| AuroraCore_ReadSpanBufferT |  2.138 us | 0.0342 us | 0.0320 us |         - |

[WriteStream](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmark/Benchmarks/WriteStream.cs)
|                 Method |  MB |         Mean |      Error |     StdDev |      Gen0 |      Gen1 |      Gen2 |   Allocated |
|----------------------- |---- |-------------:|-----------:|-----------:|----------:|----------:|----------:|------------:|
|     MemoryStream_Write |   1 |    124.16 us |   2.234 us |   1.865 us |  199.4629 |  199.2188 |  199.2188 |   1049128 B |
| MemoryPoolStream_Write |   1 |     21.24 us |   0.257 us |   0.240 us |         - |         - |         - |        64 B |
|     MemoryStream_Write |  10 | 11,166.33 us | 206.271 us | 192.946 us | 1328.1250 | 1328.1250 | 1328.1250 |  32506479 B |
| MemoryPoolStream_Write |  10 |  1,586.54 us |  25.563 us |  22.661 us |         - |         - |         - |        66 B |
|     MemoryStream_Write | 100 | 83,138.23 us | 460.102 us | 384.206 us | 1166.6667 | 1166.6667 | 1166.6667 | 267387709 B |
| MemoryPoolStream_Write | 100 | 21,697.72 us | 188.423 us | 157.341 us |         - |         - |         - |        92 B |
