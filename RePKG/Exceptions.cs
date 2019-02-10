using System;

namespace RePKG
{
    public class UnknownImageFormat : Exception
    {
        public byte[] Bytes;

        public UnknownImageFormat(byte[] bytes)
        {
            Bytes = bytes;
        }
    }

    public class InvalidTexHeaderMagic : Exception
    {
        public string Expected;
        public string Got;

        public InvalidTexHeaderMagic(string expected, string got)
        {
            Expected = expected;
            Got = got;
        }
    }
}
