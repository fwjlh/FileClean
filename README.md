# FileClean

A small, fast C# console utility targeting .NET Framework 4.8.

## What it does

- Scans files directly inside the input folder (not subfolders).
- Converts grouped kilogram values such as `19,500 kg` to `19500`.
- Copies every readable text file to a separate output folder.
- Leaves the source files untouched.
- Compiles the regular expression once and reuses it.
- Rejects an output folder inside the input folder, preventing a later run from processing its own output.

Example:

```text
1656231,19,500 kg,12,800 kg,32,300 kg,170107
```

Becomes:

```text
1656231,19500,12800,32300,170107
```

## Run

```powershell
FileClean.exe "C:\Data\Input" "C:\Data\Output"
```

The input and output folders must be different, and the output folder must not be inside the input folder. Existing files with the same names in the output folder are overwritten.

## Build

Open `FileClean.sln` in Visual Studio and build the **Release** configuration. The executable is created at:

`src\FileClean\bin\Release\FileClean.exe`

Requires the .NET Framework 4.8 Developer Pack / targeting pack to build.
