using AuroraLib.Core.Exceptions;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Collections
{

    /// <summary>
    /// A memory-efficient list implementation that utilizes a pooled backing array to reduce allocations.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the list.</typeparam>
    public sealed class PoolList<T> : IList<T>, IList, IReadOnlyList<T>, IDisposable
    {
        private const int MinimumCapacity = 64;

        internal T[] _items;
        internal int _size;
        internal int _version;

        [NonSerialized]
        internal readonly ArrayPool<T> _pool;

#if NET6_0_OR_GREATER
        private static bool IsReferenceType => RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
        private static bool IsReferenceType => !typeof(T).IsValueType;
#endif

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                ThrowIf.GreaterThanOrEqual(index, _size);
                return _items[index];
            }
            set
            {
                ThrowIf.GreaterThanOrEqual(index, _size);
                _items[index] = value;
                _version++;
            }
        }

        /// <inheritdoc/>
        public int Count => _size;

        /// <summary>
        /// Gets the total number of elements that the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity => _items.Length;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolList{T}"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the list can hold before resizing. Must be greater than or equal to the minimum capacity.</param>
        [DebuggerStepThrough]
        public PoolList(int capacity = MinimumCapacity) : this(ArrayPool<T>.Shared, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolList{T}"/> class using a specified array pool and initial capacity.
        /// </summary>
        /// <param name="pool">The <see cref="ArrayPool{T}"/> to use for renting and returning arrays.</param>
        /// <param name="capacity">The initial number of elements that the list can hold before resizing. Must be greater than or equal to the minimum capacity.</param>
        [DebuggerStepThrough]
        public PoolList(ArrayPool<T> pool, int capacity = MinimumCapacity)
        {
            ThrowIf.Null(pool);
            ThrowIf.Negative(capacity);

            // Ensure capacity is at least the minimum size
            if (capacity < MinimumCapacity) capacity = MinimumCapacity;

            _pool = pool;
            _items = pool.Rent(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolList{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public PoolList(IEnumerable<T> collection) : this()
            => AddRange(collection);

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolList{T}"/> class that contains elements copied from the specified span.
        /// </summary>
        /// <param name="items">The span whose elements are copied to the new list.</param>
        public PoolList(ReadOnlySpan<T> items) : this(items.Length)
            => AddRange(items);

        /// <inheritdoc cref="PoolList{T}.PoolList(ReadOnlySpan{T})"/>
        public PoolList(T[] items) : this(items.AsSpan())
        { }

        #endregion

        /// <summary>
        /// Ensures that the internal array has a capacity of at least the specified minimum value. 
        /// </summary>
        /// <param name="minimum">The minimum capacity required.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="minimum"/> is less than the current size of the list.</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SetMinimumCapacity(int minimum)
        {
            ThrowIf.LessThan(minimum, _size);

            if (minimum > _items.Length)
            {
                T[] newItems = _pool.Rent(minimum);
                if (_size > 0)
                {
                    Array.Copy(_items, newItems, _size);
                }

                _pool.Return(_items);
                _items = newItems;
            }
        }
        /// <summary>
        /// Adds a range of elements from the specified <see cref="ReadOnlySpan{T}"/> to the end of the list.
        /// </summary>
        /// <param name="values">A read-only span containing the elements to be added.</param>
        public void AddRange(ReadOnlySpan<T> values)
        {
            if (!values.IsEmpty)
            {
                int newSize = _size + values.Length;
                if (newSize > _items.Length)
                    SetMinimumCapacity(newSize);

                values.CopyTo(_items.AsSpan(_size));
                _size = newSize;
                _version++;
            }
        }

        /// <inheritdoc cref="AddRange(ReadOnlySpan{T})"/>
        public void AddRange(T[] values) => AddRange(values.AsSpan());

        /// <summary>
        /// Inserts a range of elements at the specified index in the list.
        /// </summary>
        /// <param name="index">The zero-based index at which the elements should be inserted.</param>
        /// <param name="values">The collection of elements to insert into the list.</param>
        public void InsertRange(int index, ReadOnlySpan<T> values)
        {
            ThrowIf.GreaterThan((uint)index, (uint)_size);

            if (!values.IsEmpty)
            {
                int newSize = _size + values.Length;
                if (newSize > _items.Length)
                    SetMinimumCapacity(newSize);

                if (index < _size)
                    Array.Copy(_items, index, _items, index + values.Length, _size - index);

                values.CopyTo(_items.AsSpan(index));
                _size = newSize;
                _version++;
            }
        }

        /// <summary>
        /// Returns a <see cref="Span{T}"/> representing the underlying buffer up to the current size of the list.
        /// </summary>
        /// <returns>A span containing the elements of the list from the beginning to the current size.</returns>
        public Span<T> UnsafeAsSpan() => _items.AsSpan(0, _size);

        #region IList<T>
        /// <inheritdoc cref="List{T}.Add(T)"/>
        public void Add(T item)
        {
            if (_size == _items.Length)
                SetMinimumCapacity(_size + 1);

            _items[_size++] = item;
            _version++;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            if (IsReferenceType)
                Array.Clear(_items, 0, _size);

            _size = 0;
            _version++;
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            ThrowIf.GreaterThan((uint)index, (uint)_size);

            if (_size == _items.Length)
                SetMinimumCapacity(_size + 1);

            if (index < _size)
                Array.Copy(_items, index, _items, index + 1, _size - index);

            _items[index] = item;
            _size++;
            _version++;
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            ThrowIf.GreaterThanOrEqual(index, _size);
            _size--;
            if (index < _size)
                Array.Copy(_items, index + 1, _items, index, _size - index);

            if (IsReferenceType)
                _items[_size] = default!;

            _version++;
        }

        /// <inheritdoc/>
        public bool Contains(T item) => _size != 0 && IndexOf(item) >= 0;

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, _size);

        /// <inheritdoc/>
        public int IndexOf(T item) => Array.IndexOf(_items, item, 0, _size);
        #endregion

        #region Explizit IList & ICollection<T>

        object? IList.this[int index]
        {
            get => this[index];
            set
            {
                ThrowIf.Null(nameof(value));
                this[index] = (T)value!;
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => this;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        int IList.Add(object? item)
        {
            ThrowIf.Null(item);
            Add((T)item!);
            return Count - 1;
        }

        bool IList.Contains(object? item)
        {
            if (item is T itemT)
                return Contains(itemT);
            return false;
        }

        int IList.IndexOf(object? item)
        {
            if (item is T itemT)
                return IndexOf(itemT);
            return -1;
        }

        void IList.Insert(int index, object? item)
        {
            ThrowIf.Null(item);
            Insert(index, (T)item!);
        }

        void IList.Remove(object? value)
        {
            if (value is T valueT)
                Remove(valueT);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            ThrowIf.Null(array);
            Array.Copy(_items, 0, array!, arrayIndex, _size);
        }

        #endregion

        #region List compatibility

        /// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
        public void AddRange(IEnumerable<T> collection)
        {
            ThrowIf.Null(collection);

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count > 0)
                {
                    if (_items.Length - _size < count)
                        SetMinimumCapacity(checked(_size + count));

                    c.CopyTo(_items, _size);
                    _size += count;
                    _version++;
                }
            }
            else
            {
                foreach (T item in collection)
                    Add(item);
            }
        }

        /// <inheritdoc cref="List{T}.RemoveRange(int, int)"/>
        public void RemoveRange(int index, int count)
        {
            ThrowIf.Null(index);
            ThrowIf.Null(count);
            ThrowIf.GreaterThan(count, _size - index);

            if (count > 0)
            {
                _size -= count;
                if (index < _size)
                    Array.Copy(_items, index + count, _items, index, _size - index);

                if (IsReferenceType)
                    Array.Clear(_items, _size, count);

                _version++;
            }
        }

        /// <inheritdoc cref="List{T}.Reverse()"/>
        public void Reverse()
            => Reverse(0, Count);

        /// <inheritdoc cref="List{T}.Reverse(int, int)"/>
        public void Reverse(int index, int count)
        {
            ThrowIf.Negative(index);
            ThrowIf.Negative(count);
            ThrowIf.LessThan(_size - index, count);

            if (count > 1)
                Array.Reverse(_items, index, count);

            _version++;
        }

        /// <inheritdoc cref="List{T}.Sort()"/>
        public void Sort()
            => Sort(0, Count, null);

        /// <inheritdoc cref="List{T}.Sort(IComparer{T}?)"/>
        public void Sort(IComparer<T>? comparer)
            => Sort(0, Count, comparer);

        /// <inheritdoc cref="List{T}.Sort(int, int, IComparer{T}?)"/>
        public void Sort(int index, int count, IComparer<T>? comparer)
        {
            ThrowIf.Negative(index);
            ThrowIf.Negative(count);
            ThrowIf.LessThan(_size - index, count);

            if (count > 1)
            {
                Array.Sort(_items, index, count, comparer);
            }
            _version++;
        }

        /// <inheritdoc cref="List{T}.Sort(Comparison{T})"/>
        public void Sort(Comparison<T> comparison)
        {
            ThrowIf.Null(comparison);

#if NET6_0_OR_GREATER
            if (_size > 1)
                _items.AsSpan(0, _size).Sort(comparison);
#else

            if (_size > 1)
            {
                IComparer<T> comparer = new FunctorComparer(comparison);
                Array.Sort(_items, 0, _size, comparer);
            }
#endif

            _version++;
        }

#if !NET6_0_OR_GREATER
        private sealed class FunctorComparer : IComparer<T>
        {
            private Comparison<T> comparison;
            public FunctorComparer(Comparison<T> comparison) => this.comparison = comparison;
            public int Compare(T x, T y) => comparison(x, y);
        }
#endif

        /// <inheritdoc cref="List{T}.ToArray()"/>
        public T[] ToArray()
        {
            T[] array = new T[_size];
            Array.Copy(_items, array, _size);
            return array;
        }

        #endregion

        #region IEnumerable

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        private struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly PoolList<T> _list;
            private int _index;
            private readonly int _version;
            public readonly T Current => _list._items[_index];

            internal Enumerator(PoolList<T> list)
            {
                _list = list;
                _version = list._version;
                _index = -1;
            }

            public bool MoveNext()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Collection was modified during enumeration.");
                return unchecked(++_index) < _list.Count;
            }

            readonly object? IEnumerator.Current => Current;

            void IEnumerator.Reset() => _index = -1;

            readonly public void Dispose()
            { }
        }
        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            _version = _size = -1;
            _pool.Return(_items);
        }
    }
}
