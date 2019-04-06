#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool nuget:?package=vswhere&version=2.6.7

#reference "System.IO.Compression"
#reference "System.IO.Compression.FileSystem"
#reference "System.IO.Compression.ZipFile"

using System.IO.Compression;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var outputDir = Directory("./output");
var buildDir = outputDir + Directory("build");
var solution = File("./vs-eye-tracking.sln");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(outputDir);
    EnsureDirectoryExists(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        var latestInstallationPath = VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.Component.MSBuild" });
        var msbuildPath = latestInstallationPath.CombineWithFilePath("MSBuild/current/Bin/MSBuild.exe");

        var settings = new MSBuildSettings
        {
            ToolPath = msbuildPath,
            Configuration = configuration,
            PlatformTarget = PlatformTarget.x86,
        }
            .WithTarget("Rebuild")
            .WithProperty("OutDir", "../../" + buildDir);


        // Use MSBuild
        MSBuild(solution, settings);
    }
    else
    {
      // Use XBuild
      XBuild(solution, settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("PostProcess")
    .IsDependentOn("Build")
    .Does(() =>
{
    // add native dlls to vsix package
    using (var zipArchive = ZipFile.Open(buildDir + File("EyeTrackingVsix.vsix"), ZipArchiveMode.Update))
    {
        zipArchive.CreateEntryFromFile(
            buildDir + File("tobii_stream_engine.dll"),
            "tobii_stream_engine.dll");
    }

});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("PostProcess");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
