using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;

namespace Vs.AddMultipleProjectsToSolution.Gui
{
    public class LoadProjectsWindow : DialogWindow
    {
        public LoadProjectsWindow(LoadProjectsWindowViewModel viewModel)
        {
            Title = "Load Projects From Folder";
            WindowStyle = WindowStyle.None;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            Width = 300;
            Height = 250;
            Content = new LoadProjectsWindowControl(viewModel);
        }
    }
}