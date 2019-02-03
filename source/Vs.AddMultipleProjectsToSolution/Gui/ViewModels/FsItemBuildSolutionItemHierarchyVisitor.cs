using System.Collections.Generic;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public sealed class FsItemBuildSolutionItemHierarchyVisitor : IFsItemViewModelVisitor
    {
        private readonly IList<ISolutionItemViewModel> _CurrentContextItems;
        private readonly SolutionItemsCount _ItemsCount;

        public FsItemBuildSolutionItemHierarchyVisitor()
        {
            _CurrentContextItems = new List<ISolutionItemViewModel>();
            _ItemsCount = new SolutionItemsCount();
        }

        private FsItemBuildSolutionItemHierarchyVisitor(SolutionDirectoryViewModel viewModel,
            SolutionItemsCount itemsCount)
        {
            _CurrentContextItems = viewModel.ChildItems;
            _ItemsCount = itemsCount;
        }

        void IFsItemViewModelVisitor.Visit(FsDirectoryViewModel directory)
        {
            if (directory.IsSelected == false)
            {
                return;
            }

            _ItemsCount.NumberOfSolutionFolders++;
            IFsItemViewModelVisitor childVisitor;

            if (directory.CreateSolutionFolder)
            {
                var solutionFolder = new SolutionDirectoryViewModel(directory.Name);
                _CurrentContextItems.Add(solutionFolder);

                childVisitor = new FsItemBuildSolutionItemHierarchyVisitor(solutionFolder, _ItemsCount);
            }
            else
            {
                childVisitor = this;
            }

            foreach (var directoryChild in directory.ChildItems)
            {
                directoryChild.Accept(childVisitor);
            }
        }

        void IFsItemViewModelVisitor.Visit(FsProjectDirectoryViewModel project)
        {
            if (project.IsSelected == false)
            {
                return;
            }

            _ItemsCount.NumberOfProjects++;
            var solutionProject = new SolutionProjectViewModel(project.Name, project.FilePath);
            _CurrentContextItems.Add(solutionProject);
        }

        public SolutionItemHierarchy BuildSolutionItemHierarchy(IEnumerable<IFsItemViewModel> items)
        {
            foreach (var item in items)
            {
                item.Accept(this);
            }

            var hierarchy = new SolutionItemHierarchy();
            hierarchy.NumberOfItemsToCreate = _ItemsCount;
            hierarchy.SolutionItems = _CurrentContextItems;
            return hierarchy;
        }
    }
}