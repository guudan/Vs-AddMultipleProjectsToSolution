using System;
using EnvDTE;
using EnvDTE80;
using Vs.AddMultipleProjectsToSolution.Annotations;
using ProjectKinds = Vs.AddMultipleProjectsToSolution.Utilities.VsConsts.ProjectKinds;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public class NVsProject : INVsProject
    {
        [NotNull] private readonly Solution2 _Solution;
        [NotNull] private readonly Project _Project;

        public NVsProject(
            [NotNull] Solution2 solution,
            [NotNull] Project project)
        {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            if (project == null) throw new ArgumentNullException(nameof(project));
            if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                throw new ArgumentException("Invalid project type. Project can not be solution folder.");
            _Solution = solution;
            _Project = project;

            Name = project.Name;
            FilePath = project.FileName;
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
        }

        public string FilePath { get; }

        public string Name { get; }

        public void Remove()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            _Solution.Remove(_Project);
        }
    }
}