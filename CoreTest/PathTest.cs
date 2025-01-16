using AuroraLib.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoreUnitTest
{
#if !NET5_0_OR_GREATER
    [TestClass]
    public class PathTest
    {
        private static readonly string[] TestPaths = new string[]
        {
            "C:\\Users\\Venomalia\\source\\repos\\AuroraLib.Core\\icon.png",
            "D:\\Documents\\Example\\test.jpg",
            "C:\\Program Files\\Test\\app.exe",
            "D:\\Pictures\\image.jpg",
            "relative\\path\\to\\file.txt",
            "D:\\temp\\folder\\",
            "C:\\Users\\Public\\Documents\\",
            "/home/user/test.txt",
            "/var/www/html/index.html",
            "/home/user/.config/app/config.json"
        };

        public static IEnumerable<object[]> GetTestPaths()
            => TestPaths.Select(path => new object[] { path });

        [TestMethod]
        [DynamicData(nameof(GetTestPaths), DynamicDataSourceType.Method)]
        public void GetExtension(string path)
        {
            string vaule = Path.GetExtension(path);
            string actualVaule = PathX.GetExtension(path.AsSpan()).ToString();

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DynamicData(nameof(GetTestPaths), DynamicDataSourceType.Method)]
        public void GetDirectoryName(string path)
        {
            string vaule = Path.GetDirectoryName(path);
            string actualVaule = PathX.GetDirectoryName(path.AsSpan()).ToString();
            actualVaule = actualVaule.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DynamicData(nameof(GetTestPaths), DynamicDataSourceType.Method)]
        public void GetFileName(string path)
        {
            string vaule = Path.GetFileName(path);
            string actualVaule = PathX.GetFileName(path.AsSpan()).ToString();

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DynamicData(nameof(GetTestPaths), DynamicDataSourceType.Method)]
        public void GetFileNameWithoutExtension(string path)
        {
            string vaule = Path.GetFileNameWithoutExtension(path);
            string actualVaule = PathX.GetFileNameWithoutExtension(path.AsSpan()).ToString();

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow("C:\\Users\\Venomalia\\Documents", "file.txt")]
        [DataRow("/home/user/docs/", "image.png")]
        [DataRow("", "file.txt")]
        [DataRow("", "")]
        public void Join2(string path1, string path2)
        {
            string vaule = Path.Combine(path1, path2);
            string actualVaule = PathX.Join(path1.AsSpan(), path2.AsSpan()).ToString();

            Assert.AreEqual(vaule, actualVaule);
        }

        [TestMethod]
        [DataRow("/home/user/docs/", "Venomalia", "image.png")]
        [DataRow("C:\\Users\\Venomalia\\Documents", "file.txt", "")]
        [DataRow("/home/user/docs/", "", "image.png")]
        [DataRow("", "", "file.txt")]
        [DataRow("", "", "")]
        public void Join3(string path1, string path2, string path3)
        {
            string vaule = Path.Combine(path1, path2, path3);
            string actualVaule = PathX.Join(path1.AsSpan(), path2.AsSpan(), path3.AsSpan()).ToString();

            Assert.AreEqual(vaule, actualVaule);
        }
    }
#endif
}
