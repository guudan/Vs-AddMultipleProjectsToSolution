using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;

namespace Vs.AddMultipleProjectsToSolution.Gui
{
    public interface IWindowService
    {
        bool CloseAddMultipleProjectsWindow();
        bool CloseLoadProjectsWindow();
        void OpenAddMultipleProjectsWindow(AddMultipleProjectsConfigurationViewModel viewModel);
        void OpenLoadProjectsWindow(LoadProjectsWindowViewModel viewModel);
        string OpenOpenFolderDialog();
        void SetAddMultipleProjectsWindowContent(AddMultipleProjectsProgressViewModel addProjectsProgressViewModel);

        void SetAddMultipleProjectsWindowContent(
            AddMultipleProjectsConfigurationViewModel addProjectsConfigurationViewModel);
    }
}