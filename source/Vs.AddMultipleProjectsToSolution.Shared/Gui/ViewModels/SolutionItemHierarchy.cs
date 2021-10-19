using System.Collections.Generic;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class SolutionItemHierarchy
    {
        public SolutionItemsCount NumberOfItemsToCreate { get; set; }
        public IList<ISolutionItemViewModel> SolutionItems { get; set; }
    }
}