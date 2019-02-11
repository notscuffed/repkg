# RePKG

Wallpaper engine PKG unpacker/TEX to png converter, written in C#.
Made using information obtained from reverse engineering "resourcecompiler.exe" in wallpaper engine bin's folder.

I don't know if anyone will find this project useful, uploaded it mainly because I wanted to learn to use git.
Made in 2 days including reverse engineering, feel free to report errors or request a pull if you have fixed/added something.

Initially I wanted to make it be able to extract and repack stuff but I found that repacking is not really that useful. 

# Features

  - Unpack PKG files
  - Convert TEX to PNG
  - Show info about PKG/TEX files

### Commands

 - help - shows those commands, use `help "extract"` and `help "info"` to see options for them
 - extract - extracts specified PKG/TEX file, or files from folder
 ```
-o, --output        (Default: ./output) Path to output directory
-i, --ignoreexts    Ignore files with specified extension (split extensions using ,)
-e, --onlyexts      Only extract files with specified extension (split extensions using ,)
-d, --debuginfo     (Default: false) Show debug info about extracted items
-t, --tex           (Default: false) Extract directory of tex files
-s, --singledir     (Default: false) Should all extracted files be put in one directory instead of their specific subfolders
-r, --recursive     (Default: false) Recursive search in all subfolders of specified directory
```
 - info - Shows info about specified PKG/TEX file
 ```
-s, --sort             (Default: false) Sort entries a-z
-b, --sortby           (Default: name) Sort by ... (available options: name, extension, size)
 ```
 
 ### Examples
Find .pkg files in subfolders of specified directory and only extract+convert .tex entries to png and put them in ./output omitting their paths from .pkg:
 ```
 repkg extract -e tex -s -o ./output E:\Games\steamapps\workshop\content\431960
 ```
 Extract all files from specific .pkg to output directory while keeping their paths from .pkg
 ```
  repkg extract -o ./output E:\Games\steamapps\workshop\content\431960
 ```
 Convert all .tex files from specific folder to png
 ```
 repkg extract -t -s E:\path\to\dir\with\tex\files
 ```
