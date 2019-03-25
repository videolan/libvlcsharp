Remove-Item ..\LibVLCSharp.GTK\bin -Force -Recurse
Remove-Item ..\LibVLCSharp.GTK\obj -Force -Recurse

$msbuild =  '"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"'
iex ("& {0} {1}" -f $msbuild, "../LibVLCSharp.GTK/LibVLCSharp.GTK.csproj /t:Restore,Clean,Build /p:Configuration=Release")

robocopy  ..\LibVLCSharp.GTK\bin\Release\ . *.nupkg