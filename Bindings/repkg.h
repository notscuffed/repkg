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
    int frames_length;
    CTexFrameInfo *frames;
    int gif_width;
    int gif_height;
} CTexFrameInfoContainer;

typedef struct {
    const char *bytes_data;
    int bytes_count;
    int width;
    int height;
    int decompressed_bytes_count;
    int is_lz4_compressed;
    enum MipmapFormat format; // MipmapFormat
} CTexMipmap;

typedef struct {
    int mipmap_count;
    int mipmaps_length;
    CTexMipmap *mipmaps;
} CTexImage;

typedef struct {
    const char *magic;
    int image_format; // FreeImageFormat
    int image_count;
    int images_length;
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
    CTexImageContainer image_container;
    CTexFrameInfoContainer *frameinfo_container;
} CTex;

//////////////////////////////
// Package
//////////////////////////////

typedef struct {
    const char *full_path;
    int offset;
    int length;
    const char *bytes_data;
    int type;
} CPackageEntry;

typedef struct {
    const char *magic;
    int header_size;
    int entry_count;
    CPackageEntry* entries;
} CPackage;

//////////////////////////////
// Data
//////////////////////////////

typedef struct {
    const char *bytes_data;
    int length;
} CBytesResult;

//////////////////////////////
// Functions
//////////////////////////////

typedef void log_function(const char *log, int log_level);
void set_logger(log_function log);

const char *get_last_error();
int free_resource(void *owner, void *resource);


CPackage *pkg_load_file(const char *path);
int pkg_destroy(CPackage *pkg);


CTex *tex_load_file(const char *path);
CTex *tex_load(const char *memory);

int tex_save_file(CTex *tex, const char *path);
CBytesResult tex_save(CTex *tex, const char *path);

int tex_to_image_file(CTex *tex, const char *path);
CBytesResult tex_to_image(CTex *tex);

//const char *tex_generate_json_info(CTex *tex);

int tex_destroy(CTex *tex);