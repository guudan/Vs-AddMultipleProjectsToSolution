using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Vs.AddMultipleProjectsToSolution.Annotations;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public class NVsSolution : INVsSolution
    {
        private readonly DTE2 _Dte;

        private NVsSolution([NotNull] DTE2 dte)
        {
            _Dte = dte ?? throw new ArgumentNullException(nameof(dte));
        }

        private IEnumerable<Project> InnerProjects => Solution
            .Projects
            .Cast<Project>();

        private Solution2 Solution => (Solution2) _Dte.Solution;

        public INVsProject AddExistingProject(string projectFilePath)
        {
            var project = Solution.AddFromFile(projectFilePath);
            return new NVsProject(Solution, project);
        }

        public INVsSolutionFolder AddSolutionFolder(string name)
        {
            var solutionFolder = Solution.AddSolutionFolder(name);
            return new NVsSolutionFolder(Solution, solutionFolder);
        }

        public INVsProject GetProjectByFilePath(string projectFilePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var project = InnerProjects
                .FilterToProjects()
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                .Where(p => string.Equals(p.FileName, projectFilePath, StringComparison.OrdinalIgnoreCase))
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                .Select(p => new NVsProject(Solution, p))
                .SingleOrDefault();
            
            return project;
        }

        public INVsProject GetProjectFromAnywhereInSolution(string projectFilePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var project = 
                GetProjectByFilePath(projectFilePath) ??
                FindProjectRecursivelyInSolutionFolders(SolutionFolders, projectFilePath);
            return project;
        }

        private INVsProject FindProjectRecursivelyInSolutionFolders(
            IEnumerable<INVsSolutionFolder> solutionFolders, string projectFilePath)
        {
            var folders = solutionFolders.ToArray();
            foreach (var folder in folders)
            {
                var project = folder.GetProjectByFilePath(projectFilePath);
                if (project != null)
                {
                    return project;
                }
            }

            foreach (var folder in folders)
            {
                var project = FindProjectRecursivelyInSolutionFolders(folder.SolutionFolders, projectFilePath);
                if (project != null)
                {
                    return project;
                }
            }

            return null;
        }

        public INVsSolutionFolder GetSolutionFolder(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            ThreadHelper.ThrowIfNotOnUIThread();
            var solutionFolder = InnerProjects
                .FilterToSolutionFolders()
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                .Where(sf => string.Equals(sf.Name, name, StringComparison.OrdinalIgnoreCase))
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                .Select(sf => new NVsSolutionFolder(Solution, sf))
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
                    .Select(p => new NVsProject(Solution, p));
            }
        }

        public IEnumerable<INVsSolutionFolder> SolutionFolders
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return InnerProjects
                    .FilterToSolutionFolders()
                    .Select(sf => new NVsSolutionFolder(Solution, sf));
            }
        }

        public static NVsSolution Create()
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            return new NVsSolution(dte);
        }
    }
}