using AuroraLib.Core.Buffers;
using AuroraLib.Core.Collections;
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
        /// <param name="order">The endianness of the data in the stream. Default is <see cref="Endian.Little"/>.</param>
        /// <returns>The value <typeparamref name="T"/> that were read.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        [DebuggerStepThrough]
        public static unsafe T Read<T>(this Stream stream, Endian order = Endian.Little) where T : unmanaged
        {

            if (Unsafe.SizeOf<T>() <= 4)
                return stream.ReadPrimitiveHelper<T>(order);

            return stream.ReadGenericHelper<T>(order);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static T ReadGenericHelper<T>(this Stream stream, Endian order) where T : unmanaged
        {
            T value;
            Span<byte> buffer = new Span<byte>(&value, sizeof(T));

            if (stream.Read(buffer) != buffer.Length)
                ThrowHelper.EndOfStreamException<T>();

            if (order != SystemOrder)
            {
                if (sizeof(T) > 8)
                    BitConverterX.ReverseEndianness(new Span<T>(&value, 1));
                else
                    value = BitConverterX.ReverseEndianness(value);
            }

            return value;
        }

        private static T ReadPrimitiveHelper<T>(this Stream stream, Endian order) where T : unmanaged
        {
            Type typeT = typeof(T);
            switch (Unsafe.SizeOf<T>())
            {
                case 0:
                    return default;
                case 1:
                    byte bValue = (byte)ReadByteHelper<T>(stream);
                    return Unsafe.As<byte, T>(ref bValue);
                case 2:
#if NET5_0_OR_GREATER
                    if (order == SystemOrder || typeT == typeof(short) || typeT == typeof(ushort) || typeT == typeof(Half) || typeT.IsEnum)
#else
                    if (order == SystemOrder || typeT == typeof(short) || typeT == typeof(ushort) || typeT.IsEnum)
#endif
                    {
                        short iValue = (short)ReadInt16Helper<T>(stream, order);
                        return Unsafe.As<short, T>(ref iValue);
                    }
                    break;
                case 3:
                    if (order == SystemOrder || typeT == typeof(Int24) || typeT == typeof(UInt24))
                    {
                        Int24 iValue = ReadInt24Helper<T>(stream, order);
                        return Unsafe.As<Int24, T>(ref iValue);
                    }
                    break;
                default:
                    if (order == SystemOrder || typeT == typeof(int) || typeT == typeof(uint) || typeT == typeof(float) || typeT.IsEnum)
                    {
                        int iValue = ReadInt32Helper<T>(stream, order);
                        return Unsafe.As<int, T>(ref iValue);
                    }
                    break;
            }
            return stream.ReadGenericHelper<T>(order);
        }

        /// <summary>
        /// Reads a span of <typeparamref name="T"/> from the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the values in the span.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="values">The span of values to read into.</param>
        /// <param name="order">The endianness of the data in the stream. Default is <see cref="Endian.Little"/>.</param>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public unsafe static void Read<T>(this Stream stream, Span<T> values, Endian order = Endian.Little) where T : unmanaged
        {
            Span<byte> buffer = MemoryMarshal.Cast<T, byte>(values);

            if (stream.Read(buffer) != buffer.Length)
                ThrowHelper.EndOfStreamException<T>(values.Length);

            if (order != SystemOrder && sizeof(T) > 1)
                BitConverterX.ReverseEndianness(values);
        }
        #endregion

        #region Write
        /// <summary>
        /// Writes the specified Value of <typeparamref name="T"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        /// <param name="stream">The stream to write the value to.</param>
        /// <param name="value">The value to write to the stream.</param>
        /// <param name="order">The endianness of the data to write. Default is <see cref="Endian.Little"/>.</param>
        [DebuggerStepThrough]
#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public static unsafe void Write<T>(this Stream stream, T value, Endian order = Endian.Little) where T : unmanaged
        {
            if (order != SystemOrder && sizeof(T) > 1)
            {
                if (sizeof(T) > 8)
                    BitConverterX.ReverseEndianness(new Span<T>(&value, 1));
                else
                    value = BitConverterX.ReverseEndianness(value);
            }

            Span<byte> buffer = new Span<byte>(&value, sizeof(T));
            stream.Write(buffer);
        }

        /// <summary>
        /// Writes the data from the specified <see cref="ReadOnlySpan{T}"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the span.</typeparam>
        /// <param name="stream">The stream to write the data to.</param>
        /// <param name="span">The ReadOnlySpan containing the data to write.</param>
        /// <param name="order">The byte order of the data. Default is Endian.Little.</param>
        public static unsafe void Write<T>(this Stream stream, ReadOnlySpan<T> span, Endian order = Endian.Little) where T : unmanaged
        {
            if (order != SystemOrder && sizeof(T) > 1)
            {
                using (SpanBuffer<T> copy = new SpanBuffer<T>(span))
                {
                    Span<T> spanCopy = copy.Span;
                    BitConverterX.ReverseEndianness(spanCopy);
                    stream.Write(copy.GetBuffer(), 0, sizeof(T) * span.Length);
                }
            }
            else
            {
                ReadOnlySpan<byte> buffer = MemoryMarshal.Cast<T, byte>(span);
                stream.Write(buffer);
            }
        }

        /// <summary>
        /// Writes multiple instances of the specified <typeparamref name="T"/> <paramref name="objekt"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to write.</typeparam>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="objekt">The object to write.</param>
        /// <param name="count">The number of times to write the object.</param>
        /// <param name="order">The endianness of the data to write. Default is <see cref="Endian.Little"/>.</param>
        [DebuggerStepThrough]
#if !(NETSTANDARD || NET20_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public static unsafe void Write<T>(this Stream stream, T objekt, uint count, Endian order = Endian.Little) where T : unmanaged
        {
            if (order != SystemOrder && sizeof(T) > 1)
            {
                if (sizeof(T) > 8)
                    BitConverterX.ReverseEndianness(new Span<T>(&objekt, 1));
                else
                    objekt = BitConverterX.ReverseEndianness(objekt);
            }

            Span<byte> buffer = new Span<byte>(&objekt, sizeof(T));
            for (int i = 0; i < count; i++)
            {
                stream.Write(buffer);
            }
        }

        /// <inheritdoc cref="Write{T}(Stream, T, uint, Endian)"/>
        public static unsafe void Write<T>(this Stream stream, T objekt, int count, Endian order = Endian.Little) where T : unmanaged
            => Write(stream, objekt, (uint)count, order);

        #endregion

        #region Read&Write Collection
        /// <summary>
        /// Reads a collection of type <typeparamref name="T"/> from the provided <paramref name="stream"/> into the specified <paramref name="Collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="Collection">The collection to populate with the read data.</param>
        /// <param name="count">The number of elements to read from the stream.</param>
        /// <param name="order">The byte order (endianness) of the data. Defaults to LittleEndian.</param>
        public static void ReadCollection<T>(this Stream stream, ICollection<T> Collection, int count, Endian order = Endian.Little) where T : unmanaged
        {
            ThrowIf.Null(Collection, nameof(Collection));
            ThrowIf.ReadOnly(Collection.IsReadOnly, nameof(Collection));

#if !NET8_0_OR_GREATER
            if (Collection is List<T> list)
            {
                ReadList(stream, list, count, order);
                return;
            }
#endif
            using (SpanBuffer<T> copy = new SpanBuffer<T>(count))
            {
                stream.Read<T>(copy, order);
                Collection.AddRange(copy);
            }
        }

#if !NET8_0_OR_GREATER
        private static void ReadList<T>(Stream stream, List<T> list, int count, Endian order = Endian.Little) where T : unmanaged
        {
            ThrowIf.Negative(count, nameof(count));

            if (count * Unsafe.SizeOf<T>() <= 512)
            {
                T[] data = new T[count];
                stream.Read(data.AsSpan(), order);
                list.AddRange(data);
            }
            else
            {
                T[] copy = ArrayPool<T>.Shared.Rent(count);
                try
                {
                    Span<T> values = new Span<T>(copy, 0, count);
                    stream.Read(values, order);

                    list.AddRange(copy);
                    if (copy.Length > count)
                    {
                        int dif = copy.Length - count;
                        list.RemoveRange(list.Count - dif, dif);
                    }
                }
                finally
                {
                    ArrayPool<T>.Shared.Return(copy);
                }
            }
        }
#endif

        /// <summary>
        /// Writes a <paramref name="Collection"/> of type <typeparamref name="T"/> to the provided <paramref name="stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stream">The stream to write data to.</param>
        /// <param name="Collection">The collection containing the data to be written.</param>
        /// <param name="order">The byte order (endianness) to use when writing. Defaults to LittleEndian.</param>
        public static void WriteCollection<T>(this Stream stream, ICollection<T> Collection, Endian order = Endian.Little) where T : unmanaged
        {
            ThrowIf.Null(Collection, nameof(Collection));

            if (Collection is List<T> list)
            {
                Write(stream, (ReadOnlySpan<T>)list.UnsafeAsSpan(), order);
                return;
            }
            else if (Collection is PoolList<T> pool)
            {
                Write(stream, (ReadOnlySpan<T>)pool.UnsafeAsSpan(), order);
                return;
            }
            else if (Collection is T[] array)
            {
                Write(stream, (ReadOnlySpan<T>)array, order);
                return;
            }

            T[] copy = ArrayPool<T>.Shared.Rent(Collection.Count);
            try
            {
                Collection.CopyTo(copy, 0);
                Span<T> values = new Span<T>(copy, 0, Collection.Count);

                if (order != SystemOrder && Unsafe.SizeOf<T>() > 1)
                    BitConverterX.ReverseEndianness(values);

                Span<byte> buffer = MemoryMarshal.Cast<T, byte>(copy);
                stream.Write(buffer);
            }
            finally
            {
                ArrayPool<T>.Shared.Return(copy);
            }
        }
        #endregion

        #region Read&Write IBinaryObject
        /// <summary>
        /// Reads an object of type T that implements <see cref="IBinaryObject"/> from the stream.
        /// </summary>
        /// <typeparam name="T">The type of object to read.</typeparam>
        /// <param name="stream">The stream from which to read the object.</param>
        /// <returns>The deserialized object of type T.</returns>
        /// <inheritdoc cref="ThrowHelper.EndOfStreamException{T}()"/>
        public static T Read<T>(this Stream stream) where T : IBinaryObject, new()
        {
            T value = new T();
            value.BinaryDeserialize(stream);
            return value;
        }

        /// <summary>
        /// Writes an object that implements <see cref="IBinaryObject"/> to the stream.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="stream">The stream to which the object is written.</param>
        /// <param name="objekt">The object to be serialized and written to the stream.</param>
        public static void Write<T>(this Stream stream, T objekt) where T : IBinaryObject
            => objekt.BinarySerialize(stream);
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
        /// <param name="order">The endianness of the data in the stream. Default is <see cref="Endian.Little"/>.</param>
        /// <returns>The value of type T read from the stream.</returns>
        /// <exception cref="EndOfStreamException">Thrown when attempting to read <typeparamref name="T"/> beyond the end of the stream.</exception>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T Peek<T>(this Stream stream, Endian order = Endian.Little) where T : unmanaged
        {
            T value = stream.Read<T>(order);
            stream.Position -= sizeof(T);
            return value;
        }
        #endregion

    }
}
