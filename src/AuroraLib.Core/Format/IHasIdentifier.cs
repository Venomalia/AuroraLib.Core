using AuroraLib.Core.Format.Identifier;

namespace AuroraLib.Core.Format
{
    /// <summary>
    /// Represents an object that has an identifier.
    /// </summary>
    public interface IHasIdentifier
    {
        /// <summary>
        /// Gets the identifier associated with the object.
        /// </summary>
        IIdentifier Identifier { get; }
    }
}
