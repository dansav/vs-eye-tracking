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
var sourceDir = Directory("./source");
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

Task("UpdateVersionNumbers")
    .Does(() =>
{
    CreateAssemblyInfo(outputDir + File("AssemblyVersion.generated.cs"), new AssemblyInfoSettings {
        Version = version.MajorMinorPatch,
        FileVersion = version.MajorMinorPatch,
        InformationalVersion = version.InformationalVersion,
    });

    XmlPoke(
        sourceDir + File("EyeTrackingVsix/source.extension.vsixmanifest"),
        "/ns:PackageManifest/ns:Metadata/ns:Identity/@Version",
        version.MajorMinorPatch, new XmlPokeSettings
        {
            Namespaces = new Dictionary<string, string>
            {
                { "ns", "http://schemas.microsoft.com/developer/vsx-schema/2011" }
            }
        });
});

Task("Build")
    .Does(() =>
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

        MSBuild(solution, settings);
});

Task("Publish")
    .Does(() =>
{
    var fileName = "EyeTrackingVsix";
    MoveFile($"{buildDir}/{fileName}.vsix", publishDir + File($"{fileName}_{version.SemVer}.vsix"));
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
    .IsDependentOn("UpdateVersionNumbers")
    .IsDependentOn("Build")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
