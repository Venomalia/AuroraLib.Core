using AuroraLib.Core.Interfaces;

namespace AuroraLib.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when an identifier is invalid.
    /// </summary>
    public class InvalidIdentifierException : Exception
    {
        /// <summary>
        /// The expected identifier.
        /// </summary>
        public readonly string ExpectedIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidIdentifierException"/> class.
        /// </summary>
        public InvalidIdentifierException() : base()
            => ExpectedIdentifier = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidIdentifierException"/> class with a specified expected identifier.
        /// </summary>
        /// <param name="expectedIdentifier">The expected identifier.</param>
        public InvalidIdentifierException(string expectedIdentifier) : base($"Expected \"{expectedIdentifier}\"")
            => ExpectedIdentifier = expectedIdentifier;

        /// <inheritdoc cref="InvalidIdentifierException(string)"/>
        public InvalidIdentifierException(ReadOnlySpan<byte> expectedIdentifier) : this(BitConverterX.ToString(expectedIdentifier))
        { }

        /// <inheritdoc cref="InvalidIdentifierException(string)"/>
        public InvalidIdentifierException(IIdentifier expectedIdentifier) : this(expectedIdentifier.AsSpan())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidIdentifierException"/> class with specified identifier and expected identifier.
        /// </summary>
        /// <param name="identifier">The actual identifier.</param>
        /// <param name="expectedIdentifier">The expected identifier.</param>
        public InvalidIdentifierException(string identifier, string expectedIdentifier) : base($"\"{identifier}\" Expected: \"{expectedIdentifier}\"")
            => ExpectedIdentifier = expectedIdentifier;

        /// <inheritdoc cref="InvalidIdentifierException(string,string)"/>
        public InvalidIdentifierException(ReadOnlySpan<byte> identifier, ReadOnlySpan<byte> expectedIdentifier) : this(BitConverterX.ToString(identifier), BitConverterX.ToString(expectedIdentifier))
        { }

        /// <inheritdoc cref="InvalidIdentifierException(string,string)"/>
        public InvalidIdentifierException(IIdentifier identifier, IIdentifier expectedIdentifier) : this(identifier.AsSpan(), expectedIdentifier.AsSpan())
        { }
    }

}
