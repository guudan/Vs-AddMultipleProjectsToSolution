namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public interface ISolutionItemViewModel
    {
        SolutionItemCreateStatus CreateStatus { get; set; }
        string CreateStatusMessage { get; set; }
        string Name { get; }
    }
}