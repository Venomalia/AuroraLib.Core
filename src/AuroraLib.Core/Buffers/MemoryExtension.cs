using System;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Buffers
{
    public static class MemoryExtension
    {
        #region Cast
        /// <summary>
        /// Casts a Memory of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/> without copying its underlying data.
        /// </summary>
        /// <typeparam name="TFrom">The source element type.</typeparam>
        /// <typeparam name="TTo">The target element type.</typeparam>
        /// <param name="memory">The memory instance to cast.</param>
        /// <returns>A new <see cref="Memory{TTo}"/> instance that represents the same underlying data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<TTo> Cast<TFrom, TTo>(this Memory<TFrom> memory) where TFrom : struct where TTo : struct
        {
            MemoryHelper helper = Unsafe.As<Memory<TFrom>, MemoryHelper>(ref memory);
            // If the underlying object implements IMemoryCast, perform a memory cast using the CastMemory method
            if (helper.Object is IMemoryCast castMemory)
                return castMemory.CastMemory<TTo>();

            // If the underlying object is a TFrom[] array and the sizes of TFrom and TTo match, perform a direct cast
            if (helper.Object is TFrom[] && Unsafe.SizeOf<TFrom>() == Unsafe.SizeOf<TTo>())
                return Unsafe.As<Memory<TFrom>, Memory<TTo>>(ref memory);

            // Use MemoryCastManager for dynamic memory conversion
            return new MemoryCastManager<TFrom, TTo>(memory).Memory;
        }

        /// <summary>
        /// Casts a ReadOnlyMemory of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/> without copying its underlying data.
        /// </summary>
        /// <typeparam name="TFrom">The source element type.</typeparam>
        /// <typeparam name="TTo">The target element type.</typeparam>
        /// <param name="memory">The memory instance to cast.</param>
        /// <returns>A new <see cref="ReadOnlyMemory{TTo}"/> instance that represents the same underlying data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<TTo> Cast<TFrom, TTo>(this ReadOnlyMemory<TFrom> memory) where TFrom : struct where TTo : struct
        {
            Memory<TFrom> mem = Unsafe.As<ReadOnlyMemory<TFrom>, Memory<TFrom>>(ref memory);
            return mem.Cast<TFrom, TTo>();
        }

        private readonly struct MemoryHelper
        {
            public readonly object? Object;
            public readonly int Index;
            public readonly int Length;
        }
        #endregion
    }
}
