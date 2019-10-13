param([string]$msbuild)

Remove-Item ..\LibVLCSharp.GTK\bin -Force -Recurse -ErrorAction Ignore
Remove-Item ..\LibVLCSharp.GTK\obj -Force -Recurse -ErrorAction Ignore

if(!$msbuild)
{
    $msbuild = "'C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe'";
}

iex ("& {0} {1}" -f $msbuild, "../LibVLCSharp.GTK/LibVLCSharp.GTK.csproj /t:Restore,Clean,Build /p:Configuration=Release")

robocopy  ..\LibVLCSharp.GTK\bin\Release\ . *.nupkg