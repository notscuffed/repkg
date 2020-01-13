//////////////////////////////
// Tex
//////////////////////////////

typedef struct {
    int image_id;
    float frametime;
    float x;
    float y;
    float width;
    float unk0;
    float unk1;
    float height;
} CTexFrameInfo;

typedef struct {
    const char *magic;
    int frame_count;
    CTexFrameInfo *frames;
    int gif_width;
    int gif_height;
} CTexFrameInfoContainer;

typedef struct {
    const char *bytes;
    int bytes_count;
    int width;
    int height;
    int decompressed_bytes_count;
    int is_lz4_compressed;
    int format; // MipmapFormat
} CTexMipmap;

typedef struct {
    int mipmap_count;
    CTexMipmap *mipmaps;
} CTexImage;

typedef struct {
    const char *magic;
    int image_format; // FreeImageFormat
    int image_count;
    CTexImage *images;
    int container_version; // TexImageContainerVersion
} CTexImageContainer;

typedef struct {
    int format; // TexFormat
    int flags; // TexFlags
    int texture_width;
    int texture_height;
    int image_width;
    int image_height;
    unsigned int unk_int0;
} CTexHeader;

typedef struct {
    const char *magic1;
    const char *magic2;
    CTexHeader header;
    CTexImageContainer images_container;
    CTexFrameInfoContainer *frameinfo_container;
} CTex;

//////////////////////////////
// Package
//////////////////////////////

typedef struct {
    const char *full_path;
    int offset;
    int length;
    const char *bytes;
    int type;
} CPackageEntry;

typedef struct {
    const char *magic;
    int header_size;
    int entry_count;
    CPackageEntry* entries;
} CPackage;

//////////////////////////////
// Functions
//////////////////////////////

CPackage *pkg_load(const char *path, int read_entry_bytes);
int pkg_destroy(CPackage *);

CTex *tex_load_file(const char *path);
CTex *tex_load(const char *memory);

const char *get_last_error();