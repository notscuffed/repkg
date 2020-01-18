from distutils.core import setup
from distutils.extension import Extension
from Cython.Build import cythonize

ext_modules = [
    Extension("repkg.repkg",
              sources=["repkg/repkg.pyx"],
              libraries=["RePKG.Native"],
              include_dirs=["repkg/"]
              ),
    Extension("repkg.tex",
              sources=["repkg/tex.pyx"],
              libraries=["RePKG.Native"],
              include_dirs=["repkg/"]
              )
]

setup(name="RePKG",
      ext_modules=cythonize(
          ext_modules,
          compiler_directives={"language_level": "3"})
      )
