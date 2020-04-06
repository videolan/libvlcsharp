//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solutionName = "LibVLCSharp";
var solutionFile = IsRunningOnWindows() ? $"{solutionName}.sln" : $"{solutionName}.Mac.sln";
var solutionPath = $"../src/{solutionFile}";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./build") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    DeleteFiles("./**/bin/Release/*.nupkg");
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionPath);
    MoveDirectory("../src/packages", "../packages");
});

Task("BuildUnity")
    .Does(() =>
{
    DotNetCoreBuild($"{solutionName}/{solutionName}.csproj", new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("/p:UNITY=true"),
    });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild(solutionPath, settings => settings.SetConfiguration(configuration));
});

Task("CopyNugets")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyFiles(GetFiles("./**/bin/Release/*.nupkg"), buildDir);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("BuildUnity");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
