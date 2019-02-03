namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public interface IFsItemViewModelVisitor
    {
        void Visit(FsDirectoryViewModel directory);
        void Visit(FsProjectDirectoryViewModel project);
    }
}