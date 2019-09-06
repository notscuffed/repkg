using System;
using System.IO;
using System.Text;
using RePKG.Application.Exceptions;

namespace RePKG.Application
{
    internal static class Extensions
    {
        public static string ReadNString(this BinaryReader reader, int maxLength = -1)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var builder = new StringBuilder(16);
            var c = reader.ReadChar();
            
            while (c != '\0' && (maxLength == -1 || builder.Length < maxLength))
            {
                builder.Append(c);
                c = reader.ReadChar();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Checks if enum has correct value (note: doesn't work on flags)
        /// </summary>
        public static void AssertValid<T>(this T @enum) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), @enum))
                throw new EnumNotValidException<T>(@enum);
        }
    }
}