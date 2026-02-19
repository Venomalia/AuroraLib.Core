using AuroraLib.Core.Collections;
using AuroraLib.Core.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.IO
{
    public static partial class StreamEx
    {
        /// <summary>
        /// Specifies the system's byte order as either <see cref="Endian.Little"/> or <see cref="Endian.Big"/>.
        /// </summary>
#if BIGENDIAN
        public const Endian SystemOrder = Endian.Big;
#else
        public const Endian SystemOrder = Endian.Little;
#endif

        #region Read
        /// <summary>
        /// Reads a unmanaged struct of <typeparamref name="T"/> from the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of element.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The value <typeparamref name="T"/> that were read.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T Read<T>(this Stream stream) where T : unmanaged
        {
            T value;
            Span<byte> buffer = new Span<byte>(&value, sizeof(T));

            if (stream.Read(buffer) != buffer.Length)
                ThrowHelper.EndOfStreamException<T>();

            return value;
        }

        /// <summary>
        /// Reads a span of <typeparamref name="T"/> from the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the values in the span.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="values">The span of values to read into.</param>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void Read<T>(this Stream stream, Span<T> values) where T : unmanaged
        {
            Span<byte> buffer = MemoryMarshal.Cast<T, byte>(values);

            if (stream.Read(buffer) != buffer.Length)
                ThrowHelper.EndOfStreamException<T>(values.Length);
        }
        #endregion

        #region Write
        /// <summary>
        /// Writes the specified Value of <typeparamref name="T"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        /// <param name="stream">The stream to write the value to.</param>
        /// <param name="value">The value to write to the stream.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write<T>(this Stream stream, T value) where T : unmanaged
        {
            Span<byte> buffer = new Span<byte>(&value, sizeof(T));
            stream.Write(buffer);
        }

        /// <summary>
        /// Writes the data from the specified <see cref="ReadOnlySpan{T}"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span.</typeparam>
        /// <param name="stream">The stream to write the data to.</param>
        /// <param name="span">The ReadOnlySpan containing the data to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write<T>(this Stream stream, ReadOnlySpan<T> span) where T : unmanaged
        {
            ReadOnlySpan<byte> buffer = MemoryMarshal.Cast<T, byte>(span);
            stream.Write(buffer);
        }

        /// <summary>
        /// Writes multiple instances of the specified <typeparamref name="T"/> <paramref name="objekt"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to write.</typeparam>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="objekt">The object to write.</param>
        /// <param name="count">The number of times to write the object.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write<T>(this Stream stream, T objekt, uint count) where T : unmanaged
        {
            Span<byte> buffer = new Span<byte>(&objekt, sizeof(T));
            for (int i = 0; i < count; i++)
            {
                stream.Write(buffer);
            }
        }

        /// <inheritdoc cref="Write{T}(Stream, T, uint)"/>
        public static unsafe void Write<T>(this Stream stream, T objekt, int count) where T : unmanaged
            => Write(stream, objekt, (uint)count);

        #endregion

        #region Read&Write Collection
        /// <summary>
        /// Reads a collection of type <typeparamref name="T"/> from the provided <paramref name="stream"/> into the specified <paramref name="Collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="Collection">The collection to populate with the read data.</param>
        /// <param name="count">The number of elements to read from the stream.</param>
        public static void ReadCollection<T>(this Stream stream, ICollection<T> Collection, int count) where T : unmanaged
        {
            ThrowIf.Null(Collection, nameof(Collection));
            ThrowIf.Negative(count, nameof(count));
            ThrowIf.ReadOnly(Collection.IsReadOnly, nameof(Collection));

            int sizeInb = count * Unsafe.SizeOf<T>();
#if !NET8_0_OR_GREATER
            if (sizeInb < 0x400)
            {
                Span<T> data = stackalloc T[count];
                stream.Read(data);
                Collection.AddRange(data);
                return;
            }
#endif
            byte[] buffer = ArrayPool<byte>.Shared.Rent(sizeInb);
            try
            {
                stream.ReadExactly(buffer, 0, sizeInb);
                Span<T> bSpan = MemoryMarshal.Cast<byte, T>(buffer.AsSpan());
                Collection.AddRange(bSpan);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        /// <summary>
        /// Writes a <paramref name="Collection"/> of type <typeparamref name="T"/> to the provided <paramref name="stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stream">The stream to write data to.</param>
        /// <param name="Collection">The collection containing the data to be written.</param>
        public static void WriteCollection<T>(this Stream stream, ICollection<T> Collection) where T : unmanaged
        {
            ThrowIf.Null(Collection, nameof(Collection));

            if (Collection is List<T> list)
            {
                Write(stream, (ReadOnlySpan<T>)list.UnsafeAsSpan());
                return;
            }
            else if (Collection is PoolList<T> pool)
            {
                Write(stream, (ReadOnlySpan<T>)pool.UnsafeAsSpan());
                return;
            }
            else if (Collection is T[] array)
            {
                Write(stream, (ReadOnlySpan<T>)array);
                return;
            }

            T[] copy = ArrayPool<T>.Shared.Rent(Collection.Count);
            try
            {
                Collection.CopyTo(copy, 0);
                Span<T> values = new Span<T>(copy, 0, Collection.Count);

                Span<byte> buffer = MemoryMarshal.Cast<T, byte>(copy.AsSpan());
                stream.Write(buffer);
            }
            finally
            {
                ArrayPool<T>.Shared.Return(copy);
            }
        }
        #endregion

        #region For
        /// <summary>
        /// Invokes <paramref name="func"/> of <typeparamref name="T"/> for <paramref name="count"/> times within this <typeparamref name="S"/>/>.
        /// </summary>
        /// <typeparam name="T">The value returned by <paramref name="func"/>.</typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="stream"></param>
        /// <param name="count">How many times the <paramref name="func"/> should be Invoke</param>
        /// <param name="func">a function to be called <paramref name="count"/> times x</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T[] For<T, S>(this S stream, uint count, Func<S, T> func) where S : Stream
        {
            T[] values = new T[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = func(stream);
            }
            return values;
        }

        /// <inheritdoc cref="For{T, S}(S, uint, Func{S, T})"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] For<T, S>(this S stream, int count, Func<S, T> func) where S : Stream
            => stream.For((uint)count, func);
        #endregion

        #region At
        /// <summary>
        /// Invokes <paramref name="func"/> at the given <paramref name="offset"/> and <paramref name="origin"/> within the <typeparamref name="S"/>, retains the current position within the <typeparamref name="S"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="stream"></param>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <param name="func">a function to be Invoke at the desired position</param>
        /// <returns>The value <typeparamref name="T"/> returned by <paramref name="func"/>.</returns>
        /// <exception cref="IOException">An I/O error occurred, or Another thread may have caused an unexpected change in the position of the operating system's file handle</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed</exception>
        /// <exception cref="NotSupportedException">The current stream instance does not support writing</exception>
        [DebuggerStepThrough]
        public static T At<T, S>(this S stream, long offset, SeekOrigin origin, Func<S, T> func) where S : Stream
        {
            long orpos = stream.Position;
            stream.Seek(offset, origin);
            T value = func(stream);
            stream.Seek(orpos, SeekOrigin.Begin);
            return value;
        }

        /// <summary>
        /// Invokes <paramref name="func"/> at the given <paramref name="offset"/> and <paramref name="origin"/> within the <typeparamref name="S"/>, retains the current position within the <typeparamref name="S"/>.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="stream"></param>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <param name="func">a function to be Invoke at the desired position</param>
        /// <exception cref="IOException">An I/O error occurred, or Another thread may have caused an unexpected change in the position of the operating system's file handle</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed</exception>
        /// <exception cref="NotSupportedException">The current stream instance does not support writing</exception>
        [DebuggerStepThrough]
        public static void At<S>(this S stream, long offset, SeekOrigin origin, Action<S> func) where S : Stream
        {
            long orpos = stream.Position;
            stream.Seek(offset, origin);
            func(stream);
            stream.Seek(orpos, SeekOrigin.Begin);
        }

        /// <summary>
        /// Invokes <paramref name="func"/> at the given <paramref name="position"/> within the <typeparamref name="S"/>, retains the current position within the <typeparamref name="S"/>.
        /// </summary>
        /// <typeparam name="T">The value returned by <paramref name="func"/>.</typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="stream"></param>
        /// <param name="position">the position within the current</param>
        /// <param name="func">a function to be Invoke at the desired position</param>
        /// <returns>The value <typeparamref name="T"/> returned by <paramref name="func"/>.</returns>
        /// <exception cref="IOException">An I/O error occurred, or Another thread may have caused an unexpected change in the position of the operating system's file handle</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed</exception>
        /// <exception cref="NotSupportedException">The current stream instance does not support writing</exception>
        [DebuggerStepThrough]
        public static T At<T, S>(this S stream, long position, Func<S, T> func) where S : Stream
            => stream.At(position, SeekOrigin.Begin, func);

        /// <summary>
        /// Invokes <paramref name="func"/> at the given <paramref name="position"/> within the <typeparamref name="S"/>, retains the current position within the <typeparamref name="S"/>.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="stream"></param>
        /// <param name="position">the position within the current</param>
        /// <param name="func">a function to be Invoke at the desired position</param>
        /// <exception cref="IOException">An I/O error occurred, or Another thread may have caused an unexpected change in the position of the operating system's file handle</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed</exception>
        /// <exception cref="NotSupportedException">The current stream instance does not support writing</exception>
        [DebuggerStepThrough]
        public static void At<S>(this S stream, long position, Action<S> func) where S : Stream
            => stream.At(position, SeekOrigin.Begin, func);
        #endregion

        #region Peek
        /// <summary>
        /// Returns the result of the given function on the specified <paramref name="stream"/> without changing the stream position.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the function.</typeparam>
        /// <typeparam name="S">The type of the stream.</typeparam>
        /// <param name="stream">The stream to peek at.</param>
        /// <param name="func">The function to apply to the stream.</param>
        /// <returns>The result of the given function.</returns>
        [DebuggerStepThrough]
        public static T Peek<T, S>(this S stream, Func<S, T> func) where S : Stream
        {
            long orpos = stream.Position;
            T value = func(stream);
            stream.Seek(orpos, SeekOrigin.Begin);
            return value;
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> from the given <paramref name="stream"/> without advancing its position.
        /// </summary>
        /// <typeparam name="T">The type of the value to read.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The value of type T read from the stream.</returns>
        /// <exception cref="EndOfStreamException">Thrown when attempting to read <typeparamref name="T"/> beyond the end of the stream.</exception>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T Peek<T>(this Stream stream) where T : unmanaged
        {
            T value = stream.Read<T>();
            stream.Position -= sizeof(T);
            return value;
        }
        #endregion

    }
}
