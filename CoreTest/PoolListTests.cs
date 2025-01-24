using AuroraLib.Core.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CoreUnitTest
{
    [TestClass]
    public class PoolListTests
    {
        private static readonly int[] _items = new[] { 1, 2, 3, 4 };

        [TestMethod]
        public void SetMinimumCapacity()
        {
            var list = new PoolList<int>();
            int newCapacity = list.Capacity + 1;
            list.SetMinimumCapacity(newCapacity);
            Assert.IsTrue(list.Capacity >= newCapacity);
        }

        [TestMethod]
        public void Add()
        {
            var list = new PoolList<int>();
            list.Add(42);

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(42, list[0]);
        }

        [TestMethod]
        public void Insert()
        {
            var list = new PoolList<int>(_items);

            list.Insert(1, 42);

            Assert.AreEqual(_items.Length + 1, list.Count); // The list should now have 4 elements
            Assert.AreEqual(_items[0], list[0]);
            Assert.AreEqual(42, list[1]);
            Assert.AreEqual(_items[1], list[2]);
            Assert.AreEqual(_items[2], list[3]);
            Assert.AreEqual(_items[3], list[4]);
        }

        [TestMethod]
        public void AddRange_HandleSpan()
        {
            ReadOnlySpan<int> items = _items;
            var list = new PoolList<int>();
            list.AddRange(items);

            Assert.AreEqual(_items.Length, list.Count);
            for (int i = 0; i < _items.Length; i++)
                Assert.AreEqual(_items[i], list[i]);
        }

        [TestMethod]
        public void AddRange_HandleEmptySpan()
        {
            var list = new PoolList<int>();
            list.AddRange(ReadOnlySpan<int>.Empty);

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void AddRange_HandleEnumerablen()
        {
            IEnumerable<int> items = _items;
            var list = new PoolList<int>();
            list.AddRange(items);

            Assert.AreEqual(_items.Length, list.Count);
            for (int i = 0; i < _items.Length; i++)
                Assert.AreEqual(_items[i], list[i]);
        }

        [TestMethod]
        public void AddRange_HandleEmptyEnumerablen()
        {
            var list = new PoolList<int>();
            list.AddRange(new List<int>());
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void Remove_Item()
        {
            var list = new PoolList<int>(_items);
            bool result = list.Remove(2);
            Assert.IsTrue(result);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(4, list[2]);
        }

        [TestMethod]
        public void Remove_ReturnFalseWhenElementNotFound()
        {
            var list = new PoolList<int>(_items);
            bool result = list.Remove(99);
            Assert.IsFalse(result);
            Assert.AreEqual(_items.Length, list.Count);
        }

        [TestMethod]
        public void InsertRange()
        {
            var list = new PoolList<int>(_items);

            list.InsertRange(1, new[] { 4, 5 });

            Assert.AreEqual(6, list.Count);
            Assert.AreEqual(_items[0], list[0]);
            Assert.AreEqual(4, list[1]);
            Assert.AreEqual(5, list[2]);
            Assert.AreEqual(_items[1], list[3]);
            Assert.AreEqual(_items[2], list[4]);
            Assert.AreEqual(_items[3], list[5]);
        }

        [TestMethod]
        public void ShouldThrowOnInvalidIndex()
        {
            var list = new PoolList<int>(_items);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(_items.Length));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list[_items.Length] = 100);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Insert(_items.Length, default));
        }

        [TestMethod]
        public void Enumerator()
        {
            var list = new PoolList<int>(_items);

            var enumerator = list.GetEnumerator();
            for (int i = 0; i < _items.Length; i++)
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(_items[i], enumerator.Current);
            }
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_ThrowWhenModified()
        {
            var list = new PoolList<int>(_items);
            var enumerator = list.GetEnumerator();

            list.Add(4);

            Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_ResetCorrectly()
        {
            var list = new PoolList<int>(_items);
            var enumerator = list.GetEnumerator();

            enumerator.MoveNext();
            enumerator.MoveNext();
            enumerator = list.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(_items[0], enumerator.Current);
        }
    }
}
