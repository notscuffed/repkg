using System;

namespace RePKG.Application.Exceptions
{
    public class UnknownTexMipmapContainerMagicException : Exception
    {
        public UnknownTexMipmapContainerMagicException(string magic) : base($"Unknown tex mipmap container magic: '{magic}'")
        {}
    }
}