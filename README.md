# RePKG
Wallpaper engine PKG unpacker/TEX decompiler, written in C#.
Made using information obtained from reverse engineering "resourcecompiler.exe" in wallpaper engine bin's folder.

Feel free to report errors or request a pull if you have fixed/added something.

# Features
- Convert PKG into wallpaper engine project
- Unpack PKG files
- Decompile TEX
- Show info about PKG/TEX files

### Commands
- help - shows those commands, use `help "extract"` and `help "info"` to see options for them
- extract - extracts specified PKG/TEX file, or files from folder
```
-o, --output          (Default: ./output) Output directory
-i, --ignoreexts      Don't extract files with specified extensions (delimited by comma ",")
-e, --onlyexts        Only extract files with specified extensions (delimited by comma ",")
-d, --debuginfo       Print debug info while extracting/decompiling
-t, --tex             Decompile all tex files from specified directory in input
-s, --singledir       Should all extracted files be put in one directory instead of their entry path
-r, --recursive       Recursive search in all subfolders of specified directory
-c, --copyproject     Copy project.json and preview.jpg from beside .pkg into output directory
-n, --usename         Use name from project.json as project subfolder name instead of id
--no-tex-decompile    Don't decompile .tex files while extracting .pkg
--overwrite           Overwrite all existing files
```
 - info - Shows info and entries in PKG/TEX file
```
-s, --sort             Sort entries
-b, --sortby           (Default: name) Sort by ... (available options: name, extension, size)
-t, --tex              Get info about all tex files from specified directory in input
-p, --projectinfo      Select info from project.json to print (delimit using comma)
-e, --printentries     Print entries in packages
--title-filter         Title filter
```
 
### Examples
Find .pkg files in subfolders of specified directory and make wallpaper engine projects out of them in output directory
```
repkg extract -c E:\Games\steamapps\workshop\content\431960
```
Find .pkg files in subfolders of specified directory and only decompile .tex entries to png and put them in ./output omitting their paths from .pkg:
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