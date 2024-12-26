using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace AuroraLib.Core.Buffers
{
    /// <summary>
    /// Used in <see cref="MemoryCastManager{TFrom, TTo}"/> to support recasting of the internal <see cref="Memory{TFrom}"/>.
    /// </summary>
    internal interface IMemoryCast
    {
        Memory<Tnew> CastMemory<Tnew>() where Tnew : struct;
    }

    /// <summary>
    /// A custom <see cref="MemoryManager{TTo}"/> implementation that enables casting from <see cref="Memory{TFrom}"/> to <see cref="Memory{TTo}"/>.
    /// </summary>
    /// <typeparam name="TFrom">The source element type.</typeparam>
    /// <typeparam name="TTo">The target element type.</typeparam>
    internal sealed class MemoryCastManager<TFrom, TTo> : MemoryManager<TTo>, IMemoryCast where TFrom : struct where TTo : struct
    {
        private readonly Memory<TFrom> _from;

        public MemoryCastManager(Memory<TFrom> from)
        {
            _from = from;
        }

        public override Span<TTo> GetSpan() => MemoryMarshal.Cast<TFrom, TTo>(_from.Span);

        protected override void Dispose(bool disposing) { }

        public override MemoryHandle Pin(int elementIndex = 0)
            => _from.Pin();

        public override void Unpin()
        { }

        public Memory<Tnew> CastMemory<Tnew>() where Tnew : struct
            => new MemoryCastManager<TFrom, Tnew>(_from).Memory;
    }
}
