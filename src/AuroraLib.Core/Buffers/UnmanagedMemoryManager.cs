using System;
using System.Buffers;

namespace AuroraLib.Core.Buffers
{
    /// <summary>
    /// A custom <see cref="MemoryManager{T}"/> implementation that provides access to unmanaged memory using a pointer.
    /// </summary>
    /// <typeparam name="T">The unmanaged type that the memory holds.</typeparam>
    public sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
    {
        private readonly void* _pointer;
        private readonly int _length;
        private readonly Action<bool>? _dispose;

        /// <inheritdoc cref="UnmanagedMemoryManager{T}.UnmanagedMemoryManager(void*, int, Action{bool})"/>
        public UnmanagedMemoryManager(void* pointer, int length)
        {
            _pointer = pointer;
            _length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnmanagedMemoryManager{T}"/> class.
        /// </summary>
        /// <param name="pointer">A pointer to the unmanaged memory.</param>
        /// <param name="length">The length of the unmanaged memory in bytes.</param>
        /// <param name="dispose">An action that is invoked when the memory is disposed, passing a boolean indicating if the disposal should occur.</param>
        public UnmanagedMemoryManager(void* pointer, int length, Action<bool> dispose) : this(pointer, length)
            => _dispose = dispose;

        /// <inheritdoc/>
        public override Span<T> GetSpan() => new Span<T>(_pointer, _length);

        /// <inheritdoc/>
        public override MemoryHandle Pin(int elementIndex = 0)
            => new MemoryHandle(((T*)_pointer) + elementIndex, pinnable: this);

        /// <inheritdoc/>
        public override void Unpin() { }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
            => _dispose?.Invoke(disposing);
    }
}
