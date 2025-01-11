using System;

namespace AuroraLib.Core.Interfaces
{
    /// <summary>
    /// Supports generic cloning, offering a type-safe way to create a copy of the current instance.
    /// </summary>
    /// <typeparam name="T">The type of the object that this interface can clone.</typeparam>
    public interface ICloneable<out T> : ICloneable
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> that is a copy of this instance.
        /// </returns>
        new T Clone();

#if NET6_0_OR_GREATER
        object ICloneable.Clone() => Clone();
#endif
    }
}
