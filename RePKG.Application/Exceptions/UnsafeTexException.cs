using System;

namespace RePKG.Application.Exceptions
{
    // Thrown on unorganic values
    // For example when entry count is way higher than usual
    public class UnsafeTexException : Exception
    {
        public UnsafeTexException(string reason) : base($"Unsafe TEX detected, reason: {reason}")
        {}
    }
}