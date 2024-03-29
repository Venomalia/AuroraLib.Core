﻿using AuroraLib.Core;
using AuroraLib.Core.Buffers;
using AuroraLib.Core.IO;
using System.Buffers.Binary;

namespace CoreUnitTest
{
    [TestClass]
    public class StreamTest
    {
        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void WriteGeneric(int vaule, Endian order)
        {
            using MemoryPoolStream stream = new();
            stream.Write(vaule, order);
            stream.Position = 0;

            Span<byte> bytes = stackalloc byte[4];
            stream.Read(bytes);
            int actualVaule;
            if (order == Endian.Little)
                actualVaule = BinaryPrimitives.ReadInt32LittleEndian(bytes);
            else
                actualVaule = BinaryPrimitives.ReadInt32BigEndian(bytes);

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadGeneric(int vaule, Endian order)
        {
            using MemoryPoolStream stream = new();
            stream.Write(vaule, order);
            stream.Position = 0;
            int actualVaule = stream.Read<int>(order);

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow((short)10, Endian.Little)]
        [DataRow((short)10, Endian.Big)]
        public void ReadInt16(short vaule, Endian order)
        {
            using MemoryPoolStream stream = new();
            stream.Write(vaule, order);
            stream.Position = 0;
            short actualVaule = stream.ReadInt16(order);

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadInt24(int intVaule, Endian order)
        {
            Int24 vaule = intVaule;
            using MemoryPoolStream stream = new();
            stream.Write(vaule, order);
            stream.Position = 0;
            Int24 actualVaule = stream.ReadInt24(order);

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadInt32(int vaule, Endian order)
        {
            using MemoryPoolStream stream = new();
            stream.Write(vaule, order);
            stream.Position = 0;
            int actualVaule = stream.ReadInt32(order);

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadInt64(long vaule, Endian order)
        {
            using MemoryPoolStream stream = new();
            stream.Write(vaule, order);
            stream.Position = 0;
            long actualVaule = stream.ReadInt64(order);

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadSpan(int length, Endian order)
        {
            using MemoryPoolStream stream = new();
            using SpanBuffer<int> buffer = new(length);
            for (int i = 0; i < length; i++)
                stream.Write(i, order);

            stream.Position = 0;
            stream.Read(buffer.Span, order);

            for (int i = 0; i < length; i++)
                Assert.AreEqual(i, buffer[i]);
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void WriteSpan(int length, Endian order)
        {
            using MemoryPoolStream stream = new();
            using SpanBuffer<int> buffer = new(length);
            for (int i = 0; i < length; i++)
                buffer[i] = i;

            stream.Write<int>(buffer, order);
            stream.Position = 0;

            for (int i = 0; i < length; i++)
            {
                int vaule = stream.ReadInt32(order);
                Assert.AreEqual(i, vaule);
            }
        }

        [TestMethod]
        [DataRow("Test123")]
        public void ReadString(string actualVaule)
        {
            using MemoryPoolStream stream = new();
            for (int i = 0; i < actualVaule.Length; i++)
            {
                stream.WriteByte((byte)actualVaule[i]);
            }
            stream.Position = 0;

            string vaule = stream.ReadString(actualVaule.Length);
            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow("Test123")]
        public void WriteString(string actualVaule)
        {
            using MemoryPoolStream stream = new();
            stream.WriteString(actualVaule);
            stream.Position = 0;

            Span<char> chars = stackalloc char[actualVaule.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)stream.ReadInt8();
            }
            Assert.AreEqual(chars.ToString(), actualVaule);
        }

        [TestMethod]
        [DataRow("Test123")]
        public void ReadStringTerminate(string actualVaule)
        {
            using MemoryPoolStream stream = new();
            for (int i = 0; i < actualVaule.Length; i++)
            {
                stream.WriteByte((byte)actualVaule[i]);
            }
            stream.WriteByte(0);
            stream.Position = 0;

            string vaule = stream.ReadString();
            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow("Test123")]
        public void WriteStringTerminate(string actualVaule)
        {
            using MemoryPoolStream stream = new();
            stream.WriteString(actualVaule, 0);
            stream.Position = 0;

            Span<char> chars = stackalloc char[actualVaule.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)stream.ReadInt8();
            }
            Assert.AreEqual(chars.ToString(), actualVaule);
            Assert.AreEqual(stream.ReadUInt8(), 0);
        }

        [TestMethod]
        [DataRow(1, Endian.Little)]
        [DataRow(1, Endian.Big)]
        [DataRow(3, Endian.Little)]
        [DataRow(3, Endian.Big)]
        public void DetectByteOrder(int count, Endian order)
        {
            using MemoryPoolStream stream = new();
            for (int i = 1; i < count + 1; i++)
                stream.Write(i, order);

            stream.Position = 0;
            Endian detect = stream.DetectByteOrder<int>(count);

            Assert.AreEqual(order, detect);
        }

        [TestMethod]
        [DataRow(15, Endian.Little)]
        [DataRow(15, Endian.Big)]
        [DataRow(2, Endian.Little)]
        [DataRow(2, Endian.Big)]
        public void DetectByteOrderByDistance(int reference, Endian order)
        {
            using MemoryPoolStream stream = new();
            stream.Write(reference, order);

            stream.Position = 0;
            Endian detect = stream.DetectByteOrderByDistance(reference);

            Assert.AreEqual(order, detect);
        }

        [TestMethod]
        [DataRow((uint)24, (uint)10)]
        [DataRow((uint)10, (uint)10)]
        public void MatchVaule(uint expected, uint actual)
        {
            using MemoryPoolStream stream = new();
            stream.Write(actual);
            stream.Position = 0;

            bool vaule = stream.Match(new Identifier32(expected));

            Assert.AreEqual(vaule, expected == actual);
        }

        [TestMethod]
        [DataRow("Test", "XD_D")]
        [DataRow("Test", "Test")]
        public void MatchString(string expected, string actual)
        {
            using MemoryPoolStream stream = new();
            stream.WriteString(actual);
            stream.Position = 0;

            bool vaule = stream.Match(expected);

            Assert.AreEqual(vaule, expected == actual);
        }
    }
}
