# FF.Sim.PatchTools
---

Console command line to run to create VPX table patches. This will save the end user having to edit tables to be compatible with score keeping.

* Needs .net6.0 or later installed as it is not self contained (to keep build sizes down). https://dotnet.microsoft.com/en-us/download/dotnet/current

## Patch creating
---

Supply the original table and the edited table to create a diff patch file

`FlipsJdiff.exe -d "oldFile.vpx" "newFile.vpx"`

This will create a .diff file in a directory under new table file name. 

Text file in the created directory includes CRC32 for both versions.

## Create table from original and diff file
---

`FlipsJdiff.exe -p "oldFile.vpx" "diffFile.diff"` = Creates a NEW patched table (only reads from original table file)

## About

---

Uses `Jojos Binary Diff` https://sourceforge.net/projects/jojodiff/ . License included.
