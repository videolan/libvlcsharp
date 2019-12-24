param([string]$msbuild)

Remove-Item ..\LibVLCSharp.GTK\bin -Force -Recurse -ErrorAction Ignore
Remove-Item ..\LibVLCSharp.GTK\obj -Force -Recurse -ErrorAction Ignore

iex ("& {0} {1}" -f $msbuild, "../LibVLCSharp.GTK/LibVLCSharp.GTK.csproj /t:Restore,Clean,Build /p:Configuration=Release")

robocopy  ..\LibVLCSharp.GTK\bin\Release\ . *.nupkg