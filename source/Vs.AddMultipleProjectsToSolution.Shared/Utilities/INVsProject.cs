namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public interface INVsProject
    {
        string FilePath { get; }
        string Name { get; }
        void Remove();
    }
}