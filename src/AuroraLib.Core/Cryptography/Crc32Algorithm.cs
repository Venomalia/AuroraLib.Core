namespace AuroraLib.Core.Cryptography
{
    /// <summary>
    /// Predefined CRC-32 algorithms.
    /// </summary>
    public enum Crc32Algorithm
    {
        Default,
        BZIP2,
        JAMCRC,
        MPEG2,
        POSIX,
        SATA,
        XFER,
        CRC32C,
        CRC32D,
        CRC32Q,
    }

    internal static class Crc32Info
    {
        public static uint Polynomial(this Crc32Algorithm algorithm) => algorithm switch
        {
            Crc32Algorithm.Default => 0x04C11DB7,
            Crc32Algorithm.BZIP2 => 0x04C11DB7,
            Crc32Algorithm.JAMCRC => 0x04C11DB7,
            Crc32Algorithm.MPEG2 => 0x04C11DB7,
            Crc32Algorithm.POSIX => 0x04C11DB7,
            Crc32Algorithm.SATA => 0x04C11DB7,
            Crc32Algorithm.XFER => 0x000000AF,
            Crc32Algorithm.CRC32C => 0x1EDC6F41,
            Crc32Algorithm.CRC32D => 0xA833982B,
            Crc32Algorithm.CRC32Q => 0x814141AB,
            _ => throw new NotImplementedException(),
        };

        public static bool Reverse(this Crc32Algorithm algorithm) => algorithm switch
        {
            Crc32Algorithm.Default => false,
            Crc32Algorithm.BZIP2 => true,
            Crc32Algorithm.JAMCRC => false,
            Crc32Algorithm.MPEG2 => true,
            Crc32Algorithm.POSIX => true,
            Crc32Algorithm.SATA => true,
            Crc32Algorithm.XFER => true,
            Crc32Algorithm.CRC32C => false,
            Crc32Algorithm.CRC32D => false,
            Crc32Algorithm.CRC32Q => true,
            _ => throw new NotImplementedException(),
        };

        public static uint Initial(this Crc32Algorithm algorithm) => algorithm switch
        {
            Crc32Algorithm.Default => 0xFFFFFFFF,
            Crc32Algorithm.BZIP2 => 0xFFFFFFFF,
            Crc32Algorithm.JAMCRC => 0xFFFFFFFF,
            Crc32Algorithm.MPEG2 => 0xFFFFFFFF,
            Crc32Algorithm.POSIX => 0x00000000,
            Crc32Algorithm.SATA => 0x52325032,
            Crc32Algorithm.XFER => 0x00000000,
            Crc32Algorithm.CRC32C => 0xFFFFFFFF,
            Crc32Algorithm.CRC32D => 0xFFFFFFFF,
            Crc32Algorithm.CRC32Q => 0x00000000,
            _ => throw new NotImplementedException(),
        };

        public static uint XorOut(this Crc32Algorithm algorithm) => algorithm switch
        {
            Crc32Algorithm.Default => 0xFFFFFFFF,
            Crc32Algorithm.BZIP2 => 0xFFFFFFFF,
            Crc32Algorithm.JAMCRC => 0x00000000,
            Crc32Algorithm.MPEG2 => 0x00000000,
            Crc32Algorithm.POSIX => 0xFFFFFFFF,
            Crc32Algorithm.SATA => 0x00000000,
            Crc32Algorithm.XFER => 0x00000000,
            Crc32Algorithm.CRC32C => 0xFFFFFFFF,
            Crc32Algorithm.CRC32D => 0xFFFFFFFF,
            Crc32Algorithm.CRC32Q => 0x00000000,
            _ => throw new NotImplementedException(),
        };
    }
}
