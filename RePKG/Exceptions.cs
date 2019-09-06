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
}
