using System;

namespace RePKG.Application.Exceptions
{
    /// <summary>
    /// Thrown on unorganic values.
    /// For example when entry count is way higher than usual
    /// </summary>
    public class UnsafeTexException : Exception
    {
        public UnsafeTexException(string reason) : base($"Unsafe TEX detected, reason: {reason}")
        {}
    }
}