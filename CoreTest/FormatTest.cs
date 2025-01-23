using AuroraLib.Core.Format;
using AuroraLib.Core.Format.Identifier;
using AuroraLib.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace CoreUnitTest
{
    [TestClass]
    public class FormatTest
    {
        private static FormatDictionary formats = new FormatDictionary() {
            new FormatInfo("ZIP File", new MediaType(MIMEType.Application,"zip"), ".zip", new Identifier32(67324752)),
            new FormatInfo("Portable Network Graphics", new MediaType(MIMEType.Image,"png"), ".png", new Identifier64( 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a)),
            new FormatInfo("Windows Bitmap", new MediaType(MIMEType.Image,"bmp"), ".bmp", new Identifier32("BMP ".AsSpan())),
            new FormatInfo("Markdown", new MediaType(MIMEType.Text,"markdown"), new string[] { ".md",".markdown" }),
        };

        [TestMethod]
        [DataRow("..\\..\\..\\..\\icon.png", "image/png")]
        [DataRow("..\\..\\..\\..\\README.md", "text/markdown")]
        public void Identify(string path, string expectedMime)
        {
            ReadOnlySpan<char> fileNameAndExtension = PathX.GetFileName(path.AsSpan());
            using (FileStream data = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                if (formats.Identify(data, fileNameAndExtension, out IFormatInfo format))
                {
                    Assert.AreEqual(format.MIMEType.ToString(), expectedMime);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        [DataRow("application/zip", ".zip")]
        [DataRow("image/png", ".png")]
        [DataRow("text/bmp", ".bmp")]
        [DataRow("text/markdown", ".md")]
        public void GetFileExtension(string mimeTyp, string expected)
        {
            if (formats.TryGetValue(mimeTyp, out IFormatInfo format))
            {
                Assert.AreEqual(format.FileExtensions.First(), expected);
            }
        }
    }
}
