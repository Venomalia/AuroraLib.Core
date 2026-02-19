using AuroraLib.Core.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace AuroraLib.Core.Collections
{
    /// <summary>
    /// Represents a dictionary that raises notifications when items are added, removed or changed.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged where TKey : notnull
    {
        readonly Dictionary<TKey, TValue> _dictionary;

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public virtual TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                if (_dictionary.TryGetValue(key, out TValue oldValue))
                {
                    _dictionary[key] = value;
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, oldValue)));

                    if (!(PropertyChanged is null))
                    {
                        PropertyChanged(this, _PropertyChangedEventArgs_Keys);
                        PropertyChanged(this, _PropertyChangedEventArgs_Values);
                    }
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys => _dictionary.Keys;

        /// <inheritdoc/>
        public ICollection<TValue> Values => _dictionary.Values;

        /// <inheritdoc/>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class that is empty and uses the default equality comparer for the key type.
        /// </summary>
        public ObservableDictionary() => _dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class that is empty and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys, or <c>null</c> to use the default equality comparer for the type of the key.</param>
        public ObservableDictionary(IEqualityComparer<TKey>? comparer) => _dictionary = new Dictionary<TKey, TValue>(comparer);

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class that is empty, has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the dictionary can contain.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys, or <c>null</c> to use the default equality comparer for the type of the key.</param>
        public ObservableDictionary(int capacity, IEqualityComparer<TKey>? comparer = null) => _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);


#if NET6_0_OR_GREATER
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class with elements from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the dictionary.</param>
        /// <param name="comparer">The equality comparer to use when comparing keys.</param>
        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer = null)
            => _dictionary = new Dictionary<TKey, TValue>(collection, comparer);
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class with elements from the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary whose elements are copied to the dictionary.</param>
        /// <param name="comparer">The equality comparer to use when comparing keys.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer = null)
        {
            if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
            _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }
#endif

        /// <inheritdoc/>
        public virtual void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
            DictionaryPropertyChanged();
        }

        /// <inheritdoc/>
        public virtual void Clear()
        {
            _dictionary.Clear();
            CollectionChanged?.Invoke(this, _NotifyCollectionChangedEventArgs_Reset);
            DictionaryPropertyChanged();
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        /// <inheritdoc/>
        public virtual bool Remove(TKey key)
        {
            if (_dictionary.Remove(key, out TValue value))
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value)));
                DictionaryPropertyChanged();
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
#if NET6_0_OR_GREATER
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dictionary.TryGetValue(key, out value);
#else
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
#endif

#if NET6_0_OR_GREATER
        /// <inheritdoc cref="Dictionary{TKey, TValue}.EnsureCapacity(int)"/>
        public int EnsureCapacity(int capacity) => _dictionary.EnsureCapacity(capacity);

        /// <inheritdoc cref="Dictionary{TKey, TValue}.TrimExcess()"/>
        public void TrimExcess() => _dictionary.TrimExcess();
#endif

        #region IEnumerator
        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => _dictionary.GetEnumerator();
        #endregion

        #region ICollection & IDictionary

        bool IDictionary.IsReadOnly => false;

        bool IDictionary.IsFixedSize => false;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        ICollection IDictionary.Keys => _dictionary.Keys;

        ICollection IDictionary.Values => _dictionary.Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

        object? IDictionary.this[object key]
        {
            get => key is TKey tKey ? _dictionary[tKey] : (object?)null;
            set
            {
                ThrowIf.Null(key, nameof(key));
                ThrowIf.Null(value, nameof(value));

                this[(TKey)key] = (TValue)value!;
            }
        }

        bool IDictionary.Contains(object key) => key is TKey tKey && _dictionary.ContainsKey(tKey);

        void IDictionary.Add(object key, object? value)
        {
            ThrowIf.Null(key, nameof(key));
            ThrowIf.Null(value, nameof(value));
            Add((TKey)key, (TValue)value!);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);

        void IDictionary.Remove(object key)
        {
            if (key is TKey tKey)
                Remove(tKey);
        }

        void ICollection.CopyTo(Array array, int index)
            => ((ICollection)_dictionary).CopyTo(array, index);

        object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)_dictionary).IsSynchronized;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item))
                return _dictionary.Remove(item.Key);

            return false;
        }

        #endregion

        private void DictionaryPropertyChanged()
        {
            if (!(PropertyChanged is null))
            {
                PropertyChanged(this, _PropertyChangedEventArgs_Count);
                PropertyChanged(this, _PropertyChangedEventArgs_Keys);
                PropertyChanged(this, _PropertyChangedEventArgs_Values);
            }
        }

        private static readonly PropertyChangedEventArgs _PropertyChangedEventArgs_Count = new PropertyChangedEventArgs(nameof(Count));
        private static readonly PropertyChangedEventArgs _PropertyChangedEventArgs_Keys = new PropertyChangedEventArgs(nameof(Keys));
        private static readonly PropertyChangedEventArgs _PropertyChangedEventArgs_Values = new PropertyChangedEventArgs(nameof(Values));
        private static readonly NotifyCollectionChangedEventArgs _NotifyCollectionChangedEventArgs_Reset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}
