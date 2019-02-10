using System;
using System.IO;
using System.Text;

namespace RePKG
{
    public static class Extensions
    {
        public static string ReadStringI32Size(this BinaryReader reader)
        {
            var size = reader.ReadInt32();
            var bytes = reader.ReadBytes(size);

            return Encoding.UTF8.GetString(bytes);
        }

        public static void WriteStringI32Size(this BinaryWriter writer, string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public static string ReadNString(this BinaryReader reader)
        {
            var builder = new StringBuilder(16);
            var c = reader.ReadChar();

            while (c != '\0')
            {
                builder.Append(c);
                c = reader.ReadChar();
            }

            return builder.ToString();
        }

        public static string[] SplitArguments(this string commandLine)
        {
            var parmChars = commandLine.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;

            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return new string(parmChars).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
