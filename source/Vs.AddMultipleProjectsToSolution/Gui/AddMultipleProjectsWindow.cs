using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;

namespace Vs.AddMultipleProjectsToSolution.Gui
{
    public class AddMultipleProjectsWindow : DialogWindow
    {
        public AddMultipleProjectsWindow(AddMultipleProjectsConfigurationViewModel viewModel)
        {
            Title = "Add Projects To Solution";
            HasMaximizeButton = true;
            Width = 600;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Content = new AddMultipleProjectsConfigurationControl(viewModel);
        }
    }
}