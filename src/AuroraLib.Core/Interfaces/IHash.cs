namespace AuroraLib.Core.Interfaces
{
    /// <summary>
    /// Represents a hash algorithm.
    /// </summary>
    public interface IHash
    {
        /// <summary>
        /// Gets the size of the hash result in bytes.
        /// </summary>
        int ByteSize { get; }

        /// <summary>
        /// Resets the hash algorithm to its initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Computes the hash of the specified input data.
        /// </summary>
        /// <param name="input">The input data to compute the hash for.</param>
        void Compute(ReadOnlySpan<byte> input);

        /// <summary>
        /// Gets the computed hash value as a byte array.
        /// </summary>
        /// <returns>The computed hash value as a byte array.</returns>
        byte[] GetBytes();

        /// <summary>
        /// Writes the computed hash value to the provided destination span.
        /// </summary>
        /// <param name="destination">The span to write the hash value to.</param>
        void Write(Span<byte> destination);
    }
}
