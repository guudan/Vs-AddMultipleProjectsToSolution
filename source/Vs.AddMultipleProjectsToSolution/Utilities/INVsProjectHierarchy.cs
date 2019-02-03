using System.Collections.Generic;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public interface INVsProjectHierarchy
    {
        IEnumerable<INVsProject> Projects { get; }
        IEnumerable<INVsSolutionFolder> SolutionFolders { get; }
        INVsProject AddExistingProject(string projectFilePath);
        INVsSolutionFolder AddSolutionFolder(string name);
        INVsProject GetProjectByFilePath(string projectFilePath);
        INVsSolutionFolder GetSolutionFolder(string name);
    }
}