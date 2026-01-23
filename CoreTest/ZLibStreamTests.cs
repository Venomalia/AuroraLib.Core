using AuroraLib.Core;
using AuroraLib.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Reflection.Emit;
using System.Text;

namespace CoreUnitTest
{
    [TestClass]
    public class ZLibStreamTests
    {
        [TestMethod]
        public void ZLib_Compress_And_Decompress_Should_Return_Original_Data()
        {
            // Arrange
            byte[] originalData = Encoding.UTF8.GetBytes("Test data for ZLibStream, 123123 Test DATA.");

            // Act
            byte[] compressedData = Compress(originalData);
            byte[] decompressedData = Decompress(compressedData);

            // Assert
            Assert.AreEqual(originalData.Length, decompressedData.Length);
            for (int i = 0; i < originalData.Length; i++)
                Assert.AreEqual(originalData[i], decompressedData[i]);
        }

        private static byte[] Compress(byte[] data)
        {
            using var output = new MemoryStream();
            using (var zlib = new ZLibStream(output, CompressionLevel.Optimal))
            {
                zlib.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        private static byte[] Decompress(byte[] compressedData)
        {
            using var input = new MemoryStream(compressedData);
            using var zlib = new ZLibStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            zlib.CopyTo(output);
            return output.ToArray();
        }

    }
}
