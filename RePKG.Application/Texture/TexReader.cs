using System.IO;
using System.Text;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexReader : ITexReader
    {
        private readonly ITexHeaderReader _texHeaderReader;
        private readonly ITexMipmapContainerReader _texMipmapContainerReader;
        private readonly ITexFrameInfoReader _texFrameInfoReader;

        public TexReader(
            ITexHeaderReader texHeaderReader,
            ITexMipmapContainerReader texMipmapContainerReader,
            ITexFrameInfoReader texFrameInfoReader)
        {
            _texHeaderReader = texHeaderReader;
            _texMipmapContainerReader = texMipmapContainerReader;
            _texFrameInfoReader = texFrameInfoReader;
        }

        public Tex ReadFromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var tex = new Tex {Magic1 = reader.ReadNString(16)};

                if (tex.Magic1 != "TEXV0005")
                    throw new UnknownTexHeaderMagicException(nameof(tex.Magic1), tex.Magic1);

                tex.Magic2 = reader.ReadNString(16);

                if (tex.Magic2 != "TEXI0001")
                    throw new UnknownTexHeaderMagicException(nameof(tex.Magic2), tex.Magic2);

                tex.Header = _texHeaderReader.ReadFromStream(stream);
                tex.MipmapsContainer = _texMipmapContainerReader.ReadFromStream(stream);

                _texMipmapContainerReader.ReadMipmapsFromStream(stream, tex);

                if (tex.IsGif)
                    _texFrameInfoReader.ReadFromStream(stream);

                return tex;
            }
        }
    }
}