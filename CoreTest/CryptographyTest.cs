using AuroraLib.Core.Cryptography;
using AuroraLib.Core.Interfaces;
using System;
using System.Collections;

namespace CoreUnitTest
{
    [TestClass]
    public class CryptographyTest
    {
        public const string TestData = "Hash Test String, 123";

        public static IEnumerable<object[]> HashTestData()
        {
            yield return new object[] { new Adler32(), "BE06B84F" };
            yield return new object[] { new Adler64(), "BE060000B84F0000" };
            yield return new object[] { new CityHash32(), "20B22194" };
            yield return new object[] { new CityHash64(), "AA8DB73547338E34" };
            yield return new object[] { new CityHash128(), "D2CB74C404956ED6FE64B68B1904D2CD" };
            yield return new object[] { new Fnv1_32(), "5EF94AE2" };
            yield return new object[] { new Fnv1_32(true), "FCE30CA9" }; // Fnv1a_32
            yield return new object[] { new Fnv1_64(), "DEA365A8B42728EE" };
            yield return new object[] { new Fnv1_64(true), "DCE43A4A2526021C" }; // Fnv1a_64
            yield return new object[] { new MurmurHash3_32(), "E0A41D8F" };
            yield return new object[] { new MurmurHash3_128(), "A43C6FD6958EDC42D2981BF9E0A45891" };
            yield return new object[] { new XXHash32(), "F7B3F8E7" };
            yield return new object[] { new XXHash64(), "730D0421E98CE8C4" };
            yield return new object[] { new Crc32(Crc32Algorithm.Default), "340C0B54" };
            yield return new object[] { new Crc32(Crc32Algorithm.BZIP2), "50237BB7" };
            yield return new object[] { new Crc32(Crc32Algorithm.MPEG2), "AFDC8448" };
            yield return new object[] { new Crc32(Crc32Algorithm.JAMCRC), "CBF3F4AB" };
            yield return new object[] { new Crc32(Crc32Algorithm.POSIX), "2DFC2799" };
            yield return new object[] { new Crc32(Crc32Algorithm.XFER), "A533C11A" };
            yield return new object[] { new Crc32(Crc32Algorithm.SATA), "C21D8BFE" };
            yield return new object[] { new Crc32(Crc32Algorithm.CRC32C), "1B63195F" };
            yield return new object[] { new Crc32(Crc32Algorithm.CRC32D), "7AA669CE" };
            yield return new object[] { new Crc32(Crc32Algorithm.CRC32Q), "FC9989FC" };
        }

        [TestMethod]
        [DynamicData(nameof(HashTestData), DynamicDataSourceType.Method)]
        public void HashMatchTest(IHash algorithm, string expectedHash)
        {
            algorithm.Reset();

            algorithm.Compute(TestData);

            string actualHash = BitConverter.ToString(algorithm.GetBytes()).Replace("-", "");
            Assert.AreEqual(expectedHash, actualHash);
        }
    }
}
