﻿using AuroraLib.Core.Interfaces;

namespace AuroraLib.Core.Exceptions
{

    public class InvalidIdentifierException : Exception
    {
        public string ExpectedIdentifier { get; set; }

        public InvalidIdentifierException()
        { }

        public InvalidIdentifierException(string ExpectedIdentifier) : base($"Expected \"{ExpectedIdentifier}\"")
        {
            this.ExpectedIdentifier = ExpectedIdentifier;
        }

        public InvalidIdentifierException(IIdentifier ExpectedIdentifier) : base($"Expected \"{ExpectedIdentifier}\"")
        {
            this.ExpectedIdentifier = ExpectedIdentifier.ToString();
        }

        public InvalidIdentifierException(IIdentifier Identifier, IIdentifier ExpectedIdentifier) : base($"\"{Identifier}\" Expected: \"{ExpectedIdentifier}\"")
        {
            this.ExpectedIdentifier = ExpectedIdentifier.ToString();
        }
    }
}
