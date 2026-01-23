#if !NET6_0_OR_GREATER
using System;
using System.Buffers.Binary;

namespace AuroraLib.Core.IO
{
    internal sealed class Adler32
    {
        private const int NMAX = 5552;
        private const uint MOD_ADLER = 65521;
        private uint A, B;

        public Adler32() => Reset();

        public void Append(ReadOnlySpan<byte> data)
        {
            uint a = A;
            uint b = B;

            while (!data.IsEmpty)
            {
                int len = Math.Min(NMAX, data.Length);
                for (int i = 0; i < len; i++)
                {
                    a += data[i];
                    b += a;
                }
                a %= MOD_ADLER;
                b %= MOD_ADLER;

                data = data.Slice(len);
            }
            A = a;
            B = b;
        }

        public void Reset()
        {
            A = 1;
            B = 0;
        }

        public uint GetCurrentHashAsUInt32() => (B << 16) | A;
    }
}
#endif
