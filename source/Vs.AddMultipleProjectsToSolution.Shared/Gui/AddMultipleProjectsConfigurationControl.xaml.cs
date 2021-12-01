using System.Windows.Controls;
using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;

namespace Vs.AddMultipleProjectsToSolution.Gui
{
    public partial class AddMultipleProjectsConfigurationControl : UserControl
    {
        public AddMultipleProjectsConfigurationControl(AddMultipleProjectsConfigurationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}