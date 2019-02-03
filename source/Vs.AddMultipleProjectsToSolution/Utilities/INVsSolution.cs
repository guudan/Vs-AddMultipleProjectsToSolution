namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public interface INVsSolution : INVsProjectHierarchy
    {
        INVsProject GetProjectFromAnywhereInSolution(string projectFilePath);
    }
}