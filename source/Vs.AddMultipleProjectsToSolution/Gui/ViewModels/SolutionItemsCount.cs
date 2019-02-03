namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class SolutionItemsCount
    {
        public int NumberOfProjects { get; set; }
        public int NumberOfSolutionFolders { get; set; }
        public int TotalNumberOfItems => NumberOfProjects + NumberOfSolutionFolders;
    }
}