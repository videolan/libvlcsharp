param([string]$msbuild)

Remove-Item ..\LibVLCSharp.Forms.Platforms.GTK\bin -Force -Recurse -ErrorAction Ignore
Remove-Item ..\LibVLCSharp.Forms.Platforms.GTK\obj -Force -Recurse -ErrorAction Ignore

iex ("& {0} {1}" -f $msbuild, "../LibVLCSharp.Forms.Platforms.GTK/LibVLCSharp.Forms.Platforms.GTK.csproj /t:Restore,Clean,Build /p:Configuration=Release")

robocopy  ..\LibVLCSharp.Forms.Platforms.GTK\bin\Release\ . *.nupkg