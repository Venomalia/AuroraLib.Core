using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.IO
{
    public static partial class StreamEx
    {
        private static class ThrowHelper
        {
            /// <exception cref="System.IO.EndOfStreamException">Thrown when attempting to read beyond the end of the stream.</exception>
            /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
            /// <exception cref="IOException">An I/O error occurred.</exception>
            /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            [DoesNotReturn]
#endif
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void EndOfStreamException<T>()
                => throw new EndOfStreamException($"Cannot read a {typeof(T)}, is beyond the end of the stream.");

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            [DoesNotReturn]
#endif
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void EndOfStreamException<T>(int count)
                => throw new EndOfStreamException($"Cannot read {typeof(T)}[{count}] is beyond the end of the stream.");
        }
    }
}
