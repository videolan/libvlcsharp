#tool nuget:?package=NuGet.CommandLine&version=5.11.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solutionName = "LibVLCSharp";
var solutionFile = IsRunningOnWindows() ? $"{solutionName}.sln" : $"{solutionName}.Mac.sln";
var solutionPath = $"../src/{solutionFile}";
var libvlcsharpCsproj = "../src/libvlcsharp/libvlcsharp.csproj";
var testCsproj = "../src/LibVLCSharp.Tests/LibVLCSharp.Tests.csproj";

var packagesDir = "../packages";
var isCiBuild = BuildSystem.AzurePipelines.IsRunningOnAzurePipelines;
var suffixVersion = $"alpha-{DateTime.Today.ToString("yyyyMMdd")}-{BuildSystem.AzurePipelines.Environment.Build.Id}";
var feedzLVSSource = "https://f.feedz.io/videolan/preview/nuget/index.json";
var FEEDZ = "FEEDZ";
const uint totalPackageCount = 10;

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

Task("Test")
    .Does(() =>
{
    var settings = new DotNetTestSettings
    {
        Loggers = new []{ "console;verbosity=detailed" }
    };

    DotNetTest(testCsproj, settings);
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
    var settings = new MSBuildSettings();
    settings.SetConfiguration(configuration)
            .WithProperty("PackageOutputPath", MakeAbsolute(artifactsDir).FullPath);

    if(isCiBuild)
    {
        settings.WithProperty("VersionSuffix", suffixVersion);
    }

    settings.ToolVersion = MSBuildToolVersion.VS2022;

    MSBuild(project, settings);
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
