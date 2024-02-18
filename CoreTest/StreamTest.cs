using AuroraLib.Core;
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
    }
}
