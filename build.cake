#tool "nuget:?package=xunit.runner.console&version=2.4.1"

var target = Argument("target", "Default");

public class BuildConsts
{
   public const string RootDir = "./source";
   public static readonly string SolutionFile = $"{RootDir}/Vs.AddMultipleProjectsToSolution.sln";
   public const string VsPackageProjectName = "Vs.AddMultipleProjectsToSolution";
   public const string VsixManifestFileName = "source.extension.vsixmanifest";
   public static string VsixManifestFilePath = $"{RootDir}/{VsPackageProjectName}/{VsixManifestFileName}";
   public static string VsPackageAssemblyInfoPath = $"{RootDir}/{VsPackageProjectName}/Properties/AssemblyInfo.cs";
   public static string VsixFileName => $"{VsPackageProjectName}.vsix";
   public const string VsTestProjectName = "Vs.AddMultipleProjectsToSolution.Tests";
   public static string VsTestDllFileName = $"{VsTestProjectName}.dll";

   public const string BuildOutputDir = "bin";
}

public class BuildInfo
{
   public int BuildNumber {get;set;}
   public string Configuration {get; set;}

   public string VsixOutputPath => 
      $"{BuildConsts.RootDir}/{BuildConsts.VsPackageProjectName}/bin/{Configuration}/{BuildConsts.VsixFileName}";

   public string TestProjectOutputDirPath =>
      $"{BuildConsts.RootDir}/{BuildConsts.VsTestProjectName}/bin/{Configuration}";
   public string TestProjectDllOutputPath =>
      $"{TestProjectOutputDirPath}/{BuildConsts.VsTestDllFileName}";
}

Setup<BuildInfo>(ctx =>
{
   Information("Running tasks...");
   var buildConfiguration = new BuildInfo();
   buildConfiguration.Configuration = Argument("configuration", "Release");

   Verbose("Getting build number from environment variable BUILD_BUILDNUMBER.");
   var buildNumberString = EnvironmentVariable("BUILD_BUILDNUMBER");
   buildConfiguration.BuildNumber = string.IsNullOrWhiteSpace(buildNumberString) ? 0 : int.Parse(buildNumberString);
   Information($"Build number: {buildConfiguration.BuildNumber}");

   return buildConfiguration;
});

Teardown(ctx =>
{
   Information("Finished running tasks.");
});

Task("Clean")
   .Does(()=>{
      CleanDirectory(BuildConsts.BuildOutputDir);
   });

Task("UpdatePackageVersion")
   .Does<BuildInfo>(config=>{
      var versionXPath = "/x:PackageManifest/x:Metadata/x:Identity/@Version";
      var vsixNamespace = new KeyValuePair<string, string>("x", "http://schemas.microsoft.com/developer/vsx-schema/2011");

      Verbose($"Reading version of the VSIX package from file {BuildConsts.VsixManifestFilePath}.");
      var peekSettings = new XmlPeekSettings();
      peekSettings.Namespaces.Add(vsixNamespace);
      var currentVersionString = XmlPeek(BuildConsts.VsixManifestFilePath, versionXPath, peekSettings);
      var currentVersion = Version.Parse(currentVersionString);
      Information($"Original version of the VSIX = {currentVersionString}");

      var newVersion = new Version(currentVersion.Major, currentVersion.Minor, config.BuildNumber);
      Information($"New version of the VSIX = {newVersion}");

      
      Verbose($"Setting version of the VSIX package from file {BuildConsts.VsixManifestFilePath} to {newVersion}.");
      var pokeSettings = new XmlPokeSettings();
      pokeSettings.Namespaces.Add(vsixNamespace);
      XmlPoke(BuildConsts.VsixManifestFilePath, versionXPath, newVersion.ToString(), pokeSettings);

      Verbose($"Reading assembly info from ${BuildConsts.VsPackageAssemblyInfoPath} file.");
      var currentAssemblyInfo = ParseAssemblyInfo(BuildConsts.VsPackageAssemblyInfoPath);

      Verbose($"Setting assembly info in ${BuildConsts.VsPackageAssemblyInfoPath} file.");
      var newAssemblyInfo = new AssemblyInfoSettings();
      newAssemblyInfo.Title = currentAssemblyInfo.Title;
      newAssemblyInfo.Description = currentAssemblyInfo.Description;
      newAssemblyInfo.FileVersion = newVersion.ToString();
      newAssemblyInfo.Version = newVersion.ToString();
      CreateAssemblyInfo(BuildConsts.VsPackageAssemblyInfoPath, newAssemblyInfo);
   });

Task("RestoreNuget")
   .Does(()=>{
      NuGetRestore(BuildConsts.SolutionFile);
   });

Task("Build")
   .IsDependentOn("Clean")
   .IsDependentOn("RestoreNuget")
   .IsDependentOn("UpdatePackageVersion")
   .Does<BuildInfo>(config=>{
      MSBuild(BuildConsts.SolutionFile, msBuildSettings =>{
         msBuildSettings.SetConfiguration(config.Configuration);
         msBuildSettings.ToolVersion = MSBuildToolVersion.VS2017;
         msBuildSettings.SetMSBuildPlatform(MSBuildPlatform.x86);
      });
   });

Task("Test")
   .IsDependentOn("Build")
   .Does<BuildInfo>(config=>{
      var testSettings = new XUnit2Settings();
      testSettings.UseX86 = true;
      testSettings.ReportName = "TestResults";
      testSettings.XmlReport = true;
      testSettings.OutputDirectory = config.TestProjectOutputDirPath;
      XUnit2(config.TestProjectDllOutputPath, testSettings);

      if(TFBuild.IsRunningOnVSTS)
      {
         var publishData = new TFBuildPublishTestResultsData();
         publishData.Configuration = config.Configuration;
         publishData.TestResultsFiles.Add(new FilePath($"{config.TestProjectOutputDirPath}/TestResults.xml"));
         publishData.TestRunTitle = "Unit Tests";
         publishData.TestRunner = TFTestRunnerType.XUnit;
         TFBuild.Commands.PublishTestResults(publishData);
      }
   });

Task("CopyFiles")
   .IsDependentOn("Build")
   .Does<BuildInfo>(config=>{
      CreateDirectory(BuildConsts.BuildOutputDir);
      CopyFileToDirectory(config.VsixOutputPath, BuildConsts.BuildOutputDir);
   });

Task("Default")
   .IsDependentOn("Build")
   .IsDependentOn("Test")
   .IsDependentOn("CopyFiles")
   .Does(() => {
   });

RunTarget(target);