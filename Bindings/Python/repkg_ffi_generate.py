import cffi
import os

ffi = cffi.FFI()

with open(os.path.join(os.path.dirname(__file__), "../repkg.h")) as f:
    ffi.cdef(f.read())

ffi.set_source("_repkg",
    '#include "../repkg.h"',
    libraries=["repkg.native"],
    library_dirs=[os.path.dirname(__file__),],
)

ffi.compile()