using System;

namespace RePKG.Application.Exceptions
{
    public class UnknownTexHeaderMagicException : Exception
    {
        public UnknownTexHeaderMagicException(string magicName,  string magic) : base($"Unknown tex header {magicName} '{magic}'")
        {}
    }
}