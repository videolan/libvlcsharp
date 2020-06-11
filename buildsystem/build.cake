//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solutionName = "LibVLCSharp";
var solutionFile = IsRunningOnWindows() ? $"{solutionName}.sln" : $"{solutionName}.Mac.sln";
var solutionPath = $"../src/{solutionFile}";
var libvlcsharpCsproj = "../src/libvlcsharp/libvlcsharp.csproj";

var packagesDir = "../packages";
var isCiBuild = BuildSystem.IsRunningOnAzurePipelines || BuildSystem.IsRunningOnAzurePipelinesHosted;
var suffixVersion = $"alpha-{DateTime.Today.ToString("yyyyMMdd")}-{BuildSystem.AzurePipelines.Environment.Build.Id}";
var feedzLVSSource = "https://f.feedz.io/videolan/libvlcsharp/nuget/index.json";
var FEEDZ = "FEEDZ";
const uint totalPackageCount = 8;
var buildProp = new FilePath("../src/Directory.build.props");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var artifactsDir = Directory("../nugets");
var artifactRelativePathPattern = "./../nugets/*";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    if(DirectoryExists(packagesDir))
    {
        DeleteDirectory(packagesDir, new DeleteDirectorySettings 
        {
            Recursive = true,
        });
    }
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionPath);
    MoveDirectory("../src/packages", packagesDir);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    Build(solutionPath);
});

// just for (faster) testing
Task("Build-only-libvlcsharp")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    Build(libvlcsharpCsproj);
});

Task("CIDeploy")
    .Does(() =>
{
    var packages = GetFiles(artifactRelativePathPattern);
    
    Information($"packages count: {packages.Count}");

    if(packages.Count != totalPackageCount)
    {
        throw new Exception($"There should be {totalPackageCount} packages but there is {packages.Count} packages");
    }

    NuGetPush(packages, new NuGetPushSettings 
    {
        Source = feedzLVSSource,
        ApiKey = EnvironmentVariable(FEEDZ)
    });
});

void Build(string project)
{
    if(isCiBuild)
    {
        XmlPoke(buildProp, "//Project/PropertyGroup/VersionSuffix", suffixVersion);
    }

    MSBuild(project, settings => settings.SetConfiguration(configuration).WithProperty("PackageOutputPath", MakeAbsolute(artifactsDir).FullPath));
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
