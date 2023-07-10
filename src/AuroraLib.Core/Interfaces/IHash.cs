namespace AuroraLib.Core.Interfaces
{
    /// <summary>
    /// Represents a hash algorithm.
    /// </summary>
    public interface IHash
    {
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
    }
}
