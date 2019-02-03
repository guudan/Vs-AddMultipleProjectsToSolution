namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public interface INVsSolutionFolder : INVsProjectHierarchy
    {
        string Name { get; }
    }
}