namespace AuroraLib.Core.Interfaces
{
    /// <summary>
    /// Represents types that support reversing their endianness.
    /// </summary>
    /// <typeparam name="TSelf">The type implementing the interface.</typeparam>
    public interface IReversibleEndianness<TSelf> where TSelf : notnull
    {
        /// <summary>
        /// Reverses the endianness of the current value.
        /// </summary>
        TSelf ReverseEndianness();

#if NET6_0_OR_GREATER
        /// <summary>
        /// Reverses the endianness of the provided value.
        /// </summary>
        static TSelf ReverseEndianness(TSelf vaule) => ((IReversibleEndianness<TSelf>)vaule).ReverseEndianness();
#endif
    }
}
