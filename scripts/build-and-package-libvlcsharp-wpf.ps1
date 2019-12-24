param([string]$msbuild)

Remove-Item ..\LibVLCSharp.WPF\bin -Force -Recurse -ErrorAction Ignore
Remove-Item ..\LibVLCSharp.WPF\obj -Force -Recurse -ErrorAction Ignore

iex ("& {0} {1}" -f $msbuild, "../LibVLCSharp.WPF/LibVLCSharp.WPF.csproj /t:Restore,Clean,Build /p:Configuration=Release")

robocopy  ..\LibVLCSharp.WPF\bin\Release\ . *.nupkg