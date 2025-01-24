using AuroraLib.Core;
using AuroraLib.Core.Buffers;
using AuroraLib.Core.Format.Identifier;
using AuroraLib.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;

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
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
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
        }

        [TestMethod]
        [DataRow((short)10, Endian.Little)]
        [DataRow((short)10, Endian.Big)]
        public void ReadGenericShort(short vaule, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(vaule, order);
                stream.Position = 0;
                short actualVaule = stream.Read<short>(order);

                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadGenericInt(int vaule, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(vaule, order);
                stream.Position = 0;
                int actualVaule = stream.Read<int>(order);

                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow((long)10, Endian.Little)]
        [DataRow((long)10, Endian.Big)]
        public void ReadGenericLong(long vaule, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(vaule, order);
                stream.Position = 0;
                long actualVaule = stream.Read<long>(order);

                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow((short)10, Endian.Little)]
        [DataRow((short)10, Endian.Big)]
        public void ReadInt16(short vaule, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(vaule, order);
                stream.Position = 0;
                short actualVaule = stream.ReadInt16(order);

                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadInt24(int intVaule, Endian order)
        {
            Int24 vaule = intVaule;
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(vaule, order);
                stream.Position = 0;
                Int24 actualVaule = stream.ReadInt24(order);

                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadInt32(int vaule, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(vaule, order);
                stream.Position = 0;
                int actualVaule = stream.ReadInt32(order);

                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadInt64(long vaule, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(vaule, order);
                stream.Position = 0;
                long actualVaule = stream.ReadInt64(order);

                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void ReadSpan(int length, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            using (SpanBuffer<int> buffer = new SpanBuffer<int>(length))
            {
                for (int i = 0; i < length; i++)
                    stream.Write(i, order);

                stream.Position = 0;
                stream.Read(buffer.Span, order);

                for (int i = 0; i < length; i++)
                    Assert.AreEqual(i, buffer.Span[i]);
            }
        }

        [TestMethod]
        [DataRow(10, Endian.Little)]
        [DataRow(10, Endian.Big)]
        public void WriteSpan(int length, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            using (SpanBuffer<int> buffer = new SpanBuffer<int>(length))
            {
                for (int i = 0; i < length; i++)
                    buffer.Span[i] = i;

                stream.Write<int>(buffer, order);
                stream.Position = 0;

                for (int i = 0; i < length; i++)
                {
                    int vaule = stream.ReadInt32(order);
                    Assert.AreEqual(i, vaule);
                }
            }
        }

        [TestMethod]
        [DataRow("Test123")]
        public void ReadString(string actualVaule)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                for (int i = 0; i < actualVaule.Length; i++)
                {
                    stream.WriteByte((byte)actualVaule[i]);
                }
                stream.Position = 0;

                string vaule = stream.ReadString(actualVaule.Length);
                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow("Test123")]
        public void WriteString(string actualVaule)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {

                stream.WriteString(actualVaule.AsSpan());
                stream.Position = 0;

                Span<char> chars = stackalloc char[actualVaule.Length];
                for (int i = 0; i < chars.Length; i++)
                {
                    chars[i] = (char)stream.ReadInt8();
                }
                Assert.AreEqual(chars.ToString(), actualVaule);
            }
        }

        [TestMethod]
        [DataRow("Test123")]
        public void ReadStringTerminate(string actualVaule)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                for (int i = 0; i < actualVaule.Length; i++)
                {
                    stream.WriteByte((byte)actualVaule[i]);
                }
                stream.WriteByte(0);
                stream.Position = 0;

                string vaule = stream.ReadCString();
                Assert.AreEqual(vaule, actualVaule);
            }
        }

        [TestMethod]
        [DataRow("Test123")]
        public void WriteStringTerminate(string actualVaule)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.WriteCString(actualVaule.AsSpan(), 0);
                stream.Position = 0;

                Span<char> chars = stackalloc char[actualVaule.Length];
                for (int i = 0; i < chars.Length; i++)
                {
                    chars[i] = (char)stream.ReadInt8();
                }
                Assert.AreEqual(chars.ToString(), actualVaule);
                Assert.AreEqual(stream.ReadUInt8(), 0);
            }
        }

        [TestMethod]
        [DataRow(1, Endian.Little)]
        [DataRow(1, Endian.Big)]
        [DataRow(3, Endian.Little)]
        [DataRow(3, Endian.Big)]
        public void DetectByteOrder(int count, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                for (int i = 1; i < count + 1; i++)
                    stream.Write(i, order);

                stream.Position = 0;
                Endian detect = stream.DetectByteOrder<int>(count);

                Assert.AreEqual(order, detect);
            }
        }

        [TestMethod]
        [DataRow(15, Endian.Little)]
        [DataRow(15, Endian.Big)]
        [DataRow(2, Endian.Little)]
        [DataRow(2, Endian.Big)]
        public void DetectByteOrderByDistance(int reference, Endian order)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(reference, order);

                stream.Position = 0;
                Endian detect = stream.DetectByteOrderByDistance(reference);

                Assert.AreEqual(order, detect);
            }
        }

        [TestMethod]
        [DataRow((uint)24, (uint)10)]
        [DataRow((uint)10, (uint)10)]
        public void MatchVaule(uint expected, uint actual)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(actual);
                stream.Position = 0;

                bool vaule = stream.Match(new Identifier32(expected));

                Assert.AreEqual(vaule, expected == actual);
            }
        }

        [TestMethod]
        [DataRow("Test", "XD_D")]
        [DataRow("Test", "Test")]
        public void MatchString(string expected, string actual)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.WriteString(actual.AsSpan());
                stream.Position = 0;

                bool vaule = stream.Match(expected.AsSpan());

                Assert.AreEqual(vaule, expected == actual);
            }
        }

        [TestMethod]
        [DataRow(Endian.Little)]
        [DataRow(Endian.Big)]
        public void WriteList(Endian order)
        {
            List<uint> list = new List<uint>() { 5, 10, 6, 8, 6 };

            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.WriteCollection(list, order);

                stream.Position = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    uint vaule = stream.ReadUInt32(order);
                    Assert.AreEqual(list[i], vaule);
                }
            }
        }


        [TestMethod]
        [DataRow(Endian.Little)]
        [DataRow(Endian.Big)]
        public void ReadList(Endian order)
        {
            ReadOnlySpan<int> data = new int[] { 5, 10, 6, 8, 6 };
            List<int> list = new List<int>();

            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(data, order);
                stream.Position = 0;

                stream.ReadCollection(list, data.Length, order);
                for (int i = 0; i < data.Length; i++)
                {
                    Assert.AreEqual(list[i], data[i]);
                }
            }
        }
    }
}
