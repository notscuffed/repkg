using System;

namespace RePKG.Application.Exceptions
{
    public class UnknownTexFrameInfoContainerMagicException : Exception
    {
        public UnknownTexFrameInfoContainerMagicException(string magic) : base(
            $"Unknown tex frame info container magic: '{magic}'")
        {
        }
    }
}