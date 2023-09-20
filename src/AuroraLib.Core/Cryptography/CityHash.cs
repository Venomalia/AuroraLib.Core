using System.Runtime.CompilerServices;

namespace AuroraLib.Core.Cryptography
{
    //copy of https://github.com/google/cityhash/blob/master/src/city.cc
    internal static class CityHash
    {
        // Some primes between 2^63 and 2^64 for various uses.
        private const ulong k0 = 0xc3a5c85c97cb3127UL;
        private const ulong k1 = 0xb492b66fbe98f273UL;
        private const ulong k2 = 0x9ae16a3b2f90404fUL;
        // Magic numbers for 32-bit hashing.  Copied from Murmur3.
        private const uint c1 = 0xcc9e2d51;
        private const uint c2 = 0x1b873593;


        #region Hash
        internal static uint Hash32(ReadOnlySpan<byte> input)
        {
            uint len = (uint)input.Length;

            if (len <= 4)
                return Hash32Len0to4(input);

            if (len <= 12)
                return Hash32Len5to12(input);

            if (len <= 24)
                return Hash32Len13to24(input);

            // len > 24
            uint h = len, g = c1 * len, f = g;
            uint a0 = Rotate32(Fetch32(input[^4..]) * c1, 17) * c2;
            uint a1 = Rotate32(Fetch32(input[^8..]) * c1, 17) * c2;
            uint a2 = Rotate32(Fetch32(input[^16..]) * c1, 17) * c2;
            uint a3 = Rotate32(Fetch32(input[^12..]) * c1, 17) * c2;
            uint a4 = Rotate32(Fetch32(input[^20..]) * c1, 17) * c2;

            h ^= a0;
            h = Rotate32(h, 19);
            h = h * 5 + 0xe6546b64;
            h ^= a2;
            h = Rotate32(h, 19);
            h = h * 5 + 0xe6546b64;

            g ^= a1;
            g = Rotate32(g, 19);
            g = g * 5 + 0xe6546b64;
            g ^= a3;
            g = Rotate32(g, 19);
            g = g * 5 + 0xe6546b64;

            f += a4;
            f = Rotate32(f, 19);
            f = f * 5 + 0xe6546b64;

            for (var i = 0; i < (len - 1) / 20; i++)
            {
                a0 = Rotate32(Fetch32(input[(20 * i)..]) * c1, 17) * c2;
                a1 = Fetch32(input[(20 * i + 4)..]);
                a2 = Rotate32(Fetch32(input[(20 * i + 8)..]) * c1, 17) * c2;
                a3 = Rotate32(Fetch32(input[(20 * i + 12)..]) * c1, 17) * c2;
                a4 = Fetch32(input[(20 * i + 16)..]);

                h ^= a0;
                h = Rotate32(h, 18);
                h = h * 5 + 0xe6546b64;

                f += a1;
                f = Rotate32(f, 19);
                f = f * c1;

                g += a2;
                g = Rotate32(g, 18);
                g = g * 5 + 0xe6546b64;

                h ^= a3 + a1;
                h = Rotate32(h, 19);
                h = h * 5 + 0xe6546b64;

                g ^= a4;
                g = BSwap32(g) * 5;

                h += a4 * 5;
                h = BSwap32(h);

                f += a0;

                Permute3(ref f, ref h, ref g);
            }

            g = Rotate32(g, 11) * c1;
            g = Rotate32(g, 17) * c1;

            f = Rotate32(f, 11) * c1;
            f = Rotate32(f, 17) * c1;

            h = Rotate32(h + g, 19);
            h = h * 5 + 0xe6546b64;
            h = Rotate32(h, 17) * c1;

            h = Rotate32(h + f, 19);
            h = h * 5 + 0xe6546b64;
            h = Rotate32(h, 17) * c1;

            return h;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong Hash64(ReadOnlySpan<byte> input, ulong seed)
            => Hash64(input, k2, seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong Hash64(ReadOnlySpan<byte> input, ulong seed0, ulong seed1)
            => HashLen16(Hash64(input) - seed0, seed1);

        internal static ulong Hash64(ReadOnlySpan<byte> input)
        {
            if (input.Length <= 16)
                return HashLen0to16(input);

            if (input.Length <= 32)
                return HashLen17to32(input);

            if (input.Length <= 64)
                return HashLen33to64(input);

            // For strings over 64 bytes we hash the end first, and then as we
            // loop we keep 56 bytes of state: v, w, x, y, and z.

            ulong x = Fetch64(input, input.Length - 40);
            ulong y = Fetch64(input, input.Length - 16) + Fetch64(input, input.Length - 56);
            ulong z = HashLen16(
                Fetch64(input, input.Length - 48) + (ulong)input.Length,
                Fetch64(input, input.Length - 24));

            var v = WeakHashLen32WithSeeds(input, input.Length - 64, (ulong)input.Length, z);
            var w = WeakHashLen32WithSeeds(input, input.Length - 32, y + k1, x);

            x = x * k1 + Fetch64(input);

            // Decrease len to the nearest multiple of 64, and operate on 64-byte chunks.

            var pos = 0;
            var len = (input.Length) - 1 & ~63;
            do
            {
                x = Rotate(x + y + v.Low + Fetch64(input, pos + 8), 37) * k1;
                y = Rotate(y + v.High + Fetch64(input, pos + 48), 42) * k1;
                x ^= w.High;
                y += v.Low + Fetch64(input, pos + 40);
                z = Rotate(z + w.Low, 33) * k1;
                v = WeakHashLen32WithSeeds(input, pos, v.High * k1, x + w.Low);
                w = WeakHashLen32WithSeeds(input, pos + 32, z + w.High, y + Fetch64(input, pos + 16));
                Swap(ref z, ref x);

                pos += 64;
                len -= 64;
            } while (len != 0);

            return HashLen16(HashLen16(v.Low, w.Low) + ShiftMix(y) * k1 + z, HashLen16(v.High, w.High) + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static UInt128 Hash128(ReadOnlySpan<byte> value)
        {
            return value.Length >= 16
                ? Hash128(value[16..], new UInt128(Fetch64(value[8..]) + k0, Fetch64(value)))
                : Hash128(value, new UInt128(k1, k0));
        }

        internal static UInt128 Hash128(ReadOnlySpan<byte> value, UInt128 seed)
        {
            if (value.Length < 128)
                return CityMurmur(value, seed);

            // We expect len >= 128 to be the common case.  Keep 56 bytes of state:
            // v, w, x, y, and z.
            int len = value.Length;
            ulong x = seed.Low;
            ulong y = seed.High;
            ulong z = (ulong)len * k1;

            UInt128 v = new(0, Rotate(y ^ k1, 49) * k1 + Fetch64(value));
            v = new(Rotate(v.Low, 42) * k1 + Fetch64(value, 8), v.Low);

            UInt128 w = new(Rotate(x + Fetch64(value, 88), 53) * k1, Rotate(y + z, 35) * k1 + x);


            // This is the same inner loop as CityHash64(), manually unrolled.
            int s = 0;
            do
            {
                var temp = Fetch64(value, s + 8);
                x = Rotate(x + y + v.Low + temp, 37) * k1;
                y = Rotate(y + v.High + Fetch64(value, s + 48), 42) * k1;
                x ^= w.High;
                y += v.Low + Fetch64(value, s + 40);
                z = Rotate(z + w.Low, 33) * k1;
                v = WeakHashLen32WithSeeds(value, s, v.High * k1, x + w.Low);
                w = WeakHashLen32WithSeeds(value, s + 32, z + w.High, y + Fetch64(value, s + 16));
                Swap(ref z, ref x);
                s += 64;
                x = Rotate(x + y + v.Low + Fetch64(value, s + 8), 37) * k1;
                y = Rotate(y + v.High + Fetch64(value, s + 48), 42) * k1;
                x ^= w.High;
                y += v.Low + Fetch64(value, s + 40);
                z = Rotate(z + w.Low, 33) * k1;
                v = WeakHashLen32WithSeeds(value, s, v.High * k1, x + w.Low);
                w = WeakHashLen32WithSeeds(value, s + 32, z + w.High, y + Fetch64(value, s + 16));
                Swap(ref z, ref x);
                s += 64;
                len -= 128;

            } while (len >= 128);

            x += Rotate(v.Low + z, 49) * k0;
            y = y * k0 + Rotate(w.High, 37);
            z = z * k0 + Rotate(w.Low, 27);
            w = new(w.High, w.Low * 9);
            v = new(v.High, v.Low * k0);

            // If 0 < len < 128, hash up to 4 chunks of 32 bytes each from the end of s.
            for (int tail = 0; tail < len;)
            {
                tail += 32;

                y = Rotate(x + y, 42) * k0 + v.High;
                w = new(w.High, w.Low + Fetch64(value, s + len - tail + 16));
                x = x * k0 + w.Low;
                z += w.High + Fetch64(value, s + len - tail);
                w = new(w.High + v.Low, w.Low);
                v = WeakHashLen32WithSeeds(value, s + len - tail, v.Low + z, v.High);
                v = new(v.High, v.Low * k0);
            }


            // At this point our 56 bytes of state should contain more than
            // enough information for a strong 128-bit hash.  We use two
            // different 56-byte-to-8-byte hashes to get a 16-byte final result.
            x = HashLen16(x, v.Low);
            y = HashLen16(y + z, w.Low);

            return new UInt128(HashLen16(x + v.High, w.High) + y, HashLen16(x + w.High, y + v.High));
        }

        #endregion

        #region HashLen

        private static uint Hash32Len0to4(ReadOnlySpan<byte> value)
        {
            uint l = (uint)value.Length;
            uint b = 0u;
            uint c = 9u;
            for (var i = 0; i < l; i++)
            {
                b = b * c1 + (uint)((sbyte)value[i]);
                c ^= b;
            }

            return FMix(Mur(b, Mur(l, c)));
        }

        private static uint Hash32Len5to12(ReadOnlySpan<byte> value)
        {
            uint a = (uint)value.Length, b = a * 5, c = 9, d = b;

            a += Fetch32(value, 0);
            b += Fetch32(value, value.Length - 4);
            c += Fetch32(value, (value.Length >> 1) & 4);

            return FMix(Mur(c, Mur(b, Mur(a, d))));
        }

        private static uint Hash32Len13to24(ReadOnlySpan<byte> value)
        {
            uint a = Fetch32(value, (value.Length >> 1) - 4);
            uint b = Fetch32(value, 4);
            uint c = Fetch32(value, value.Length - 8);
            uint d = Fetch32(value, value.Length >> 1);
            uint e = Fetch32(value, 0);
            uint f = Fetch32(value, value.Length - 4);
            uint h = (uint)value.Length;

            return FMix(Mur(f, Mur(e, Mur(d, Mur(c, Mur(b, Mur(a, h)))))));
        }

        private static ulong HashLen0to16(ReadOnlySpan<byte> value)
        {
            uint len = (uint)(value.Length);

            if (len >= 8)
            {

                ulong mul = k2 + (ulong)len * 2;
                ulong a = Fetch64(value) + k2;
                ulong b = Fetch64(value, value.Length - 8);
                ulong c = Rotate(b, 37) * mul + a;
                ulong d = (Rotate(a, 25) + b) * mul;

                return HashLen16(c, d, mul);
            }

            if (len >= 4)
            {
                var mul = k2 + (ulong)len * 2;
                ulong a = Fetch32(value);
                return HashLen16(len + (a << 3), Fetch32(value, (int)(len - 4)), mul);
            }

            if (len > 0)
            {
                var a = value[0];
                var b = value[value.Length >> 1];
                var c = value[value.Length - 1];

                var y = a + ((uint)b << 8);
                var z = len + ((uint)c << 2);

                return ShiftMix((y * k2 ^ z * k0)) * k2;
            }

            return k2;
        }

        private static ulong HashLen17to32(ReadOnlySpan<byte> value)
        {
            var len = (ulong)value.Length;

            var mul = k2 + len * 2ul;
            var a = Fetch64(value) * k1;
            var b = Fetch64(value, 8);
            var c = Fetch64(value, value.Length - 8) * mul;
            var d = Fetch64(value, value.Length - 16) * k2;

            return HashLen16(Rotate(a + b, 43) + Rotate(c, 30) + d, a + Rotate(b + k2, 18) + c, mul);
        }

        private static ulong HashLen33to64(ReadOnlySpan<byte> value)
        {
            ulong mul = k2 + (ulong)value.Length * 2ul;
            ulong a = Fetch64(value) * k2;
            ulong b = Fetch64(value, 8);
            ulong c = Fetch64(value, value.Length - 24);
            ulong d = Fetch64(value, value.Length - 32);
            ulong e = Fetch64(value, 16) * k2;
            ulong f = Fetch64(value, 24) * 9;
            ulong g = Fetch64(value, value.Length - 8);
            ulong h = Fetch64(value, value.Length - 16) * mul;

            ulong u = Rotate(a + g, 43) + (Rotate(b, 30) + c) * 9;
            ulong v = ((a + g) ^ d) + f + 1;
            ulong w = BSwap64((u + v) * mul) + h;
            ulong x = Rotate(e + f, 42) + c;
            ulong y = (BSwap64((v + w) * mul) + g) * mul;
            ulong z = e + f + c;

            a = BSwap64((x + z) * mul + y) + b;
            b = ShiftMix((z + a) * mul + d + h) * mul;
            return b + x;
        }

        private static UInt128 CityMurmur(ReadOnlySpan<byte> value, UInt128 seed)
        {
            var a = seed.Low;
            var b = seed.High;
            ulong c;
            ulong d;

            var len = value.Length;
            var l = len - 16;

            if (l <= 0)
            {  // len <= 16
                a = ShiftMix(a * k1) * k1;
                c = b * k1 + HashLen0to16(value);
                d = ShiftMix(a + (len >= 8 ? Fetch64(value) : c));
            }
            else
            {  // len > 16

                c = HashLen16(Fetch64(value, len - 8) + k1, a);
                d = HashLen16(b + (ulong)len, c + Fetch64(value, len - 16));
                a += d;

                var p = 0;
                do
                {
                    a ^= ShiftMix(Fetch64(value, p) * k1) * k1;
                    a *= k1;
                    b ^= a;
                    c ^= ShiftMix(Fetch64(value, p + 8) * k1) * k1;
                    c *= k1;
                    d ^= c;

                    p += 16;
                    l -= 16;
                } while (l > 0);

            }
            a = HashLen16(a, c);
            b = HashLen16(d, b);
            return new UInt128(a ^ b, HashLen16(b, a));
        }

        private static ulong Hash128to64(UInt128 x)
        {
            const ulong kMul = 0x9ddfea08eb382d69UL;

            var a = (x.Low ^ x.High) * kMul;
            a ^= (a >> 47);

            var b = (x.High ^ a) * kMul;
            b ^= (b >> 47);
            b *= kMul;

            return b;
        }

        // Return a 16-byte hash for 48 bytes.  Quick and dirty.
        // Callers do best to use "random-looking" values for a and b.
        private static UInt128 WeakHashLen32WithSeeds(ulong w, ulong x, ulong y, ulong z, ulong a, ulong b)
        {
            a += w;
            b = Rotate(b + a + z, 21);

            var c = a;
            a += x;
            a += y;

            b += Rotate(a, 44);

            return new UInt128(b + c, a + z);
        }
        private static UInt128 WeakHashLen32WithSeeds(ReadOnlySpan<byte> value, int offset, ulong a, ulong b)
        {
            return WeakHashLen32WithSeeds(
                Fetch64(value[offset..]),
                Fetch64(value[(offset + 8)..]),
                Fetch64(value[(offset + 16)..]),
                Fetch64(value[(offset + 24)..]),
                a,
                b);
        }

        #endregion

        #region Helper 

        // A 32-bit to 32-bit integer hash copied from Murmur3.
        static uint FMix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        // Helper from Murmur3 for combining two 32-bit values.
        static uint Mur(uint a, uint h)
        {
            a *= c1;
            a = Rotate32(a, 17);
            a *= c2;
            h ^= a;
            h = Rotate32(h, 19);
            return h * 5 + 0xe6546b64;
        }

        // Avoid shifting by 32: doing so yields an undefined result.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint Rotate32(uint val, int shift)
            => shift == 0 ? val : ((val >> shift) | (val << (32 - shift)));

        // Avoid shifting by 64: doing so yields an undefined result.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Rotate(ulong val, int shift)
            => shift == 0 ? val : ((val >> shift) | (val << (64 - shift)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Fetch32(ReadOnlySpan<byte> value, int index = 0)
            => BitConverter.ToUInt32(value[index..]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Fetch64(ReadOnlySpan<byte> value, int index = 0)
            => BitConverter.ToUInt64(value[index..]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ShiftMix(ulong val)
            => val ^ (val >> 47);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap<T>(ref T a, ref T b)
            => (a, b) = (b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Permute3<T>(ref T a, ref T b, ref T c)
            => (a, b, c) = (c, a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint BSwap32(uint value)
            => BitConverterX.Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong BSwap64(ulong value)
            => BitConverterX.Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong HashLen16(ulong u, ulong v)
            => Hash128to64(new UInt128(v, u));

        // Murmur-inspired hashing.
        private static ulong HashLen16(ulong u, ulong v, ulong mul)
        {
            ulong a = (u ^ v) * mul;
            a ^= (a >> 47);
            ulong b = (v ^ a) * mul;
            b ^= (b >> 47);
            b *= mul;
            return b;
        }
        #endregion
    }
}
