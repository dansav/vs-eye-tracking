#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool nuget:?package=vswhere&version=2.6.7
#tool nuget:?package=GitVersion.CommandLine&version=4.0.0
#tool nuget:?package=NUnit.ConsoleRunner&version=3.10.0

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
    EnsureDirectoryExists(publishDir);
});

Task("Restore")
    .Does(() =>
{
    // workaround to not pick msbuild from VS2019 Preview
    var latestInstallationPath = VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.Component.MSBuild" });
    var msbuildPath = latestInstallationPath.CombineWithFilePath("MSBuild/current/Bin/MSBuild.exe");

    NuGetRestore(solution, new NuGetRestoreSettings()
    {
        MSBuildPath = System.IO.Path.GetDirectoryName(msbuildPath.ToString())
    });
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
            .WithTarget("Rebuild");

        MSBuild(solution, settings);
});

Task("Test")
    .Does(() =>
{
   NUnit3("./source/**/bin/Release/*.Tests.dll", new NUnit3Settings
   {
       X86 = true,
       Results = new[]
       {
            new NUnit3Result
            {
               FileName = outputDir + File("TestResult.xml")
            }
        },  
   });
});

Task("Publish")
    .IsDependentOn("Publish.Prepare")
    .IsDependentOn("Publish.UploadToVsixGallery")
    .IsDependentOn("Publish.UploadToVisualStudioMarketplace")
    ;

Task("Publish.Prepare")
    .Does(() =>
{
    var baseFileName = "EyeTrackingVsix";

    var sourcePath = $"{sourceDir}/EyeTrackingVsix/bin/x86/{configuration}/{baseFileName}.vsix";
    var targetPath = publishDir + File($"{baseFileName}_{version.SemVer}.vsix");

    Information($"Moving vsix to: {targetPath}");
    MoveFile(sourcePath, targetPath);
});

Task("Publish.UploadToVsixGallery")
    // TODO: when it is time to publish to VS Marketplace, change this to target develop-branch and/or release-branches
    .WithCriteria(() => version.BranchName == "master" && !BuildSystem.IsLocalBuild && !BuildSystem.IsPullRequest)
    .Does(() =>
{
    var file = GetFiles($"{publishDir}/*.vsix").First();
    Information($"Found file to upload: {file}");

    var repoUrl = "https://github.com/dansav/vs-eye-tracking/";
    var repo = System.Web.HttpUtility.UrlEncode(repoUrl);
    var issueTrackerUrl = $"{repoUrl}issues/";
    var issueTracker = System.Web.HttpUtility.UrlEncode(issueTrackerUrl);

    var uploadUrl = $"http://vsixgallery.com/api/upload?repo={repo}&issuetracker={issueTracker}";
    Information("Uploading vsix...");
    Information($"Url: {uploadUrl}");

    var client = new System.Net.Http.HttpClient();
    var fileContent = new System.Net.Http.ByteArrayContent(System.IO.File.ReadAllBytes(file.ToString()));
    var response = client.PostAsync(uploadUrl, fileContent).Result;
    Information(response.Content.ReadAsStringAsync().Result);

    Information("Done!");
});

Task("Publish.PreProcessReadmeFile").Does(() => 
{
    var filter = new TextLineFilter(new []
    {
        "",
        "^#",
        "^## Status",
        "^#",
    });

    var readme = System.IO.File.OpenRead(MakeAbsolute(File("README.md")).FullPath);
    var readmeCopy = System.IO.File.Create(MakeAbsolute(outputDir + File("README.md")).FullPath);
    using (var reader = new StreamReader(readme))
    using (var writer = new StreamWriter(readmeCopy))
    {
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (filter.CanWrite(line))
            {
                writer.WriteLine(line);
            }
        }
    }
});

Task("Publish.UploadToVisualStudioMarketplace")
    .WithCriteria(() => version.BranchName == "master" && !BuildSystem.IsLocalBuild && !BuildSystem.IsPullRequest)
    .IsDependentOn("Publish.PreProcessReadmeFile")
    .Does(() => 
{
    var manifest = sourceDir + File("extension-manifest.json");
    var file = GetFiles($"{publishDir}/*.vsix").First();
    var token = EnvironmentVariable("VSMARKETPLACE_TOKEN");

    Information($"Found file to publish: {file}");

    if (string.IsNullOrEmpty(token))
    {
        Error("Could not access PAT for publishing on VS Marketplace");
        throw new Exception("Could not access PAT for publishing on VS Marketplace");
    }

    var installationPath = VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.VisualStudio.Component.VSSDK" });
    var publisherPath = installationPath + File("/VSSDK/VisualStudioIntegration/Tools/Bin/VsixPublisher.exe");

    if (!FileExists(publisherPath))
    {
        Error("Could not find VsixPublisher.exe");
    }

    var exitCode = StartProcess(publisherPath,  new ProcessSettings
    {
        Arguments = $"publish -payload {file} -publishManifest {manifest} -personalAccessToken {token}"
    });

    if (exitCode != 0)
    {
       Error($"publisher exit code: '{exitCode}'");
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("UpdateVersionNumbers")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

public class TextLineFilter
{
    private readonly string[] _data;
    private int _pos;

    private bool _filtering;

    public TextLineFilter(string[] data)
    {
        _data = data;
        _filtering = string.IsNullOrEmpty(_data[0]);
        _pos = _filtering ? 1 : 0;
    }

    public bool CanWrite(string input)
    {
        if (_pos < _data.Length)
        {
            var match = System.Text.RegularExpressions.Regex.IsMatch(input, _data[_pos]);
            if (match)
            {
                _pos++;
                _filtering = !_filtering;
            }
        }

        return !_filtering;
    }
}