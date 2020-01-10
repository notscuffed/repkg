using System;

namespace RePKG.Application.Exceptions
{
    /// <summary>
    /// Thrown when enum raw value doesn't match any label
    /// </summary>
    /// <typeparam name="T">The enum</typeparam>
    public class EnumNotValidException<T> : Exception where T : Enum
    {
        public EnumNotValidException(T @enum) : base($"Invalid value '{@enum}' for enum '{typeof(T)}'")
        {
        }
    }
}