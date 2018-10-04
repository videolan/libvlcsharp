$msbuild =  '"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"'
iex ("& {0} {1}" -f $msbuild, "../LibVLCSharp/LibVLCSharp.csproj /t:Restore,Clean,Build /p:Configuration=Release")
..\nuget.exe pack ..\LibVLCSharp.nuspec