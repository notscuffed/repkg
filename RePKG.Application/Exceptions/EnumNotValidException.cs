using System;

namespace RePKG.Application.Exceptions
{
    public class EnumNotValidException<T> : Exception where T : Enum
    {
        public EnumNotValidException(T @enum) : base($"Invalid value '{@enum}' for enum '{typeof(T)}'")
        {
        }
    }
}