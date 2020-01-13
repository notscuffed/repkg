using System;
using System.IO;
using System.Text;

namespace RePKG.Application
{
    internal static class Extensions
    {
        public static string ReadNString(this BinaryReader reader, int maxLength = -1)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            
            var builder = new StringBuilder(maxLength <= 0 ? 16 : maxLength);
            var c = reader.ReadChar();

            while (c != '\0' && (maxLength == -1 || builder.Length < maxLength))
            {
                builder.Append(c);
                c = reader.ReadChar();
            }

            return builder.ToString();
        }

        public static void WriteNString(this BinaryWriter writer, string input)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (input == null) throw new ArgumentNullException(nameof(input));

            writer.Write(Encoding.UTF8.GetBytes(input));
            writer.Write((byte) 0);
        }

        public static string ReadStringI32Size(this BinaryReader reader, int maxLength = -1)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            var size = reader.ReadInt32();

            if (maxLength > -1)
                size = Math.Min(size, maxLength);

            if (size < 0)
                throw new Exception("Size cannot be negative");

            var bytes = reader.ReadBytes(size);

            return Encoding.UTF8.GetString(bytes);
        }

        public static void WriteStringI32Size(this BinaryWriter writer, string input)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (input == null) throw new ArgumentNullException(nameof(input));

            writer.Write(input.Length);
            writer.Write(Encoding.UTF8.GetBytes(input));
        }
    }
}