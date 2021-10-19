using Vs.AddMultipleProjectsToSolution.Utilities;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public partial class AddMultipleProjectsProgressViewModel
    {
        public class ProcessingContext
        {
            public ProcessingContext(
                INVsProjectHierarchy parent,
                ISolutionItemViewModel solutionItem)
            {
                Parent = parent;
                SolutionItem = solutionItem;
            }

            public INVsProjectHierarchy Parent { get; }
            public ISolutionItemViewModel SolutionItem { get; }
        }
    }
}