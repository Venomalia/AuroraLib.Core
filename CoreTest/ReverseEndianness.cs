using AuroraLib.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace CoreUnitTest
{
    [TestClass]
    public class ReverseEndiannessGenericTests
    {
        [TestMethod]
        [DataRow(16)]
        [DataRow(0xFF0E2F0)]
        public void ReverseEndianness_Int(int value)
        {
            int reverseValue = BitConverterX.ReverseEndianness<int>(value);
            int actualVaule = BinaryPrimitives.ReverseEndianness(value);

            Assert.AreEqual(reverseValue, actualVaule);
        }

        [TestMethod]
        public void ReverseEndianness_IntSpan()
        {
            ReadOnlySpan<int> values = new int[] { 16, 17, 18, 19 };
            var reverseValues = values.ToArray().AsSpan();
            BitConverterX.ReverseEndianness(reverseValues);

            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(reverseValues[i], BinaryPrimitives.ReverseEndianness(values[i]));
            }
        }

        [TestMethod]
        public void ReverseEndianness_Tuple()
        {
            (int, short, short) value = (7, 3, 21);
            var reverseValue = BitConverterX.ReverseEndianness(value);

            Assert.AreEqual(reverseValue.Item1, BinaryPrimitives.ReverseEndianness(value.Item1));
            Assert.AreEqual(reverseValue.Item2, BinaryPrimitives.ReverseEndianness(value.Item2));
            Assert.AreEqual(reverseValue.Item3, BinaryPrimitives.ReverseEndianness(value.Item3));
        }

        [TestMethod]
        public void ReverseEndianness_FixedBuffer()
        {
            var value = new FixedBufferStruct(16, 12, 15, 7);

            var reverseValue = BitConverterX.ReverseEndianness(value);

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(reverseValue[i], BinaryPrimitives.ReverseEndianness(value[i]));
            }
        }

        unsafe struct FixedBufferStruct
        {
            public fixed int Buffer[4];

            public ref int this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    ThrowIf.Negative(index);
                    ThrowIf.GreaterThan(index, 3);
                    fixed (int* ptr = Buffer)
                    {
                        return ref ptr[index];
                    }
                }
            }

            public FixedBufferStruct(int value1, int value2, int value3, int value4)
            {
                fixed (int* ptr = Buffer)
                {
                    ptr[0] = value1;
                    ptr[1] = value2;
                    ptr[2] = value3;
                    ptr[3] = value4;
                }
            }
        }

#if NET6_0_OR_GREATER
        [TestMethod]
        public void ReverseEndianness_Record()
        {
            var value = new RecordStruct(16, 12, 15, 7, 48);

            var reverseValue = BitConverterX.ReverseEndianness(value);

            Assert.AreEqual(reverseValue.Test1, BinaryPrimitives.ReverseEndianness(value.Test1));
            Assert.AreEqual(reverseValue.Test2, BinaryPrimitives.ReverseEndianness(value.Test2));
            Assert.AreEqual(reverseValue.Test3, BinaryPrimitives.ReverseEndianness(value.Test3));
            Assert.AreEqual(reverseValue.Test4, value.Test4);
            Assert.AreEqual(reverseValue.Test5, value.Test5);
        }

        public readonly record struct RecordStruct(long Test1, int Test2, short Test3, byte Test4, byte Test5);
#endif
    }
}
