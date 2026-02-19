# AuroraLib.Core

AuroraLib.Core offers high-performance and memory-efficient tools for binary data manipulation and stream handling.
It simplifies working across different .NET versions by providing extension methods that bridge functionality gaps, ensuring compatibility.
 
[![NuGet](https://img.shields.io/nuget/v/AuroraLib.Core.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/AuroraLib.Core)

## Features

### Stream Utilities

| Utility              | Description                                                                  |
|----------------------|------------------------------------------------------------------------------|
| `MemoryPoolStream`   | A memory-efficient `MemoryStream` using `ArrayPool` for reduced allocations. |
| `SubStream`          | Provides a view into a specific portion of an underlying `Stream`.           |
| `BitReader`          | Methods for reading bit streams with fine-grained control.                   |
| `BitWriter`          | Methods for writing bit streams efficiently.                                 |
| `PathX`              | Performs operations on spans that contain path information.                  |
| `StreamExtension`    | Extensions for `Stream`s to simplify reading, writing, and data manipulation.|
| `ZLibStream`         | Provides methods to compress & decompress zlib data for netstandard and older.|

### Buffer Management

| Utility             | Description                                                                  |
|---------------------|------------------------------------------------------------------------------|
| `SpanExtension`     | Extension methods and utilities for spans.                                   |
| `MemoryExtension`   | Casts a `Memory<T>` of one type to another.                                  |

### Collections

| Utility               | Description                                                                |
|-----------------------|----------------------------------------------------------------------------|
| `PoolList`            | A memory-efficient list implementation that utilizes a pooled.             |
| `ObservableDictionary`| A dictionary that raises notifications when items are changed.             |
| `CollectionExtension` | Extension methods for Collections.                                         |

### Value Types

| Type                | Description                                                                  |
|---------------------|------------------------------------------------------------------------------|
| `Int24`             | A 24-bit signed integer.                                                     |
| `UInt24`            | A 24-bit unsigned integer.                                                   |
| `Identifier32`      | A 4-byte identifier, used for file signatures (similar to `DWORD`).          |
| `Identifier64`      | An 8-byte identifier, used for extended file signatures.                     |

[Benchmarks](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmarks.md)
