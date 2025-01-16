namespace AuroraLib.Core.Format
{
    /// <summary>
    /// Defines a provider that supplies file format information.
    /// </summary>
    public interface IFormatInfoProvider
    {
        /// <summary>
        /// Gets the file format information associated with the provider.
        /// </summary>
        IFormatInfo Info { get; }
    }
}
