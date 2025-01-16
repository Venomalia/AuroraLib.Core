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
| `BackwardsMemoryStream`| Enables reading and writing streams in reverse order.                      |
| `StreamEx`           | Extensions for `Stream`s to simplify reading, writing, and data manipulation.|

### Buffer Management

| Utility             | Description                                                                  |
|---------------------|------------------------------------------------------------------------------|
| `SpanBuffer`        | A buffer allocated from `ArrayPool` for zero-allocation data management.     |
| `CircularBuffer`    | A circular buffer allocated from `ArrayPool`.                                |
| `SpanEx`            | Extension methods and utilities for spans.                                   |
| `MemoryEx`          | Casts a `Memory<T>` of one type to another.                                  |
| `ValueStringBuilder`| A stack-based string builder for efficient string creation.                  |

### Format Handling

| Utility             | Description                                                                  |
|---------------------|------------------------------------------------------------------------------|
| `MediaType`         | Works with MIME Media types.                                                 |
| `FormatInfo`        | Provides information for specific file formats.                              |
| `FormatDictionary`  | A dictionary for looking up `FormatInfo` by MIME type or detecting stream content.|

### Value Types

| Type                | Description                                                                  |
|---------------------|------------------------------------------------------------------------------|
| `Int24`             | A 24-bit signed integer.                                                     |
| `UInt24`            | A 24-bit unsigned integer.                                                   |
| `Identifier32`      | A 4-byte identifier, used for file signatures (similar to `DWORD`).          |
| `Identifier64`      | An 8-byte identifier, used for extended file signatures.                     |

[Benchmarks](https://github.com/Venomalia/AuroraLib.Core/blob/main/Benchmarks.md)
