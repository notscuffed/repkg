cimport crepkg
from libc.stdlib cimport calloc, free

#
# DO NOT EDIT - GENERATED FILE
#

cdef class BytesResult(object):
    cdef crepkg.CBytesResult * bytesresult
    cdef crepkg.CBytesResult * allocated

    # bytes_data
    @property
    def bytes_data(self) -> bytes:
        if self.bytesresult.bytes_data == NULL:
            return None
        return self.bytesresult.bytes_data

    @bytes_data.setter
    def bytes_data(self, value: bytes):
        self.bytesresult.bytes_data = value

    # length
    @property
    def length(self) -> int:
        return self.bytesresult.length

    @length.setter
    def length(self, value: int):
        self.bytesresult.length = value

    def __init__(self, dontallocate: bool = False):
        if not dontallocate:
            bytesresult = <crepkg.CBytesResult *> calloc(1, sizeof(crepkg.CBytesResult))
            self.allocated = bytesresult
            self.bytesresult = bytesresult

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CBytesResult * bytesresult):
        self.bytesresult = bytesresult
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef copyto(self, crepkg.CBytesResult * dst):
        dst[0] = self.bytesresult[0]

    cdef moveto(self, crepkg.CBytesResult * dst):
        dst[0] = self.bytesresult[0]
        self.bytesresult = dst

cdef class Package(object):
    cdef crepkg.CPackage * package
    cdef crepkg.CPackage * allocated

    # magic
    @property
    def magic(self) -> bytes:
        if self.package.magic == NULL:
            return None
        return self.package.magic

    @magic.setter
    def magic(self, value: bytes):
        self.package.magic = value

    # header_size
    @property
    def header_size(self) -> int:
        return self.package.header_size

    @header_size.setter
    def header_size(self, value: int):
        self.package.header_size = value

    # entry_count
    @property
    def entry_count(self) -> int:
        return self.package.entry_count

    @entry_count.setter
    def entry_count(self, value: int):
        self.package.entry_count = value

    def __init__(self, dontallocate: bool = False):
        if not dontallocate:
            package = <crepkg.CPackage *> calloc(1, sizeof(crepkg.CPackage))
            self.allocated = package
            self.package = package

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CPackage * package):
        self.package = package
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef copyto(self, crepkg.CPackage * dst):
        dst[0] = self.package[0]

    cdef moveto(self, crepkg.CPackage * dst):
        dst[0] = self.package[0]
        self.package = dst

cdef class PackageEntry(object):
    cdef crepkg.CPackageEntry * packageentry
    cdef crepkg.CPackageEntry * allocated

    # full_path
    @property
    def full_path(self) -> bytes:
        if self.packageentry.full_path == NULL:
            return None
        return self.packageentry.full_path

    @full_path.setter
    def full_path(self, value: bytes):
        self.packageentry.full_path = value

    # offset
    @property
    def offset(self) -> int:
        return self.packageentry.offset

    @offset.setter
    def offset(self, value: int):
        self.packageentry.offset = value

    # length
    @property
    def length(self) -> int:
        return self.packageentry.length

    @length.setter
    def length(self, value: int):
        self.packageentry.length = value

    # bytes_data
    @property
    def bytes_data(self) -> bytes:
        if self.packageentry.bytes_data == NULL:
            return None
        return self.packageentry.bytes_data

    @bytes_data.setter
    def bytes_data(self, value: bytes):
        self.packageentry.bytes_data = value

    # type
    @property
    def type(self) -> int:
        return self.packageentry.type

    @type.setter
    def type(self, value: int):
        self.packageentry.type = value

    def __init__(self, dontallocate: bool = False):
        if not dontallocate:
            packageentry = <crepkg.CPackageEntry *> calloc(1, sizeof(crepkg.CPackageEntry))
            self.allocated = packageentry
            self.packageentry = packageentry

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CPackageEntry * packageentry):
        self.packageentry = packageentry
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef copyto(self, crepkg.CPackageEntry * dst):
        dst[0] = self.packageentry[0]

    cdef moveto(self, crepkg.CPackageEntry * dst):
        dst[0] = self.packageentry[0]
        self.packageentry = dst

cdef class Tex(object):
    cdef crepkg.CTex * tex
    cdef crepkg.CTex * allocated
    cdef TexFrameInfoContainer _frameinfo_container
    cdef crepkg.CTexFrameInfoContainer * _frameinfo_container_last
    cdef TexHeader _texheader
    cdef TexImageContainer _teximagecontainer

    # magic1
    @property
    def magic1(self) -> bytes:
        if self.tex.magic1 == NULL:
            return None
        return self.tex.magic1

    @magic1.setter
    def magic1(self, value: bytes):
        self.tex.magic1 = value

    # magic2
    @property
    def magic2(self) -> bytes:
        if self.tex.magic2 == NULL:
            return None
        return self.tex.magic2

    @magic2.setter
    def magic2(self, value: bytes):
        self.tex.magic2 = value

    # frameinfo_container
    @property
    def frameinfo_container(self) -> TexFrameInfoContainer:
        if self._frameinfo_container_last != self.tex.frameinfo_container:
            self._frameinfo_container_last = self.tex.frameinfo_container
            if self.tex.frameinfo_container == NULL:
                self._frameinfo_container = None
            else:
                self._frameinfo_container = TexFrameInfoContainer()
                self._frameinfo_container.init(self.tex.frameinfo_container)
        return self._frameinfo_container

    @frameinfo_container.setter
    def frameinfo_container(self, value: TexFrameInfoContainer):
        if value is None:
            self._frameinfo_container_last = NULL
            self.tex.frameinfo_container = NULL
            self._frameinfo_container = None
            return
        self._frameinfo_container = value
        self._frameinfo_container_last = value.texframeinfocontainer
        self.tex.frameinfo_container = value.texframeinfocontainer

    def __init__(self, dontallocate: bool = False):
        self._teximagecontainer = TexImageContainer()
        self._texheader = TexHeader()
        self._frameinfo_container = None
        self._frameinfo_container_last = NULL
        if not dontallocate:
            tex = <crepkg.CTex *> calloc(1, sizeof(crepkg.CTex))
            self.allocated = tex
            self.tex = tex
            self._texheader.init(&tex.header)
            self._teximagecontainer.init(&tex.image_container)

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CTex * tex):
        self._teximagecontainer.init(&tex.image_container)
        self._texheader.init(&tex.header)
        if tex.frameinfo_container != NULL:
            self._frameinfo_container = TexFrameInfoContainer()
            self._frameinfo_container.init(tex.frameinfo_container)
            self._frameinfo_container_last = tex.frameinfo_container
        self.tex = tex
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef copyto(self, crepkg.CTex * dst):
        dst[0] = self.tex[0]

    cdef moveto(self, crepkg.CTex * dst):
        dst[0] = self.tex[0]
        self.tex = dst

    @classmethod
    def fromfile(cls, filepath: str):
        tex = Tex(True)
        tex.init(crepkg.tex_load_file(filepath.encode("utf-8")))
        return tex

    @classmethod
    def frombytes(cls, a_bytes: bytes):
        tex = Tex(True)
        tex.init(crepkg.tex_load(a_bytes))
        return tex

    def savetofile(self, filepath: str):
        return crepkg.tex_save_file(self.tex, filepath.encode("utf-8")) == 1

    def __dealloc__(self):
        if self.tex != NULL:
            crepkg.tex_destroy(self.tex)
            self.tex = NULL

    def saveimagetofile(self, filepath: str):
        return crepkg.tex_to_image_file(self.tex, filepath.encode("utf-8")) == 1

    @property
    def header(self) -> TexHeader:
        return self._texheader

    @property
    def image_container(self) -> TexImageContainer:
        return self._teximagecontainer

cdef class TexFrameInfo(object):
    cdef crepkg.CTexFrameInfo * texframeinfo
    cdef crepkg.CTexFrameInfo * allocated

    # image_id
    @property
    def image_id(self) -> int:
        return self.texframeinfo.image_id

    @image_id.setter
    def image_id(self, value: int):
        self.texframeinfo.image_id = value

    # frametime
    @property
    def frametime(self) -> float:
        return self.texframeinfo.frametime

    @frametime.setter
    def frametime(self, value: float):
        self.texframeinfo.frametime = value

    # x
    @property
    def x(self) -> float:
        return self.texframeinfo.x

    @x.setter
    def x(self, value: float):
        self.texframeinfo.x = value

    # y
    @property
    def y(self) -> float:
        return self.texframeinfo.y

    @y.setter
    def y(self, value: float):
        self.texframeinfo.y = value

    # width
    @property
    def width(self) -> float:
        return self.texframeinfo.width

    @width.setter
    def width(self, value: float):
        self.texframeinfo.width = value

    # unk0
    @property
    def unk0(self) -> float:
        return self.texframeinfo.unk0

    @unk0.setter
    def unk0(self, value: float):
        self.texframeinfo.unk0 = value

    # unk1
    @property
    def unk1(self) -> float:
        return self.texframeinfo.unk1

    @unk1.setter
    def unk1(self, value: float):
        self.texframeinfo.unk1 = value

    # height
    @property
    def height(self) -> float:
        return self.texframeinfo.height

    @height.setter
    def height(self, value: float):
        self.texframeinfo.height = value

    def __init__(self, dontallocate: bool = False):
        if not dontallocate:
            texframeinfo = <crepkg.CTexFrameInfo *> calloc(1, sizeof(crepkg.CTexFrameInfo))
            self.allocated = texframeinfo
            self.texframeinfo = texframeinfo

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CTexFrameInfo * texframeinfo):
        self.texframeinfo = texframeinfo
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef copyto(self, crepkg.CTexFrameInfo * dst):
        dst[0] = self.texframeinfo[0]

    cdef moveto(self, crepkg.CTexFrameInfo * dst):
        dst[0] = self.texframeinfo[0]
        self.texframeinfo = dst

cdef class TexFrameInfoContainer(object):
    cdef crepkg.CTexFrameInfoContainer * texframeinfocontainer
    cdef crepkg.CTexFrameInfoContainer * allocated
    cdef TexFrameInfoCList _frames

    # magic
    @property
    def magic(self) -> bytes:
        if self.texframeinfocontainer.magic == NULL:
            return None
        return self.texframeinfocontainer.magic

    @magic.setter
    def magic(self, value: bytes):
        self.texframeinfocontainer.magic = value

    # frame_count
    @property
    def frame_count(self) -> int:
        return self.texframeinfocontainer.frame_count

    @frame_count.setter
    def frame_count(self, value: int):
        self.texframeinfocontainer.frame_count = value

    # frames_length
    @property
    def frames_length(self) -> int:
        return self.texframeinfocontainer.frames_length

    @frames_length.setter
    def frames_length(self, value: int):
        self.texframeinfocontainer.frames_length = value

    # gif_width
    @property
    def gif_width(self) -> int:
        return self.texframeinfocontainer.gif_width

    @gif_width.setter
    def gif_width(self, value: int):
        self.texframeinfocontainer.gif_width = value

    # gif_height
    @property
    def gif_height(self) -> int:
        return self.texframeinfocontainer.gif_height

    @gif_height.setter
    def gif_height(self, value: int):
        self.texframeinfocontainer.gif_height = value

    def __init__(self, dontallocate: bool = False):
        self._frames = TexFrameInfoCList()
        if not dontallocate:
            texframeinfocontainer = <crepkg.CTexFrameInfoContainer *> calloc(1, sizeof(crepkg.CTexFrameInfoContainer))
            self.allocated = texframeinfocontainer
            self.texframeinfocontainer = texframeinfocontainer
            self._frames.init(&texframeinfocontainer.frames, &texframeinfocontainer.frame_count, &texframeinfocontainer.frames_length)

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CTexFrameInfoContainer * texframeinfocontainer):
        self.texframeinfocontainer = texframeinfocontainer
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL
        self._frames.init(&texframeinfocontainer.frames, &texframeinfocontainer.frame_count, &texframeinfocontainer.frames_length)

    cdef copyto(self, crepkg.CTexFrameInfoContainer * dst):
        dst[0] = self.texframeinfocontainer[0]

    cdef moveto(self, crepkg.CTexFrameInfoContainer * dst):
        dst[0] = self.texframeinfocontainer[0]
        self.texframeinfocontainer = dst

    @property
    def frames(self) -> TexFrameInfoCList:
        return self._frames

cdef class TexHeader(object):
    cdef crepkg.CTexHeader * texheader
    cdef crepkg.CTexHeader * allocated

    # format
    @property
    def format(self) -> int:
        return self.texheader.format

    @format.setter
    def format(self, value: int):
        self.texheader.format = value

    # flags
    @property
    def flags(self) -> int:
        return self.texheader.flags

    @flags.setter
    def flags(self, value: int):
        self.texheader.flags = value

    # texture_width
    @property
    def texture_width(self) -> int:
        return self.texheader.texture_width

    @texture_width.setter
    def texture_width(self, value: int):
        self.texheader.texture_width = value

    # texture_height
    @property
    def texture_height(self) -> int:
        return self.texheader.texture_height

    @texture_height.setter
    def texture_height(self, value: int):
        self.texheader.texture_height = value

    # image_width
    @property
    def image_width(self) -> int:
        return self.texheader.image_width

    @image_width.setter
    def image_width(self, value: int):
        self.texheader.image_width = value

    # image_height
    @property
    def image_height(self) -> int:
        return self.texheader.image_height

    @image_height.setter
    def image_height(self, value: int):
        self.texheader.image_height = value

    # unk_int0
    @property
    def unk_int0(self) -> int:
        return self.texheader.unk_int0

    @unk_int0.setter
    def unk_int0(self, value: int):
        self.texheader.unk_int0 = value

    def __init__(self, dontallocate: bool = False):
        if not dontallocate:
            texheader = <crepkg.CTexHeader *> calloc(1, sizeof(crepkg.CTexHeader))
            self.allocated = texheader
            self.texheader = texheader

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CTexHeader * texheader):
        self.texheader = texheader
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef copyto(self, crepkg.CTexHeader * dst):
        dst[0] = self.texheader[0]

    cdef moveto(self, crepkg.CTexHeader * dst):
        dst[0] = self.texheader[0]
        self.texheader = dst

cdef class TexImage(object):
    cdef crepkg.CTexImage * teximage
    cdef crepkg.CTexImage * allocated
    cdef TexMipmapCList _mipmaps

    # mipmap_count
    @property
    def mipmap_count(self) -> int:
        return self.teximage.mipmap_count

    @mipmap_count.setter
    def mipmap_count(self, value: int):
        self.teximage.mipmap_count = value

    # mipmaps_length
    @property
    def mipmaps_length(self) -> int:
        return self.teximage.mipmaps_length

    @mipmaps_length.setter
    def mipmaps_length(self, value: int):
        self.teximage.mipmaps_length = value

    def __init__(self, dontallocate: bool = False):
        self._mipmaps = TexMipmapCList()
        if not dontallocate:
            teximage = <crepkg.CTexImage *> calloc(1, sizeof(crepkg.CTexImage))
            self.allocated = teximage
            self.teximage = teximage
            self._mipmaps.init(&teximage.mipmaps, &teximage.mipmap_count, &teximage.mipmaps_length)

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CTexImage * teximage):
        self.teximage = teximage
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL
        self._mipmaps.init(&teximage.mipmaps, &teximage.mipmap_count, &teximage.mipmaps_length)

    cdef copyto(self, crepkg.CTexImage * dst):
        dst[0] = self.teximage[0]

    cdef moveto(self, crepkg.CTexImage * dst):
        dst[0] = self.teximage[0]
        self.teximage = dst

    @property
    def mipmaps(self) -> TexMipmapCList:
        return self._mipmaps

cdef class TexImageContainer(object):
    cdef crepkg.CTexImageContainer * teximagecontainer
    cdef crepkg.CTexImageContainer * allocated
    cdef TexImageCList _images

    # magic
    @property
    def magic(self) -> bytes:
        if self.teximagecontainer.magic == NULL:
            return None
        return self.teximagecontainer.magic

    @magic.setter
    def magic(self, value: bytes):
        self.teximagecontainer.magic = value

    # image_format
    @property
    def image_format(self) -> int:
        return self.teximagecontainer.image_format

    @image_format.setter
    def image_format(self, value: int):
        self.teximagecontainer.image_format = value

    # image_count
    @property
    def image_count(self) -> int:
        return self.teximagecontainer.image_count

    @image_count.setter
    def image_count(self, value: int):
        self.teximagecontainer.image_count = value

    # images_length
    @property
    def images_length(self) -> int:
        return self.teximagecontainer.images_length

    @images_length.setter
    def images_length(self, value: int):
        self.teximagecontainer.images_length = value

    # container_version
    @property
    def container_version(self) -> int:
        return self.teximagecontainer.container_version

    @container_version.setter
    def container_version(self, value: int):
        self.teximagecontainer.container_version = value

    def __init__(self, dontallocate: bool = False):
        self._images = TexImageCList()
        if not dontallocate:
            teximagecontainer = <crepkg.CTexImageContainer *> calloc(1, sizeof(crepkg.CTexImageContainer))
            self.allocated = teximagecontainer
            self.teximagecontainer = teximagecontainer
            self._images.init(&teximagecontainer.images, &teximagecontainer.image_count, &teximagecontainer.images_length)

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CTexImageContainer * teximagecontainer):
        self.teximagecontainer = teximagecontainer
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL
        self._images.init(&teximagecontainer.images, &teximagecontainer.image_count, &teximagecontainer.images_length)

    cdef copyto(self, crepkg.CTexImageContainer * dst):
        dst[0] = self.teximagecontainer[0]

    cdef moveto(self, crepkg.CTexImageContainer * dst):
        dst[0] = self.teximagecontainer[0]
        self.teximagecontainer = dst

    @property
    def images(self) -> TexImageCList:
        return self._images

cdef class TexMipmap(object):
    cdef crepkg.CTexMipmap * texmipmap
    cdef crepkg.CTexMipmap * allocated

    # bytes_data
    @property
    def bytes_data(self) -> bytes:
        if self.texmipmap.bytes_data == NULL:
            return None
        return self.texmipmap.bytes_data

    @bytes_data.setter
    def bytes_data(self, value: bytes):
        self.texmipmap.bytes_data = value

    # bytes_count
    @property
    def bytes_count(self) -> int:
        return self.texmipmap.bytes_count

    @bytes_count.setter
    def bytes_count(self, value: int):
        self.texmipmap.bytes_count = value

    # width
    @property
    def width(self) -> int:
        return self.texmipmap.width

    @width.setter
    def width(self, value: int):
        self.texmipmap.width = value

    # height
    @property
    def height(self) -> int:
        return self.texmipmap.height

    @height.setter
    def height(self, value: int):
        self.texmipmap.height = value

    # decompressed_bytes_count
    @property
    def decompressed_bytes_count(self) -> int:
        return self.texmipmap.decompressed_bytes_count

    @decompressed_bytes_count.setter
    def decompressed_bytes_count(self, value: int):
        self.texmipmap.decompressed_bytes_count = value

    # is_lz4_compressed
    @property
    def is_lz4_compressed(self) -> int:
        return self.texmipmap.is_lz4_compressed

    @is_lz4_compressed.setter
    def is_lz4_compressed(self, value: int):
        self.texmipmap.is_lz4_compressed = value

    def __init__(self, dontallocate: bool = False):
        if not dontallocate:
            texmipmap = <crepkg.CTexMipmap *> calloc(1, sizeof(crepkg.CTexMipmap))
            self.allocated = texmipmap
            self.texmipmap = texmipmap

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef init(self, crepkg.CTexMipmap * texmipmap):
        self.texmipmap = texmipmap
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    cdef copyto(self, crepkg.CTexMipmap * dst):
        dst[0] = self.texmipmap[0]

    cdef moveto(self, crepkg.CTexMipmap * dst):
        dst[0] = self.texmipmap[0]
        self.texmipmap = dst

cdef class TexImageCList(object):
    cdef crepkg.CTexImage ** images_ptr
    cdef crepkg.CTexImage * last_images
    cdef int * image_count
    cdef int * images_length
    cdef crepkg.CTexImage * allocated

    def __init__(self):
        self.images_ptr = NULL
        self.last_images = NULL
        self.image_count = NULL
        self.images_length = NULL
        self.allocated = NULL

    cdef init(self, crepkg.CTexImage ** images_ptr, int * image_count, int * images_length):
        self.images_ptr = images_ptr
        self.image_count = image_count
        self.images_length = images_length

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    def __len__(self) -> int:
        return self.image_count[0]

    def __getitem__(self, index: int) -> TexImage:
        if self.images_ptr[0] == NULL:
            self._increase_size()
        if index < 0 or index >= self.image_count[0]:
            raise IndexError()
        item = TexImage(True)
        item.init(&self.images_ptr[0][index])
        return item

    def get(self, index: int) -> TexImage:
        self.__getitem__(index)

    def __setitem__(self, index: int, value: TexImage):
        if self.images_ptr[0] == NULL:
            self._increase_size()
        if index < 0 or index >= self.image_count[0]:
            raise IndexError()
        value.moveto(&self.images_ptr[0][index])

    def _increase_size(self):
        old_items = self.images_ptr[0]
        old_length = self.images_length[0]
        new_length = max(2 * old_length, 2)
        allocated = <crepkg.CTexImage *> calloc(new_length, sizeof(crepkg.CTexImage))
        for i in range(0, old_length):
            allocated[i] = old_items[i]
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL
        self.images_length[0] = new_length
        self.images_ptr[0] = allocated

    def append(self, value: TexImage):
        if self.images_ptr[0] == NULL:
            self._increase_size()
        new_count = self.image_count[0] + 1
        if new_count >= self.images_length[0]:
            self._increase_size()
        value.moveto(&self.images_ptr[0][self.image_count[0]])
        self.image_count[0] = new_count

    def __iter__(self) -> TexImageCListIter:
        iterator = TexImageCListIter()
        iterator.init(self.images_ptr[0], self.image_count[0])
        return iterator

    def clear(self):
        self.image_count[0] = 0

cdef class TexImageCListIter(object):
    cdef crepkg.CTexImage * _images
    cdef int _count
    cdef int _current

    def __init__(self):
        self._images = NULL
        self._current = -1
        self._count = 0

    cdef init(self, crepkg.CTexImage * images, int count):
        self._images = images
        self._count = count

    def __next__(self) -> TexImage:
        self._current = self._current + 1
        if self._current >= self._count:
            raise StopIteration()
        item = TexImage(True)
        item.init(&self._images[self._current])
        return item

cdef class TexMipmapCList(object):
    cdef crepkg.CTexMipmap ** mipmaps_ptr
    cdef crepkg.CTexMipmap * last_mipmaps
    cdef int * mipmap_count
    cdef int * mipmaps_length
    cdef crepkg.CTexMipmap * allocated

    def __init__(self):
        self.mipmaps_ptr = NULL
        self.last_mipmaps = NULL
        self.mipmap_count = NULL
        self.mipmaps_length = NULL
        self.allocated = NULL

    cdef init(self, crepkg.CTexMipmap ** mipmaps_ptr, int * mipmap_count, int * mipmaps_length):
        self.mipmaps_ptr = mipmaps_ptr
        self.mipmap_count = mipmap_count
        self.mipmaps_length = mipmaps_length

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    def __len__(self) -> int:
        return self.mipmap_count[0]

    def __getitem__(self, index: int) -> TexMipmap:
        if self.mipmaps_ptr[0] == NULL:
            self._increase_size()
        if index < 0 or index >= self.mipmap_count[0]:
            raise IndexError()
        item = TexMipmap(True)
        item.init(&self.mipmaps_ptr[0][index])
        return item

    def get(self, index: int) -> TexMipmap:
        self.__getitem__(index)

    def __setitem__(self, index: int, value: TexMipmap):
        if self.mipmaps_ptr[0] == NULL:
            self._increase_size()
        if index < 0 or index >= self.mipmap_count[0]:
            raise IndexError()
        value.moveto(&self.mipmaps_ptr[0][index])

    def _increase_size(self):
        old_items = self.mipmaps_ptr[0]
        old_length = self.mipmaps_length[0]
        new_length = max(2 * old_length, 2)
        allocated = <crepkg.CTexMipmap *> calloc(new_length, sizeof(crepkg.CTexMipmap))
        for i in range(0, old_length):
            allocated[i] = old_items[i]
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL
        self.mipmaps_length[0] = new_length
        self.mipmaps_ptr[0] = allocated

    def append(self, value: TexMipmap):
        if self.mipmaps_ptr[0] == NULL:
            self._increase_size()
        new_count = self.mipmap_count[0] + 1
        if new_count >= self.mipmaps_length[0]:
            self._increase_size()
        value.moveto(&self.mipmaps_ptr[0][self.mipmap_count[0]])
        self.mipmap_count[0] = new_count

    def __iter__(self) -> TexMipmapCListIter:
        iterator = TexMipmapCListIter()
        iterator.init(self.mipmaps_ptr[0], self.mipmap_count[0])
        return iterator

    def clear(self):
        self.mipmap_count[0] = 0

cdef class TexMipmapCListIter(object):
    cdef crepkg.CTexMipmap * _mipmaps
    cdef int _count
    cdef int _current

    def __init__(self):
        self._mipmaps = NULL
        self._current = -1
        self._count = 0

    cdef init(self, crepkg.CTexMipmap * mipmaps, int count):
        self._mipmaps = mipmaps
        self._count = count

    def __next__(self) -> TexMipmap:
        self._current = self._current + 1
        if self._current >= self._count:
            raise StopIteration()
        item = TexMipmap(True)
        item.init(&self._mipmaps[self._current])
        return item

cdef class TexFrameInfoCList(object):
    cdef crepkg.CTexFrameInfo ** frames_ptr
    cdef crepkg.CTexFrameInfo * last_frames
    cdef int * frame_count
    cdef int * frames_length
    cdef crepkg.CTexFrameInfo * allocated

    def __init__(self):
        self.frames_ptr = NULL
        self.last_frames = NULL
        self.frame_count = NULL
        self.frames_length = NULL
        self.allocated = NULL

    cdef init(self, crepkg.CTexFrameInfo ** frames_ptr, int * frame_count, int * frames_length):
        self.frames_ptr = frames_ptr
        self.frame_count = frame_count
        self.frames_length = frames_length

    def __dealloc__(self):
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL

    def __len__(self) -> int:
        return self.frame_count[0]

    def __getitem__(self, index: int) -> TexFrameInfo:
        if self.frames_ptr[0] == NULL:
            self._increase_size()
        if index < 0 or index >= self.frame_count[0]:
            raise IndexError()
        item = TexFrameInfo(True)
        item.init(&self.frames_ptr[0][index])
        return item

    def get(self, index: int) -> TexFrameInfo:
        self.__getitem__(index)

    def __setitem__(self, index: int, value: TexFrameInfo):
        if self.frames_ptr[0] == NULL:
            self._increase_size()
        if index < 0 or index >= self.frame_count[0]:
            raise IndexError()
        value.moveto(&self.frames_ptr[0][index])

    def _increase_size(self):
        old_items = self.frames_ptr[0]
        old_length = self.frames_length[0]
        new_length = max(2 * old_length, 2)
        allocated = <crepkg.CTexFrameInfo *> calloc(new_length, sizeof(crepkg.CTexFrameInfo))
        for i in range(0, old_length):
            allocated[i] = old_items[i]
        if self.allocated != NULL:
            free(self.allocated)
            self.allocated = NULL
        self.frames_length[0] = new_length
        self.frames_ptr[0] = allocated

    def append(self, value: TexFrameInfo):
        if self.frames_ptr[0] == NULL:
            self._increase_size()
        new_count = self.frame_count[0] + 1
        if new_count >= self.frames_length[0]:
            self._increase_size()
        value.moveto(&self.frames_ptr[0][self.frame_count[0]])
        self.frame_count[0] = new_count

    def __iter__(self) -> TexFrameInfoCListIter:
        iterator = TexFrameInfoCListIter()
        iterator.init(self.frames_ptr[0], self.frame_count[0])
        return iterator

    def clear(self):
        self.frame_count[0] = 0

cdef class TexFrameInfoCListIter(object):
    cdef crepkg.CTexFrameInfo * _frames
    cdef int _count
    cdef int _current

    def __init__(self):
        self._frames = NULL
        self._current = -1
        self._count = 0

    cdef init(self, crepkg.CTexFrameInfo * frames, int count):
        self._frames = frames
        self._count = count

    def __next__(self) -> TexFrameInfo:
        self._current = self._current + 1
        if self._current >= self._count:
            raise StopIteration()
        item = TexFrameInfo(True)
        item.init(&self._frames[self._current])
        return item
    