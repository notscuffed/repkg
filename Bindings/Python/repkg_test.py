from _repkg import ffi, lib
import sys

result = lib.tex_load_file(b"../../RePKG.Tests/TestTextures/V3_RGBA8888_GIF_TEXS0003.tex")

def eprint(*args, **kwargs):
    print(*args, file=sys.stderr, **kwargs)

if not result:
    err = ffi.string(lib.get_last_error())
    eprint(err.decode("utf-8"))
    exit(1)

print("== TEX == ")
print(f"Magic1: {ffi.string(result.magic1).decode('utf-8')}")
print(f"Magic2: {ffi.string(result.magic2).decode('utf-8')}")
print()
print("== TEX HEADER == ")
print(f"Format: {result.header.format}")
print(f"Flags: {result.header.flags}")
print(f"Tex size: {result.header.texture_width}x{result.header.texture_height}")
print(f"Img size: {result.header.image_width}x{result.header.image_height}")
print(f"Unk int 0: {result.header.unk_int0}")
print()
print("== TEX IMG CONTAINER == ")
print(f"Magic: {ffi.string(result.images_container.magic).decode('utf-8')}")
print(f"Container version: {result.images_container.container_version}")
print(f"Image format: {result.images_container.image_format}")
print(f"Image count: {result.images_container.image_count}")
print()

for i in range(result.images_container.image_count):
    image = result.images_container.images[i]
    print(f"* [{i}] Image (mipmap count: {image.mipmap_count})")
    for j in range(image.mipmap_count):
        mipmap = image.mipmaps[j]
        print(f"   - [{j}] Mipmap")
        print(f"     Size: {mipmap.width}x{mipmap.height}")

print()
print("== FRAME INFO CONTAINER ==")
if not result.frameinfo_container:
    print("No frame info")
else:
    print(f"Magic: {ffi.string(result.frameinfo_container.magic).decode('utf-8')}")
    print(f"Frame count: {result.frameinfo_container.frame_count}")
    print(f"Gif size: {result.frameinfo_container.gif_width}x{result.frameinfo_container.gif_height}")

    for i in range(result.frameinfo_container.frame_count):
        frame = result.frameinfo_container.frames[i]
        print(f"* [{i}] Frame")
        print(f"  Image ID: {frame.image_id}")
        print(f"  Frametime: {frame.frametime}")
        print(f"  X: {frame.x}")
        print(f"  Y: {frame.y}")
        print(f"  Size: {frame.width}x{frame.height}")
        print(f"  Unk0: {frame.unk0}")
        print(f"  Unk1: {frame.unk1}")

print()
print("Done")