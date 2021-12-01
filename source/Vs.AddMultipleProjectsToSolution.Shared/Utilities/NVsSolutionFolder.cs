using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Vs.AddMultipleProjectsToSolution.Annotations;
using ProjectKinds = Vs.AddMultipleProjectsToSolution.Utilities.VsConsts.ProjectKinds;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public class NVsSolutionFolder : INVsSolutionFolder
    {
        [NotNull] private readonly Solution2 _Solution;
        private readonly Project _SolutionFolderProject;
        private readonly SolutionFolder _SolutionFolder;

        public NVsSolutionFolder(
            [NotNull] Solution2 solution,
            [NotNull] Project solutionFolderProject)
        {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            if (solutionFolderProject == null) throw new ArgumentNullException(nameof(solutionFolderProject));
            if (solutionFolderProject.Kind != ProjectKinds.vsProjectKindSolutionFolder)
                throw new ArgumentException("Unknown type of project. Project is not solution folder.");

            Name = solutionFolderProject.Name;
            _Solution = solution;
            _SolutionFolderProject = solutionFolderProject;
            _SolutionFolder = solutionFolderProject.Object as SolutionFolder;
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
        }

        private IEnumerable<Project> InnerProjects
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return _SolutionFolderProject
                    .ProjectItems
                    .Cast<ProjectItem>()
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                    .Select(pi => pi.Object as Project)
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                    .Where(p => p != null);
            }
        }

        public INVsProject AddExistingProject(string projectFilePath)
        {
            var project = _SolutionFolder.AddFromFile(projectFilePath);
            return new NVsProject(_Solution, project);
        }

        public INVsSolutionFolder AddSolutionFolder(string name)
        {
            var solutionFolder = _SolutionFolder.AddSolutionFolder(name);
            return new NVsSolutionFolder(_Solution, solutionFolder);
        }

        public INVsProject GetProjectByFilePath(string projectFilePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var project = InnerProjects
                .FilterToProjects()
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                .Where(p => string.Equals(p.FileName, projectFilePath, StringComparison.OrdinalIgnoreCase))
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                .Select(p => new NVsProject(_Solution, p))
                .SingleOrDefault();
            
            return project;
        }

        public INVsSolutionFolder GetSolutionFolder(string name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var solutionFolder = InnerProjects
                .FilterToSolutionFolders()
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                .Where(sf => string.Equals(sf.Name, name, StringComparison.OrdinalIgnoreCase))
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                .Select(sf => new NVsSolutionFolder(_Solution, sf))
                .SingleOrDefault();
            return solutionFolder;
        }

        public IEnumerable<INVsProject> Projects
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return InnerProjects
                    .FilterToProjects()
                    .Select(p => new NVsProject(_Solution, p));
            }
        }

        public IEnumerable<INVsSolutionFolder> SolutionFolders
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return InnerProjects
                    .FilterToSolutionFolders()
                    .Select(sf => new NVsSolutionFolder(_Solution, sf));
            }
        }

        public string Name { get; }
    }
}