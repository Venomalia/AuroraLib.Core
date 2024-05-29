using AuroraLib.Core;
using AuroraLib.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreUnitTest
{
    [TestClass]
    public class BitStreamProcessorTest
    {
        #region Int Test
        [TestMethod]
        [DataRow(-1, Endian.Big)]
        [DataRow(1, Endian.Big)]
        [DataRow(0x3C0, Endian.Big)]
        [DataRow(-1, Endian.Little)]
        [DataRow(1, Endian.Little)]
        [DataRow(0x3C0, Endian.Little)]
        public void BitReader_ReadInt32(int value, Endian byteOrder)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                stream.Write(value, byteOrder);
                stream.Position = 0;

                BitReader reader = new BitReader(stream, byteOrder);
                int actualvalue = reader.ReadInt32();

                Assert.AreEqual(value, actualvalue);
            }
        }

        [TestMethod]
        [DataRow(-1, Endian.Big)]
        [DataRow(1, Endian.Big)]
        [DataRow(0x3C0, Endian.Big)]
        [DataRow(-1, Endian.Little)]
        [DataRow(1, Endian.Little)]
        [DataRow(0x3C0, Endian.Little)]
        public void BitWriter_WriteInt32(int value, Endian byteOrder)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                BitWriter writer = new BitWriter(stream, byteOrder);
                writer.Write(value);
                writer.Flush();
                stream.Position = 0;

                int actualvalue = stream.ReadInt32(byteOrder); ;

                Assert.AreEqual(value, actualvalue);
            }
        }
        #endregion

        #region Bit Test
        [TestMethod]
        [DataRow(true, Endian.Big)]
        [DataRow(false, Endian.Big)]
        [DataRow(true, Endian.Little)]
        [DataRow(false, Endian.Little)]
        public void BitReader_ReadBit(bool value, Endian bitOrder)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                if (bitOrder == Endian.Little)
                    stream.WriteByte((byte)(value ? 1 : 254));
                else
                    stream.WriteByte((byte)(value ? 128 : 127));
                stream.Position = 0;

                BitReader reader = new BitReader(stream, bitOrder);
                bool actualvalue = reader.ReadBit();

                Assert.AreEqual(value, actualvalue);
            }
        }

        [TestMethod]
        [DataRow(true, Endian.Big)]
        [DataRow(false, Endian.Big)]
        [DataRow(true, Endian.Little)]
        [DataRow(false, Endian.Little)]
        public void BitWriter_WriteBit(bool value, Endian bitOrder)
        {
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                BitWriter writer = new BitWriter(stream, bitOrder);
                writer.Write(value);
                for (int i = 1; i < 8; i++)
                {
                    writer.Write(value);
                }
                writer.Flush();
                stream.Position = 0;

                bool actualvalue;
                if (bitOrder == Endian.Little)
                    actualvalue = (stream.ReadByte() & 1) == 1;
                else
                    actualvalue = (stream.ReadByte() & 0x80) != 0;

                Assert.AreEqual(value, actualvalue);
            }
        }
        #endregion

        #region Bits Test
        [TestMethod]
        [DataRow(3, Endian.Big)]
        [DataRow(3, Endian.Little)]
        [DataRow(5, Endian.Big)]
        [DataRow(5, Endian.Little)]
        [DataRow(7, Endian.Big)]
        [DataRow(7, Endian.Little)]
        [DataRow(9, Endian.Big)]
        [DataRow(9, Endian.Little)]
        [DataRow(11, Endian.Big)]
        [DataRow(11, Endian.Little)]
        [DataRow(15, Endian.Big)]
        [DataRow(15, Endian.Little)]
        public void BitReader_ReadBitsAsUInt(int bits, Endian bitOrder)
        {
            uint value = (uint)((1 << bits) - 2);
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                if (bitOrder == Endian.Little)
                    stream.Write(value);
                else
                    stream.Write((value << (32 - bits)), Endian.Big);
                stream.Position = 0;

                BitReader reader = new BitReader(stream, bitOrder);
                uint actualvalue = (uint)reader.ReadUInt(bits);

                Assert.AreEqual(value, actualvalue);
            }
        }

        [TestMethod]
        [DataRow(3, Endian.Big)]
        [DataRow(3, Endian.Little)]
        [DataRow(5, Endian.Big)]
        [DataRow(5, Endian.Little)]
        [DataRow(7, Endian.Big)]
        [DataRow(7, Endian.Little)]
        [DataRow(9, Endian.Big)]
        [DataRow(9, Endian.Little)]
        [DataRow(11, Endian.Big)]
        [DataRow(11, Endian.Little)]
        [DataRow(15, Endian.Big)]
        [DataRow(15, Endian.Little)]
        public void BitWriter_WriteBitsAsUInt(int bits, Endian bitOrder)
        {
            uint value = (uint)((1 << bits) - 2);
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                BitWriter writer = new BitWriter(stream, bitOrder);
                writer.Write(value, bits);
                writer.Write(0, 32 - bits);
                writer.Flush();
                stream.Position = 0;

                uint actualvalue;
                if (bitOrder == Endian.Little)
                    actualvalue = stream.ReadUInt32();
                else
                    actualvalue = stream.ReadUInt32(Endian.Big) >> (32 - bits);

                Assert.AreEqual(value, actualvalue);
            }
        }
        [TestMethod]
        [DataRow(3, Endian.Big)]
        [DataRow(3, Endian.Little)]
        [DataRow(5, Endian.Big)]
        [DataRow(5, Endian.Little)]
        [DataRow(7, Endian.Big)]
        [DataRow(7, Endian.Little)]
        [DataRow(9, Endian.Big)]
        [DataRow(9, Endian.Little)]
        [DataRow(11, Endian.Big)]
        [DataRow(11, Endian.Little)]
        [DataRow(15, Endian.Big)]
        [DataRow(15, Endian.Little)]
        public void BitReader_ReadBitsAsInt(int bits, Endian bitOrder)
        {
            int value = (1 << bits - 1) * -1;
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                if (bitOrder == Endian.Little)
                    stream.Write(value);
                else
                    stream.Write((value << (32 - bits)), Endian.Big);
                stream.Position = 0;

                BitReader reader = new BitReader(stream, bitOrder);
                int actualvalue = (int)reader.ReadInt(bits);

                Assert.AreEqual(value, actualvalue);
            }
        }

        [TestMethod]
        [DataRow(3, Endian.Big)]
        [DataRow(3, Endian.Little)]
        [DataRow(5, Endian.Big)]
        [DataRow(5, Endian.Little)]
        [DataRow(7, Endian.Big)]
        [DataRow(7, Endian.Little)]
        [DataRow(9, Endian.Big)]
        [DataRow(9, Endian.Little)]
        [DataRow(11, Endian.Big)]
        [DataRow(11, Endian.Little)]
        [DataRow(15, Endian.Big)]
        [DataRow(15, Endian.Little)]
        public void BitWriter_WriteBitsAsInt(int bits, Endian bitOrder)
        {
            int value = (1 << bits-1) * -1;
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                BitWriter writer = new BitWriter(stream, bitOrder);
                writer.Write(value, bits);
                writer.Write(0, 32 - bits);
                writer.Flush();
                stream.Position = 0;

                BitReader reader = new BitReader(stream, bitOrder);
                int actualvalue = (int)reader.ReadInt(bits);

                Assert.AreEqual(value, actualvalue);
            }
        }

        [TestMethod]
        [DataRow(3, Endian.Big)]
        [DataRow(3, Endian.Little)]
        [DataRow(5, Endian.Big)]
        [DataRow(5, Endian.Little)]
        [DataRow(7, Endian.Big)]
        [DataRow(7, Endian.Little)]
        [DataRow(9, Endian.Big)]
        [DataRow(9, Endian.Little)]
        [DataRow(11, Endian.Big)]
        [DataRow(11, Endian.Little)]
        [DataRow(15, Endian.Big)]
        [DataRow(15, Endian.Little)]
        [DataRow(62, Endian.Big)]
        [DataRow(62, Endian.Little)]
        public void BitWriterBitReader_CrossCheck(int bits, Endian bitOrder)
        {
            ulong value = (1ul << bits-1) + 1;
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                BitWriter writer = new BitWriter(stream, bitOrder);
                for (int i = 0; i < 4; i++)
                    writer.Write(value, bits);
                writer.Flush();
                stream.Position = 0;

                BitReader reader = new BitReader(stream, bitOrder);

                for (int i = 0; i < 4; i++)
                {
                    ulong actualvalue = reader.ReadUInt(bits);

                    Assert.AreEqual(value, actualvalue);
                }
            }
        }
        #endregion

    }
}
