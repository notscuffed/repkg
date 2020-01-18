cdef extern from "../repkg.h":

    ##############################
    ## Tex
    ##############################

    ctypedef struct CTexFrameInfo:
        int image_id
        float frametime
        float x
        float y
        float width
        float unk0
        float unk1
        float height

    ctypedef struct CTexFrameInfoContainer:
        char *magic
        int frame_count
        int frames_length
        CTexFrameInfo *frames
        int gif_width
        int gif_height

    ctypedef struct CTexMipmap:
        char *bytes_data
        int bytes_count
        int width
        int height
        int decompressed_bytes_count
        int is_lz4_compressed
        int format  # MipmapFormat

    ctypedef struct CTexImage:
        int mipmap_count
        int mipmaps_length
        CTexMipmap *mipmaps

    ctypedef struct CTexImageContainer:
        char *magic
        int image_format
        int image_count
        int images_length
        CTexImage *images
        int container_version

    ctypedef struct CTexHeader:
        int format  # TexFormat
        int flags  # TexFlags
        int texture_width
        int texture_height
        int image_width
        int image_height
        unsigned int unk_int0

    ctypedef struct CTex:
        char *magic1
        char *magic2
        CTexHeader header
        CTexImageContainer image_container
        CTexFrameInfoContainer *frameinfo_container

    ##############################
    ## Pkg
    ##############################

    ctypedef struct CPackageEntry:
        char *full_path
        int offset
        int length
        char *bytes_data
        int type

    ctypedef struct CPackage:
        char *magic
        int header_size
        int entry_count
        CPackageEntry* entries

    ##############################
    ## Data
    ##############################

    ctypedef struct CBytesResult:
        char *bytes_data
        int length

    ##############################
    ## Functions
    ##############################

    ctypedef void log_function(const char *log, int log_level)
    void set_logger(log_function log)

    const char *get_last_error()
    int free_resource(void *owner, void *resource)

    CPackage *pkg_load_file(const char *path)
    int pkg_destroy(CPackage *pkg)

    CTex *tex_load(const char *memory)
    CTex *tex_load_file(const char *path)

    int tex_save_file(CTex *tex, const char *path)
    CBytesResult tex_save(CTex *tex, const char *path)

    int tex_to_image_file(CTex *tex, const char *path)
    CBytesResult tex_to_image(CTex *tex)

    void tex_destroy(CTex *tex)
