using System;

namespace RePKG.Application.Exceptions
{
    /// <summary>
    /// Thrown on magic values that have not been tested before
    /// </summary>
    public class UnknownMagicException : Exception
    {
        public UnknownMagicException(string source, string magic) : base(
            $"Unknown magic: '{magic}' in '{source}'")
        {
        }
        
        public UnknownMagicException(string source, string property, string magic) : base(
            $"Unknown magic: '{magic}' in '{source}:{property}'")
        {
        }
    }
}