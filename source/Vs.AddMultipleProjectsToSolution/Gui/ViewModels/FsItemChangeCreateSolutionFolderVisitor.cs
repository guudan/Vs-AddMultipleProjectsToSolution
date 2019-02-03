namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class FsItemChangeCreateSolutionFolderVisitor : IFsItemViewModelVisitor
    {
        private readonly bool _CreateSolutionFolders;

        public FsItemChangeCreateSolutionFolderVisitor(bool createSolutionFolders)
        {
            _CreateSolutionFolders = createSolutionFolders;
        }

        public void Visit(FsDirectoryViewModel directory)
        {
            directory.CreateSolutionFolder = _CreateSolutionFolders;
        }

        public void Visit(FsProjectDirectoryViewModel project)
        {
        }
    }
}