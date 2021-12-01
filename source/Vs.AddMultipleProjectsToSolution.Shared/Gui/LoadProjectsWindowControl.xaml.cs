using System.Windows.Controls;
using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;

namespace Vs.AddMultipleProjectsToSolution.Gui
{
    public partial class LoadProjectsWindowControl : UserControl
    {
        public LoadProjectsWindowControl(LoadProjectsWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}