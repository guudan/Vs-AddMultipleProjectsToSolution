public static class DeploymentArgs
{
   public const string Target = "target";
   public const string DeploymentPackagePath = "deploymentPackagePath";

   public const string Verbosity = "verbosity";

   public static string[] ArgumentList = new string[]{ Target, DeploymentPackagePath, Verbosity };

}
   