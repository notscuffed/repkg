from repkg.repkg import init
from repkg.tex import Tex, TexImage, TexMipmap, TexFrameInfoContainer


def main():
    init()
    tex = Tex.fromfile("../../RePKG.Tests/TestTextures/V3_RGBA8888_GIF_TEXS0003.tex")

    for frame in tex.frameinfo_container.frames:
        frame.frametime /= 1.5

    l = []

    for img in tex.image_container.images:
        for mipmap in img.mipmaps:
            data = mipmap.bytes_data
            for i in range(0, mipmap.width * mipmap.height * 4, 4):
                data[i] = int(data[i] / 4)
                data[i+1] = int(data[i+1] / 4)
                data[i+2] = int(data[i+2] / 4)
            mipmap.bytes_data = data
            l.append(data)

    tex.saveimagetofile("test")


if __name__ == "__main__":
    main()
