del *.nupkg

$libvlcsharpPackagesCount = 7;

.\build-and-package-libvlcsharp.ps1
.\build-and-package-libvlcsharp-forms.ps1
.\build-and-package-libvlcsharp-wpf.ps1
.\build-and-package-libvlcsharp-forms-wpf.ps1
.\build-and-package-libvlcsharp-gtk.ps1
.\build-and-package-libvlcsharp-forms-gtk.ps1
.\build-and-package-libvlcsharp-winforms.ps1

$nugetPackages = Get-ChildItem -recurse -filter *.nupkg | Group-Object -Property Directory

if($nugetPackages.Count -eq $libvlcsharpPackagesCount)
{
    Write-Output "All $($libvlcsharpPackagesCount) packages built successfully"
}
else
{
    Write-Error "Not all $($libvlcsharpPackagesCount) packages built successfully"
}