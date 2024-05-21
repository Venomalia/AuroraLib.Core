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

                BitReader reader = new BitReader(stream, Endian.Little, bitOrder);
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
                BitWriter writer = new BitWriter(stream, bitOrder, bitOrder);
                writer.Write(value);
                for (int i = 1; i < 8; i++)
                {
                    writer.Write(!value);
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
        public void BitReader_ReadBits(int bits, Endian bitOrder)
        {
            byte value = (byte)(bits << bits);
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                if (bitOrder == Endian.Little)
                    stream.WriteByte(value);
                else
                    stream.WriteByte((byte)(bits << (8 - bits)));
                stream.Position = 0;

                BitReader reader = new BitReader(stream, bitOrder, bitOrder);
                byte actualvalue = (byte)reader.ReadInt(bits);

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
        public void BitWriter_WriteBits(int bits, Endian bitOrder)
        {
            byte value = (byte)(bits << bits);
            using (MemoryPoolStream stream = new MemoryPoolStream())
            {
                BitWriter writer = new BitWriter(stream, bitOrder, bitOrder);
                writer.Write(value, bits);
                writer.Flush();
                stream.Position = 0;

                byte actualvalue;
                if (bitOrder == Endian.Little)
                    actualvalue = (byte)stream.ReadByte();
                else
                    actualvalue = (byte)(stream.ReadByte() >> (8 - bits));

                Assert.AreEqual(value, actualvalue);
            }
        }
        #endregion

    }
}
