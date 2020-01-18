from repkg.repkg import init
from repkg.tex import Tex, TexImage, TexMipmap, TexFrameInfoContainer


def main():
    init()
    tex = Tex.fromfile("../../RePKG.Tests/TestTextures/V3_RGBA8888_GIF_TEXS0003.tex")

    for frame in tex.frameinfo_container.frames:
        frame.frametime /= 1.5

    tex.saveimagetofile("test")


if __name__ == "__main__":
    main()
