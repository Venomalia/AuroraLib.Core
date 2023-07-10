namespace AuroraLib.Core.Interfaces
{
    /// <summary>
    /// Represents a hash algorithm with a specific output type.
    /// </summary>
    /// <typeparam name="T">The type of the hash output.</typeparam>
    public interface IHash<T> : IHash where T : unmanaged
    {
        /// <summary>
        /// Gets the computed hash value of type <typeparamref name="T"/>.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Sets the seed value for the hash algorithm.
        /// </summary>
        /// <param name="seed">The seed value to set.</param>
        void SetSeed(T seed);
    }
}
