#addin nuget:?package=Cake.Powershell&version=0.4.7
#load "deploy_common.cake"

var target = Argument(DeploymentArgs.Target, "Default");

public static class DeploymentConsts
{

}

public class DeploymentInfo
{
    public string DeploymentPackagePath {get; set;}
}

Setup<DeploymentInfo>(ctx =>
{
    Information("Running tasks...");
    var deploymentConfig = new DeploymentInfo();
    deploymentConfig.DeploymentPackagePath = Argument(DeploymentArgs.DeploymentPackagePath, "./Extension");
    
    return deploymentConfig;
});

Teardown(ctx =>
{
   Information("Finished running tasks.");
});

Task("PublishToVsixGallery")
    .Does<DeploymentInfo>(config => {
        Information("Publishing extension to staging environment Vsix Gallery (http://vsixgallery.com/). ");
        Verbose($"Serching for the VSIX file in {config.DeploymentPackagePath}.");
        var packageFile = GetFiles($"{config.DeploymentPackagePath}/*.vsix").FirstOrDefault()?.FullPath;
        if(string.IsNullOrEmpty(packageFile)){
            throw new Exception("Package file not found.");
        }

        Verbose($"Calling powershell script with argument -VsixPath \"{packageFile}\".");
        var settings = new PowershellSettings();
        settings.WorkingDirectory = new DirectoryPath("deployment_tools");
        settings.WithArguments(args => args
            .AppendQuoted("VsixPath", packageFile));
        StartPowershellFile("VsixGalleryPublish.ps1", settings);
    });

Task("PublishToGithub")
    .Does<DeploymentInfo>(ctx =>{

    });

Task("PublishToVsMarketplace")
    .Does<DeploymentInfo>(ctx =>{
        Information("Publishing extension to official Visual Studio Marketplace (https://marketplace.visualstudio.com/).");
    });

Task("Default")
    .Does(()=>{
        Warning("Nothing to do pick different target.");
    });

RunTarget(target);