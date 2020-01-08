using System;

namespace RePKG.Application.Exceptions
{
    public class UnknownTexImageContainerMagicException : Exception
    {
        public UnknownTexImageContainerMagicException(string magic) : base($"Unknown tex mipmap container magic: '{magic}'")
        {}
    }
}