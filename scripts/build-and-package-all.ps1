del *.nupkg

$libvlcsharpPackagesCount = 8;
[bool] $isCIBuild = $false;

if([System.Environment]::GetEnvironmentVariable('TF_BUILD') -eq "True")
{
    $msbuild = '"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"';
    $isCIBuild = $true;
}

.\build-and-package-libvlcsharp.ps1 $msbuild
.\build-and-package-libvlcsharp-forms.ps1 $msbuild
.\build-and-package-libvlcsharp-wpf.ps1 $msbuild
.\build-and-package-libvlcsharp-forms-wpf.ps1 $msbuild
.\build-and-package-libvlcsharp-gtk.ps1 $msbuild
.\build-and-package-libvlcsharp-forms-gtk.ps1 $msbuild
.\build-and-package-libvlcsharp-winforms.ps1 $msbuild
.\build-and-package-libvlcsharp-uno.ps1 $msbuild

$nugetPackages = Get-ChildItem -recurse -filter *.nupkg | Group-Object -Property Directory

if($nugetPackages.Count -eq $libvlcsharpPackagesCount)
{
    Write-Output "All $($libvlcsharpPackagesCount) packages built successfully"
    if($isCIBuild)
    { 
        [Environment]::Exit(0)
    }
}
else
{
    Write-Error "Not all $($libvlcsharpPackagesCount) packages built successfully"
    if($isCIBuild)
    {
        [Environment]::Exit(1)
    }
}

