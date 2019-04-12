#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool nuget:?package=vswhere&version=2.6.7
#tool nuget:?package=GitVersion.CommandLine&version=4.0.0

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
var publishDir = outputDir + Directory("publish");
var solution = File("./vs-eye-tracking.sln");

var version = GitVersion();

Information($"Version: {version.SemVer}");
Information($"Git branch: {version.BranchName}");
Information($"Build provider: {BuildSystem.Provider}");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(outputDir);
    EnsureDirectoryExists(buildDir);
    EnsureDirectoryExists(publishDir);
});

Task("Restore")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
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

Task("Publish")
    .Does(() =>
{
    MoveFiles($"{buildDir}/*.vsix", publishDir);
});

Task("UploadToVsixGallery")
    .WithCriteria(() => version.BranchName == "master" && !BuildSystem.IsLocalBuild && !BuildSystem.IsPullRequest)
    .Does(() =>
{
    var file = GetFiles($"{publishDir}/*.vsix").First();
    var repoUrl = "https://github.com/dansav/vs-eye-tracking";
    var repo = System.Web.HttpUtility.UrlEncode(repoUrl);
    var issueTrackerUrl = $"{repoUrl}/issues";
    var issueTracker = System.Web.HttpUtility.UrlEncode(issueTrackerUrl);
    UploadFile($"http://vsixgallery.com/api/upload?repo={repo}&issuetracker={issueTracker}", file);
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
